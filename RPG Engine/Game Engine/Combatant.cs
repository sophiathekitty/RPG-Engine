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
        }
        //-----------------------------------------------------------------------
    }
}
