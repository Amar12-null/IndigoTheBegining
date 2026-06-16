using UnityEngine;

public class ServingTray : MonoBehaviour
{
    public Transform[] traySlots; 
    public Transform playerHand;       
    public Transform leftHandPoint;    
    public Transform trayHomePosition; 

    public bool isPickedUp = false;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        
        if (isPickedUp)
        {
            CupPickup heldCup = playerHand.GetComponentInChildren<CupPickup>();
            bool hasCups = HasCupsOnTray();
            bool isFull = IsTrayFull();

            // DESNI KLIK (M2) za žongliranje kafama u hodu
            if (Input.GetMouseButtonDown(1)) 
            {
                // Ako ti je ruka prazna, a na tacni ima kafa -> Izvlači kafu u desnu ruku
                if (heldCup == null && hasCups)
                {
                    TakeCupFromTrayToHand();
                }
                // Ako držiš kafu, a tacna nije puna -> Vrati je na tacnu
                else if (heldCup != null && !isFull)
                {
                    PutCupFromHandToTray(heldCup);
                }
            }
            
            // Tutorijal UI (Prikazujemo ga samo kad nisi blizu stola, da se ne sudara sa TrayStation tekstom)
            bool isNearTable = Vector3.Distance(playerHand.position, trayHomePosition.position) < 2.5f;
            if (!isNearTable && Indigo.Settings.TutorialSettings.ShowTutorial)
            {
                if (heldCup == null && hasCups)
                    Indigo.UI.UIEvents.SetPrompt("M2", "Uzmi kafu sa tacne");
                else if (heldCup != null && !isFull)
                    Indigo.UI.UIEvents.SetPrompt("M2", "Vrati kafu na tacnu");
                else
                    Indigo.UI.UIEvents.SetPrompt("", ""); 
            }
        }
    }

    // Ovu metodu i dalje zove TrayStation kada pritisneš E pored stola
    public void InteractWithTray()
    {
        CupPickup heldCup = playerHand.GetComponentInChildren<CupPickup>();
        PortafilterPickup heldPortafilter = playerHand.GetComponentInChildren<PortafilterPickup>();

        if (heldCup != null && heldCup.isFilled && !IsTrayFull())
        {
            PutCupFromHandToTray(heldCup); 
        }
        else if (isPickedUp)
        {
            PutDownTray();
        }
        else if (!isPickedUp && heldPortafilter == null && (heldCup == null || !heldCup.isFilled))
        {
            PickUpTray();
        }
    }

    // --- POMOĆNE METODE ---

    bool HasCupsOnTray()
    {
        foreach (Transform slot in traySlots)
            if (slot.childCount > 0) return true;
        return false;
    }

    bool IsTrayFull()
    {
        foreach (Transform slot in traySlots)
            if (slot.childCount == 0) return false;
        return true;
    }

    void TakeCupFromTrayToHand()
    {
        // Idemo unazad kroz slotove da uzmemo zadnju stavljenu kafu (LIFO sistem)
        for (int i = traySlots.Length - 1; i >= 0; i--)
        {
            if (traySlots[i].childCount > 0)
            {
                CupPickup cup = traySlots[i].GetComponentInChildren<CupPickup>();
                if (cup != null)
                {
                    cup.handPoint = playerHand; 
                    cup.PickUp(); 
                    
                    if (Indigo.Audio.SoundManager.Instance != null)
                        Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.Pickup);
                    break; 
                }
            }
        }
    }

    void PutCupFromHandToTray(CupPickup heldCup)
    {
        foreach (Transform slot in traySlots)
        {
            if (slot.childCount == 0) // Nalazimo prvo prazno mjesto
            {
                heldCup.PutOnTray(slot);
                
                ArmOverride arm = playerHand.GetComponentInParent<ArmOverride>();
                if (arm != null) arm.isHoldingRight = false; 
                
                if (Indigo.Audio.SoundManager.Instance != null)
                    Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.Place);
                    
                break;
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

        ArmOverride arm = playerHand.GetComponentInParent<ArmOverride>();
        if (arm != null) arm.isHoldingLeft = true;

        if (Indigo.Audio.SoundManager.Instance != null)
            Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.Pickup);
    }

    void PutDownTray()
    {
        isPickedUp = false;
        
        transform.SetParent(trayHomePosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;

        ArmOverride arm = leftHandPoint.GetComponentInParent<ArmOverride>();
        if (arm != null) arm.isHoldingLeft = false;

        if (Indigo.Audio.SoundManager.Instance != null)
            Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.Place);
            
        
        Indigo.UI.UIEvents.SetPrompt("", "");
    }
}