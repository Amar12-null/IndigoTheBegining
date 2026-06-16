using UnityEngine;

public class TrayStation : MonoBehaviour
{
    [Header("Poveznica sa tacnom")]
    public ServingTray servingTrayScript;

    private bool isPlayerNear = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            // Čistimo UI sa ekrana čim se odmakneš od stola!
            Indigo.UI.UIEvents.SetPrompt("", "");
        }
    }

    private void Update()
    {
        if (isPlayerNear && servingTrayScript != null)
        {
            // --- 1. TUTORIJAL UI PRIKAZ ---
            if (Indigo.Settings.TutorialSettings.ShowTutorial)
            {
                // Provjera za shop
                if (!Indigo.Shop.Upgrades.IsOwned(Indigo.Shop.UpgradeId.ServingTray))
                {
                    Indigo.UI.UIEvents.SetPrompt("", "Tacna je zaključana (Kupi u Shopu)");
                }
                else
                {
                    CupPickup heldCup = servingTrayScript.playerHand.GetComponentInChildren<CupPickup>();
                    PortafilterPickup heldPortafilter = servingTrayScript.playerHand.GetComponentInChildren<PortafilterPickup>();

                    if (heldCup != null && heldCup.isFilled)
                    {
                        Indigo.UI.UIEvents.SetPrompt("E", "Stavi kafu na tacnu");
                    }
                    else if (servingTrayScript.isPickedUp)
                    {
                        Indigo.UI.UIEvents.SetPrompt("E", "Vrati tacnu na sto");
                    }
                    else if (!servingTrayScript.isPickedUp && heldPortafilter == null && (heldCup == null || !heldCup.isFilled))
                    {
                        Indigo.UI.UIEvents.SetPrompt("E", "Uzmi tacnu");
                    }
                    else
                    {
                        Indigo.UI.UIEvents.SetPrompt("", ""); // Sakrij ako nema akcije
                    }
                }
            }

            // --- 2. LOGIKA ZA KLIK NA 'E' ---
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Blokada ako nije kupljeno
                if (!Indigo.Shop.Upgrades.IsOwned(Indigo.Shop.UpgradeId.ServingTray))
                {
                    return; 
                }

                servingTrayScript.InteractWithTray();
            }
        }
    }
}