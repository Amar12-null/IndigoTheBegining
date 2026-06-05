using UnityEngine;

public class CoffeeGrinder : MonoBehaviour
{
    public Transform playerHand; // Referenca na desnu ruku
    
    public float timeToGrind = 1.5f; // Koliko sekundi držiš E
    private float currentGrindTimer = 0f;
    private bool playerNear = false;

    void Update()
    {
        if (playerNear)
        {
            PortafilterPickup heldPortafilter = playerHand.GetComponentInChildren<PortafilterPickup>();

            // Ako držiš portafilter i on je PRAZAN
            if (heldPortafilter != null && 
               (heldPortafilter.currentState == PortafilterPickup.PortafilterState.Empty 
                || heldPortafilter.currentState == PortafilterPickup.PortafilterState.UsedCoffee 
                ))
            {
                if (Input.GetKey(KeyCode.E)) // GetKey znači da DRŽIŠ tipku
                {
                    currentGrindTimer += Time.deltaTime;
                    Debug.Log("Meljem kafu... " + Mathf.RoundToInt((currentGrindTimer / timeToGrind) * 100) + "%");

                    if (currentGrindTimer >= timeToGrind)
                    {
                        // NOVO: Pozivamo funkciju koja mijenja state i PALI vizual
                        heldPortafilter.FillWithCoffee();
                        
                        currentGrindTimer = 0f;
                        Debug.Log("Kafa spremna! Nosi na aparat.");
                    }
                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    currentGrindTimer = 0f; // Resetujemo ako pustiš E prerano
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
        if (other.CompareTag("Player")) playerNear = false;
    }
}