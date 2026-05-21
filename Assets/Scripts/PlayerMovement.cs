using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform playerModel;
    public Animator AllInOne;
    public Camera playerCamera;
    public Transform cameraPivot;

    // NOVO: Referenca na našu kameru da znamo da li je First Person!
    public ThirdPersonCamera cameraScript;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    public float defaultHeight = 3.9f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 1.5f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalVelocity;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Pokušava automatski naći skriptu kamere ako zaboraviš prevući u inspektoru
        if (cameraScript == null && cameraPivot != null)
        {
            cameraScript = cameraPivot.GetComponent<ThirdPersonCamera>();
        }
    }

    void Update()
    {
        if (!canMove) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * vertical + camRight * horizontal;

        if (move.magnitude > 1f) move.Normalize();

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.R);
        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

        moveDirection = move * currentSpeed;

        // --- KLJUČNA PROMJENA: ROTACIJA MODELA ZAVISNO OD KAMERE ---
        if (cameraScript != null && cameraScript.isFirstPerson)
        {
            // FIRST PERSON: Model se NE vrti po WASD tipkama. 
            // On striktno gleda naprijed, tačno tamo gdje i kamera.
            playerModel.forward = camForward;
        }
        else
        {
            // THIRD PERSON: Staro ponašanje (model se vrti prema tome gdje ideš)
            if (move.magnitude > 0.1f)
            {
                playerModel.forward = move;
            }
        }

        // Animacija
        float animSpeed = new Vector2(horizontal, vertical).magnitude;
        AllInOne.SetFloat("Speed", animSpeed);
        AllInOne.SetFloat("Vertical", vertical);

        characterController.height = isCrouching ? crouchHeight : defaultHeight;

        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = -2f;
            if (Input.GetButtonDown("Jump")) verticalVelocity = jumpPower;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}