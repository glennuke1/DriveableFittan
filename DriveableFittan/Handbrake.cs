using MSCLoader;
using UnityEngine;

namespace DriveableFittan
{
    public class Handbrake : MonoBehaviour
    {
        HutongGames.PlayMaker.FsmString currentVeh;
        bool braking;
        Wheel[] wheels;

        void Start()
        {
            currentVeh = PlayMakerGlobals.Instance.Variables.GetFsmString("PlayerCurrentVehicle");
            wheels = driveablefittan.drivetrain.poweredWheels;
        }

        void Update()
        {
            if (currentVeh.Value == "DrivableFittan")
            {
                if (cInput.GetKeyDown("Handbrake"))
                {
                    if (!braking)
                    {
                        braking = true;
                        MasterAudio.PlaySound3DAndForget("CarFoley", driveablefittan.Fittan.transform, true, 0.5f, null, 0, "handbrake_on");
                    }
                    else
                    {
                        braking = false;
                        MasterAudio.PlaySound3DAndForget("CarFoley", driveablefittan.Fittan.transform, true, 0.5f, null, 0, "handbrake_off");
                    }
                }
            }

            if (braking)
            {
                foreach (Wheel wheel in wheels)
                {
                    wheel.handbrake = 1;
                }
            }
            else
            {
                foreach (Wheel wheel in wheels)
                {
                    wheel.handbrake = 0;
                }
            }
        }
    }
}