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
                    if (ACC.throttleAxis == null)
                    {
                        ACC.throttleAxis = "Throttle";
                        ACC.brakeAxis = "Brake";
                        ACC.steerAxis = "Horizontal";
                        ACC.handbrakeAxis = "Handbrake";
                        ACC.clutchAxis = "Clutch";
                        ACC.shiftUpButton = "ShiftUp";
                        ACC.shiftDownButton = "ShiftDown";
                        driveablefittan.player.transform.localEulerAngles = new Vector3(0, 0, 3);
                    }
                }
                else
                {
                    if (ACC.throttleAxis == "Throttle")
                    {
                        ACC.throttleAxis = null;
                        ACC.brakeAxis = null;
                        ACC.steerAxis = null;
                        ACC.handbrakeAxis = null;
                        ACC.clutchAxis = null;
                        ACC.shiftUpButton = null;
                        ACC.shiftDownButton = null;
                    }
                }
            }
        }
    }
}