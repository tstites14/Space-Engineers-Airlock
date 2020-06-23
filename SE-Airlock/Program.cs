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
        Color Red = new Color(255, 0, 0);
        Color Green = new Color(0, 255, 0);

        List<IMyInteriorLight> LightList;
        List<IMyDoor> DoorList;
        List<IMySensorBlock> SensorList;
        List<IMyTextPanel> ScreenList;
        List<IMyButtonPanel> ButtonList;

        IMyReflectorLight SpinningLight;
        IMyAirVent Vent;

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

        }

        public void Main(string argument, UpdateType updateSource)
        {
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
            if (Vent.CanPressurize && Vent.GetOxygenLevel() < 0.0f)
            {
                return true;
            }

            return false;
        }
    }
}
