using Mirror;
using UnityEngine;
using UnityMultiplayer.Manager;

public class PlayerMovementController : NetworkBehaviour
{
    public static PlayerMovementController Instance;
    [SerializeField] GameObject Other;

    public bool canMove = true;
    public bool canCameraMove = true;

    private Animator anim;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    [SerializeField] private float acceleration = 2.0f;
    [SerializeField] private float deceleration = 2.0f;
    [SerializeField] private float maximumWalkVelocity = 0.5f;
    [SerializeField] private float maximumRunVelocity = 2.0f;

    int velocityZHash;
    int velocityXHash;

    //Movement
    private CharacterController controller;
    private Vector3 playerVelocity;

    bool isGrounded;

    float jumpHeight = 3.0f;
    float gravity = -9.81f;
    private float currentMaxVelocity;

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
   

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Other.SetActive(isLocalPlayer);
        Camera.GetComponent<Camera>().enabled = isLocalPlayer;
        Camera.GetComponent<AudioListener>().enabled = isLocalPlayer;

        if (!isLocalPlayer) { return; }
        controller = gameObject.GetComponent<CharacterController>();
        controller.enabled = false;
        transform.position = new Vector3(Random.Range(0,-25), 5, Random.Range(0,-11));
        controller.enabled = true;
        anim = GetComponent<Animator>();
        _inputManager = GetComponent<InputManager>();
        velocityZHash = Animator.StringToHash("Velocity Z");
        velocityXHash = Animator.StringToHash("Velocity X");
    }
    void Update()
    {

        if (transform.position == new Vector3(0, 5, 0))
            Debug.Log("knonumdayýz");
        if (isLocalPlayer)
        {
            bool forwardPressed = Input.GetKey(KeyCode.W);
            bool backPressed = Input.GetKey(KeyCode.S);
            bool leftPressed = Input.GetKey(KeyCode.A);
            bool rightPressed = Input.GetKey(KeyCode.D);
            bool runPressed = Input.GetKey(KeyCode.LeftShift);

            currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

            if (canMove)
            {
                changeVelocity(forwardPressed, backPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
                LockOrResetVelocity(forwardPressed, backPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
            

            anim.SetFloat(velocityZHash, velocityZ);
            anim.SetFloat(velocityXHash, velocityX);
            Move();
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

           
            GetMousePosition();
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
        if (!forwardPressed && !backPressed && velocityZ != 0.0f && (velocityZ > -0.05f && velocityZ < 0.05f))
            velocityZ = 0.0f;


        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
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
        //lock left
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX -= -currentMaxVelocity;

        }
        //decelarate to the maximum walk velocity
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.0f))
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

    private void Move()
    {
        
            isGrounded = controller.isGrounded;

            if (isGrounded && playerVelocity.y < 0)
                playerVelocity.y = -2f;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * currentMaxVelocity * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        
        
    }

    // Update is called once per frame
    private void CamMovements()
    {
  
            if (!anim) return;

            var Mouse_X = _inputManager.Look.x;
            var Mouse_Y = _inputManager.Look.y;
            Camera.transform.position = CameraRoot.position;

        if (canCameraMove)
        {
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
    }


    [Command]
    private void OnAnimatorIK()
    {
        if (!anim) return;
        if (isLocalPlayer)
        {

            anim.SetLookAtWeight(0.6f, 0.2f, 1.2f, 0.5f, 0.5f);
            Ray lookAtRay = new Ray(transform.position, Camera.transform.forward);
            anim.SetLookAtPosition(lookAtRay.GetPoint(25));


        }
    }
    void GetMousePosition()
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseLayerMask))
        {
            target.position = raycastHit.point;
        }
    }



}
