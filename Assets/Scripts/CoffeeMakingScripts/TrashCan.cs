using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public Transform playerHand; 

    private bool playerNear = false;

    void Update()
    {
        if (playerNear)
        {
            CupPickup heldCup = playerHand.GetComponentInChildren<CupPickup>();

            // --- UI TUTORIJAL ---
            if (Indigo.Settings.TutorialSettings.ShowTutorial)
            {
                if (heldCup != null)
                    Indigo.UI.UIEvents.SetPrompt("Q", "Baci u smeće");
                else
                    Indigo.UI.UIEvents.SetPrompt("", "");
            }

            // --- LOGIKA BACANJA ---
            if (Input.GetKeyDown(KeyCode.Q) && heldCup != null)
            {
                // Zvuk bacanja u smeće (koristimo Drop zvuk)
                if (Indigo.Audio.SoundManager.Instance != null)
                    Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.Drop);

                Destroy(heldCup.gameObject); 
                playerHand.GetComponentInParent<ArmOverride>().isHoldingRight = false; 
                
                Indigo.UI.UIEvents.SetPrompt("", ""); // Odmah gasi UI
                Debug.Log("Šolja je bačena u smeće!");
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
            // Čistimo ekran kad se odmaknemo
            Indigo.UI.UIEvents.SetPrompt("", ""); 
        }
    }
}