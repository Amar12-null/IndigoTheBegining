using UnityEngine;

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
    public bool StartDay = true; // Započinje igru ujutro
    public float SecondsInAFullDay = 900f; // 15 minuta u stvarnom vremenu (15 * 60)
    
    [Range(0,1)]
    public float currentTime = 0; // 0 = Ponoć, 0.5 = Podne, 1 = Ponoć
    
    [HideInInspector]
    public float timeMultiplier = 1f; 
    public int currentDay = 0; 

    [Header("Objekti na Sceni")]
    public Light directionalLight; // Tvoje Sunce
    public GameObject StarDome; // Zvjezdano nebo

    [Header("Boje Atmosfere")]
    public DayColors dawnColors;
    public DayColors dayColors;
    public DayColors nightColors;
    
    [Header("UI Debugging")]
    public bool showUI;
    
    // Privatne varijable
    private float lightIntensity; 
    private Material starMat;

    void Start () 
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        
        lightIntensity = directionalLight.intensity; 
        starMat = StarDome.GetComponentInChildren<MeshRenderer>().material;
        
        // Ako je označeno da počne dan, postavlja vrijeme tačno na 7:00 h
        if (StartDay) 
        {
            currentTime = 0.2917f; // 7 / 24 = 0.291666...
            starMat.color = new Color(1f, 1f, 1f, 0f); // Gasi zvijezde ujutro
        }
    }
    
    void Update () 
    {
        UpdateLight();
        
        // Povećava vrijeme na osnovu brzine i sekundi u danu
        currentTime += (Time.deltaTime / SecondsInAFullDay) * timeMultiplier;
        
        // Resetuje vrijeme kada dođe do ponoći i prelazi u novi dan
        if (currentTime >= 1f) 
        {
            currentTime = 0f;
            currentDay++; 
        }
    }

    void UpdateLight()
    {
        // --- SINHRONIZACIJA ROTACIJE ---
        // Zvjezdano nebo (StarDome) prati tačan procenat trenutnog vremena
        StarDome.transform.localRotation = Quaternion.Euler(0, currentTime * 360f, 0);
        
        // Sunce se kreće po nebu (X osa)
        directionalLight.transform.localRotation = Quaternion.Euler((currentTime * 360f) - 90, 170, 0);

        // --- INTENZITET SUNCA I ZVIJEZDA ---
        float intensityMultiplier = 1;

        if (currentTime <= 0.23f || currentTime >= 0.75f) 
        {
            intensityMultiplier = 0f; // Sunce je zašlo
            starMat.color = new Color(1, 1, 1, Mathf.Lerp(starMat.color.a, 1f, Time.deltaTime)); // Zvijezde se pojavljuju
        }
        else if (currentTime <= 0.25f) 
        {
            intensityMultiplier = Mathf.Clamp01((currentTime - 0.23f) * (1 / 0.02f));
            starMat.color = new Color(1, 1, 1, Mathf.Lerp(starMat.color.a, 0f, Time.deltaTime)); // Zvijezde blijede (Jutro)
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

        // Postavljanje konačnog intenziteta svjetla
        directionalLight.intensity = lightIntensity * intensityMultiplier;
    }
    
    // Čista "Helper" funkcija za miješanje ambijentalnih boja
    void LerpAmbientColors(DayColors targetColors)
    {
        RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, targetColors.skyColor, Time.deltaTime);
        RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, targetColors.equatorColor, Time.deltaTime);
        RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, targetColors.horizonColor, Time.deltaTime);
    }

    public string TimeOfDay ()
    {
        string dayState = "";
        if (currentTime >= 0f && currentTime < 0.1f) dayState = "Midnight";
        else if (currentTime >= 0.1f && currentTime < 0.5f) dayState = "Morning";
        else if (currentTime >= 0.5f && currentTime < 0.6f) dayState = "Mid Noon";
        else if (currentTime >= 0.6f && currentTime < 0.8f) dayState = "Evening";
        else if (currentTime >= 0.8f && currentTime <= 1f) dayState = "Night";
        return dayState;
    }

    void OnGUI()
    {
        if (showUI) {
            GUILayout.Box ("Day: " + currentDay);
            GUILayout.Box (TimeOfDay ());
            GUILayout.Box ("Time slider");
            currentTime = GUILayout.HorizontalSlider(currentTime, 0f, 1f); 
        }
    }
}