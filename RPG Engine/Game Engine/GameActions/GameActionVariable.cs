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
            GameData gameData; // where data is stored
            string address; // address of the variable
            string value; // value of the property
            public GameActionVariable(string address, GameData gameData, string defaultValue = "")
            {
                this.gameData = gameData;
                if(address.Contains("."))
                {
                    this.address = address;
                    value = defaultValue;
                }
                else
                {
                    value = address;
                    this.address = "";
                    GridInfo.Echo("GameActionVariable: " + value);
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
                    else if (parts[0] == "Inventory" && gameData.Inventory.ContainsKey(parts[1])) return gameData.Inventory[parts[1]].ToString();
                    else if (parts[0] == "Strings" && gameData.Strings.ContainsKey(parts[1])) return gameData.Strings[parts[1]];
                    else if (parts[0] == "Player")
                    {
                        if (parts[1] == "X") return gameData.playerSprite.X.ToString();
                        if (parts[1] == "Y") return gameData.playerSprite.Y.ToString();
                        if (parts[1] == "Direction") return gameData.playerSprite.Direction.ToString();
                        if (parts[1] == "SpriteId") return gameData.playerSprite.SpriteIndex.ToString();
                    }
                    else if (parts[0] == "NPC")
                    {
                        if (parts[1] == "X") return gameData.npc.X.ToString();
                        if (parts[1] == "Y") return gameData.npc.Y.ToString();
                        if (parts[1] == "Direction") return gameData.npc.Direction.ToString();
                        if (parts[1] == "SpriteId") return gameData.npc.SpriteIndex.ToString();
                        if (parts[1] == "Enabled") return gameData.npc.Enabled.ToString();
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
                            else gameData.Inventory.Add(parts[1], int.Parse(value));
                        }
                        else if (parts[0] == "Strings")
                        {
                            if (gameData.Strings.ContainsKey(parts[1])) gameData.Strings[parts[1]] = value;
                            else gameData.Strings.Add(parts[1], value);
                        }
                        else if (parts[0] == "Player")
                        {
                            if (parts[1] == "X") gameData.playerSprite.X = int.Parse(value);
                            if (parts[1] == "Y") gameData.playerSprite.Y = int.Parse(value);
                            if (parts[1] == "Direction") gameData.playerSprite.Direction = value[0];
                            if (parts[1] == "SpriteId") gameData.playerSprite.SpriteIndex = int.Parse(value);
                        }
                        else if (parts[0] == "NPC")
                        {
                            if (parts[1] == "X") gameData.npc.X = int.Parse(value);
                            if (parts[1] == "Y") gameData.npc.Y = int.Parse(value);
                            if (parts[1] == "Direction") gameData.npc.Direction = value[0];
                            if (parts[1] == "SpriteId") gameData.npc.SpriteIndex = int.Parse(value);
                            if (parts[1] == "Enabled") gameData.npc.Enabled = bool.Parse(value);
                        }
                    }
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
