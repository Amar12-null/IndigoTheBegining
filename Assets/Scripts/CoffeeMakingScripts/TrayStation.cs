using UnityEngine;

public class TrayStation : MonoBehaviour
{
    [Header("Poveznica sa tacnom")]
    public ServingTray servingTrayScript; // Ovdje uvuci tacnu iz hijerarhije

    private bool isPlayerNear = false;

    private void OnTriggerEnter(Collider other)
    {
        // Provjeravamo da li je Player ušao u Box Collider
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
        }
    }

    private void Update()
    {
        // Ako je igrač blizu stola i pritisne E, pozivamo logiku sa tacne
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            // --- NOVO: PROVJERA DA LI JE TACNA KUPLJENA ---
            if (!Indigo.Shop.Upgrades.IsOwned(Indigo.Shop.UpgradeId.ServingTray))
            {
                // Ispisujemo igraču da prvo mora kupiti tacnu u shopu
                Indigo.UI.UIEvents.SetPrompt("", "Morate kupiti tacnu u Management Station-u!");
                return; // Prekidamo dalje izvršavanje
            }

            if (servingTrayScript != null)
            {
                servingTrayScript.InteractWithTray();
            }
        }
    }
}