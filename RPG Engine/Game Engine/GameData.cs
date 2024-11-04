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
        // holds the data for the game
        //-----------------------------------------------------------------------
        public class GameData
        {
            //-----------------------------------------------------------------------
            // fields
            //-----------------------------------------------------------------------
            // game settings (some also used for save data)
            public string gameName;
            public Vector2 playerPos = Vector2.Zero;
            public int mapIndex = 0;
            public bool actionMenu = false; // dragon warrior style action menu for all interactions with NPCs
            public int maxPartySize = 4; // max number of characters in the party
            public Dictionary<string, bool> Bools = new Dictionary<string, bool>();
            public Dictionary<string, double> Numbers = new Dictionary<string, double>();
            public Dictionary<string, GameAction> Actions = new Dictionary<string, GameAction>();
            // save data
            public int saveIndex = 0;
            public Dictionary<string, int> Inventory = new Dictionary<string, int>();
            public List<PlayerCharacter> CharacterList = new List<PlayerCharacter>();
            public List<EnemyCombatant> EnemyList = new List<EnemyCombatant>();
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public GameData(string game)
            {
                gameName = game;
                string Data = GridDB.Get(gameName + ".GameData.0.CustomData");
                string[] parts = Data.Split('║');
                string[] playerStart = parts[0].Split(',');
                if(playerStart.Length < 3) return;
                playerPos = new Vector2(float.Parse(playerStart[0]), float.Parse(playerStart[1]));
                mapIndex = int.Parse(playerStart[2]);
                // bools
                if (parts.Length < 2) return;
                string[] bools = parts[1].Split('\n');
                foreach (string b in bools)
                {
                    if(!b.Contains(":")) continue;
                    string[] bParts = b.Split(':');
                    if (bParts.Length == 2)
                    {
                        Bools.Add(bParts[0].Trim(), bool.Parse(bParts[1].Trim()));
                    }
                }
                if(parts.Length < 3) return;
                // numbers
                string[] numbers = parts[2].Split('\n');
                foreach (string n in numbers)
                {
                    if (!n.Contains(":")) continue;
                    string[] nParts = n.Split(':');
                    if (nParts.Length == 2)
                    {
                        Numbers.Add(nParts[0].Trim(), double.Parse(nParts[1].Trim()));
                    }
                }
                if (parts.Length < 4) return;
                // actions
                string[] actions = parts[3].Split('\n');
                foreach (string a in actions)
                {
                    if (!a.Contains("═")) continue;
                    string[] aParts = a.Split('═');
                    if (aParts.Length == 2)
                    {
                        Actions.Add(aParts[0].Trim(), new GameAction(aParts[1].Trim()));
                    }
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
