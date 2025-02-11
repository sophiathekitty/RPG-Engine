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
            public Dictionary<string, Dictionary<string, string>> Skills = new Dictionary<string, Dictionary<string, string>>(); // skills
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
                try
                {

                    GridInfo.Echo("GameData: Loading...");
                    if (uiBuilder != null) uiBuilder._gameData = this;
                    gameName = game;
                    string Data = GridDB.Get(gameName + ".GameData.0.CustomData");
                    string[] parts = Data.Split('║');
                    //
                    // player start
                    //
                    string[] playerStart = parts[0].Split(',');
                    if (playerStart.Length < 3) return;
                    playerPos = new Vector2(float.Parse(playerStart[0]), float.Parse(playerStart[1]));
                    mapIndex = int.Parse(playerStart[2]);
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
                    if (parts.Length < 3) return;
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
                            if (Strings.ContainsKey(sParts[0].Trim())) throw new Exception("GameData: Duplicate string: " + sParts[0].Trim());
                            Strings.Add(sParts[0].Trim(), sParts[1].Trim());
                        }
                    }
                    if (parts.Length < 5) return;
                    //
                    // actions
                    //
                    //GridInfo.Echo("GameData:loading actions...");
                    string[] actions = parts[4].Split('═');
                    foreach (string a in actions)
                    {
                        if (!a.Contains("╗")) continue;
                        string[] aParts = a.Split('╗');
                        if (aParts.Length == 2)
                        {
                            try
                            {

                                //GridInfo.Echo("GameData:loading action: " + aParts[0].Trim());
                                if (Actions.ContainsKey(aParts[0].Trim())) throw new Exception("GameData: Duplicate action: " + aParts[0].Trim());
                                Actions.Add(aParts[0].Trim(), new GameAction(aParts[0].Trim(), aParts[1].Trim(), this, uiBuilder));
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Action: " + aParts[0].Trim() + " - " + e.Message);
                            }
                        }
                    }
                    if (parts.Length < 6) return;
                    //GridInfo.Echo("GameData:loading items...");
                    //
                    // items
                    //
                    string[] items = parts[5].Trim().Split('═');
                    foreach (string i in items)
                    {
                        //if (!i.Contains("╗")) continue;
                        string[] iParts = i.Trim().Split('╗');
                        Dictionary<string, string> item = new Dictionary<string, string>();
                        if (iParts.Length == 2)
                        {
                            string[] itemParts = iParts[1].Trim().Split('╔');
                            foreach (string ip in itemParts)
                            {
                                if (!ip.Contains("╚")) continue;
                                string[] ipParts = ip.Trim().Split('╚');
                                if (ipParts.Length == 2)
                                {
                                    if (item.ContainsKey(ipParts[0])) throw new Exception("GameData: Duplicate item var for " + iParts[0] + ": " + ipParts[0]);
                                    item.Add(ipParts[0], ipParts[1]);
                                }
                            }
                        }
                        if (Items.ContainsKey(iParts[0].Trim())) throw new Exception("GameData: Duplicate item: " + iParts[0]);
                        Items.Add(iParts[0], item);
                        //GridInfo.Echo("Item: " + iParts[0].Trim()+" - stats count: " + item.Count);
                    }
                    //GridInfo.Echo("GameData:loading jobs...");
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
                    //GridInfo.Echo("GameData:loading enemies...");
                    //
                    // enemies
                    //
                    if (parts.Length < 8) return;
                    string[] enemies = parts[7].Split('═');
                    foreach (string e in enemies)
                    {
                        EnemyCombatant enemy = new EnemyCombatant(e.Trim());
                        if (EnemyDefenitions.ContainsKey(enemy.Name)) throw new Exception("GameData: Duplicate enemy: " + enemy.Name);
                        EnemyDefenitions.Add(enemy.Name, enemy);
                    }
                    //GridInfo.Echo("GameData:loading skills...");
                    //
                    // skills
                    //
                    if (parts.Length < 9) return;
                    string[] skills = parts[8].Trim().Split('═');
                    foreach (string s in skills)
                    {
                        string[] sParts = s.Trim().Split('╗');
                        if (sParts.Length == 2)
                        {
                            string[] skillVars = sParts[1].Trim().Split('╔');
                            Dictionary<string, string> vars = new Dictionary<string, string>();
                            foreach (string v in skillVars)
                            {
                                string[] vParts = v.Trim().Split('╚');
                                if (vParts.Length == 2)
                                {
                                    if (vars.ContainsKey(vParts[0])) throw new Exception("GameData: Duplicate skill var for " + sParts[0] + ": " + vParts[0]);
                                    vars.Add(vParts[0], vParts[1]);
                                }
                            }
                            if (Skills.ContainsKey(sParts[0])) throw new Exception("GameData: Duplicate skill: " + sParts[0]);
                            Skills.Add(sParts[0], vars);
                        }
                    }
                    //GridInfo.Echo("GameData: " + gameName + " loaded");
                }
                catch (Exception e)
                {
                    throw new Exception("Error Loading data: " + e.Message);
                }
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
            //-----------------------------------------------------------------------
            // parse a string for variables
            //-----------------------------------------------------------------------
            public string ParseString(string text)
            {
                string newText = text;
                foreach (KeyValuePair<string, string> s in Strings)
                {
                    newText = newText.Replace("$" + s.Key, s.Value);
                }
                foreach (KeyValuePair<string, int> i in Ints)
                {
                    newText = newText.Replace("#" + i.Key, i.Value.ToString());
                }
                return newText;
            }
        }
        //-----------------------------------------------------------------------
    }
}
