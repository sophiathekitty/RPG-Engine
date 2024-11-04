using Sandbox.Engine.Platform;
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
using VRage.Input;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        //----------------------------------------------------------------------
        // MapOptionsForm
        //----------------------------------------------------------------------
        public class MapOptionsForm : LayoutMenu, ILayoutInteractable
        {
            //---------------------------------------------------------------------------
            // properties
            //---------------------------------------------------------------------------
            public Vector2 MapSize { get { return new Vector2(((LayoutNumberSelect)Items[0]).Value, ((LayoutNumberSelect)Items[1]).Value); } set { ((LayoutNumberSelect)Items[0]).Value = (int)value.X; ((LayoutNumberSelect)Items[1]).Value = (int)value.Y; } }
            public int TileSetIndex { get { return (int)((LayoutNumberSelect)Items[2]).Value; } set { ((LayoutNumberSelect)Items[2]).Value = value; } }
            public MapExit Exit { get { return new MapExit((int)((LayoutNumberSelect)Items[3]).Value, (int)((LayoutNumberSelect)Items[4]).Value, (int)((LayoutNumberSelect)Items[5]).Value); } set { ((LayoutNumberSelect)Items[3]).Value = value.Id; ((LayoutNumberSelect)Items[4]).Value = value.X; ((LayoutNumberSelect)Items[5]).Value = value.Y; } }
            //---------------------------------------------------------------------------
            // constructor
            //---------------------------------------------------------------------------
            public MapOptionsForm(GameInput input, string game) : base(new Vector2(100, 100), new Vector2(100, 200), new Vector2(5, 5), input, Color.Black, Color.White, 1f)
            {
                LayoutText title = new LayoutText("Map Options", Color.White, 0.5f);
                title.Alignment = TextAlignment.CENTER;
                title.Padding = new Vector2(0, -15);
                extras.Add(title);
                Items.Add(new LayoutNumberSelect("Width", 24, 12, 100, 1, 5, input));
                Items.Add(new LayoutNumberSelect("Height", 24, 12, 100, 1, 5, input));
                Items.Add(new LayoutNumberSelect("TileSet", 0, 0, TileSet.GetTileSetCount(game) - 1, 1, 5, input));
                Items.Add(new LayoutNumberSelect("Exit ID", -1, -1, 100, 1, 5, input));
                Items.Add(new LayoutNumberSelect("Exit X", 0, 0, 200, 1, 10, input));
                Items.Add(new LayoutNumberSelect("Exit Y", 0, 0, 200, 1, 10, input));
                Add("Apply");
            }
            //---------------------------------------------------------------------------
        }
        //----------------------------------------------------------------------
    }
}
