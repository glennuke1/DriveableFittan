using MSCLoader;
using System.Collections;
using UnityEngine;

namespace DriveableFittan
{
    public class CoroutineHelper : MonoBehaviour
    {
        public AxisCarController ACC;
        public HutongGames.PlayMaker.FsmString currentVeh;

        void Start()
        {
            StartCoroutine(setACC());
        }

        IEnumerator setACC()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                if (currentVeh.Value == "DrivableFittan")
                {
                    if (ACC.steerAxis == null)
                    {
                        ACC.steerAxis = "Horizontal";
                        ACC.handbrakeAxis = "Handbrake";
                        ACC.shiftUpButton = "ShiftUp";
                        ACC.shiftDownButton = "ShiftDown";
                        driveablefittan.player.transform.localEulerAngles = new Vector3(0, 0, 3.5f);
                        if (driveablefittan.pedalsPart.installed)
                        {
                            ACC.throttleAxis = "Throttle";
                            ACC.brakeAxis = "Brake";
                            ACC.clutchAxis = "Clutch";
                        }
                    }
                }
                else
                {
                    if (ACC.shiftDownButton == "ShiftDown")
                    {
                        ACC.steerAxis = null;
                        ACC.handbrakeAxis = null;
                        ACC.shiftUpButton = null;
                        ACC.shiftDownButton = null;

                        ACC.throttleAxis = null;
                        ACC.brakeAxis = null;
                        ACC.clutchAxis = null;
                    }
                }
            }
        }
    }
}