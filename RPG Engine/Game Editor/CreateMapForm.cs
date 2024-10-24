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
        // Create Map Form
        //---------------------------------------------------------------------------
        public class CreateMapForm : LayoutMenu, ILayoutInteractable
        {
            //---------------------------------------------------------------------------
            // properties
            //---------------------------------------------------------------------------
            public Vector2 MapSize { get { return new Vector2(((LayoutNumberSelect)Items[0]).Value, ((LayoutNumberSelect)Items[1]).Value); } set { ((LayoutNumberSelect)Items[0]).Value = (int)value.X; ((LayoutNumberSelect)Items[1]).Value = (int)value.Y; } }
            public int TileSetIndex { get { return (int)((LayoutNumberSelect)Items[2]).Value; } set { ((LayoutNumberSelect)Items[2]).Value = value; } }
            //---------------------------------------------------------------------------
            // constructor
            //---------------------------------------------------------------------------
            public CreateMapForm(GameInput input, string game) : base(new Vector2(100,100), new Vector2(100,100), new Vector2(5,5), input,Color.Black, Color.White,1f)
            {
                LayoutText title = new LayoutText("Create Map", Color.White, 0.5f);
                title.Alignment = TextAlignment.CENTER;
                title.Padding = new Vector2(0, -15);
                extras.Add(title);
                Items.Add(new LayoutNumberSelect("Width", 24, 12, 100, 1, 5, input));                
                Items.Add(new LayoutNumberSelect("Height", 24, 12, 100, 1, 5, input));
                Items.Add(new LayoutNumberSelect("TileSet", 0, 0, TileSet.GetTileSetCount(game)-1, 1, 5, input));
                Add("Create");
            }
            //---------------------------------------------------------------------------
            // ILayoutInteractable
            //---------------------------------------------------------------------------
            int editingIndex = -1;
            public override string Run()
            {
                if (editingIndex == -1)
                {
                    string cmd = base.Run();
                    if (cmd == "Width")
                    {
                        editingIndex = 0;
                        ((LayoutNumberSelect)Items[editingIndex]).ValueColor = SelectedColor;
                    }
                    else if (cmd == "Height")
                    {
                        editingIndex = 1;
                        ((LayoutNumberSelect)Items[editingIndex]).ValueColor = SelectedColor;
                    }
                    else if (cmd == "TileSet")
                    {
                        editingIndex = 2;
                        ((LayoutNumberSelect)Items[editingIndex]).ValueColor = SelectedColor;
                    }
                    else return cmd;
                }
                else
                {
                    LayoutNumberSelect item = (LayoutNumberSelect)Items[editingIndex];
                    if (item == null) return "error";
                    string cmd = item.Run();
                    if (cmd == "done" || cmd == "reset")
                    {
                        ((LayoutNumberSelect)Items[editingIndex]).ValueColor = Color;
                        editingIndex = -1;
                    }
                }
                return "";
            }
            
            //---------------------------------------------------------------------------
        }
        //---------------------------------------------------------------------------
    }
}
