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
        // one of the player's characters
        //-----------------------------------------------------------------------
        public class PlayerCharacter : Combatant
        {
            public Dictionary<string,string> Gear = new Dictionary<string, string>();
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public PlayerCharacter(string data) : base(data)
            {
                string[] parts = data.Split('╔');
                foreach (string part in parts)
                {
                    string[] subParts = part.Split(':');
                    if (subParts[0] == "gear")
                    {
                        string[] gearParts = subParts[1].Split('=');
                        Gear.Add(gearParts[0], gearParts[1]);
                    }
                }
            }
            public PlayerCharacter(string name, PlayerClass playerClass) : base(name)
            {
                GridInfo.Echo("PlayerCharacter: " + Name);
                foreach (KeyValuePair<string, int> stat in playerClass.Stats)
                {
                    Stats.Add(stat.Key, stat.Value);
                    MaxStats.Add(stat.Key, stat.Value);
                    GridInfo.Echo("stat: " + stat.Key + " = " + stat.Value);
                }
                foreach (string skill in playerClass.Skills)
                {
                    Actions.Add(skill);
                    GridInfo.Echo("skill: " + skill);
                }
            }
            //-----------------------------------------------------------------------
            // save the character to a string
            //-----------------------------------------------------------------------
            public string Save()
            {
                string data = Name;
                foreach (KeyValuePair<string, string> gear in Gear)
                {
                    data += "╔gear:" + gear.Key + "=" + gear.Value;
                }
                foreach (KeyValuePair<string, double> stat in Stats)
                {
                    data += "╔stat:" + stat.Key + "=" + stat.Value;
                }
                foreach (KeyValuePair<string, int> maxStat in MaxStats)
                {
                    data += "╔maxstat:" + maxStat.Key + "=" + maxStat.Value;
                }
                foreach (string status in Status)
                {
                    data += "╔status:" + status;
                }
                return data;
            }
        }
        //-----------------------------------------------------------------------
    }
}
