using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        //----------------------------------------------------------------------
        // MapDoor
        //----------------------------------------------------------------------
        public class MapDoor
        {
            //----------------------------------------------------------------------
            // Fields
            //----------------------------------------------------------------------
            public int X;
            public int Y;
            public MapExit exit;
            //----------------------------------------------------------------------
            // Constructor
            //----------------------------------------------------------------------
            public MapDoor(int x, int y, MapExit exit)
            {
                X = x;
                Y = y;
                this.exit = exit;
            }
            public MapDoor(string data)
            {
                string[] parts = data.Split(',');
                X = int.Parse(parts[0]);
                Y = int.Parse(parts[1]);
                exit = new MapExit(parts[2] + "," + parts[3] + "," + parts[4]);
            }
            //----------------------------------------------------------------------
            // Methods
            //----------------------------------------------------------------------
            public override string ToString()
            {
                return X + "," + Y + "," + exit;
            }
        }
        //----------------------------------------------------------------------
    }
}
