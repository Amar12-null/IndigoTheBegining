using UnityEngine;

public class CoffeeMachineSlot : MonoBehaviour
{
    [Header("Portafilter Postavke")]
    public PortafilterPickup portafilter; // Trenutni portafilter na aparatu (skripta ga sama popunjava)
    public Transform portafilterSlot;     // NOVO: Mjesto (prazan objekat) gdje se portafilter kači na aparat
    
    [Header("Multi-Cup Postavke")]
    public Transform[] cupSlots;         
    
    public Transform playerHand;         
    public float brewTime = 3f;          
    
    private float currentBrewTimer = 0f;
    private bool playerNear = false;

    void Update()
    {
        if (!playerNear) return;

        // 1. POSTAVLJANJE I UZIMANJE STVARI (Tipka E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            CupPickup heldCup = playerHand.GetComponentInChildren<CupPickup>();
            PortafilterPickup heldPortafilter = playerHand.GetComponentInChildren<PortafilterPickup>();
                ArmOverride arm = playerHand.GetComponentInParent<ArmOverride>();

            // Slučaj A: Držiš PORTAFILTER -> Stavi ga na aparat
            
            if (heldPortafilter != null && portafilter == null)
            {
                portafilter = heldPortafilter; 
                
                portafilter.transform.SetParent(portafilterSlot);
                portafilter.transform.localPosition = Vector3.zero;
                portafilter.transform.localRotation = Quaternion.identity;
                portafilter.isOnMachine = true;
                
                // NOVO: Moramo reći igri da ga više NE DRŽIŠ!
                portafilter.isPickedUp = false; 
                
                playerHand.GetComponentInParent<ArmOverride>().isHoldingRight = false; 
                Debug.Log("Portafilter postavljen na aparat!");
            }
            // Slučaj B: Držiš ŠOLJICU -> Stavi je na aparat
            else if (heldCup != null)
            {
                Transform emptySlot = GetEmptyCupSlot();
                if (emptySlot != null)
                {
                    heldCup.transform.SetParent(emptySlot);
                    heldCup.transform.localPosition = Vector3.zero;
                    heldCup.transform.localRotation = Quaternion.identity;
                    
                    playerHand.GetComponentInParent<ArmOverride>().isHoldingRight = false;
                    Debug.Log("Šoljica stavljena na aparat!");
                }
                else
                {
                    Debug.Log("Nema mjesta za još šoljica!");
                }
            }
            // Slučaj C: RUKE SU PRAZNE -> Uzmi nešto sa aparata
            else if (heldCup == null && heldPortafilter == null)
            {
                CupPickup readyCup = GetReadyCup();
                
                // Prioritet 1: Uzmi kafu ako je ima
                if (readyCup != null)
                {
                    readyCup.transform.SetParent(playerHand);
                    readyCup.transform.localPosition = Vector3.zero;
                    readyCup.transform.localRotation = Quaternion.identity;

                    if (arm != null) arm.isHoldingRight = true;
                    Debug.Log("Uzeta kafa sa aparata!");
                }
                // Prioritet 2: Ako nema kafe za uzeti, skini portafilter
               // Prioritet 2: Ako nema kafe za uzeti, skini portafilter
                else if (portafilter != null)
                {
                    portafilter.transform.SetParent(playerHand);
                    portafilter.transform.localPosition = Vector3.zero;
                    portafilter.transform.localRotation = Quaternion.identity;
                    portafilter.isOnMachine = false;
                    
                    // NOVO: Igra mora znati da si ga OPET UZEO!
                    portafilter.isPickedUp = true; 
                    
                    if (arm != null) arm.isHoldingRight = true;
                    
                    portafilter = null; 
                    Debug.Log("Portafilter skinut sa aparata!");
                }
            }
        }

        // 2. LOGIKA KUHANJA KAFE (Držanje lijevog klika)
        if (portafilter != null && portafilter.currentState == PortafilterPickup.PortafilterState.GroundCoffee && HasUnfilledCup())
        {
            if (Input.GetMouseButton(0)) 
            {
                currentBrewTimer += Time.deltaTime;
                Debug.Log("Kuham kafu... " + Mathf.RoundToInt((currentBrewTimer / brewTime) * 100) + "%");

                if (currentBrewTimer >= brewTime)
                {
                    FillAllCups();
                    
                    portafilter.currentState = PortafilterPickup.PortafilterState.UsedCoffee;
                    if (portafilter.coffeeVisual != null) portafilter.coffeeVisual.SetActive(false);

                    currentBrewTimer = 0f;
                    Debug.Log("Kafa uspješno skuhana!");
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                currentBrewTimer = 0f; 
            }
        }
    }

    private Transform GetEmptyCupSlot()
    {
        foreach (Transform slot in cupSlots)
            if (slot.childCount == 0) return slot;
        return null;
    }

    private CupPickup GetReadyCup()
    {
        foreach (Transform slot in cupSlots)
        {
            if (slot.childCount > 0)
            {
                CupPickup cup = slot.GetComponentInChildren<CupPickup>();
                if (cup != null && cup.isFilled) return cup;
            }
        }
        foreach (Transform slot in cupSlots)
        {
            if (slot.childCount > 0) return slot.GetComponentInChildren<CupPickup>();
        }
        return null;
    }

    private bool HasUnfilledCup()
    {
        foreach (Transform slot in cupSlots)
        {
            if (slot.childCount > 0)
            {
                CupPickup cup = slot.GetComponentInChildren<CupPickup>();
                if (cup != null && !cup.isFilled) return true;
            }
        }
        return false;
    }

    private void FillAllCups()
    {
        foreach (Transform slot in cupSlots)
        {
            if (slot.childCount > 0)
            {
                CupPickup cup = slot.GetComponentInChildren<CupPickup>();
                if (cup != null && !cup.isFilled)
                {
                    cup.isFilled = true;
                    if (cup.coffeeLiquid != null) cup.coffeeLiquid.SetActive(true); 
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) playerNear = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) playerNear = false; }
}