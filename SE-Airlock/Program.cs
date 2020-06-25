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

        bool IsCycling;
        bool PositivePressure;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

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
            if (IsArgumentValid(argument))
            {
                Echo("Please rerun this program with the argument positive or negative");
                return;
            }

            InitialSetup();

            CycleAirlock(argument);
        }

        public bool IsArgumentValid(string arg)
        {
            return arg == "" || arg == null || (arg != "positive" && arg != "negative");
        }

        public void InitialSetup()
        {
            ChangeLightColor(Red);
            SpinningLight.Enabled = true;

            if (!AreDoorsShut())
            {
                CycleDoors();
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

        public bool IsAirlockPressurized()
        {
            if (Vent.CanPressurize && Vent.GetOxygenLevel() == 1.0f)
            {
                return true;
            }

            return false;
        }

        public void CycleAirlock(string type)
        {
            IsCycling = true;

            switch (type)
            {
                case "positive":
                    Vent.Depressurize = false;
                    PositivePressure = true;
                    break;
                case "negative":
                    Vent.Depressurize = true;
                    PositivePressure = false;
                    break;
            }
        }

        public void ChangeLightColor(Color color)
        {
            foreach (var light in LightList)
            {
                light.Color = color;
            }
        }
    }
}
