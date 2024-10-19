using MSCLoader;
using System.Collections;
using UnityEngine;

namespace DriveableFittan
{
    public class IgnitionKnob : MonoBehaviour
    {
        public static IgnitionKnob Instance;
        public bool ElectricsON;
        public bool carburetorAttached;
        public bool engineOn;
        bool starting;
        public int starterTime;
        GameObject key;
        public GameObject EngineSound;

        void Start()
        {
            Instance = this;

            EngineSound = transform.parent.Find("EngineSound").gameObject;
            gameObject.layer = LayerMask.NameToLayer("Dashboard");
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.center = new Vector3(0, -0.015f, 0);
            col.radius = 0.015f;
            col.isTrigger = true;
            key = transform.GetChild(0).GetChild(0).gameObject;
            gameObject.AddComponent<Rigidbody>().isKinematic = true;

            starterTime = 5;
        }

        void OnMouseOver()
        {
            PlayMakerGlobals.Instance.Variables.GetFsmBool("GUIuse").Value = true;
            PlayMakerGlobals.Instance.Variables.GetFsmString("GUIinteraction").Value = "Ignition";
        }

        void OnMouseExit()
        {
            PlayMakerGlobals.Instance.Variables.GetFsmBool("GUIuse").Value = false;
            PlayMakerGlobals.Instance.Variables.GetFsmString("GUIinteraction").Value = "";
        }

        void OnMouseDown()
        {
            if (!key.activeSelf)
            {
                key.SetActive(true);
                key.transform.parent.localEulerAngles = new Vector3(0, -50, 0);
                MasterAudio.PlaySound3DAndForget("CarFoley", key.transform, true, variationName: "carkeys_in");
                if (!engineOn)
                    StartCoroutine(delayStart());
            }
            else
            {
                key.SetActive(false);
                MasterAudio.PlaySound3DAndForget("CarFoley", key.transform, true, variationName: "carkeys_out");
                if (engineOn)
                {
                    engineOn = false;
                    MasterAudio.PlaySound3DAndForget("Ruscko", key.transform, true, variationName: "shutoff");
                    driveablefittan.drivetrain.enabled = false;
                    driveablefittan.drivetrain.rpm = 0;
                    EngineSound.SetActive(false);
                }
            }
        }

        IEnumerator delayStart()
        {
            starting = true;
            yield return new WaitForSeconds(0.5f);
            if (starting)
                StartCoroutine(startEngine());
        }

        IEnumerator startEngine()
        {
            MasterAudio.PlaySound3DAndForget("Ruscko", key.transform, true, variationName: "start1");
            for (int i = 0; i < starterTime; i++)
            {
                yield return new WaitForSeconds(1.23f);
                if (starting)
                {
                    driveablefittan.drivetrain.rpm = 0;
                    driveablefittan.drivetrain.gear = 1;
                    MasterAudio.PlaySound3DAndForget("Ruscko", key.transform, true, variationName: "start2");
                    if (!driveablefittan.carburetorPart.installed)
                    {
                        driveablefittan.drivetrain.maxPower = 33;
                        driveablefittan.drivetrain.maxTorque = 27;
                        driveablefittan.drivetrain.CV2KW = 0.3358f;
                        starterTime = 8;
                    }
                    else
                    {
                        driveablefittan.drivetrain.maxPower = 53;
                        driveablefittan.drivetrain.maxTorque = 67;
                        driveablefittan.drivetrain.CV2KW = 0.7358f;
                        starterTime = 3;
                    }
                    if (driveablefittan.fuel < 100)
                    {
                        starterTime = 1000000;
                    }
                }
                else
                {
                    MasterAudio.StopAllOfSound("Ruscko");
                    yield break;
                }
            }
            MasterAudio.StopAllOfSound("Ruscko");
            MasterAudio.PlaySound3DAndForget("Ruscko", key.transform, true, variationName: "start3");
            engineOn = true;
            driveablefittan.drivetrain.enabled = true;
            EngineSound.SetActive(true);
        }

        void Update()
        {
            if (key.activeSelf && !starting)
            {
                ElectricsON = true;
            }
            else
            {
                ElectricsON = false;
            }
        }

        void OnMouseUp()
        {
            key.transform.parent.localEulerAngles = new Vector3(0, 0, 0);
            starting = false;
        }
    }
}