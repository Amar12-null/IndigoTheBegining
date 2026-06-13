using UnityEngine;
using Indigo.Machines; // Pristup progress traci

public class CoffeeGrinder : MonoBehaviour
{
    public Transform playerHand; // Referenca na desnu ruku
    
    [Header("Mlin Postavke")]
    public float timeToGrind = 1.5f; // Koliko sekundi držiš klik
    public Vector3 uiLocalOffset = new Vector3(0f, 1.0f, 0f); // Pozicija trake iznad mlina
    
    private float currentGrindTimer = 0f;
    private bool playerNear = false;
    private MachineProgressWorldUI _progressUI; 

    void Start()
    {
        // Automatski stvaramo traku iznad mlina na startu igre
        _progressUI = MachineProgressWorldUI.Create(transform, uiLocalOffset);
    }

    void Update()
    {
        if (playerNear)
        {
            PortafilterPickup heldPortafilter = playerHand.GetComponentInChildren<PortafilterPickup>();

            // Ako držiš portafilter i on je PRAZAN ili USED
            if (heldPortafilter != null && 
               (heldPortafilter.currentState == PortafilterPickup.PortafilterState.Empty 
                || heldPortafilter.currentState == PortafilterPickup.PortafilterState.UsedCoffee))
            {
                // NOVO: Sada provjeravamo da li igrač drži LIJEVI KLIK na mišu
                if (Input.GetMouseButton(0)) 
                {
                    currentGrindTimer += Time.deltaTime;

                    // Izračunavamo procenat i šaljemo našoj traci na ekranu
                    float fraction = currentGrindTimer / timeToGrind;
                    bool isDone = currentGrindTimer >= timeToGrind;
                    
                    if (_progressUI != null) _progressUI.UpdateProgress(fraction, isDone);

                    Debug.Log("Meljem kafu mišem... " + Mathf.RoundToInt(fraction * 100) + "%");

                    if (isDone)
                    {
                        // Pozivamo funkciju koja mijenja state i PALI vizual
                        heldPortafilter.FillWithCoffee();
                        
                        currentGrindTimer = 0f;
                        Debug.Log("Kafa spremna! Nosi na aparat.");
                        
                        // Gasimo traku nakon uspjeha
                        if (_progressUI != null) _progressUI.UpdateProgress(0f);
                    }
                }
                // NOVO: Ako igrač pusti lijevi klik miša
                else if (Input.GetMouseButtonUp(0))
                {
                    currentGrindTimer = 0f; // Resetujemo progres ako pustiš prerano
                    
                    // Sakrij traku
                    if (_progressUI != null) _progressUI.UpdateProgress(0f);
                }
            }
            else
            {
                // Sigurnosni reset ako portafilter više nije u ispravnom stanju
                if (currentGrindTimer > 0f)
                {
                    currentGrindTimer = 0f;
                    if (_progressUI != null) _progressUI.UpdateProgress(0f);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerNear = false;
            
            // Ako igrač pobjegne od mlina dok melje, resetujemo progres i sklanjamo traku
            currentGrindTimer = 0f;
            if (_progressUI != null) _progressUI.UpdateProgress(0f);
        }
    }
}