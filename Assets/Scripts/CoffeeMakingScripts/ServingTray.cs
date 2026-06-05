using UnityEngine;

public class ServingTray : MonoBehaviour
{
    public Transform[] traySlots; 
    public Transform playerHand;       // Desna ruka (za provjeru držiš li kafu)
    public Transform leftHandPoint;    // Lijeva ruka (gdje ide tacna)
    public Transform trayHomePosition; // Mjesto na stolu gdje tacna stoji

    public bool isPickedUp = false;
    private Vector3 originalScale;

    void Start()
    {
        // Pamtimo originalnu veličinu tacne kako se ne bi poremetila
        originalScale = transform.localScale;
    }

    // Ovu metodu sada poziva nova TrayStation skripta kada igrač pritisne 'E'
    public void InteractWithTray()
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
                
                // Oslobađamo desnu ruku (ugasimo override) jer smo spustili kafu iz nje
                ArmOverride arm = playerHand.GetComponentInParent<ArmOverride>();
                if (arm != null) arm.isHoldingRight = false;
                
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

    void PickUpTray()
    {
        isPickedUp = true;
        
        // Postavljamo tacnu u lijevu ruku
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
        
        // Vraćamo tacnu na home poziciju (sto)
        transform.SetParent(trayHomePosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;

        // Spuštamo lijevu ruku (gasimo override za tacnu)
        ArmOverride arm = leftHandPoint.GetComponentInParent<ArmOverride>();
        if (arm != null) arm.isHoldingLeft = false;
    }
}