using UnityEngine;
using Indigo.UI;

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
   

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalVelocity;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        
    }

    void Update()
    {
        if (!canMove) return;
        if (UIEvents.IsPaused) return;

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
    
        //  ROTACIJA MODELA ZAVISNO OD KAMERE 
        if (cameraScript != null && cameraScript.isFirstPerson)
        {
            
            playerModel.forward = camForward;
        }
        else
        {
            //model se vrti prema tome gdje ideš
            if (move.magnitude > 0.1f)
            {
                playerModel.forward = move;
            }
        }

       // Animacija
        float inputMagnitude = Mathf.Clamp01(new Vector2(horizontal, vertical).magnitude);
        float animMultiplier = 0f; // 0 znači da animacija stoji (Idle)

        if (inputMagnitude > 0.1f) // Ako se igrač kreće
        {
           
             if (isRunning) 
                animMultiplier = 3f; // Trčanje: animacija se ubrzava za 50%
            else 
                animMultiplier = 1.3f; // Hodanje: normalna brzina animacije
        }

        AllInOne.SetFloat("Speed", animMultiplier);
        AllInOne.SetFloat("Vertical", vertical);

        characterController.height =  defaultHeight;

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