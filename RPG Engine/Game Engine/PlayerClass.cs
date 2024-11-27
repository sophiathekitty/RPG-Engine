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
        //-----------------------------------------------------------------------
        // a player class
        //-----------------------------------------------------------------------
        public class PlayerClass
        {
            public string Name = "";
            public Dictionary<string, int> Stats = new Dictionary<string, int>();
            public List<string> Skills = new List<string>();
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public PlayerClass(string data)
            {
                string[] parts = data.Split(',');
                Name = parts[0].Trim();
                //GridInfo.Echo("PlayerClass: " + Name);
                for (int i = 1; i < parts.Length; i++)
                {
                    string[] stat = parts[i].Split(':');
                    Stats.Add(stat[0].Trim(), int.Parse(stat[1].Trim()));
                    //GridInfo.Echo("stat: " + stat[0] + " = " + stat[1]);
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
