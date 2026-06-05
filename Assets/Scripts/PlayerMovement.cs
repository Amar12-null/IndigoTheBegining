using UnityEngine;
using Indigo.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform playerModel;
    public Animator AllInOne;
    public Camera playerCamera;
    public Transform cameraPivot;

    public ThirdPersonCamera cameraScript;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float defaultHeight = 3.9f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalVelocity;

    private bool canMove = false; // Postavljeno na false na početku

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Slušamo promjene stanja iz GameManagera
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        // Odjavljujemo se da spriječimo greške
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        // Igrač se može kretati samo ako je stanje Playing
        canMove = (state == GameState.Playing);

        // Kursor se zaključava i skriva samo tokom igranja
        if (state == GameState.Playing)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // U meniju, pauzi, ili shopu, kursor mora biti slobodan
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (!canMove) return;
        if (UIEvents.IsPaused) return; // Zadržali smo i kolegin uslov za UI pauzu

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
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        moveDirection = move * currentSpeed;
    
        if (cameraScript != null && cameraScript.isFirstPerson)
        {
            playerModel.forward = camForward;
        }
        else
        {
            if (move.magnitude > 0.1f)
            {
                playerModel.forward = move;
            }
        }

        float inputMagnitude = Mathf.Clamp01(new Vector2(horizontal, vertical).magnitude);
        float animMultiplier = 0f; 

        if (inputMagnitude > 0.1f) 
        {
             if (isRunning) 
                animMultiplier = 3f; 
            else 
                animMultiplier = 1.3f; 
        }

        AllInOne.SetFloat("Speed", animMultiplier);
        AllInOne.SetFloat("Vertical", vertical);

        characterController.height = defaultHeight;

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