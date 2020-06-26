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
        List<IMyTextPanel> ScreenList;
        List<IMyButtonPanel> ButtonList;

        IMyReflectorLight SpinningLight;
        IMyAirVent Vent;

        //Airlock is currently cycling
        bool IsCycling;

        //The airlock contains oxygen
        bool IsPressurized;

        //Is the airlock going to pump oxygen in or not
        bool VentMode;

        //Is there a player actually in the airlock right now
        bool PlayerInAirlock;

        public Program()
        {
            //Update block every 10 ticks since running every tick is unnecessary
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            //Attempt to convert value from Storage into IsCycling
            Boolean.TryParse(Storage, out IsCycling);

            LightList = new List<IMyInteriorLight>();
            DoorList = new List<IMyDoor>();
            SensorList = new List<IMySensorBlock>();
            ScreenList = new List<IMyTextPanel>();
            ButtonList = new List<IMyButtonPanel>();

            IMyBlockGroup Lights = GridTerminalSystem.GetBlockGroupWithName("Airlock Lights");
            IMyBlockGroup Doors = GridTerminalSystem.GetBlockGroupWithName("Airlock Doors");
            IMyBlockGroup Sensors = GridTerminalSystem.GetBlockGroupWithName("Airlock Sensors");
            IMyBlockGroup Screens = GridTerminalSystem.GetBlockGroupWithName("Airlock Screens");
            IMyBlockGroup Buttons = GridTerminalSystem.GetBlockGroupWithName("Airlock Buttons");

            Lights.GetBlocksOfType(LightList);
            Doors.GetBlocksOfType(DoorList);
            Sensors.GetBlocksOfType(SensorList);
            Screens.GetBlocksOfType(ScreenList);
            Buttons.GetBlocksOfType(ButtonList);

            SpinningLight = GridTerminalSystem.GetBlockWithName("Airlock Rotating Light") as IMyReflectorLight;
            Vent = GridTerminalSystem.GetBlockWithName("Airlock Vent") as IMyAirVent;
        }

        public void Save()
        {
            Storage = IsCycling.ToString();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!IsArgumentValid(argument))
            {
                Echo("Argument provided: " + argument);
                Echo("Please rerun this program with the argument positive or negative");
                return;
            }
            else
            {
                VentMode = ArgumentToBoolean(argument);
                Echo("Vent Mode: " + VentMode.ToString());
            }
            Echo("Update Type: " + updateSource.ToString());
            //Airlock only needs to run initial setup once so that is kept separate for optimization reasons
            if (updateSource == UpdateType.Trigger && updateSource != UpdateType.Update10)
            {
                if (!IsAirlockPressurized() == VentMode)
                {
                    InitialSetup();

                    CycleAirlock(VentMode);
                }
            }
            else if (updateSource == UpdateType.Update10)
            {
                Echo("UPDATE 10");
                //Doors should always remain closed during either cycle, so check every time script is run
                if (!AreDoorsShut())
                {
                    CycleDoors();
                }

                if (IsAirlockPressurized() == VentMode)
                {
                    Echo("Airlock pressurized");
                    IsCycling = false;
                    GetOpeningDoor().OpenDoor();

                    ChangeLightProperties(NormalColor, 2.0f);
                    SpinningLight.Enabled = false;

                    SetSensorEnable(true);

                    Runtime.UpdateFrequency = UpdateFrequency.None;
                }
            }
        }

        public bool IsArgumentValid(string arg)
        {
            return arg == "" || arg == null || (arg != "positive" && arg != "negative");
        }

        public bool ArgumentToBoolean(string arg)
        {
            return arg == "positive";
        }

        public void InitialSetup()
        {
            PlayerInAirlock = true;

            ChangeLightProperties(Red, 0.75f);
            SpinningLight.Enabled = true;
            Echo("Lights changed");

            if (!AreDoorsShut())
            {
                CycleDoors();
                Echo("Doors cycled");
            }

            SetSensorEnable(false);
            Echo("Sensors disabled");
        }

        public void SetSensorEnable(bool enabled)
        {
            foreach (var sensor in SensorList)
            {
                sensor.Enabled = enabled;
            }
        }

        public bool AreDoorsShut()
        {
            foreach (var door in DoorList)
            {
                if (door.Status != DoorStatus.Closed)
                {
                    return false;
                }
            }

            return true;
        }

        public void CycleDoors()
        {
            foreach (var door in DoorList)
            {
                if (door.Status == DoorStatus.Open || door.Status == DoorStatus.Opening)
                {
                    door.CloseDoor();
                }
            }
        }

        public IMyDoor GetOpeningDoor()
        {
            return DoorList.Find(match =>
            {
                if (VentMode)
                {
                    return match.CustomName.Contains("External");
                }
                else
                {
                    return match.CustomName.Contains("Internal");
                }
            });
        }

        public IMySensorBlock GetOpeningSensor()
        {
            return SensorList.Find(match =>
            {
                if (VentMode)
                {
                    return match.CustomName.Contains("Exterior");
                }
                else
                {
                    return match.CustomName.Contains("Interior");
                }
            });
        }

        public bool IsAirlockPressurized()
        {
            if (Vent.CanPressurize && Vent.GetOxygenLevel() == 1.0f)
            {
                return true;
            }

            return false;
        }

        public void CycleAirlock(bool mode)
        {
            IsCycling = true;

            //True = positive pressure
            //False = no pressure
            if (mode)
            {
                Vent.Depressurize = false;
                Echo("Room Pressurizing");
            }
            else
            {
                Vent.Depressurize = true;
                Echo("Room Depressurizing");
            }
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
