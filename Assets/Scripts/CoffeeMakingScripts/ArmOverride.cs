using UnityEngine;

public class ArmOverride : MonoBehaviour
{
    [Header("Desna ruka (Kafa, Portafilter)")]
    public Transform rightUpperArm; // Ovdje ćemo uvući arm_stretch.r
    public Transform rightForearm;  // Ovdje ćemo uvući forearm_stretch.r
    public bool isHoldingRight = false;
    
    // Ugao rotacije koji ćemo uštimati u inspektoru
    public Vector3 rightUpperArmRotation = new Vector3(0, 0, 90); 
    public Vector3 rightForearmRotation = new Vector3(0, 0, 0);

    [Header("Lijeva ruka (Tacna)")]
    public Transform leftUpperArm; // arm_stretch.l
    public Transform leftForearm;  // forearm_stretch.l
    public bool isHoldingLeft = false;
    
    public Vector3 leftUpperArmRotation = new Vector3(0, 0, 90);
    public Vector3 leftForearmRotation = new Vector3(0, 0, 90); // Savijena ruka za tacnu

    // LateUpdate se poziva zadnji, NAKON što animacija završi svoj posao za ovaj frame
    void LateUpdate()
    {
        // Ako držiš nešto u desnoj ruci, "zaključavamo" njene kosti
        if (isHoldingRight && rightUpperArm != null && rightForearm != null)
        {
            rightUpperArm.localEulerAngles = rightUpperArmRotation;
            rightForearm.localEulerAngles = rightForearmRotation;
        }

        // Ako držiš nešto u lijevoj ruci
        if (isHoldingLeft && leftUpperArm != null && leftForearm != null)
        {
            leftUpperArm.localEulerAngles = leftUpperArmRotation;
            leftForearm.localEulerAngles = leftForearmRotation;
        }
    }
}