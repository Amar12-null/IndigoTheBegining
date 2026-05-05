using UnityEngine;

public class PortafilterHome : MonoBehaviour
{
    public PortafilterPickup portafilter;

    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (portafilter != null && portafilter.IsPickedUp())
            {
                portafilter.ReturnToHome();
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