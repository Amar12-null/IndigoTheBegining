using UnityEngine;

public class ArmOverride : MonoBehaviour
{
    [Header("Desna ruka (Kafa, Portafilter)")]
    public Transform rightUpperArm; // arm_stretch.r
    public Transform rightForearm;  // forearm_stretch.r
    public bool isHoldingRight = false;
    
    [Range(0f, 1f)] public float rightArmWeight = 1f; // 1 = potpuno zaključano
    public Vector3 rightUpperArmRotation = new Vector3(0, 0, 90); 
    public Vector3 rightForearmRotation = new Vector3(0, 0, 0);

    [Header("Lijeva ruka (Tacna)")]
    public Transform leftUpperArm; // arm_stretch.l
    public Transform leftForearm;  // forearm_stretch.l
    public Transform leftHand;     // NOVO: hand.l - jako bitno za ravnanje tacne!
    public bool isHoldingLeft = false;
    
    [Range(0f, 1f)] 
    [Tooltip("Koliko jako zaključavamo ruku? 0 = prati animaciju hoda, 0.5 = pola-pola, 1 = skroz zaključano")]
    public float leftArmWeight = 0.7f; // Probajte staviti na 0.5 ili 0.7 u Inspectoru

    public Vector3 leftUpperArmRotation = new Vector3(0, 0, 90);
    public Vector3 leftForearmRotation = new Vector3(0, 0, 90);
    public Vector3 leftHandRotation = new Vector3(0, 0, 0); // Ovdje štimamo ugao šake da tacna ne pada

    void LateUpdate()
    {
        // DESNA RUKA
        if (isHoldingRight && rightUpperArm != null && rightForearm != null)
        {
            Quaternion targetUpperR = Quaternion.Euler(rightUpperArmRotation);
            Quaternion targetForearmR = Quaternion.Euler(rightForearmRotation);

            rightUpperArm.localRotation = Quaternion.Slerp(rightUpperArm.localRotation, targetUpperR, rightArmWeight);
            rightForearm.localRotation = Quaternion.Slerp(rightForearm.localRotation, targetForearmR, rightArmWeight);
        }

        // LIJEVA RUKA (Tacna)
        if (isHoldingLeft && leftUpperArm != null && leftForearm != null)
        {
            // Računamo željene uglove
            Quaternion targetUpperL = Quaternion.Euler(leftUpperArmRotation);
            Quaternion targetForearmL = Quaternion.Euler(leftForearmRotation);
            
            // Slerp blenda rotaciju izmedju onog sto kaze Animator i onog sto mi zelimo
            leftUpperArm.localRotation = Quaternion.Slerp(leftUpperArm.localRotation, targetUpperL, leftArmWeight);
            leftForearm.localRotation = Quaternion.Slerp(leftForearm.localRotation, targetForearmL, leftArmWeight);

            // Zaključavamo i šaku da tacna uvijek stoji vodoravno!
            if (leftHand != null)
            {
                Quaternion targetHandL = Quaternion.Euler(leftHandRotation);
                leftHand.localRotation = Quaternion.Slerp(leftHand.localRotation, targetHandL, leftArmWeight);
            }
        }
    }
}