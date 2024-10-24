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
                        ACC.clutchAxis = "Clutch";
                        ACC.steerAxis = "Horizontal";
                        ACC.handbrakeAxis = "";
                        ACC.shiftUpButton = "ShiftUp";
                        ACC.shiftDownButton = "ShiftDown";
                        driveablefittan.player.transform.localEulerAngles = new Vector3(0, driveablefittan.player.transform.localEulerAngles.y, 3.5f);
                    }
                }
                else
                {
                    if (ACC.throttleAxis == "Throttle")
                    {
                        ACC.throttleAxis = null;
                        ACC.brakeAxis = null;
                        ACC.clutchAxis = null;
                        ACC.steerAxis = null;
                        ACC.handbrakeAxis = null;
                        ACC.shiftUpButton = null;
                        ACC.shiftDownButton = null;
                    }
                }
            }
        }
    }
}