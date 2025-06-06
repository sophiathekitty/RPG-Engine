﻿using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
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
        // an addressable variable for a game action
        //-----------------------------------------------------------------------
        public class GameActionVariable
        {
            GameAction action; // the action that owns this variable
            GameData gameData; // where data is stored
            public string address { get; private set; } // address of the variable
            string value = ""; // value of the property
            public GameActionVariable(string address, GameData gameData, GameAction action, string defaultValue = "", bool debug = false)
            {
                if(debug) GridInfo.Echo("GameActionVariable: " + address);
                this.action = action;
                this.gameData = gameData;
                if(address.StartsWith("@"))
                {
                    if (debug) GridInfo.Echo("Is a Variable");
                    this.address = address.Substring(1);
                    value = defaultValue;
                }
                else
                {
                    if (debug) GridInfo.Echo("Is a Value");
                    value = address;
                    this.address = "";
                }
                if (debug) GridInfo.Echo("GameActionVariable: " + this.address + " = " + value);
            }
            public GameActionVariable ApplyAddressIndexes()
            {
                string adr = "@"+address;
                foreach (string key in gameData.Ints.Keys)
                {
                    adr = adr.Replace("#" + key, gameData.Ints[key].ToString());
                }
                foreach (string key in gameData.Strings.Keys)
                {
                    adr = adr.Replace("$" + key, gameData.Strings[key]);
                }
                return new GameActionVariable(adr, gameData, action, value);
            }
            string parseKey(string key)
            {
                if (key.StartsWith("$")) return gameData.Strings[key.Substring(1)];
                if (key.StartsWith("#")) return gameData.Ints[key.Substring(1)].ToString();
                return key;
            }
            public string Value
            {
                get
                {
                    try
                    {

                        // see if address is null or empty
                        if (string.IsNullOrEmpty(address)) return value;
                        string[] parts = address.Split('.');
                        parts[0] = parts[0].ToLower();
                        if (parts[0] == "bools" && gameData.Bools.ContainsKey(parts[1])) return gameData.Bools[parts[1]].ToString();
                        else if (parts[0] == "ints" && gameData.Ints.ContainsKey(parts[1])) return gameData.Ints[parts[1]].ToString();
                        else if (parts[0] == "inventory")
                        {
                            if (gameData.Inventory.ContainsKey(parts[1])) return gameData.Inventory[parts[1]].ToString();
                            else
                            {
                                if (parts[1] == "Count")
                                {
                                    //GridInfo.Echo("Inventory count: " + gameData.Inventory.Count);
                                    return gameData.Inventory.Count.ToString();
                                }
                                else if (parts[1] == "Keys")
                                {
                                    if (parts.Length > 2)
                                    {
                                        if (parts[2].StartsWith("#")) return gameData.Inventory.Keys.ToArray()[gameData.Ints[parts[2].Substring(1)]];
                                        int index = 0;
                                        if (int.TryParse(parts[2], out index))
                                        {
                                            return gameData.Inventory.Keys.ToArray()[index];
                                        }
                                        else if (gameData.Ints.ContainsKey(parts[2])) return gameData.Inventory.Keys.ToArray()[gameData.Ints[parts[2]]];
                                    }
                                    return gameData.Inventory.Keys.ToString();
                                }
                                else if (parts[1].StartsWith("$"))
                                {
                                    string key = gameData.Strings[parts[1].Substring(1)];
                                    if (gameData.Inventory.ContainsKey(key)) return gameData.Inventory[key].ToString();
                                }
                            }
                            return "0";
                        }
                        else if (parts[0] == "strings" && gameData.Strings.ContainsKey(parts[1])) return gameData.Strings[parts[1]];
                        else if (parts[0] == "player")
                        {
                            if (parts[1] == "X") return gameData.playerSprite.X.ToString();
                            else if (parts[1] == "Y") return gameData.playerSprite.Y.ToString();
                            else if (parts[1] == "Direction") return gameData.playerSprite.Direction.ToString();
                            else if (parts[1] == "SpriteId") return gameData.playerSprite.SpriteIndex.ToString();
                            else if (parts[1] == "Visible") return gameData.playerSprite.Visible.ToString();
                        }
                        else if (parts[0] == "npc" && action.npc != null)
                        {
                            if (parts[1] == "X") return action.npc.X.ToString();
                            else if (parts[1] == "Y") return action.npc.Y.ToString();
                            else if (parts[1] == "Direction") return action.npc.Direction.ToString();
                            else if (parts[1] == "SpriteId") return action.npc.SpriteIndex.ToString();
                            else if (parts[1] == "Enabled") return action.npc.Enabled.ToString();
                        }
                        else if (parts[0] == "map")
                        {
                            if (parts[1] == "Visible") return gameData.map.Visible.ToString();
                            else if (parts[1] == "id") return gameData.map.index.ToString();
                            else if (parts[1] == "Layer") return gameData.map.TileLayer(gameData.playerPos).ToString();
                        }
                        else if (parts[0] == "gridinfo")
                        {
                            return GridInfo.GetVarAs<string>(parts[1]);
                        }
                        else if (parts[0] == "party")
                        {
                            if (parts[1] == "Count") return gameData.CharacterList.Count.ToString();
                            else
                            {
                                // gonna access a value on a character
                                // find the index
                                int index = 0;
                                if (parts[1].StartsWith("#")) index = gameData.Ints[parts[1].Substring(1)];
                                else int.TryParse(parts[1], out index);
                                // ok now lets see what property we are accessing
                                if (parts[2] == "Name") return gameData.CharacterList[index].Name;
                                else if (parts[2] == "Job") return gameData.CharacterList[index].Job;
                                else if (parts[2] == "Stat")
                                {
                                    if (gameData.CharacterList[index].Stats.ContainsKey(parts[3])) return gameData.CharacterList[index].Stats[parts[3]].ToString();
                                }
                                else if (parts[2] == "MaxStat")
                                {
                                    if (gameData.CharacterList[index].MaxStats.ContainsKey(parts[3])) return gameData.CharacterList[index].MaxStats[parts[3]].ToString();
                                }
                                else if (parts[2] == "Status" && parts.Length > 3) return gameData.CharacterList[index].Status.Contains(parts[3]).ToString();
                                else if (parts[2] == "Action")
                                {
                                    if (gameData.CharacterList[index].Actions.Count > 0) return gameData.CharacterList[index].Actions[0];
                                }
                                else if (parts[2] == "Gear")
                                {
                                    if (gameData.CharacterList[index].Gear.ContainsKey(parts[3])) return gameData.CharacterList[index].Gear[parts[3]];
                                    else return "NONE";
                                }
                                else if (parts[2] == "Skills")
                                {
                                    if (parts.Length > 3)
                                    {
                                        if (parts[3] == "Count") return gameData.CharacterList[index].Actions.Count.ToString();
                                        if (parts[3].StartsWith("#")) return gameData.CharacterList[index].Actions[gameData.Ints[parts[3].Substring(1)]];
                                        int skillIndex = 0;
                                        if (int.TryParse(parts[3], out skillIndex))
                                        {
                                            return gameData.CharacterList[index].Actions[skillIndex];
                                        }
                                        else if (gameData.Ints.ContainsKey(parts[3])) return gameData.CharacterList[index].Actions[gameData.Ints[parts[3]]];
                                        else
                                        {
                                            string key = parseKey(parts[3]);
                                            return gameData.CharacterList[index].Actions.Contains(key).ToString();
                                        }
                                    }
                                }
                            }
                        }
                        else if (parts[0] == "items")
                        {
                            string key = parseKey(parts[1]);
                            if (gameData.Items.ContainsKey(key))
                            {
                                string var = parseKey(parts[2]);
                                if (gameData.Items[key].ContainsKey(var)) return gameData.Items[key][var];
                                //else return "";
                            }
                        }
                        else if (parts[0] == "skills")
                        {
                            string key = parseKey(parts[1]);
                            //GridInfo.Echo("Skill key: " + key);
                            if (gameData.Skills.ContainsKey(key))
                            {
                                string var = parseKey(parts[2]);
                                //GridInfo.Echo("Skill var: " + var);
                                if (gameData.Skills[key].ContainsKey(var)) return gameData.Skills[key][var];
                                //else return "";
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("get:" + address + " - " + e.Message);
                    }
                    return value;
                }
                set 
                {
                    try
                    {
                        if (string.IsNullOrEmpty(address)) this.value = value;
                        else
                        {
                            string[] parts = address.Split('.');
                            parts[0] = parts[0].ToLower();
                            if (parts[0] == "bools")
                            {
                                if (gameData.Bools.ContainsKey(parts[1])) gameData.Bools[parts[1]] = bool.Parse(value);
                                else gameData.Bools.Add(parts[1], bool.Parse(value));
                            }
                            else if (parts[0] == "ints")
                            {
                                if (gameData.Ints.ContainsKey(parts[1])) gameData.Ints[parts[1]] = int.Parse(value);
                                else gameData.Ints.Add(parts[1], int.Parse(value));
                            }
                            else if (parts[0] == "inventory")
                            {
                                string key = parts[1];
                                if (key.StartsWith("$")) key = gameData.Strings[key.Substring(1)];
                                if (gameData.Inventory.ContainsKey(key)) gameData.Inventory[key] = int.Parse(value);
                                else if (key != "Count" && key != "Keys")
                                {
                                    //GridInfo.Echo("Adding inventory item: " + key + " = " + value);
                                    gameData.Inventory.Add(key, int.Parse(value));
                                    //GridInfo.Echo("Inventory count: " + gameData.Inventory.Count);
                                }
                            }
                            else if (parts[0] == "strings")
                            {
                                if (gameData.Strings.ContainsKey(parts[1])) gameData.Strings[parts[1]] = value;
                                else gameData.Strings.Add(parts[1], value);
                            }
                            else if (parts[0] == "player")
                            {
                                if (parts[1] == "X") gameData.playerSprite.X = int.Parse(value);
                                else if (parts[1] == "Y") gameData.playerSprite.Y = int.Parse(value);
                                else if (parts[1] == "Direction") gameData.playerSprite.Direction = value[0];
                                else if (parts[1] == "SpriteId") gameData.playerSprite.SpriteIndex = int.Parse(value);
                                else if (parts[1] == "Visible") gameData.playerSprite.Visible = bool.Parse(value);
                            }
                            else if (parts[0] == "npc" && action.npc != null)
                            {
                                if (parts[1] == "X") action.npc.X = int.Parse(value);
                                else if (parts[1] == "Y") action.npc.Y = int.Parse(value);
                                else if (parts[1] == "Direction") action.npc.Direction = value[0];
                                else if (parts[1] == "SpriteId") action.npc.SpriteIndex = int.Parse(value);
                                else if (parts[1] == "Enabled") action.npc.Enabled = bool.Parse(value);
                            }
                            else if (parts[0] == "map")
                            {
                                if (parts[1] == "Visible") gameData.map.Visible = bool.Parse(value);
                            }
                            else if (parts[0] == "gridinfo")
                            {
                                string key = parseKey(parts[1]);
                                GridInfo.SetVar(key, value);
                            }
                            else if (parts[0] == "party")
                            {
                                //GridInfo.Echo("Setting Party: " + parts[1] + " = " + value);
                                int index = 0;
                                if (parts[1].StartsWith("#")) index = gameData.Ints[parts[1].Substring(1)];
                                else int.TryParse(parts[1], out index);
                                if (parts[2] == "Name") gameData.CharacterList[index].Name = value;
                                else if (parts[2] == "Stat")
                                {
                                    if (gameData.CharacterList[index].Stats.ContainsKey(parts[3])) gameData.CharacterList[index].Stats[parts[3]] = double.Parse(value);
                                }
                                else if (parts[2] == "MaxStat")
                                {
                                    if (gameData.CharacterList[index].MaxStats.ContainsKey(parts[3])) gameData.CharacterList[index].MaxStats[parts[3]] = int.Parse(value);
                                }
                                else if (parts[2] == "Status")
                                {
                                    bool status = bool.Parse(value);
                                    bool contains = gameData.CharacterList[index].Status.Contains(parts[3]);
                                    if (status && !contains) gameData.CharacterList[index].Status.Add(parts[3]);
                                    else if (!status && contains) gameData.CharacterList[index].Status.Remove(parts[3]);
                                }
                                else if (parts[2] == "Gear")
                                {
                                    //GridInfo.Echo("Setting Gear: " + parts[3] + " = " + value);
                                    if (gameData.CharacterList[index].Gear.ContainsKey(parts[3])) gameData.CharacterList[index].Gear[parts[3]] = value;
                                    else gameData.CharacterList[index].Gear.Add(parts[3], value);
                                    //GridInfo.Echo("Gear count: " + gameData.CharacterList[index].Gear.Count);
                                    //GridInfo.Echo("Gear: " + gameData.CharacterList[index].Gear[parts[3]]);
                                }
                                else if (parts[2] == "Skills")
                                {
                                    if (parts.Length > 3)
                                    {
                                        if (parts[3].StartsWith("#")) gameData.CharacterList[index].Actions[gameData.Ints[parts[3].Substring(1)]] = value;
                                        int skillIndex = 0;
                                        if (int.TryParse(parts[3], out skillIndex))
                                        {
                                            gameData.CharacterList[index].Actions[skillIndex] = value;
                                        }
                                        else if (gameData.Ints.ContainsKey(parts[3])) gameData.CharacterList[index].Actions[gameData.Ints[parts[3]]] = value;
                                        else
                                        {
                                            string key = parseKey(parts[3]);
                                            bool contains = gameData.CharacterList[index].Actions.Contains(key);
                                            bool status = bool.Parse(value);
                                            if (status && !contains) gameData.CharacterList[index].Actions.Add(key);
                                            else if (!status && contains) gameData.CharacterList[index].Actions.Remove(key);
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Missing Party: " + parts[1] + " : " + parts[2]);
                                }
                            }
                            else
                            {
                                throw new Exception("Missing: " + parts[0]);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("set:" + address + " - " + e.Message);
                    }
                }
            }
            public T As<T>()
            {
                string val = Value;
                if (string.IsNullOrEmpty(val)) return default(T);
                return (T)Convert.ChangeType(val, typeof(T));
            }
            public void Set<T>(T value)
            {
                Value = value.ToString();
            }
        }
        //-----------------------------------------------------------------------
    }
}
