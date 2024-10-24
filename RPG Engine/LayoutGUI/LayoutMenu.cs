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
        // LayoutMenu extends LayoutArea
        //----------------------------------------------------------------------
        public class LayoutMenu : LayoutArea, ILayoutInteractable
        {
            //----------------------------------------------------------------------
            // properties
            //----------------------------------------------------------------------
            public string ButtonPrompt { get; set; } = "W/S: move, Space: select";
            GameInput input;
            int _selectedIndex = -1;
            public int SelectedIndex 
            { 
                get 
                { 
                    return _selectedIndex; 
                } 
                set 
                {
                    _selectedIndex = value;
                    for (int i = 0; i < Items.Count; i++)
                    {
                        Items[i].Color = i == SelectedIndex ? SelectedColor : Color;
                    }
                } 
            }
            public float FontSize { get; set; } = 0.5f;
            public Color Color { get; set; } = Color.White;
            public Color SelectedColor { get; set; } = Color.Yellow;
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public LayoutMenu(Vector2 position, Vector2 size, Vector2 padding, GameInput input) : base(position, size, padding)
            {
                this.input = input;
            }
            public LayoutMenu(Vector2 position, Vector2 size, Vector2 padding, GameInput input, Color backGroundColor, Color borderColor, float borderWidth) : base(position, size, padding, backGroundColor, borderColor, borderWidth)
            {
                this.input = input;
            }
            public void Add(string text)
            {
                LayoutText item = new LayoutText(text,Color.White, FontSize);
                item.Text = text;
                item.Size = new Vector2(Size.X, 30);
                item.Padding = new Vector2(5, 5);
                Items.Add(item);
            }
            //----------------------------------------------------------------------
            // run menu
            //----------------------------------------------------------------------
            public virtual string Run()
            {
                if (input.WPressed)
                {
                    SelectedIndex--;
                    if (SelectedIndex < 0) SelectedIndex = Items.Count - 1;
                }
                else if (input.SPressed)
                {
                    SelectedIndex++;
                    if (SelectedIndex >= Items.Count) SelectedIndex = 0;
                }
                else if (input.SpacePressed)
                {
                    if(SelectedIndex >= 0 && SelectedIndex < Items.Count) return Items[SelectedIndex].Text;

                }
                else if(input.QPressed)
                {
                    return "cancel";
                }
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].Color = i == SelectedIndex ? SelectedColor : Color;
                }
                return "";
            }
            public override void AddToScreen(Screen screen, int layer = 2)
            {
                base.AddToScreen(screen, layer);
            }
        }
        //----------------------------------------------------------------------
    }
}
