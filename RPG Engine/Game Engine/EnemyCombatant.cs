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
        // an enemy combatant (monster)
        //-----------------------------------------------------------------------
        public class EnemyCombatant : Combatant
        {
            public List<string> Drops = new List<string>();
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public EnemyCombatant(string data) : base(data)
            {
                string[] parts = data.Split('╔');
                foreach (string part in parts)
                {
                    string[] subParts = part.Trim().Split(':');
                    if (subParts[0] == "action") Actions.Add(subParts[1]);
                    else if (subParts[0] == "drop") Drops.Add(subParts[1]);
                }
            }
            public EnemyCombatant(EnemyCombatant enemy) : base(enemy.Name)
            {
                // copy enemy data to new enemy instance
                Stats = new Dictionary<string, double>(enemy.Stats);
                Status = new List<string>(enemy.Status);
                MaxStats = new Dictionary<string, int>(enemy.MaxStats);
                Actions = new List<string>(enemy.Actions);
                Drops = new List<string>(enemy.Drops);

            }
        }
        //-----------------------------------------------------------------------
    }
}
