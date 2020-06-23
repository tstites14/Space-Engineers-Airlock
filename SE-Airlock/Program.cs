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
        Color red = new Color(255, 0, 0);
        Color green = new Color(0, 255, 0);

        List<IMyInteriorLight> lightList;
        List<IMyDoor> doorList;
        List<IMySensorBlock> sensorList;
        List<IMyTextPanel> screenList;
        List<IMyButtonPanel> buttonList;

        IMyReflectorLight spinningLight;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            IMyBlockGroup lights = GridTerminalSystem.GetBlockGroupWithName("Airlock Lights");
            IMyBlockGroup doors = GridTerminalSystem.GetBlockGroupWithName("Airlock Doors");
            IMyBlockGroup sensors = GridTerminalSystem.GetBlockGroupWithName("Airlock Sensors");
            IMyBlockGroup screens = GridTerminalSystem.GetBlockGroupWithName("Airlock Screens");
            IMyBlockGroup buttons = GridTerminalSystem.GetBlockGroupWithName("Airlock Buttons");

            lights.GetBlocksOfType(lightList);
            doors.GetBlocksOfType(doorList);
            sensors.GetBlocksOfType(sensorList);
            screens.GetBlocksOfType(screenList);
            buttons.GetBlocksOfType(buttonList);

            spinningLight = GridTerminalSystem.GetBlockWithName("Airlock Rotating Light") as IMyReflectorLight;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.
        }
    }
}
