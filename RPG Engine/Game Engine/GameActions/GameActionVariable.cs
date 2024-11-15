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

            public string Value
            {
                get
                {
                    // see if address is null or empty
                    if (string.IsNullOrEmpty(address)) return value;
                    string[] parts = address.Split('.');
                    if (parts[0] == "Bools" && gameData.Bools.ContainsKey(parts[1])) return gameData.Bools[parts[1]].ToString();
                    else if (parts[0] == "Ints" && gameData.Ints.ContainsKey(parts[1])) return gameData.Ints[parts[1]].ToString();
                    else if (parts[0] == "Inventory")
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
                    else if (parts[0] == "Strings" && gameData.Strings.ContainsKey(parts[1])) return gameData.Strings[parts[1]];
                    else if (parts[0] == "Player")
                    {
                        if (parts[1] == "X") return gameData.playerSprite.X.ToString();
                        else if (parts[1] == "Y") return gameData.playerSprite.Y.ToString();
                        else if (parts[1] == "Direction") return gameData.playerSprite.Direction.ToString();
                        else if (parts[1] == "SpriteId") return gameData.playerSprite.SpriteIndex.ToString();
                    }

                    else if (parts[0] == "NPC" && action.npc != null)
                    {
                        if (parts[1] == "X") return action.npc.X.ToString();
                        else if (parts[1] == "Y") return action.npc.Y.ToString();
                        else if (parts[1] == "Direction") return action.npc.Direction.ToString();
                        else if (parts[1] == "SpriteId") return action.npc.SpriteIndex.ToString();
                        else if (parts[1] == "Enabled") return action.npc.Enabled.ToString();
                    }
                    return value;
                }
                set 
                {
                    if (string.IsNullOrEmpty(address)) this.value = value;
                    else
                    {
                        string[] parts = address.Split('.');
                        if (parts[0] == "Bools")
                        {
                            if (gameData.Bools.ContainsKey(parts[1])) gameData.Bools[parts[1]] = bool.Parse(value);
                            else gameData.Bools.Add(parts[1], bool.Parse(value));
                        }
                        else if (parts[0] == "Ints")
                        {
                            if (gameData.Ints.ContainsKey(parts[1])) gameData.Ints[parts[1]] = int.Parse(value);
                            else gameData.Ints.Add(parts[1], int.Parse(value));
                        }
                        else if (parts[0] == "Inventory")
                        {
                            if (gameData.Inventory.ContainsKey(parts[1])) gameData.Inventory[parts[1]] = int.Parse(value);
                            else if(parts[1] != "Count" && parts[1] != "Keys") gameData.Inventory.Add(parts[1], int.Parse(value));
                        }
                        else if (parts[0] == "Strings")
                        {
                            if (gameData.Strings.ContainsKey(parts[1])) gameData.Strings[parts[1]] = value;
                            else gameData.Strings.Add(parts[1], value);
                        }
                        else if (parts[0] == "Player")
                        {
                            if (parts[1] == "X") gameData.playerSprite.X = int.Parse(value);
                            else if (parts[1] == "Y") gameData.playerSprite.Y = int.Parse(value);
                            else if (parts[1] == "Direction") gameData.playerSprite.Direction = value[0];
                            else if (parts[1] == "SpriteId") gameData.playerSprite.SpriteIndex = int.Parse(value);
                        }
                        else if (parts[0] == "NPC" && action.npc != null)
                        {
                            if (parts[1] == "X") action.npc.X = int.Parse(value);
                            else if (parts[1] == "Y") action.npc.Y = int.Parse(value);
                            else if (parts[1] == "Direction") action.npc.Direction = value[0];
                            else if (parts[1] == "SpriteId") action.npc.SpriteIndex = int.Parse(value);
                            else if (parts[1] == "Enabled") action.npc.Enabled = bool.Parse(value);
                        }
                    }
                }
            }
            /*
            public T As<T>() where T : IConvertible
            {
                string val = Value;
                if (string.IsNullOrEmpty(val)) return default(T);
                return (T)Convert.ChangeType(val, typeof(T));
            }
            public void Set<T>(T value) where T : IConvertible
            {
                Value = value.ToString();
            }
            */
        }
        //-----------------------------------------------------------------------
    }
}
