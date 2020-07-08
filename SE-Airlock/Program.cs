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
        IMySensorBlock OppositeSensor;
        IMyDoor ActivatedDoor;

        int Update100Runs = 1;
        bool FinishedCycle = false;
        PressureStates state;

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
                calibration();
                pressurizationType = ActivatedSensor.CustomData;
                state = getRequestedPressure(pressurizationType);

                //Adjust lighting to show that the room is undergoing a pressure change
                changeLightProperties(Red, 0.75f);
                SpinningLight.Enabled = true;
                Echo("Lights modified");

                OppositeSensor.Enabled = false;

                //Change the room pressure based on which sensor was tripped
                if (state == PressureStates.Positive)
                {
                    Vent.Depressurize = false;
                    Echo("Room pressurizing");
                }
                else if (state == PressureStates.Negative)
                {
                    Vent.Depressurize = true;
                    Echo("Room depressurizing");
                }

                Runtime.UpdateFrequency = UpdateFrequency.Update100;
            }
            else if (updateSource == UpdateType.Update100)
            {
                //This allows the script to execute every 300 ticks instead of the stock 100
                Update100Runs++;
                Echo(FinishedCycle.ToString());

                if (Update100Runs % 3 == 0 && !FinishedCycle)
                {
                    Echo("i-runs: " + Update100Runs.ToString());

                    if (isAirlockPressurized(state))
                    {
                        //Change the lights to green to indicate it is finished cycling
                        changeLightProperties(Green, 0.85f);
                        SpinningLight.Enabled = false;

                        //Open the door on the other side to allow exit
                        IMyDoor oppositeDoor = getOppositeDoor();
                        oppositeDoor.OpenDoor();
                        Echo($"{oppositeDoor.CustomName} opened");

                        FinishedCycle = true;
                        Update100Runs = 0;
                    }
                    else
                    {
                        Echo("ERROR");
                    }
                }
                else if (Update100Runs % 5 == 0 && FinishedCycle)
                {
                    changeLightProperties(NormalColor, 1.5f);
                    OppositeSensor.Enabled = true;
                    Echo("Reset to beginning");

                    //The script doesn't need to run anymore so disable further runs
                    Runtime.UpdateFrequency = UpdateFrequency.None;

                    Update100Runs = 0;
                }
            }
        }

        public enum PressureStates
        {
            Negative,
            Positive
        }

        public void calibration()
        {
            FinishedCycle = false;

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
            //Determine which sensor is the one not active
            OppositeSensor = SensorList.Find(match =>
            {
                return !match.IsActive;
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

        public IMyDoor getOppositeDoor()
        {
            List<IMyDoor> tempList = new List<IMyDoor>(DoorList);
            tempList.Remove(ActivatedDoor);
            
            return tempList[0];
        }

        public PressureStates getRequestedPressure(string type)
        {
            if (int.Parse(type) == (int)PressureStates.Positive)
            {
                return PressureStates.Positive;
            }
            else if (int.Parse(type) == (int)PressureStates.Negative)
            {
                return PressureStates.Negative;
            }

            return PressureStates.Negative;
        }

        public bool isAirlockPressurized(PressureStates state)
        {
            float oxygen = Vent.GetOxygenLevel();

            if (state == PressureStates.Positive && oxygen != 1.0f)
            {
                return false;
            }
            else if (state == PressureStates.Negative && oxygen < 0.000000006f)
            {
                return false;
            }

            return true;
        }

        public void changeLightProperties(Color color, float intensity)
        {
            foreach (var light in LightList)
            {
                light.Color = color;
                light.Intensity = intensity;
            }
        }
    }
}
