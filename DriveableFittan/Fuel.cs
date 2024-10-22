using System.Collections;
using UnityEngine;

namespace DriveableFittan
{
    public class Fuel : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(fuelConsumption());
        }

        public static float fuelConsumptionRate = 0.0025f;
        public float fuelConsumptionRatePerSecond;
        public float fuel;

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.name == "lauaviin(Clone)")
            {
                Destroy(collider.gameObject);
                driveablefittan.fuel += 500;
            }
        }

        IEnumerator fuelConsumption()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (IgnitionKnob.Instance.engineOn)
                {
                    fuelConsumptionRatePerSecond = fuelConsumptionRate * driveablefittan.drivetrain.rpm;
                    driveablefittan.fuel -= fuelConsumptionRatePerSecond;
                    fuel = driveablefittan.fuel;
                    if (fuel <= 0)
                    {
                        driveablefittan.drivetrain.enabled = false;
                        IgnitionKnob.Instance.EngineSound.SetActive(false);
                        IgnitionKnob.Instance.engineOn = false;
                    }
                }
            }
        }
    }
}