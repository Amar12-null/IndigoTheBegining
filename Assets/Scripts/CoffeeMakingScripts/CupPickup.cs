using UnityEngine;

public class CupPickup : MonoBehaviour
{
    public Transform handPoint;
    public GameObject coffeeLiquid; 
    public GameObject saucer;       

    public bool isPickedUp = false;
    public bool isOnMachine = false;
    public bool isFilled = false;

    void Start()
    {
        if (coffeeLiquid != null) coffeeLiquid.SetActive(false);
        if (saucer != null) saucer.SetActive(false);
    }

   public void PickUp()
{
    Debug.Log("IsFilled status: " + isFilled);
    isPickedUp = true;
    isOnMachine = false;

    // Osiguraj referencu na ruku prije pristupa
    var arm = handPoint.GetComponentInParent<ArmOverride>();
    if (arm != null) arm.isHoldingRight = true;

    transform.SetParent(handPoint);
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;
    
    // RJEŠENJE: Ako je šoljica puna, tacna mora biti aktivna.
    // Ako želiš da tacna bude uvijek tu kad je uzmeš, koristi: saucer.SetActive(true);
    if (saucer != null) 
    {
        saucer.SetActive(isFilled); 
    }

    if (Indigo.Audio.SoundManager.Instance != null)
        Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.Pickup);
}

    public void PutOnMachine(Transform machineSlot)
    {
        isPickedUp = false;
        isOnMachine = true;

        transform.SetParent(machineSlot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
       
        // Zvuk spuštanja na mašinu
        if (Indigo.Audio.SoundManager.Instance != null)
            Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.Place);
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
    
        // OVDJE JE KLJUČ: Uvijek upali tacnu kada stavljaš na tacnu
        if (saucer != null) 
        {
            saucer.SetActive(true); 
        }

        // Zvuk spuštanja na tacnu
        if (Indigo.Audio.SoundManager.Instance != null)
            Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.Place);
    }
}