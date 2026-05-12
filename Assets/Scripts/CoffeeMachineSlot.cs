using UnityEngine;
using Indigo.UI;

public class CoffeeMachineSlot : MonoBehaviour
{
    public Transform portafilterSlot;
    public PortafilterPickup portafilter;

    private bool playerNear = false;
void Update()
{
    if (playerNear && Input.GetKeyDown(KeyCode.E))
    {
        Debug.Log("Pritisnuo E kod mašine");

        if (portafilter != null && portafilter.IsPickedUp())
        {
            Debug.Log("Stavljam portafilter na mašinu");
            portafilter.PutOnMachine(portafilterSlot);
        }
        else if (portafilter != null && !portafilter.IsPickedUp())
{
    portafilter.PickUpFromMachine();
}


        else
        {
            Debug.Log("Portafilter nije pokupljen ili nije povezan");
        }

        if (playerNear) UpdatePrompt();
    }
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            UpdatePrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            UIEvents.ClearPrompt();
        }
    }

    void UpdatePrompt()
    {
        if (portafilter == null) return;
        if (portafilter.IsPickedUp())
            UIEvents.SetPrompt("E", "Place on machine");
        else
            UIEvents.SetPrompt("E", "Take from machine");
    }
}