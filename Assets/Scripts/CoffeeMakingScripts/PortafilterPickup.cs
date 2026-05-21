using UnityEngine;

public class PortafilterPickup : MonoBehaviour
{
    public Transform handPoint;
    public Transform homePosition;
    public GameObject coffeeVisual; 

    public bool isOnMachine = false; 
    public bool isPickedUp = false;

    // Varijabla koja čuva početnu veličinu collidera i modela
    private Vector3 originalScale;

   public enum PortafilterState
    {
        Empty,         
        GroundCoffee, 
        CoffeeReady,   
        UsedCoffee     
    }

    public PortafilterState currentState = PortafilterState.Empty; 

    void Start()
    {
        // Zapamtimo originalnu veličinu čim se igra pokrene
        originalScale = transform.localScale;
    }

    // Promijenjeno u public da bi PortafilterHome mogao pozvati ovu metodu
    public void PickUp()
    {
        if (isOnMachine) return;

        isPickedUp = true;
        
        handPoint.GetComponentInParent<ArmOverride>().isHoldingRight = true;
        transform.SetParent(handPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale; // Zadržavamo originalnu veličinu
    }

    public void PickUpFromMachine()
    {
        isPickedUp = true;
        isOnMachine = false;

        transform.SetParent(handPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;
    }

    public bool IsPickedUp()
    {
        return isPickedUp;
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

   public void ReturnToHome()
    {
        isPickedUp = false;
        isOnMachine = false;

        transform.SetParent(homePosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale; 

        // Resetujemo na Empty i GASIMO vizual
        currentState = PortafilterState.Empty;
        handPoint.GetComponentInParent<ArmOverride>().isHoldingRight = false;
        if (coffeeVisual != null) coffeeVisual.SetActive(false); 
        
        Debug.Log("Portafilter resetovan na Empty!");
    }

    // NOVO: Ovu metodu će pozvati Mlinac kada napuni portafilter
    public void FillWithCoffee()
    {
        currentState = PortafilterState.GroundCoffee;
        if (coffeeVisual != null) coffeeVisual.SetActive(true); // PALIMO vizual
    }
}