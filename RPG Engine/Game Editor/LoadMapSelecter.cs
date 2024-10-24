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
        //----------------------------------------------------------------------
        // map selecter for the game editor
        //----------------------------------------------------------------------
        public class LoadMapSelecter : LayoutArea, ILayoutInteractable
        {
            //----------------------------------------------------------------------
            // properties
            //----------------------------------------------------------------------
            public string ButtonPrompt { get; set; } = "A/D: select, Space: load";
            public int MapIndex
            {
                get
                {
                    return (int)((LayoutNumberSelect)Items[1]).Value;
                }
                set
                {
                    ((LayoutNumberSelect)Items[1]).Value = value;
                }
            }
            public int MaxMapIndex
            {
                get
                {
                    return (int)((LayoutNumberSelect)Items[1]).MaxValue;
                }
                set
                {
                    ((LayoutNumberSelect)Items[1]).MaxValue = value;
                }
            }
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public LoadMapSelecter(Vector2 position, Vector2 size, Vector2 padding, GameInput input, int max = 0) : base(position, size, padding, Color.Black, Color.White, 1f)
            {
                Items.Add(new LayoutText("Load",Color.White, 0.5f));
                LayoutNumberSelect select = new LayoutNumberSelect("Map", 0, input);
                select.MaxValue = max;
                select.BigStep = 5;
                Items.Add(select);
            }
            public string Run()
            {
                return ((LayoutNumberSelect)Items[1]).Run();
            }
            public void Reset()
            {
                ((LayoutNumberSelect)Items[1]).StartValue = 0;
            }
        }
        //----------------------------------------------------------------------
    }
}
