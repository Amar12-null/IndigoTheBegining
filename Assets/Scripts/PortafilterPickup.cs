using UnityEngine;

public class PortafilterPickup : MonoBehaviour
{
    public Transform handPoint;

    private bool isOnMachine = false;
    private bool playerNear = false;
    private bool isPickedUp = false;
    private float lastActionTime = 0f;
    public Transform homePosition;

    public enum PortafilterState
{
    Empty,
    Filled,
    Used
}

public PortafilterState currentState = PortafilterState.Empty;

void Update()
{
    if (playerNear && Input.GetKeyDown(KeyCode.E) && 
        !isPickedUp && !isOnMachine && Time.time - lastActionTime > 0.5f)
    {
        PickUp();
    }
}


 void PickUp()
{
    if (isOnMachine) return;

    isPickedUp = true;
     

    transform.SetParent(handPoint);
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;

    Collider col = GetComponent<Collider>();
    if (col != null)
        col.enabled = false;
}
public void PickUpFromMachine()
{
    isPickedUp = true;
    isOnMachine = false;

    transform.SetParent(handPoint);
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;
}

    public bool IsPickedUp()
    {
        return isPickedUp;
    }

 public void PutOnMachine(Transform machineSlot)
{
    isPickedUp = false;
    isOnMachine = true;
    playerNear = false;

    lastActionTime = Time.time;

    transform.SetParent(machineSlot);
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;

    Collider col = GetComponent<Collider>();
    if (col != null)
        col.enabled = true;

    Debug.Log("Portafilter parent: " + transform.parent.name);
}

public void ReturnToHome()
{
    isPickedUp = false;
    isOnMachine = false;

    transform.SetParent(homePosition);
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;

    Collider col = GetComponent<Collider>();
    if (col != null)
        col.enabled = true;
}






    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}