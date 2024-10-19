﻿using MSCLoader;
using System;
using System.IO;
using UnityEngine;
using TommoJProductions.ModApi.Attachable;

namespace DriveableFittan
{
    public class driveablefittan : Mod
    {
        public override string ID => "driveablefittanrevamped"; //Your mod ID (unique)
        public override string Name => "Drivable Fittan Revamped"; //You mod name
        public override string Author => "glen"; //Your Username
        public override string Version => "1.0"; //Version
        public override string Description => "Make your favorite green car drivable"; //Short description of your mod

        public override void ModSetup()
        {
            SetupFunction(Setup.PreLoad, Mod_PreLoad);
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.Update, Mod_Update);
            SetupFunction(Setup.OnSave, Mod_OnSave);
            SetupFunction(Setup.PostLoad, Mod_PostLoad);
        }

        GameObject Fittan;
        GameObject leftlight;
        GameObject rightlight;
        GameObject door;
        GameObject fittan_coll10;
        GameObject pivot;

        public static float fuel = 0;

        TextMesh[] GUIGearText = new TextMesh[2];

        Camera cam;

        bool doorclosed = true;

        HutongGames.PlayMaker.FsmFloat wheel;
        public static HutongGames.PlayMaker.FsmString currentVeh;

        Rigidbody doorRB;
        Rigidbody fittanRB;
        AxisCarController axisCarController;

        public static GameObject player;

        public static Drivetrain drivetrain;

        public static Part carburetorPart;

        public override void ModSettings()
        {
            Settings.AddButton(this, "playertp", "Teleport Player to Fittan", totp);
            Settings.AddButton(this, "fittantp", "Teleport Fittan to player", tpto);
        }

        void tpto()
        {
            Fittan.transform.position = GameObject.Find("PLAYER").transform.position + new Vector3(3, 2, 3);
        }

        void totp()
        {
            GameObject.Find("PLAYER").transform.position = Fittan.transform.position + new Vector3(3, 0, 3);
        }

        private void Mod_PreLoad()
        {
            Fittan = GameObject.Instantiate(GameObject.Find("TRAFFIC/VehiclesDirtRoad/Rally/FITTAN"));
            Fittan.transform.position = new Vector3(-191.751f, -0.228f, 1212.873f);
            Fittan.transform.eulerAngles = new Vector3(359.9245f, 116.2305f, 1.9526f);

            if (File.Exists(Application.persistentDataPath + "/fittan.txt"))
            {
                string Path = Application.persistentDataPath + "/fittan.txt";
                string SaveString = File.ReadAllText(Path);
                string[] SaveContents = SaveString.Split(new[] { SAVE_SEPARATOR }, StringSplitOptions.None);
                Fittan.transform.position = new Vector3(float.Parse(SaveContents[0]), float.Parse(SaveContents[1]), float.Parse(SaveContents[2]));
                Fittan.transform.eulerAngles = new Vector3(float.Parse(SaveContents[3]), float.Parse(SaveContents[4]), float.Parse(SaveContents[5]));
            }

            Fittan.GetComponent<Rigidbody>().isKinematic = true;
            Fittan.GetComponent<Drivetrain>().enabled = false;
        }

