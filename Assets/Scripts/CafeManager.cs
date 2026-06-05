using UnityEngine;

public class CafeManager : MonoBehaviour
{
    [Header("Roditeljski objekti za nadogradnje")]
    public GameObject interior2;
    public GameObject interior3;
    public GameObject basta;
    public GameObject dekoracije;

    void Start()
    {
        // Čim se Unity scena učita, automatski gasimo sve nadogradnje
        PostaviPocetniKafic();
    }

    public void PostaviPocetniKafic()
    {
        // Gasimo sve nadogradnje (Interior1 je već upaljen u sceni pa ga ignorišemo)
        if (interior2 != null) interior2.SetActive(false);
        if (interior3 != null) interior3.SetActive(false);
        if (basta != null) basta.SetActive(false);
        if (dekoracije != null) dekoracije.SetActive(false);
        
        Debug.Log("CafeManager: Kafić je resetovan na početni nivo!");
    }

    // Ovu funkciju pozivamo kada igrač klikne na dugme za kupovinu u noćnoj smjeni
    public void KupiDogradnju(string nazivDogradnje)
    {
        if (nazivDogradnje == "Interior2" && interior2 != null)
        {
            interior2.SetActive(true);
            Debug.Log("Kupljen Interior 2!");
        }
        else if (nazivDogradnje == "Interior3" && interior3 != null)
        {
            interior3.SetActive(true);
            Debug.Log("Kupljen Interior 3!");
        }
        else if (nazivDogradnje == "Basta" && basta != null)
        {
            basta.SetActive(true);
            Debug.Log("Kupljena Bašta!");
        }
        else if (nazivDogradnje == "Dekoracije" && dekoracije != null)
        {
            dekoracije.SetActive(true);
            Debug.Log("Kupljene Dekoracije!");
        }
        else
        {
            Debug.LogWarning("CafeManager: Pokušaj kupovine nepoznate nadogradnje ili objekat nije povezan!");
        }
    }
}