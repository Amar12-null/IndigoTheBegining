using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public Transform playerHand; // Referenca na ruku igrača da provjerimo šta drži

    private bool playerNear = false;

    void Update()
    {
        // Ako si blizu i pritisneš E
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            // Tražimo da li igrač trenutno ima šoljicu u ruci
            CupPickup heldCup = playerHand.GetComponentInChildren<CupPickup>();

            // Ako ima šoljicu (bilo punu ili praznu), bacamo je!
            if (heldCup != null)
            {
                Destroy(heldCup.gameObject); // Unity funkcija koja briše objekat iz igre
                playerHand.GetComponentInParent<ArmOverride>().isHoldingRight = false; 
                Debug.Log("Šolja je bačena u smeće!");
            }
            else
            {
                Debug.Log("Nemaš šolju u ruci za baciti!");
            }
        }
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