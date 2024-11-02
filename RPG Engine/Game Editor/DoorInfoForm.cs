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
        //---------------------------------------------------------------------------
        // Door Info Form
        //---------------------------------------------------------------------------
        public class DoorInfoForm : LayoutMenu, ILayoutInteractable
        {
            //---------------------------------------------------------------------------
            // properties
            //---------------------------------------------------------------------------
            public MapExit Exit { get { return new MapExit((int)((LayoutNumberSelect)Items[0]).Value, (int)((LayoutNumberSelect)Items[1]).Value, (int)((LayoutNumberSelect)Items[2]).Value); } set { ((LayoutNumberSelect)Items[0]).Value = value.Id; ((LayoutNumberSelect)Items[1]).Value = value.X; ((LayoutNumberSelect)Items[2]).Value = value.Y; } }
            //---------------------------------------------------------------------------
            // constructor
            //---------------------------------------------------------------------------
            public DoorInfoForm(GameInput input, string game) : base(new Vector2(100, 100), new Vector2(100, 130), new Vector2(5, 5), input, Color.Black, Color.White, 1f)
            {
                LayoutText title = new LayoutText("Door", Color.White, 0.5f);
                title.Alignment = TextAlignment.CENTER;
                title.Padding = new Vector2(0, -15);
                extras.Add(title);
                Items.Add(new LayoutNumberSelect("Map", 0, 0, TileMap.GetMapCount(game) - 1, 1, 5, input));
                Items.Add(new LayoutNumberSelect("X", 24, 12, 100, 1, 5, input));
                Items.Add(new LayoutNumberSelect("Y", 24, 12, 100, 1, 5, input));
                Add("Save");
                Add("Remove");
            }
        }
        //---------------------------------------------------------------------------
    }
}
