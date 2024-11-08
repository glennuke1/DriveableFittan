using UnityEngine;

namespace DriveableFittan
{
    public class SteerLimit : MonoBehaviour
    {
        float maxAngle = 35;
        float fixedAngle;

        Wheel flWheel;
        Wheel frWheel;

        void Start()
        {
            flWheel = transform.Find("wheelFL").GetComponent<Wheel>();
            frWheel = transform.Find("wheelFR").GetComponent<Wheel>();
        }
        
        void Update()
        {
            float calculatedAngle = maxAngle - (driveablefittan.drivetrain.differentialSpeed / 4);
            fixedAngle = Mathf.Clamp(calculatedAngle, 2f, maxAngle);
            flWheel.maxSteeringAngle = fixedAngle;
            frWheel.maxSteeringAngle = fixedAngle;
        }
    }
}
