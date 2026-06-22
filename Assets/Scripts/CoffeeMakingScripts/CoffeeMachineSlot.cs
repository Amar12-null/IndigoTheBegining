using UnityEngine;
using Indigo.Machines; // Pristup progress traci
using Indigo.Integration; // Pristup DrinkSelectorStation


public class CoffeeMachineSlot : MonoBehaviour
{
    [Header("UI Integracija")]
    public Indigo.Integration.DrinkSelectorStation drinkSelector;
    [Header("Portafilter Postavke")]
    public PortafilterPickup portafilter; 
    public Transform portafilterSlot;     
    
    [Header("Multi-Cup Postavke")]
    public Transform[] cupSlots;         
    
    [Header("Player & UI Postavke")]
    public Transform playerHand;         
    public float brewTime = 3f;          
    public Vector3 uiLocalOffset = new Vector3(0f, 1.2f, 0f); 
    
    [Header("Audio")]
    public AudioSource machineAudio;
    
    private float currentBrewTimer = 0f;
    private bool playerNear = false;
    private bool _isBrewing = false; 
    private MachineProgressWorldUI _progressUI; 

    void Start()
    {
        _progressUI = MachineProgressWorldUI.Create(transform, uiLocalOffset);
    }

    void Update()
    {
        // Ako igrač ode predaleko, aparat i dalje kuva sam dok ne završi!

        // 1. POSTAVLJANJE I UZIMANJE STVARI (Tipka E) - Blokirano ako aparat kuva!
        if (Input.GetKeyDown(KeyCode.E) && playerNear && !_isBrewing)
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
                    
                    // Javljamo šoljici da je više ne držimo
                    heldCup.isPickedUp = false; 
                    
                    playerHand.GetComponentInParent<ArmOverride>().isHoldingRight = false;
                    Debug.Log("Šoljica stavljena na aparat i isPickedUp je isključen!");
                }
            }
            // Slučaj C: RUKE SU PRAZNE -> Uzmi nešto sa aparata
            else if (heldCup == null && heldPortafilter == null)
            {
                CupPickup readyCup = GetReadyCup();
                
                // Uzmi kafu ako je ima
                if (readyCup != null)
                {
                    readyCup.transform.SetParent(playerHand);
                    readyCup.transform.localPosition = Vector3.zero;
                    readyCup.transform.localRotation = Quaternion.identity;

                    // Javljamo šoljici da smo je upravo podigli
                    readyCup.isPickedUp = true;

                    if (arm != null) arm.isHoldingRight = true;
                    Debug.Log("Uzeta kafa sa aparata i isPickedUp je uključen!");
                    
                    if (_progressUI != null) _progressUI.UpdateProgress(0f);
                }
                // Ako nema kafe, skini portafilter
                else if (portafilter != null)
                {
                    portafilter.transform.SetParent(playerHand);
                    portafilter.transform.localPosition = Vector3.zero;
                    portafilter.transform.localRotation = Quaternion.identity;
                    portafilter.isOnMachine = false;
                    portafilter.isPickedUp = true; 
                    
                    if (arm != null) arm.isHoldingRight = true;
                    
                    portafilter = null; 
                    Debug.Log("Portafilter skinut sa aparata!");
                    if (_progressUI != null) _progressUI.UpdateProgress(0f);
                }
            }
        }

       
        // 2. LOGIKA OKIDANJA KUVANJA (Samo jedan klik mišem, ne držanje!)
        if (playerNear && portafilter != null && portafilter.currentState == PortafilterPickup.PortafilterState.GroundCoffee && HasUnfilledCup())
        {
            if (Input.GetMouseButtonDown(0) && !_isBrewing) 
            {
                _isBrewing = true;
                currentBrewTimer = 0f;

                // PALJENJE ZVUKA I PITCH-a ---
                if (machineAudio != null) 
                {
                    // Ako imamo brže kuvanje, ubrzaj i zvuk duplo!
                    if (Indigo.Shop.Upgrades.IsOwned(Indigo.Shop.UpgradeId.FastBrew))
                        machineAudio.pitch = 2f; // Turbo zvuk!
                    else
                        machineAudio.pitch = 1f; // Normalan zvuk
                        
                    machineAudio.Play();
                }

                Debug.Log("Kuvanje pokrenuto na jedan klik!");
            }
        }

        
        // 3. AUTOMATSKO KUVANJE (Aparat radi sam u pozadini)
        if (_isBrewing)
        {
            currentBrewTimer += Time.deltaTime;
            
           
            // Ako ima upgrade FastBrew, vrijeme kuvanja je duplo brže (brewTime * 0.5f)
            float actualBrewTime = Indigo.Shop.Upgrades.IsOwned(Indigo.Shop.UpgradeId.FastBrew) ? brewTime * 0.5f : brewTime;
            
            float fraction = currentBrewTimer / actualBrewTime;
            bool isDone = currentBrewTimer >= actualBrewTime;
            
            if (_progressUI != null) _progressUI.UpdateProgress(fraction, isDone);

            if (isDone)
            {
                FillAllCups();
                
                portafilter.currentState = PortafilterPickup.PortafilterState.UsedCoffee;
                if (portafilter.coffeeVisual != null) portafilter.coffeeVisual.SetActive(false);

                _isBrewing = false; 
                
                
                // Sada će zvučnik prirodno odsvirati tvoj "ding" na kraju i sam se ugasiti.
                
                Debug.Log("Aparat završio! Kafa skuvana!");
            }
        }
        // Na kraju Update funkcije pozivamo provjeru za UI
        if (playerNear)
        {
            if (_isBrewing)
            {
                // Dok aparat radi, ispisujemo da se kafa kuva
                Indigo.UI.UIEvents.SetPrompt("", "Aparat kuva...");
            }
            else
            {
                UpdateTutorialUI();
            }
        }
    }

    private void UpdateTutorialUI()
    {
        if (!Indigo.Settings.TutorialSettings.ShowTutorial) return;

        CupPickup heldCup = playerHand.GetComponentInChildren<CupPickup>();
        PortafilterPickup heldPortafilter = playerHand.GetComponentInChildren<PortafilterPickup>();
        
        // 1. Ako je kafa gotova i stoji na aparatu
        if (GetReadyCup() != null)
        {
            Indigo.UI.UIEvents.SetPrompt("E", "Uzmi gotovu kafu");
            return;
        }
        
        // 2. Ako držiš pun portafilter
        if (heldPortafilter != null && portafilter == null)
        {
            Indigo.UI.UIEvents.SetPrompt("E", "Stavi portafilter na aparat");
            return;
        }

        // 3. Ako držiš šoljicu
        if (heldCup != null && GetEmptyCupSlot() != null)
        {
            Indigo.UI.UIEvents.SetPrompt("E", "Stavi šoljicu");
            return;
        }

        // 4. Ako je portafilter na aparatu, pun je stare kafe, a ruke su ti prazne
        if (heldCup == null && heldPortafilter == null && portafilter != null && 
           (portafilter.currentState == PortafilterPickup.PortafilterState.UsedCoffee || portafilter.currentState == PortafilterPickup.PortafilterState.Empty))
        {
            Indigo.UI.UIEvents.SetPrompt("E", "Uzmi portafilter");
            return;
        }

       // 5. Ako je sve spremno za kuvanje (Portafilter ima svježu kafu)
        if (portafilter != null && portafilter.currentState == PortafilterPickup.PortafilterState.GroundCoffee)
        {
            if (HasUnfilledCup())
            {
                // Čitamo šta je izabrano iz one druge skripte i ispisujemo to na ekran!
                string izabranaKafa = (drinkSelector != null) ? drinkSelector.Selected.ToString() : "Kafu";
                
                Indigo.UI.UIEvents.SetPrompt("M1", $"Pravi: {izabranaKafa} (1-4 za promjenu)");
            }
            else
            {
                Indigo.UI.UIEvents.SetPrompt("", "Fali šoljica!");
            }
            return;
        }

        // Ako nijedan uslov nije ispunjen, sakrij UI
        Indigo.UI.UIEvents.SetPrompt("", "");
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
                // Sada vraćamo šolju SAMO ako je zaista puna!
                if (cup != null && cup.isFilled) return cup;
            }
        }
        // Obrisali smo drugi loop koji je vraćao praznu šolju!
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

                    // AUTOMATSKO PROSLJEĐIVANJE TIPA KAFE ---
                    // Ako portafilter ima definisan tip kafe, prepisujemo ga direktno u šolju
                    var bridge = cup.GetComponent<CupOrderBridge>();
                    if (bridge != null && portafilter != null)
                    {
                        // Ako tvoj portafilter ima npr. varijablu 'targetOrder' ili slično, stavi je ovdje.
                        // Za sada, ako nemaš, koristićemo selektor ako postoji, ili automatski prepoznati.
                        var selector = GameObject.FindAnyObjectByType<Indigo.Integration.DrinkSelectorStation>();
                        if (selector != null)
                        {
                            bridge.AssignDrinkType(selector.Selected);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) playerNear = true; }
    private void OnTriggerExit(Collider other) 
    { 
        if (other.CompareTag("Player")) 
        {
            playerNear = false;
            Indigo.UI.UIEvents.SetPrompt("", ""); // Sakrij tekst kad odeš od mašine
        }
    }
}