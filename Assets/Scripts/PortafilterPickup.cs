using UnityEngine;
using Indigo.UI;

public class PortafilterPickup : MonoBehaviour
{
    public Transform handPoint;
    public string itemName = "Portafilter";

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

    UIEvents.SetHeldItem(itemName, currentState.ToString());
    UIEvents.ClearPrompt();
}
public void PickUpFromMachine()
{
    isPickedUp = true;
    isOnMachine = false;

    transform.SetParent(handPoint);
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;

    UIEvents.SetHeldItem(itemName, currentState.ToString());
    UIEvents.ClearPrompt();
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

    UIEvents.ClearHeldItem();
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

    UIEvents.ClearHeldItem();
}






    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            if (!isPickedUp && !isOnMachine)
                UIEvents.SetPrompt("E", "Pick up " + itemName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            UIEvents.ClearPrompt();
        }
    }
}