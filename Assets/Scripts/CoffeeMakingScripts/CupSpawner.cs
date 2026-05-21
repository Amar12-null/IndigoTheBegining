using UnityEngine;

public class CupSpawner : MonoBehaviour
{
    public GameObject cupPrefab;     // Ovdje ćemo uvući Prefab šoljice
    public Transform spawnPoint;     // Mjesto gdje šoljica stoji (npr. isti ovaj objekat)
    public Transform playerHand;     // HandPoint od igrača

    private GameObject currentCupOnTable;
    private bool playerNear = false;

    void Start()
    {
        SpawnNewCup(); // Na početku odmah stvorimo jednu šolju
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            // Provjeravamo da li igrač drži nešto u ruci (da ne uzme šolju dok drži portafilter)
            CupPickup cupInHand = playerHand.GetComponentInChildren<CupPickup>();
            PortafilterPickup portafilterInHand = playerHand.GetComponentInChildren<PortafilterPickup>();

            if (cupInHand == null && portafilterInHand == null && currentCupOnTable != null)
            {
                CupPickup cupScript = currentCupOnTable.GetComponent<CupPickup>();
                cupScript.handPoint = playerHand; // Dajemo šolji informaciju gdje je ruka
                cupScript.PickUp();
                
                SpawnNewCup(); // Čim je podigneš, stvara se nova!
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
        if (other.CompareTag("Player")) playerNear = false;
    }
}