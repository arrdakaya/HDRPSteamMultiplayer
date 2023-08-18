using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityMultiplayer.Manager;

public class PlayerMovementController : NetworkBehaviour
{
 

    public static PlayerMovementController Instance;
    [SerializeField] GameObject Other;

    private Animator anim;
    private NetworkAnimator netAnim;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    [SerializeField] private float acceleration = 2.0f;
    [SerializeField] private float deceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;

    int velocityZHash;
    int velocityXHash;

    //Movement
    private CharacterController controller;
    private Vector3 playerVelocity;

    bool isGrounded;

    float jumpHeight = 3.0f;
    float gravity = -9.81f;
    public float currentMaxVelocity;
    public bool canMove;
    public bool canCameraMove;

    //Camera

    [SerializeField] private LayerMask mouseLayerMask = new LayerMask();
    [SerializeField] private Transform target;
    [SerializeField] private Transform CameraRoot;
    [SerializeField] private Camera Camera;
    private float UpperLimit = -40f;
    private float BottomLimit = 70f;
    private float MouseSensitivity = 10.0f;
    private float _xRotation;
    private float _yRotation;

    private InputManager _inputManager;

    /// <summary>//////////////////////
    [Header("Movement")]
    //public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Other.SetActive(isLocalPlayer);
        Camera.GetComponent<Camera>().enabled = isLocalPlayer;
        Camera.GetComponent<AudioListener>().enabled = isLocalPlayer;

        
        if (!isLocalPlayer) { return; }
        controller = gameObject.GetComponent<CharacterController>();
        controller.enabled = isLocalPlayer;
        anim = GetComponent<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
        _inputManager = GetComponent<InputManager>();

        velocityZHash = Animator.StringToHash("Velocity Z");
        velocityXHash = Animator.StringToHash("Velocity X");
        ///////////////////////////////////////
        //rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true;

