using Cysharp.Threading.Tasks;
using Mirror;
using StarterAssets;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif
[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class PlayerMovementController : NetworkBehaviour
{
    public static PlayerMovementController Instance;
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 3.0f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -9.81f;
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;
    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;
    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    // animation IDs
    //private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    //private int _animIDMotionSpeed;
#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private Animator _animator;
    private NetworkAnimator _networkAnimator;
    private CharacterController _controller;
    private NewInputManager _input;
    public Camera _mainCamera;
    private bool _hasAnimator;
    [Header("Camera")]
    [SerializeField] private LayerMask mouseLayerMask = new LayerMask();
    [SerializeField] private Transform target;
    [SerializeField] private Transform CameraRoot;
    private float UpperLimit = -40f;
    private float BottomLimit = 70f;
    private float MouseSensitivity = 10.0f;
    private float _xRotation;
    private float _yRotation;
    public bool canCameraMove;
    public bool canMove;
    [SerializeField] GameObject other;
    int moveXAnimationParameterId;
    int moveZAnimationParameterId;
    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;
    [SerializeField] float animationSmoothTime = 0.05f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }
    private void Start()
    {
        if (!isLocalPlayer) { return; }
        _mainCamera.GetComponent<Camera>().enabled = isLocalPlayer;
        _mainCamera.GetComponent<AudioListener>().enabled = isLocalPlayer;
        other.SetActive(isLocalPlayer);
        _hasAnimator = TryGetComponent(out _animator);
        _networkAnimator = GetComponent<NetworkAnimator>();
        _controller = GetComponent<CharacterController>();
        _controller.enabled = isLocalPlayer;
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.enabled = isLocalPlayer;
        _input = GetComponent<NewInputManager>();
        _input.enabled = isLocalPlayer;
        AssignAnimationIDs();
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }
    private void Update()
    {
        if (!isLocalPlayer) { return; }

        _hasAnimator = TryGetComponent(out _animator);

        //if (!GameManager.Instance.IsGamePlaying()) { return; }

        JumpAndGravity();
        GroundedCheck();
        Move();
    }
    private void LateUpdate()
    {
        if (!isLocalPlayer) { return; }
        if (NetworkClient.ready)
        {
            CameraRotation();
            if (!gameObject.CompareTag("Monster"))
            {
                CmdGetMousePosition();
            }
        }

    }
    private void AssignAnimationIDs()
    {
        moveXAnimationParameterId = Animator.StringToHash("MoveX");
        moveZAnimationParameterId = Animator.StringToHash("MoveZ");
        //_animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        // _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }
    private void CameraRotation()
    {
        if (!_hasAnimator) return;
        if (canCameraMove)
        {
            var Mouse_X = _input.look.x;
            var Mouse_Y = _input.look.y;
            _mainCamera.transform.position = CameraRoot.position;
            _xRotation -= Mouse_Y * MouseSensitivity * Time.deltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);
            if (_input.move == Vector2.zero)
            {
                _yRotation += Mouse_X * MouseSensitivity * Time.deltaTime;
                _yRotation = Mathf.Clamp(_yRotation, -80, 80);
                _mainCamera.transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            }
            else
            {
                transform.Rotate(Vector3.up, _yRotation * MouseSensitivity * Time.deltaTime);
                _yRotation = Mathf.Lerp(_yRotation, 0, Time.deltaTime * 10.0f);
                _mainCamera.transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0);
                transform.Rotate(Vector3.up, Mouse_X * MouseSensitivity * Time.deltaTime);
            }
        }
        else
        {
            _mainCamera.transform.position = CameraRoot.position;
        }
    }
    private void Move()
    {

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;
        if (canMove)
        {
            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, _input.move, ref animationVelocity, animationSmoothTime);
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            // normalise input direction
            Vector3 inputDirection = new Vector3(currentAnimationBlendVector.x, 0.0f, currentAnimationBlendVector.y).normalized;
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
            }
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                        new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            // move the player
            // update animator if using character
            if (_hasAnimator)
            {
                //_animator.SetFloat(_animIDSpeed, _animationBlend);
                //_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                _animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
                _animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);
            }
        }
    }
    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }
            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }
            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;
            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }
            // if we are not grounded, do not jump
            _input.jump = false;
        }
        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (!isLocalPlayer) { return; }
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
    private void OnLand(AnimationEvent animationEvent)
    {
        if (!isLocalPlayer) { return; }
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
    [Command]
    void CmdGetMousePosition()
    {
        GetMousePosition();
    }
    [ClientRpc]
    void GetMousePosition()
    {
        float maxDistance = 999f;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxDistance, mouseLayerMask))
        {
            target.position = raycastHit.point;
        }
    }
    public bool GetHasAnimator()
    {
        return _hasAnimator;
    }
    public NetworkAnimator GetAnimator()
    {
        return _networkAnimator;
    }
}