        private void Mod_OnLoad()
        {
            player = GameObject.Find("PLAYER");
            cam = Camera.main;
            GUIGearText[0] = GameObject.Instantiate(GameObject.Find("GUI/Indicators/Gear")).GetComponent<TextMesh>();
            GUIGearText[0].GetComponent<PlayMakerFSM>().enabled = false;
            GUIGearText[0].transform.parent = GameObject.Find("GUI/Indicators").transform;
            GUIGearText[0].transform.localPosition = new Vector3(12, 0, 0);
            GUIGearText[0].gameObject.name = "FittanGearIndicator";
            GUIGearText[1] = GUIGearText[0].transform.GetChild(0).GetComponent<TextMesh>();

            Fittan.name = "DrivableFittan";
            leftlight = Fittan.transform.Find("LightsNPC/BeamsShort/BeamShortAILeft").gameObject;
            rightlight = Fittan.transform.Find("LightsNPC/BeamsShort/BeamShortAIRight").gameObject;
            door = Fittan.transform.Find("DriverDoors/doorl 1").gameObject;
            fittan_coll10 = Fittan.transform.Find("Colliders/fittan_coll10").gameObject;
            wheel = Fittan.transform.Find("LOD/Steering/wheel").GetComponent<PlayMakerFSM>().Fsm.GetFsmFloat("Angle2");
            pivot = new GameObject("GetInPivot");
            pivot.transform.parent = Fittan.transform;
            pivot.transform.localPosition = new Vector3(1, -0.3f, 0);
            GameObject.Destroy(Fittan.GetComponent<MobileCarController>());

            fittanRB = Fittan.GetComponent<Rigidbody>();

            Fittan.transform.Find("fittan_body").GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, 0.7f, 0));
            Fittan.transform.Find("DriverDoors").GetChild(0).GetChild(0).Find("door").GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, 0.7f, 0));
            Fittan.transform.Find("DriverDoors").GetChild(1).GetChild(0).Find("door").GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, 0.7f, 0));

            drivetrain = Fittan.GetComponent<Drivetrain>();

            drivetrain.autoClutch = false;
            drivetrain.gear = 1;

            drivetrain.rpm = 0;

            drivetrain.wheelTireVelo = 0;

            foreach (Wheel wheel in drivetrain.poweredWheels)
            {
                wheel.angularVelocity = 0;
            }

            Fittan.GetComponent<Rigidbody>().velocity = Vector3.zero;

            drivetrain.enabled = false;

            Fittan.transform.Find("LOD").Find("EngineSound").gameObject.SetActive(false);
            Fittan.transform.Find("Driver").gameObject.SetActive(false);
            Fittan.transform.Find("LightsNPC").GetComponent<PlayMakerFSM>().enabled = false;
            Fittan.transform.Find("CrashEvent").gameObject.SetActive(false);
            fittan_coll10.SetActive(false);
            HingeJoint hingeJoint = door.AddComponent<HingeJoint>();
            hingeJoint.connectedBody = Fittan.GetComponent<Rigidbody>();
            hingeJoint.axis = new Vector3(0, 0, 90f);
            hingeJoint.useLimits = true;
            JointLimits limits = hingeJoint.limits;
            limits.min = 0f;
            limits.max = 0f;
            hingeJoint.limits = limits;
            PlayMakerFSM[] aap = Fittan.GetComponents<PlayMakerFSM>();
            PlayMakerFSM[] aap1 = Fittan.transform.Find("Navigation").GetComponents<PlayMakerFSM>();
            foreach (PlayMakerFSM fsm in aap)
            {
                fsm.enabled = false;
            }

            foreach (PlayMakerFSM fsm in aap1)
            {
                fsm.enabled = false;
            }

            doorRB = door.GetComponent<Rigidbody>();
            axisCarController = Fittan.AddComponent<AxisCarController>();
            axisCarController.brakesTime = 0.1f;
            axisCarController.brakesReleaseTime = 0.1f;

            Fittan.transform.Find("PlayerTrigger/DriveTrigger").GetComponent<PlayMakerFSM>().Fsm.GetState("Player in car").GetAction<HutongGames.PlayMaker.Actions.SetStringValue>(8).stringValue.Value = "DrivableFittan";

            Fittan.transform.Find("LOD").gameObject.SetActive(true);

            Fittan.GetComponent<Axles>().frontAxle.leftWheel.mass = 200f;
            Fittan.GetComponent<Axles>().frontAxle.rightWheel.mass = 200f;
            Fittan.GetComponent<Axles>().rearAxle.leftWheel.mass = 200f;
            Fittan.GetComponent<Axles>().rearAxle.rightWheel.mass = 200f;

            currentVeh = PlayMakerGlobals.Instance.Variables.GetFsmString("PlayerCurrentVehicle");

            CoroutineHelper coroutineHelper = Fittan.AddComponent<CoroutineHelper>();
            coroutineHelper.ACC = axisCarController;
            coroutineHelper.currentVeh = currentVeh;

            GameObject.Find("TRAFFIC/VehiclesDirtRoad/Rally/FITTAN/Colliders").transform.GetChild(5).gameObject.SetActive(false);
            GameObject Radio = GameObject.Instantiate(GameObject.Find("HAYOSIKO(1500kg, 250)/RadioPivot"));
            Radio.transform.SetParent(Fittan.transform);
            Radio.name = "RadioPivot";
            Radio.transform.localPosition = new Vector3(0.28f, 0.445f, 0.66f);
            Radio.transform.localRotation = Quaternion.Euler(288f, 0f, 180f);
            Radio.transform.localScale = new Vector3(1f, 1f, 1f);
            Radio.transform.Find("Speaker").localPosition = Vector3.zero;

            GameObject IgnitionKnob = GameObject.Instantiate(GameObject.Find("RCO_RUSCKO12(270)").transform.Find("LOD/Dashboard/Keyhole").gameObject);
            IgnitionKnob.transform.parent = Fittan.transform.Find("LOD");
            IgnitionKnob.name = "Keyhole";
            IgnitionKnob.transform.localPosition = new Vector3(-0.15f, 0.46f, 0.6f);
            IgnitionKnob.transform.localRotation = Quaternion.Euler(-75, 0, 180);
            IgnitionKnob.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            IgnitionKnob.AddComponent<IgnitionKnob>();

            AssetBundle assetBundle = LoadAssets.LoadBundle("DriveableFittan.Assets.fittan.unity3d");
            GameObject carburetor = GameObject.Instantiate(assetBundle.LoadAsset<GameObject>("carburetor.prefab"));

            Fittan.transform.Find("LightsNPC").Find("BeamsShort").gameObject.SetActive(true);

            Fittan.transform.Find("Colliders").Find("fittan_coll5").gameObject.SetActive(false);
            Fittan.transform.Find("LOD").Find("PlayerFunctions").Find("PlayerColliders").Find("PlayerCollider 7").gameObject.SetActive(false);

            drivetrain.minRPM = 1500;

            TriggerData carburetorTriggerData = TriggerData.createTriggerData("drivableFittanCarburetorTriggerData");

            carburetorPart = carburetor.AddComponent<Part>();

            PartSettings partSettings = new PartSettings()
            {
                autoSave = true,
                assembleType = AssembleType.static_rigidbodyDelete,
                setPositionRotationOnInitialisePart = true,
                assemblyTypeJointSettings = new AssemblyTypeJointSettings()
                {
                    breakForce = float.PositiveInfinity,
                },
                collisionSettings = new CollisionSettings()
                {
                    notInstalledCollisionDetectionMode = CollisionDetectionMode.ContinuousDynamic
                }
            };

            TriggerSettings carbTriggerSettings = new TriggerSettings()
            {
                triggerID = "drivableFittanCarburetorTrigger",
                useTriggerTransformData = true,
                triggerData = carburetorTriggerData,
                triggerPosition = new Vector3(0.25f, 0.48f, -1.5f),
                triggerEuler = Vector3.zero
            };

            BoltSettings boltSettings = new BoltSettings()
            {
                size = BoltSize._8mm,
                type = BoltType.nut,
                posDirection = Vector3.up,
                posStep = 0.0025f,
                rotDirection = Vector3.forward,
                rotStep = 30f,
                canUseRachet = true,
                highlightWhenActive = true,
            };

            new Trigger(Fittan.transform.Find("LOD").gameObject, carbTriggerSettings);

            carburetorPart.initPart(carburetorTriggerData, partSettings);

            Fittan.transform.Find("wheelFL").GetComponent<Wheel>().brakeFrictionTorque = 110;
            Fittan.transform.Find("wheelFR").GetComponent<Wheel>().brakeFrictionTorque = 110;
            Fittan.transform.Find("wheelRL").GetComponent<Wheel>().brakeFrictionTorque = 50;
            Fittan.transform.Find("wheelRR").GetComponent<Wheel>().brakeFrictionTorque = 50;

            GameObject fittanPoster = GameObject.Instantiate(assetBundle.LoadAsset<GameObject>("fittanposer.fbx"));
            fittanPoster.transform.position = new Vector3(-1542.78f, 4.877619f, 1185.38f);
            fittanPoster.transform.eulerAngles = new Vector3(-90f, 0, 147.5f);
            fittanPoster.transform.localScale = new Vector3(1, 1, 1);

            GameObject fuelTrigger = new GameObject("fuelTrigger");
            fuelTrigger.transform.parent = Fittan.transform;
            fuelTrigger.transform.localPosition = new Vector3(0, 0.45f, -1.6f);
            fuelTrigger.transform.localEulerAngles = new Vector3(0, 0, 0);
            BoxCollider col = fuelTrigger.AddComponent<BoxCollider>();
            col.size = new Vector3(1, 0.2f, 0.2f);
            fuelTrigger.AddComponent<Fuel>();


        }

        private void Mod_PostLoad()
        {
            Fittan.transform.Find("PlayerTrigger").Find("DriveTrigger").localPosition = new Vector3(-0.315f, -0.15f, -0.9f);
            Fittan.transform.Find("PlayerTrigger").Find("PlayerSpawnPivot").localPosition = new Vector3(-0.3f, -0.12f, -0.9f);

            Fittan.GetComponent<Rigidbody>().isKinematic = false;
        }

        bool headlightsOn;
        bool usingDoor = true;

        private void Mod_Update()
        {
            RaycastHit hit;

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1f))
            {
                if (hit.collider == door.GetComponentInChildren<Collider>())
                {
                    PlayMakerGlobals.Instance.Variables.GetFsmBool("GUIuse").Value = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (doorclosed)
                        {
                            MasterAudio.PlaySound3DAndForget("CarFoley", Fittan.transform, false, 1f, null, 0f, "open_door1");
                        }
                        usingDoor = true;
                    }

                    if (!Input.GetMouseButton(0))
                    {
                        if (door.transform.localEulerAngles.y >= 270.1f)
                        {
                            doorclosed = false;
                        }
                        else
                        {
                            doorclosed = true;
                        }
                    }
                }
            }

            if (usingDoor)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!doorclosed)
                    {
                        doorRB.AddRelativeForce(-10, 0, 0, ForceMode.Impulse);
                    }
                    else
                    {
                        JointLimits limits = door.GetComponent<HingeJoint>().limits;
                        limits.min = 0f;
                        limits.max = 80f;
                        door.GetComponent<HingeJoint>().limits = limits;
                        doorRB.AddRelativeForce(10, 0, 0, ForceMode.Impulse);
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (door.transform.localEulerAngles.y <= 270.1)
                    {
                        JointLimits limits = door.GetComponent<HingeJoint>().limits;
                        limits.min = 0f;
                        limits.max = 0f;
                        door.GetComponent<HingeJoint>().limits = limits;
                        MasterAudio.PlaySound3DAndForget("CarFoley", Fittan.transform, false, 1f, null, 0f, "close_door1");
                    }
                    usingDoor = false;
                }
            }

            if (currentVeh.Value == "DrivableFittan")
            {
                wheel.Value = axisCarController.steering;
                if (Input.GetKeyDown("l"))
                {
                    headlightsOn = !headlightsOn;
                    MasterAudio.PlaySound3DAndForget("CarFoley", Fittan.transform, false, 1f, null, 0f, "dash_button");
                }

                if (IgnitionKnob.Instance.ElectricsON)
                {
                    leftlight.SetActive(headlightsOn);
                    rightlight.SetActive(headlightsOn);
                }
                else
                {
                    if (leftlight.activeSelf)
                    {
                        leftlight.SetActive(false);
                        rightlight.SetActive(false);
                    }
                }

                if (!GUIGearText[0].gameObject.activeSelf)
                {
                    GUIGearText[0].gameObject.SetActive(true);
                    GUIGearText[0].GetComponent<MeshRenderer>().enabled = true;
                }

                if (drivetrain.gear == 0)
                {
                    GUIGearText[0].text = "R";
                    GUIGearText[1].text = "R";
                }
                else if (drivetrain.gear == 1)
                {
                    GUIGearText[0].text = "N";
                    GUIGearText[1].text = "N";
                }
                else
                {
                    GUIGearText[0].text = (drivetrain.gear - 1).ToString();
                    GUIGearText[1].text = (drivetrain.gear - 1).ToString();
                }
            }
            else
            {
                if (GUIGearText[0].gameObject.activeSelf)
                {
                    GUIGearText[0].gameObject.SetActive(false);
                    GUIGearText[0].GetComponent<MeshRenderer>().enabled = false;
                }
            }

            /*if (drivetrain.enabled)
            {
                if (drivetrain.gear != 1)
                {
                    if (drivetrain.rpm < 2000)
                    {
                        if (axisCarController.clutchInput < 0.2f)
                        {
                            if (fittanRB.velocity.magnitude < 1f)
                            {
                                drivetrain.enabled = false;
                                IgnitionKnob.Instance.EngineSound.SetActive(false);
                                IgnitionKnob.Instance.engineOn = false;
                            }
                        }
                    }
                }
            }*/
        }

        string SAVE_SEPARATOR = "\n";

        void Mod_OnSave()
        {
            string[] SaveContents = new string[]
            {
                ""+Fittan.transform.position.x,
                ""+Fittan.transform.position.y,
                ""+Fittan.transform.position.z,

                ""+Fittan.transform.eulerAngles.x,
                ""+Fittan.transform.eulerAngles.y,
                ""+Fittan.transform.eulerAngles.z,

                ""+IgnitionKnob.Instance.carburetorAttached,
            };
            string Path = Application.persistentDataPath + "/fittan.txt";
            string SaveString = string.Join(SAVE_SEPARATOR, SaveContents);
            File.WriteAllText(Path, SaveString);
        }
    }
}
