using UnityEngine;

public class PortafilterHome : MonoBehaviour
{
    public PortafilterPickup portafilter;
    public Transform playerHand; // NOVO: Referenca na ruku da znamo je li prazna!

    private bool playerNear = false;
    private float lastActionTime = 0f;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E) && Time.time - lastActionTime > 0.5f)
        {
            if (portafilter != null)
            {
                if (portafilter.IsPickedUp())
                {
                    // Ako ga već držiš, vraćaš ga (ovo je u redu, prazniš ruku)
                    portafilter.ReturnToHome();
                    lastActionTime = Time.time;
                }
                else if (!portafilter.isOnMachine)
                {
                    // NOVO: Prije nego uzmemo portafilter, provjeravamo ima li šoljice u ruci
                    CupPickup heldCup = null;
                    if (playerHand != null)
                    {
                        heldCup = playerHand.GetComponentInChildren<CupPickup>();
                    }

                    if (heldCup == null) 
                    {
                        // Ruka je prazna, uzimamo portafilter!
                        portafilter.PickUp();
                        lastActionTime = Time.time;
                    }
                    else
                    {
                        // Ruka je puna šoljice
                        Debug.Log("Ruke su ti pune! Prvo spusti šoljicu.");
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}