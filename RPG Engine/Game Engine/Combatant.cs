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
        // a character that can be in combat
        //-----------------------------------------------------------------------
        public class Combatant
        {
            public string Name = "";
            public Dictionary<string,double> Stats = new Dictionary<string, double>();
            public List<string> Status = new List<string>();
            public Dictionary<string, int> MaxStats = new Dictionary<string, int>();
            public List<string> Actions = new List<string>();
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public Combatant(string data)
            {
                string[] parts = data.Split('╔');
                Name = parts[0];
                //GridInfo.Echo("Combatant: " + Name);
                foreach (string part in parts)
                {
                    string[] subParts = part.Trim().Split(':');
                    if (subParts[0] == "stat")
                    {
                        string[] statParts = subParts[1].Split('=');
                        Stats.Add(statParts[0], double.Parse(statParts[1]));
                        //GridInfo.Echo("stat: " + statParts[0] + " = " + statParts[1]);
                    }
                    else if (subParts[0] == "maxstat")
                    {
                        string[] statParts = subParts[1].Split('=');
                        MaxStats.Add(statParts[0], int.Parse(statParts[1]));
                        //GridInfo.Echo("maxstat: " + statParts[0] + " = " + statParts[1]);
                    }
                    else if (subParts[0] == "status")
                    {
                        Status.Add(subParts[1]);
                        //GridInfo.Echo("status: " + subParts[1]);
                    }
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
