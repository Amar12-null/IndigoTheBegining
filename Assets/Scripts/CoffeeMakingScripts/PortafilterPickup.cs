using UnityEngine;

public class PortafilterPickup : MonoBehaviour
{
    public Transform handPoint;
    public GameObject coffeeVisual; 

    public bool isOnMachine = false; 
    public bool isPickedUp = false;

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

    public bool IsPickedUp()
    {
        return isPickedUp;
    }

    // Poziva se kada Mlinac napuni portafilter
    public void FillWithCoffee()
    {
        currentState = PortafilterState.GroundCoffee;
        if (coffeeVisual != null) coffeeVisual.SetActive(true); // PALIMO vizual
    }

    // NOVO: Zamjena za ReturnToHome. Ovo ćemo pozivati kada praznimo staru kafu!
    public void EmptyCoffee()
    {
        currentState = PortafilterState.Empty;
        if (coffeeVisual != null) coffeeVisual.SetActive(false); // GASIMO vizual
        Debug.Log("Portafilter ispražnjen i spreman za novu kafu!");
    }
}