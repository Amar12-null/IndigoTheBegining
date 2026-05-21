using UnityEngine;

public class ServingTray : MonoBehaviour
{
    public Transform[] traySlots; 
    public Transform playerHand;       // Desna ruka (za provjeru držiš li kafu)
    public Transform leftHandPoint;    // NOVO: Lijeva ruka (gdje ide tacna)
    public Transform trayHomePosition; // NOVO: Mjesto na stolu gdje tacna stoji

    public bool isPickedUp = false;
    private bool playerNear = false;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
{
    if (playerNear && Input.GetKeyDown(KeyCode.E))
    {
        CupPickup heldCup = playerHand.GetComponentInChildren<CupPickup>();
        PortafilterPickup heldPortafilter = playerHand.GetComponentInChildren<PortafilterPickup>();

        // PRIORITET 1: Ako držimo PUNU kafu, probaj je staviti na tacnu
        if (heldCup != null && heldCup.isFilled)
        {
            Transform emptySlot = null;
            foreach (Transform slot in traySlots)
            {
                if (slot.childCount == 0) // Nađi prazno mjesto
                {
                    emptySlot = slot;
                    
                    break; 
                }
            }

            if (emptySlot != null)
            {
                heldCup.PutOnTray(emptySlot);
                playerHand.GetComponentInParent<ArmOverride>().isHoldingRight = false;
                
                
                
                Debug.Log("Kafa je servirana na tacnu!");
            }
            else
            {
                Debug.Log("Nema više mjesta na tacni!");
            }
        }
        // PRIORITET 2: Ako već DRŽIMO tacnu, vrati je na sto
        else if (isPickedUp)
        {
            PutDownTray();
        }
        // PRIORITET 3: Ako držimo PRAZNU šolju ili NIŠTA, pokupi tacnu
        else if (!isPickedUp && heldPortafilter == null && (heldCup == null || !heldCup.isFilled))
        {
            PickUpTray();
        }
    }
}

    void PickUpTray()
    {
        isPickedUp = true;
        
        transform.SetParent(leftHandPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;

        // Kažemo ArmOverride skripti da podigne LIJEVU ruku!
        ArmOverride arm = playerHand.GetComponentInParent<ArmOverride>();
        if (arm != null) arm.isHoldingLeft = true;
    }

    void PutDownTray()
    {
        isPickedUp = false;
        
        transform.SetParent(trayHomePosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;

        // Spuštamo lijevu ruku
        leftHandPoint.GetComponentInParent<ArmOverride>().isHoldingLeft = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerNear = false;
    }
}