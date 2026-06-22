using UnityEngine;
using Indigo.Machines; 

public class CoffeeGrinder : MonoBehaviour
{
    public Transform playerHand; 
    
    [Header("Mlin Postavke")]
    public float timeToGrind = 1.5f; 
    public Vector3 uiLocalOffset = new Vector3(0f, 1.0f, 0f); 
    
  
    [Header("Audio")]
    public AudioSource grinderAudio;
    
    private float currentGrindTimer = 0f;
    private bool playerNear = false;
    private MachineProgressWorldUI _progressUI; 

    void Start()
    {
        _progressUI = MachineProgressWorldUI.Create(transform, uiLocalOffset);
    }

    void Update()
    {
        if (playerNear)
        {
            PortafilterPickup heldPortafilter = playerHand.GetComponentInChildren<PortafilterPickup>();

            if (heldPortafilter != null && 
               (heldPortafilter.currentState == PortafilterPickup.PortafilterState.Empty 
                || heldPortafilter.currentState == PortafilterPickup.PortafilterState.UsedCoffee))
            {
                if (Input.GetMouseButton(0)) 
                {
                   
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (grinderAudio != null) 
                        {
                            // Ubrzavamo zvuk ako imamo FastGrind upgrade
                            grinderAudio.pitch = Indigo.Shop.Upgrades.IsOwned(Indigo.Shop.UpgradeId.FastGrind) ? 2f : 1f;
                            grinderAudio.Play();
                        }
                    }

                    currentGrindTimer += Time.deltaTime;

                    // Provjera brzine na osnovu upgrade-a
                    float actualGrindTime = Indigo.Shop.Upgrades.IsOwned(Indigo.Shop.UpgradeId.FastGrind) ? timeToGrind * 0.5f : timeToGrind;
                    
                    float fraction = currentGrindTimer / actualGrindTime;
                    bool isDone = currentGrindTimer >= actualGrindTime;
                    
                    if (_progressUI != null) _progressUI.UpdateProgress(fraction, isDone);

                    if (isDone)
                    {
                        heldPortafilter.FillWithCoffee();
                        currentGrindTimer = 0f;
                        if (_progressUI != null) _progressUI.UpdateProgress(0f);
                        
                       
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    currentGrindTimer = 0f; 
                    if (_progressUI != null) _progressUI.UpdateProgress(0f);
                    
                   
                    if (grinderAudio != null) grinderAudio.Stop();
                }
            }
            else
            {
                // Sigurnosni reset
                if (currentGrindTimer > 0f)
                {
                    currentGrindTimer = 0f;
                    if (_progressUI != null) _progressUI.UpdateProgress(0f);
                    if (grinderAudio != null) grinderAudio.Stop();
                }
            }
                // --- UI TUTORIJAL ZA MLIN ---
            if (Indigo.Settings.TutorialSettings.ShowTutorial && currentGrindTimer == 0f)
            {
                if (heldPortafilter != null && 
                   (heldPortafilter.currentState == PortafilterPickup.PortafilterState.Empty || 
                    heldPortafilter.currentState == PortafilterPickup.PortafilterState.UsedCoffee))
                {
                    
                    Indigo.UI.UIEvents.SetPrompt("M1", "Samelji kafu");
                }
                else
                {
                    Indigo.UI.UIEvents.SetPrompt("", "");
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
            currentGrindTimer = 0f;
            if (_progressUI != null) _progressUI.UpdateProgress(0f);
            
            
            if (grinderAudio != null) grinderAudio.Stop();
        Indigo.UI.UIEvents.SetPrompt("", "");
        }
    }
}