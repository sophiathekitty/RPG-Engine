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
        // MapExit
        //----------------------------------------------------------------------
        public class MapExit
        {
            //----------------------------------------------------------------------
            // Fields
            //----------------------------------------------------------------------
            public int Id { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public bool IsValid { get { return Id != -1; } }
            //----------------------------------------------------------------------
            // Constructor
            //----------------------------------------------------------------------
            public MapExit(int id, int x, int y)
            {
                Id = id;
                X = x;
                Y = y;
            }
            public MapExit()
            {
                Id = -1;
                X = 0;
                Y = 0;
            }
            public MapExit(string data)
            {
                string[] parts = data.Split(',');
                Id = int.Parse(parts[0]);
                X = int.Parse(parts[1]);
                Y = int.Parse(parts[2]);
            }
            //----------------------------------------------------------------------
            // Methods
            //----------------------------------------------------------------------
            public string Serialize()
            {
                return Id + "," + X + "," + Y;
            }
        }
        //----------------------------------------------------------------------
    }
}
