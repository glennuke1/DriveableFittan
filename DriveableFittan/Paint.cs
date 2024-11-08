using MSCLoader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DriveableFittan
{
    public class Paint : MonoBehaviour
    {
        GameObject SprayCan;

        string[] collNames = { "fittan_coll1", "fittan_coll3", "fittan_coll6", "fittan_coll7", "fittan_coll8" };

        void Start()
        {
            SprayCan = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/SprayCan");
            StartCoroutine(paint());
        }

        IEnumerator paint()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (SprayCan.activeSelf)
                {
                    foreach (RaycastHit hit in Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 1f))
                    {
                        if (hit.collider != null)
                        {
                            if (collNames.Contains(hit.collider.gameObject.name))
                            {
                                PlayMakerGlobals.Instance.Variables.GetFsmString("GUIinteraction").Value = "Paint Body";
                                if (SprayCan.GetComponent<PlayMakerFSM>().FsmStates[1].Active)
                                {
                                    yield return new WaitForSeconds(3);
                                    GetComponent<MeshRenderer>().material.SetColor("_Color", SprayCan.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmColor("SprayColor").Value);
                                    driveablefittan.Fittan.transform.Find("DriverDoors").GetChild(0).GetChild(0).Find("door").GetComponent<MeshRenderer>().material.SetColor("_Color", SprayCan.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmColor("SprayColor").Value);
                                    driveablefittan.Fittan.transform.Find("DriverDoors").GetChild(1).GetChild(0).Find("door").GetComponent<MeshRenderer>().material.SetColor("_Color", SprayCan.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmColor("SprayColor").Value);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}