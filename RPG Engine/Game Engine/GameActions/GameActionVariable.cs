using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            string address; // address of the variable
            string value; // value of the property
            public GameActionVariable(string address, GameData gameData, GameAction action, string defaultValue = "")
            {
                this.action = action;
                this.gameData = gameData;
                if(address.StartsWith("@"))
                {
                    this.address = address.Substring(1);
                    value = defaultValue;
                }
                else
                {
                    value = address;
                    this.address = "";
                }
            }
            public GameActionVariable ApplyAddressIndexes()
            {
                string address = this.address;
                foreach (string key in gameData.Ints.Keys)
                {
                    address = address.Replace("#" + key, gameData.Ints[key].ToString());
                }
                return new GameActionVariable(address, gameData, action, value);
            }
            public string Value
            {
                get
                {
                    // see if address is null or empty
                    if (string.IsNullOrEmpty(address)) return value;
                    string[] parts = address.Split('.');
                    parts[0] = parts[0].ToLower();
                    if (parts[0] == "bools" && gameData.Bools.ContainsKey(parts[1])) return gameData.Bools[parts[1]].ToString();
                    else if (parts[0] == "ints" && gameData.Ints.ContainsKey(parts[1])) return gameData.Ints[parts[1]].ToString();
                    else if (parts[0] == "inventory")
                    {
                        if(gameData.Inventory.ContainsKey(parts[1])) return gameData.Inventory[parts[1]].ToString();
                        else
                        {
                            if (parts[1] == "Count") return gameData.Inventory.Count.ToString();
                            else if (parts[1] == "Keys")
                            {
                                if(parts.Length > 2)
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
                            }
                        }
                    }
                    return value;
                }
                set 
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
                            if (gameData.Inventory.ContainsKey(parts[1])) gameData.Inventory[parts[1]] = int.Parse(value);
                            else if(parts[1] != "count" && parts[1] != "keys") gameData.Inventory.Add(parts[1], int.Parse(value));
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
                            GridInfo.SetVar(parts[1], value);
                        }
                        else if (parts[0] == "party")
                        {
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
                                if (gameData.CharacterList[index].Gear.ContainsKey(parts[3])) gameData.CharacterList[index].Gear[parts[3]] = value;
                                else gameData.CharacterList[index].Gear.Add(parts[3], value);
                            }
                        }
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