        readyToJump = true;
    }
    void Update()
    {
        if (isLocalPlayer)
        {
            bool forwardPressed = Input.GetKey(KeyCode.W);
            bool backPressed = Input.GetKey(KeyCode.S);
            bool leftPressed = Input.GetKey(KeyCode.A);
            bool rightPressed = Input.GetKey(KeyCode.D);
            bool runPressed = Input.GetKey(KeyCode.LeftShift);

            currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
            changeVelocity(forwardPressed, backPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
            LockOrResetVelocity(forwardPressed, backPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
            
            if (canMove)
            {
                anim.SetFloat(velocityZHash, velocityZ);
                anim.SetFloat(velocityXHash, velocityX);
                // ground check playerheigh * 0.5 + 0.3f sildik
                grounded = !Physics.Raycast(transform.position, Vector3.down, playerHeight, whatIsGround);

                //Move();
                Move2();
                
            }
            else
            {
                anim.SetFloat(velocityZHash, 0);
                anim.SetFloat(velocityXHash, 0);
                
            }

            
        }
    }
  
    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
           
                CamMovements();

            if (!gameObject.CompareTag("Monster"))
            {
                GetMousePosition();
            }

        }
    }

    void changeVelocity(bool forwardPressed, bool backPressed, bool leftPressed,bool rightPressed,bool runPressed,float currentMaxVelocity)
    {
        if (forwardPressed && velocityZ < currentMaxVelocity)
            velocityZ += Time.deltaTime * acceleration;

        if (backPressed && velocityZ > -currentMaxVelocity)
            velocityZ -= Time.deltaTime * acceleration;

        if (leftPressed && velocityX > -currentMaxVelocity)
            velocityX -= Time.deltaTime * acceleration;

        if (rightPressed && velocityX < currentMaxVelocity)
            velocityX += Time.deltaTime * acceleration;

        if (!forwardPressed && velocityZ > 0.0f)
            velocityZ -= Time.deltaTime * deceleration;

        if (!backPressed && velocityZ < 0.0f)
            velocityZ += Time.deltaTime * deceleration;

        if (!leftPressed && velocityX < 0.0f)
            velocityX += Time.deltaTime * deceleration;

        if (!rightPressed && velocityX > 0.0f)
            velocityX -= Time.deltaTime * deceleration;
    }

    void LockOrResetVelocity(bool forwardPressed, bool backPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        if (!forwardPressed && !backPressed && velocityZ != 0.0f)
            velocityZ = 0.0f;


        if (!leftPressed && !rightPressed && velocityX != 0.0f)
            velocityX = 0.0f;

        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05))
            {
                velocityZ = currentMaxVelocity;
            }

        }
        //round to the currentMaxVelocity if within offset
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;

        }
        //backPress
        if (backPressed && runPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (backPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ < -currentMaxVelocity && velocityZ < (-currentMaxVelocity - 0.05))
            {
                velocityZ = -currentMaxVelocity;
            }

        }
        //round to the currentMaxVelocity if within offset
        else if (backPressed && velocityZ > -currentMaxVelocity && velocityZ > (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;

        }
        //lock left
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;

        }
        //decelarate to the maximum walk velocity
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        //round to the currentMaxVelocity
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }

        //lock right
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        //decelerate to the maximum walk velocity
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            //round to currentMaxVelocity
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }

    
  
    // Update is called once per frame
    private void CamMovements()
    {
  
            if (!anim) return;
        if (canCameraMove)
        {
            var Mouse_X = _inputManager.Look.x;
            var Mouse_Y = _inputManager.Look.y;
            Camera.transform.position = CameraRoot.position;


            _xRotation -= Mouse_Y * MouseSensitivity * Time.deltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S)))
            {
                _yRotation += Mouse_X * MouseSensitivity * Time.deltaTime;
                _yRotation = Mathf.Clamp(_yRotation, -80, 80);

                Camera.transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0);

            }
            else
            {
                transform.Rotate(Vector3.up, _yRotation * MouseSensitivity * Time.deltaTime);

                _yRotation = Mathf.Lerp(_yRotation, 0, Time.deltaTime * 10.0f);
                Camera.transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0);
                transform.Rotate(Vector3.up, Mouse_X * MouseSensitivity * Time.deltaTime);

            }
        }
        else
        {
            Camera.transform.position = CameraRoot.position;

        }


    }


    
    
    private void OnAnimatorIK()
    {
        if (!anim) return;

        if (isLocalPlayer)
        {
            if (NetworkClient.ready)
            {
                
                float distance = 25;
                //anim.SetLookAtWeight(0.6f, 0.2f, 1.2f, 0.5f, 0.5f);
                netAnim.animator.SetLookAtWeight(0.6f, 0.2f, 1.2f, 0.5f, 0.5f);
                Ray lookAtRay = new Ray(transform.position, Camera.transform.forward);
                //anim.SetLookAtPosition(lookAtRay.GetPoint(distance));
                netAnim.animator.SetLookAtPosition(lookAtRay.GetPoint(distance));


            }
        }
    }
    void GetMousePosition()
    {
        float maxDistance = 999f;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxDistance, mouseLayerMask))
        {
            target.position = raycastHit.point;
        }
    }


    //private void MyInput()
    //{
    //    horizontalInput = Input.GetAxisRaw("Horizontal");
    //    verticalInput = Input.GetAxisRaw("Vertical");

    //    // when to jump
    //    if (Input.GetKey(jumpKey) && readyToJump && grounded)
    //    {
    //        readyToJump = false;

    //        Jump();

    //        Invoke(nameof(ResetJump), jumpCooldown);
    //    }
    //}

    //private void MovePlayer()
    //{
    //    // calculate movement direction
    //    moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;

    //    // on ground NORMALIZED SONRADAN EKLEME ÇAPRAZ GÝDERKEN AYNI HIZDA GÝTSÝN DÝYE
    //    if (grounded)
    //        rb.AddForce(moveDirection.normalized * currentMaxVelocity * 50f, ForceMode.Acceleration);
    //    // in air
    //    else if (!grounded)
    //        rb.AddForce(moveDirection.normalized * currentMaxVelocity * 50f * airMultiplier, ForceMode.Acceleration);

    //}

    //private void SpeedControl()
    //{
    //    Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    //    // limit velocity if needed
    //    if (flatVel.magnitude > currentMaxVelocity)
    //    {
    //        Vector3 limitedVel = flatVel.normalized * currentMaxVelocity;
    //        rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
    //    }
    //}

    //private void Jump()
    //{
    //    // reset y velocity
    //    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    //    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    //}
    //private void ResetJump()
    //{
    //    readyToJump = true;
    //}
   
    //private void Move()
    //{
    //    MyInput();
    //    SpeedControl();
    //    MovePlayer();

    //}



    private void Move2()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(move * currentMaxVelocity   * Time.deltaTime);
        controller.Move(playerVelocity * Time.deltaTime);

    }
}
