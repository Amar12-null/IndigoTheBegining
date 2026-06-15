using UnityEngine;

public class CupSpawner : MonoBehaviour
{
    public GameObject cupPrefab;     
    public Transform spawnPoint;     
    public Transform playerHand;     

    private GameObject currentCupOnTable;
    private bool playerNear = false;

    void Start()
    {
        SpawnNewCup(); 
    }

    void Update()
    {
        CupPickup cupInHand = playerHand.GetComponentInChildren<CupPickup>();
        PortafilterPickup portafilterInHand = playerHand.GetComponentInChildren<PortafilterPickup>();
        
        // Znamo da su nam ruke prazne ako nemamo ni šolju ni portafilter
        bool handsEmpty = cupInHand == null && portafilterInHand == null;

        // 1. FIZIČKO UZIMANJE ŠOLJICE
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (handsEmpty && currentCupOnTable != null)
            {
                CupPickup cupScript = currentCupOnTable.GetComponent<CupPickup>();
                cupScript.handPoint = playerHand; 
                cupScript.PickUp();
                
                SpawnNewCup(); 
            }
        }

        // 2. TUTORIJAL UI PRIKAZ
        if (playerNear && Indigo.Settings.TutorialSettings.ShowTutorial)
        {
            if (handsEmpty && currentCupOnTable != null)
            {
                Indigo.UI.UIEvents.SetPrompt("E", "Uzmi novu šoljicu");
            }
            else
            {
                // Ako držiš nešto drugo u ruci, ugasimo tekst da ne zbunjuje
                Indigo.UI.UIEvents.SetPrompt("", "");
            }
        }
    }

    void SpawnNewCup()
    {
        currentCupOnTable = Instantiate(cupPrefab, spawnPoint.position, spawnPoint.rotation);
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
            // Obavezno čišćenje UI-a kada se igrač odmakne
            Indigo.UI.UIEvents.SetPrompt("", "");
        }
    }
}