using UnityEngine;
using Indigo.UI; 

public class ThirdPersonCamera : MonoBehaviour
{
    public Camera playerCamera;
    public Transform target;       
    public Transform playerBody;   
    
    [Header("Skrivanje Glave")]
    public Transform headBone;     

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

    void Start()
    {
        if (playerBody == null && target != null) 
        {
            playerBody = target.root; 
        }
    }

    void Update()
    {
        if (UIEvents.IsPaused) return; // Ne dozvoli promjenu kamere u meniju

        if (Input.GetKeyDown(KeyCode.F5))
        {
            isFirstPerson = !isFirstPerson; 
        }
    }

    void LateUpdate()
    {
        if (UIEvents.IsPaused) return; // Ne dozvoli okretanje kamere u meniju

        // Čitamo brzinu miša iz koleginog Settings menija
        float currentSens = SettingsService.MouseSensitivity;

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