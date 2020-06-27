using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;
using Sandbox.Game.GameSystems;
using VRageRender;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        Color NormalColor = new Color(255, 228, 206);
        Color Red = new Color(255, 0, 0);
        Color Green = new Color(0, 255, 0);

        List<IMyInteriorLight> LightList;
        List<IMyDoor> DoorList;
        List<IMySensorBlock> SensorList;

        IMyReflectorLight SpinningLight;
        IMyAirVent Vent;
        IMySensorBlock ActivatedSensor;
        IMyDoor ActivatedDoor;

        int Update100Runs = 1;

        public Program()
        {
            LightList = new List<IMyInteriorLight>();
            DoorList = new List<IMyDoor>();
            SensorList = new List<IMySensorBlock>();

            IMyBlockGroup Lights = GridTerminalSystem.GetBlockGroupWithName("Airlock Lights");
            if (Lights == null)
                Echo("Cannot find light group");
            IMyBlockGroup Doors = GridTerminalSystem.GetBlockGroupWithName("Airlock Doors");
            if (Doors == null)
                Echo("Cannot find door group");
            IMyBlockGroup Sensors = GridTerminalSystem.GetBlockGroupWithName("Airlock Sensors");
            if (Sensors == null)
                Echo("Cannot find sensor group");

            Lights.GetBlocksOfType(LightList);
            Doors.GetBlocksOfType(DoorList);
            Sensors.GetBlocksOfType(SensorList);

            SpinningLight = GridTerminalSystem.GetBlockWithName("Airlock Rotating Light") as IMyReflectorLight;
            if (SpinningLight == null)
                Echo("Cannot find spinning light");
            Vent = GridTerminalSystem.GetBlockWithName("Airlock Vent") as IMyAirVent;
            if (Vent == null)
                Echo("Cannot find air vent");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            string pressurizationType;

            if (updateSource == UpdateType.Trigger)
            {
                //Determine which side the user is entering and ensure all doors are closed
                Calibration();
                pressurizationType = ActivatedSensor.CustomData;

                //Adjust lighting to show that the room is undergoing a pressure change
                ChangeLightProperties(Red, 0.75f);
                SpinningLight.Enabled = true;
                Echo("Lights modified");

                //Change the room pressure based on which sensor was tripped
                if (int.Parse(pressurizationType) == (int) PressureStates.Positive)
                {
                    Vent.Depressurize = false;
                    Echo("Room pressurizing");
                }
                else if (int.Parse(pressurizationType) == (int) PressureStates.Negative)
                {
                    Vent.Depressurize = true;
                    Echo("Room depressurizing");
                }

                Runtime.UpdateFrequency = UpdateFrequency.Update100;
            }
            else if (updateSource == UpdateType.Update100)
            {
                //This allows the script to execute every 200 ticks instead of the stock 100
                Update100Runs++;

                if (Update100Runs % 2 == 0)
                {
                    Echo("i-runs: " + Update100Runs.ToString());
                }
            }
        }

        public enum PressureStates
        {
            Negative,
            Positive
        }

        public void Calibration()
        {
            //Find which direction the user is entering by checking
            //which sensor is detecting the user
            ActivatedSensor = SensorList.Find(match =>
            {
                return match.IsActive;
            });
            //Find which door the user entered by checking which door is open
            ActivatedDoor = DoorList.Find(match =>
            {
                return match.Status == DoorStatus.Open;
            });

            //Close all doors to prepare for airlock cycle
            foreach (var door in DoorList)
            {
                if (door.Status != DoorStatus.Closed)
                {
                    door.CloseDoor();
                    Echo($"{door.CustomName} closed");
                }
            }

            //TODO: LCD screen setup
        }

        public bool AreDoorsShut()
        {
            //stubbed
            return false;
        }

        public bool IsAirlockPressurized()
        {
            //stubbed
            return false;
        }

        public void ChangeLightProperties(Color color, float intensity)
        {
            foreach (var light in LightList)
            {
                light.Color = color;
                light.Intensity = intensity;
            }
        }
    }
}
