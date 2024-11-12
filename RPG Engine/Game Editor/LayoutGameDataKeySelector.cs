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
        // a form input for selecting one of the game data keys (bool, int, actions)
        //-----------------------------------------------------------------------
        public class LayoutGameDataKeySelector : LayoutNumberSelect
        {
            //-----------------------------------------------------------------------
            // properties
            //-----------------------------------------------------------------------
            public enum KeyType { Bool, Int, Action, String }
            public KeyType Type { get; private set; }
            GameData gameData;
            public string ValueString
            {
                get
                {
                    if (index <= -1) return "";
                    switch (Type)
                    {
                        case KeyType.Bool:      return gameData.Bools.Keys.ToList()[index];
                        case KeyType.Int:       return gameData.Ints.Keys.ToList()[index];
                        case KeyType.Action:    return gameData.Actions.Keys.ToList()[index];
                        case KeyType.String:    return gameData.Strings.Keys.ToList()[index];
                    }
                    return "";
                }
                set
                {
                    switch(Type)
                    {
                        case KeyType.Bool:      Value = gameData.Bools.Keys.ToList().IndexOf(value); break;
                        case KeyType.Int:       Value = gameData.Ints.Keys.ToList().IndexOf(value); break;
                        case KeyType.Action:    Value = gameData.Actions.Keys.ToList().IndexOf(value); break;
                        case KeyType.String:    Value = gameData.Strings.Keys.ToList().IndexOf(value); break;
                    }
                }
            }
            public override float Value 
            { 
                get { return index; } 
                set 
                { 
                    GridInfo.Echo("Value: " + value);
                    index = (int)value;
                    if(Items.Count < 2 || gameData == null) return;
                    if (index <= -1)
                    {
                        Items[1].Text = "None";
                        index = -1;
                        return;
                    }
                    if(index >= MaxValue) index = (int)MaxValue - 1;
                    switch (Type)
                    {
                        case KeyType.Bool:      Items[1].Text = gameData.Bools.Keys.ToList()[index]; break;
                        case KeyType.Int:       Items[1].Text = gameData.Ints.Keys.ToList()[index]; break;
                        case KeyType.Action:    Items[1].Text = gameData.Actions.Keys.ToList()[index]; break;
                        case KeyType.String:    Items[1].Text = gameData.Strings.Keys.ToList()[index]; break;
                    }

                }
            }
            public override float MaxValue 
            {
                set { }
                get 
                { 
                    GridInfo.Echo("MaxValue:?");
                    if(gameData == null ) return 0;
                    switch (Type)
                    {
                        case KeyType.Bool:      return gameData.Bools.Count;
                        case KeyType.Int:       return gameData.Ints.Count;
                        case KeyType.Action:    return gameData.Actions.Count;
                        case KeyType.String:    return gameData.Strings.Count;
                    }
                    return 0;
                }
            }
            int index = 0;
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public LayoutGameDataKeySelector(string label, KeyType type, GameInput input, GameData gameData) : base(label, 0, input)
            {
                MinValue = -1;
                Type = type;
                this.gameData = gameData;
                //Value = 0;
                Step = 1; BigStep = 1;
            }

        }
        //-----------------------------------------------------------------------
    }
}
