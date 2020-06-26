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

        IMyReflectorLight SpinningLight;
        IMyAirVent Vent;

        public Program()
        {
            LightList = new List<IMyInteriorLight>();
            DoorList = new List<IMyDoor>();
            SensorList = new List<IMySensorBlock>();

            IMyBlockGroup Lights = GridTerminalSystem.GetBlockGroupWithName("Airlock Lights");
            IMyBlockGroup Doors = GridTerminalSystem.GetBlockGroupWithName("Airlock Doors");
            IMyBlockGroup Sensors = GridTerminalSystem.GetBlockGroupWithName("Airlock Sensors");

            Lights.GetBlocksOfType(LightList);
            Doors.GetBlocksOfType(DoorList);
            Sensors.GetBlocksOfType(SensorList);

            SpinningLight = GridTerminalSystem.GetBlockWithName("Airlock Rotating Light") as IMyReflectorLight;
            Vent = GridTerminalSystem.GetBlockWithName("Airlock Vent") as IMyAirVent;
        }

        public void Save()
        {
             //stubbed
        }

        public void Main(string argument, UpdateType updateSource)
        {
            //stubbed
        }

        public void Calibration()
        {
            //stubbed
        }

        public bool AreDoorsShut()
        {
            //stubbed
            return false;
        }

        public void CloseDoors()
        {
            //stubbed
        }

        public IMyDoor GetOppositeDoor()
        {
            //stubbed
            return null;
        }

        public IMySensorBlock GetOppositeSensor()
        {
            //stubbed
            return null;
        }

        public bool IsAirlockPressurized()
        {
            //stubbed
            return false;
        }

        public void CycleAirlock()
        {
            //stubbed
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
