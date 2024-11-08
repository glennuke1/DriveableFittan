using UnityEngine;

namespace DriveableFittan
{
    public class FuelGauge : MonoBehaviour
    {
        void Update()
        {
            if (IgnitionKnob.Instance.ElectricsON)
                transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(-45f, 45f, Mathf.Clamp01(driveablefittan.fuel / 40000)));
        }
    }
}