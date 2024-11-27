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
            public Dictionary<string, bool> Bools = new Dictionary<string, bool>(); // also save data
            public Dictionary<string, int> Ints = new Dictionary<string, int>(); // also save data
            public Dictionary<string, string> Strings = new Dictionary<string, string>();
            public Dictionary<string, GameAction> Actions = new Dictionary<string, GameAction>();
            public Dictionary<string, Dictionary<string, string>> Items = new Dictionary<string, Dictionary<string, string>>();
            public Dictionary<string, PlayerClass> Jobs = new Dictionary<string, PlayerClass>(); // default stats for player classes
            public Dictionary<string, EnemyCombatant> EnemyDefenitions = new Dictionary<string, EnemyCombatant>(); // all possible enemies
            //string actionsTemp = "";
            // save data
            public int saveIndex = 0;
            public Dictionary<string, int> Inventory = new Dictionary<string, int>();
            public List<PlayerCharacter> CharacterList = new List<PlayerCharacter>(); // the player party
            // game objects
            public List<EnemyCombatant> EnemyList = new List<EnemyCombatant>(); // for current battles
            //public Dictionary<string, List<string>> Lists = new Dictionary<string, List<string>>();
            public PlayerSprite playerSprite;
            //public NPC npc;
            public TileMap map;
            public Stack<MapExit> ExitStack = new Stack<MapExit>();
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public GameData(string game, GameUILayoutBuilder uiBuilder)
            {
                if (uiBuilder != null) uiBuilder._gameData = this;
                gameName = game;
                string Data = GridDB.Get(gameName + ".GameData.0.CustomData");
                string[] parts = Data.Split('║');
                //
                // player start
                //
                string[] playerStart = parts[0].Split(',');
                if(playerStart.Length < 3) return;
                playerPos = new Vector2(float.Parse(playerStart[0]), float.Parse(playerStart[1]));
                mapIndex = int.Parse(playerStart[2]);
                //
                // bools
                //
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
                //
                // ints
                //
                string[] numbers = parts[2].Split('\n');
                foreach (string n in numbers)
                {
                    if (!n.Contains(":")) continue;
                    string[] nParts = n.Split(':');
                    if (nParts.Length == 2)
                    {
                        Ints.Add(nParts[0].Trim(), int.Parse(nParts[1].Trim()));
                    }
                }
                if (parts.Length < 4) return;
                //
                // strings
                //
                string[] strings = parts[3].Split('═');
                foreach (string s in strings)
                {
                    if (!s.Contains("╗")) continue;
                    string[] sParts = s.Split('╗');
                    if (sParts.Length == 2)
                    {
                        //GridInfo.Echo("Adding string: " + sParts[0].Trim() + " = " + sParts[1].Trim().Replace("\r\n","\n").Replace("\r", "\n"));
                        Strings.Add(sParts[0].Trim(), sParts[1].Trim());
                    }
                }
                if (parts.Length < 5) return;
                //
                // actions
                //
                string[] actions = parts[4].Split('═');
                foreach (string a in actions)
                {
                    if (!a.Contains("╗")) continue;
                    string[] aParts = a.Split('╗');
                    if (aParts.Length == 2)
                    {
                        Actions.Add(aParts[0].Trim(), new GameAction(aParts[1].Trim(),this,uiBuilder));
                    }
                }
                if (parts.Length < 6) return;
                //
                // items
                //
                string[] items = parts[5].Split('═');
                foreach (string i in items)
                {
                    //if (!i.Contains("╗")) continue;
                    string[] iParts = i.Split('╗');
                    Dictionary<string, string> item = new Dictionary<string, string>();
                    if (iParts.Length == 2)
                    {
                        string[] itemParts = iParts[1].Split('╔');
                        foreach (string ip in itemParts)
                        {
                            if (!ip.Contains("╚")) continue;
                            string[] ipParts = ip.Trim().Split('╚');
                            if (ipParts.Length == 2)
                            {
                                item.Add(ipParts[0], ipParts[1]);
                            }
                        }
                    }
                    Items.Add(iParts[0].Trim(), item);
                    //GridInfo.Echo("Item: " + iParts[0].Trim()+" - stats count: " + item.Count);
                }
                //
                // player classes
                //
                if (parts.Length < 7) return;
                string[] classes = parts[6].Split('═');
                foreach (string c in classes)
                {
                    PlayerClass job = new PlayerClass(c.Trim());
                    Jobs.Add(job.Name, job);
                }
                GridInfo.Echo("GameData: " + gameName + " loaded");
            }
            //-----------------------------------------------------------------------
            // save game data
            //-----------------------------------------------------------------------
            public string SaveData()
            {
                //
                // player position, map index, bools, ints, strings
                //
                string data = playerSprite.MapPosition.X + "," + playerSprite.MapPosition.Y + "," + map.index + "║";
                //
                // bools
                //
                foreach (KeyValuePair<string, bool> b in Bools)
                {
                    data += b.Key + ": " + b.Value + "\n";
                }
                data += "║";
                //
                // ints
                //
                foreach (KeyValuePair<string, int> i in Ints)
                {
                    data += i.Key + ": " + i.Value + "\n";
                }
                data += "║";
                //
                // inventory
                //
                foreach (KeyValuePair<string, int> i in Inventory)
                {
                    data += i.Key + ": " + i.Value + "\n";
                }
                // party data
                return data;
            }
            //-----------------------------------------------------------------------
            // load game data
            //-----------------------------------------------------------------------
            public void LoadData(string data)
            {
                string[] parts = data.Split('║');
                //
                // bools
                //
                if (parts.Length < 2) return;
                string[] bools = parts[1].Split('\n');
                foreach (string b in bools)
                {
                    if (!b.Contains(":")) continue;
                    string[] bParts = b.Split(':');
                    if (bParts.Length == 2)
                    {
                        Bools.Add(bParts[0].Trim(), bool.Parse(bParts[1].Trim()));
                    }
                }
                //
                // ints
                //
                string[] numbers = parts[2].Split('\n');
                foreach (string n in numbers)
                {
                    if (!n.Contains(":")) continue;
                    string[] nParts = n.Split(':');
                    if (nParts.Length == 2)
                    {
                        Ints.Add(nParts[0].Trim(), int.Parse(nParts[1].Trim()));
                    }
                }
                //
                // inventory
                //
                string[] inventory = parts[3].Split('\n');
                foreach (string i in inventory)
                {
                    if (!i.Contains(":")) continue;
                    string[] iParts = i.Split(':');
                    if (iParts.Length == 2)
                    {
                        Inventory.Add(iParts[0].Trim(), int.Parse(iParts[1].Trim()));
                    }
                }
                //
                // player location (and load map)
                //
                string[] playerStart = parts[0].Split(',');
                if (playerStart.Length < 3) throw new Exception("GameData.LoadData: Invalid player start data");
                playerSprite.MapPosition = new Vector2(float.Parse(playerStart[0]), float.Parse(playerStart[1]));
                int.TryParse(playerStart[2], out mapIndex);
                map.Load(mapIndex);
            }
        }
        //-----------------------------------------------------------------------
    }
}
