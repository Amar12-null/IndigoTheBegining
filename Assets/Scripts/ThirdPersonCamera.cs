using UnityEngine;
using Indigo.UI; // 1. Dodali smo ovo da kamera može čitati kolegine postavke

public class ThirdPersonCamera : MonoBehaviour
{
    public Camera playerCamera;
    public Transform target;       
    public Transform playerBody;   
    
    [Header("Skrivanje Glave")]
    public Transform headBone;     

    // NAPOMENA: Obrisali smo 'public float mouseSensitivity' jer to sada kontroliše UI!
    public float minY = -30f;
    public float maxY = 60f;

    [Header("Third Person Postavke")]
    public float tpDistance = 6f; 
    public LayerMask collisionLayers;

    [Header("First Person Postavke")]
    public float firstPersonHeight = 2.8f; 

    public bool isFirstPerson = false; 

    private float yaw;
    private float pitch;
    
    private bool canLook = false; 

    void Start()
    {
        if (playerBody == null && target != null) 
        {
            playerBody = target.root; 
        }

        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        canLook = (state == GameState.Playing);
    }

    void Update()
    {
        if (canLook && Input.GetKeyDown(KeyCode.F5))
        {
            isFirstPerson = !isFirstPerson; 
        }
    }

    void LateUpdate()
    {
        if (!canLook) return;

        // 2. OVDJE UZIMAMO TRENUTNU SENZITIVNOST DIREKTNO IZ POSTAVKI!
        float currentSens = SettingsService.MouseSensitivity;

        // Množimo pomjeranje miša sa tom senzitivnosti
        yaw += Input.GetAxis("Mouse X") * currentSens;
        pitch -= Input.GetAxis("Mouse Y") * currentSens;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        if (isFirstPerson)
        {
            if (headBone != null) headBone.localScale = Vector3.zero;

            if (playerBody != null)
            {
                playerBody.rotation = Quaternion.Euler(0f, yaw, 0f);
            }

            Vector3 fpPosition = target.position + Vector3.up * firstPersonHeight;
            playerCamera.transform.position = fpPosition;
            playerCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            if (headBone != null) headBone.localScale = Vector3.one;
            
            transform.position = target.position + Vector3.up * 2.8f;
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 desiredCameraPos = transform.position - transform.forward * tpDistance;

            if (Physics.Linecast(transform.position, desiredCameraPos, out RaycastHit hit, collisionLayers))
            {
                playerCamera.transform.position = hit.point + transform.forward * 0.2f;
            }
            else
            {
                playerCamera.transform.position = desiredCameraPos;
            }

            playerCamera.transform.LookAt(transform.position);
        }
    }
}