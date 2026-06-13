using UnityEngine;
using Indigo.Game; // Dodali smo ovo da može komunicirati sa DayManagerom
using Indigo.UI;   // Dodali smo ovo za pauzu

[System.Serializable]
public class DayColors
{
    public Color skyColor;
    public Color equatorColor;
    public Color horizonColor;
}

public class DayAndNightControl : MonoBehaviour 
{
    [Header("Osnovne Postavke")]
    [Range(0,1)]
    public float currentTime = 0; // 0 = Ponoć, 0.5 = Podne, 1 = Ponoć
    
    [Header("Objekti na Sceni")]
    public Light directionalLight; // Tvoje Sunce
    public GameObject StarDome; // Zvjezdano nebo

    [Header("Boje Atmosfere")]
    public DayColors dawnColors;
    public DayColors dayColors;
    public DayColors nightColors;
    
    // Privatne varijable
    private float lightIntensity; 
    private Material starMat;

    void Start () 
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        
        lightIntensity = directionalLight.intensity; 
        starMat = StarDome.GetComponentInChildren<MeshRenderer>().material;
        
        // Fiksiramo vrijeme na jutro na samom početku
        currentTime = 0.25f; 
        starMat.color = new Color(1f, 1f, 1f, 0f); 
    }
    
    void Update () 
    {
        // 1. Ako je igra pauzirana (meniji, prodavnica), sunce stoji!
        if (UIEvents.IsPaused) return;

        var dm = DayManager.Instance;
        if (dm != null)
        {
            // 2. Kontrolišemo sunce na osnovu faze smjene
            if (dm.Phase == DayManager.DayPhase.PreDay)
            {
                currentTime = 0.25f; // Zakucano na 6:00 ujutro dok smjena ne počne
            }
            else if (dm.Phase == DayManager.DayPhase.Active)
            {
                // dm.DayFraction ide od 0 (početak) do 1 (kraj). 
                // Sunce ide od 0.25 (jutro) do 0.75 (večer).
                currentTime = Mathf.Lerp(0.25f, 0.75f, dm.DayFraction);
            }
            else if (dm.Phase == DayManager.DayPhase.Ended)
            {
                currentTime = 0.85f; // Zakucano na noć kad smjena završi
            }
        }

        UpdateLight();
    }

    void UpdateLight()
    {
        // --- SINHRONIZACIJA ROTACIJE ---
        StarDome.transform.localRotation = Quaternion.Euler(0, currentTime * 360f, 0);
        directionalLight.transform.localRotation = Quaternion.Euler((currentTime * 360f) - 90, 170, 0);

        // --- INTENZITET SUNCA I ZVIJEZDA ---
        float intensityMultiplier = 1;

        if (currentTime <= 0.23f || currentTime >= 0.75f) 
        {
            intensityMultiplier = 0f; 
            starMat.color = new Color(1, 1, 1, Mathf.Lerp(starMat.color.a, 1f, Time.deltaTime)); 
        }
        else if (currentTime <= 0.25f) 
        {
            intensityMultiplier = Mathf.Clamp01((currentTime - 0.23f) * (1 / 0.02f));
            starMat.color = new Color(1, 1, 1, Mathf.Lerp(starMat.color.a, 0f, Time.deltaTime)); 
        }
        else if (currentTime <= 0.73f) 
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTime - 0.73f) * (1 / 0.02f)));
        }

        // --- PROMJENA BOJA OKOLINE ---
        if (currentTime <= 0.2f) {
            LerpAmbientColors(nightColors);
        }
        else if (currentTime > 0.2f && currentTime < 0.4f) {
            LerpAmbientColors(dawnColors);
        }
        else if (currentTime >= 0.4f && currentTime < 0.75f) {
            LerpAmbientColors(dayColors);
        }
        else if (currentTime >= 0.75f) {
            LerpAmbientColors(dayColors); 
        }

        directionalLight.intensity = lightIntensity * intensityMultiplier;
    }
    
    void LerpAmbientColors(DayColors targetColors)
    {
        RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, targetColors.skyColor, Time.deltaTime);
        RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, targetColors.equatorColor, Time.deltaTime);
        RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, targetColors.horizonColor, Time.deltaTime);
    }
}