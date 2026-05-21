using UnityEngine;

public class CupPickup : MonoBehaviour
{
    public Transform handPoint;
    public GameObject coffeeLiquid; 
    public GameObject saucer;       

    public bool isPickedUp = false;
    
    public bool isOnMachine = false;
    public bool isFilled = false;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        
        if (coffeeLiquid != null) coffeeLiquid.SetActive(false);
        if (saucer != null) saucer.SetActive(false);
    }

    public void PickUp()
    {
        isPickedUp = true;
        isOnMachine = false;

        handPoint.GetComponentInParent<ArmOverride>().isHoldingRight = true;
        transform.SetParent(handPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;
    }

    public void PutOnMachine(Transform machineSlot)
    {
        isPickedUp = false;
        isOnMachine = true;

        transform.SetParent(machineSlot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;
    }

    public void FillCoffee()
    {
        isFilled = true;
        if (coffeeLiquid != null) coffeeLiquid.SetActive(true);
    }

    public void PutOnTray(Transform traySlot)
    {
        isPickedUp = false;
        isOnMachine = false;

        transform.SetParent(traySlot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;

        if (saucer != null) saucer.SetActive(true);
    }
}