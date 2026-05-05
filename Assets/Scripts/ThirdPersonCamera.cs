using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Camera playerCamera;
    public Transform target;

    public float mouseSensitivity = 2f;
    public float minY = -30f;
    public float maxY = 60f;

    public float distance = 7f;
    public float minDistance = 1.5f;
    public float maxDistance = 6f;
    public float zoomSpeed = 3f;

    public LayerMask collisionLayers;

    private float yaw;
    private float pitch;

    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        transform.position = target.position + Vector3.up * 2.8f;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 desiredCameraPos = transform.position - transform.forward * distance;

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