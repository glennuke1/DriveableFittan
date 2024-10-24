using MSCLoader;
using UnityEngine;

namespace DriveableFittan
{
    public class Pedals : MonoBehaviour
    {
        public AxisCarController axisCarController;

        Transform clutch;
        Transform brake;
        Transform throttle;

        void Start()
        {
            clutch = transform.GetChild(0);
            brake = transform.GetChild(1);
            throttle = transform.GetChild(2);
        }

        void Update()
        {
            if (driveablefittan.pedalsPart.installed)
            {
                clutch.transform.localEulerAngles = new Vector3(axisCarController.clutchInput * -9, 0, 0);
                brake.transform.localEulerAngles = new Vector3(axisCarController.brakeInput * -9, 0, 0);
                throttle.transform.localEulerAngles = new Vector3(axisCarController.throttleInput * -8, 0, 0);
            }
        }
    }
}