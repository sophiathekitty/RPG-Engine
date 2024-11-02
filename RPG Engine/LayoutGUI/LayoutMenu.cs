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
            string defualtPrompt = "W/S: move, Space: select";
            public string ButtonPrompt { get; set; }
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
            //public Color Color { get; set; } = Color.White;
            public Color ValueColor { get; set; } = Color.White;
            public Color SelectedColor { get; set; } = Color.Yellow;
            public Color EditingColor { get; set; } = Color.Green;
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public LayoutMenu(Vector2 position, Vector2 size, Vector2 padding, GameInput input) : base(position, size, padding)
            {
                this.input = input;
                ButtonPrompt = defualtPrompt;
            }
            public LayoutMenu(Vector2 position, Vector2 size, Vector2 padding, GameInput input, Color backGroundColor, Color borderColor, float borderWidth) : base(position, size, padding, backGroundColor, borderColor, borderWidth)
            {
                this.input = input;
                ButtonPrompt = defualtPrompt;
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
            int editingIndex = -1;
            public virtual string Run()
            {
                if(editingIndex != -1)
                {
                    ILayoutInteractable item = (ILayoutInteractable)Items[editingIndex];
                    if (item == null) return "error";
                    string cmd = item.Run();
                    if (cmd == "done" || cmd == "reset")
                    {
                        item.ValueColor = Color;
                        Items[editingIndex].Color = SelectedColor;
                        editingIndex = -1;
                        ButtonPrompt = defualtPrompt;
                    }
                    return "";
                }
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
                    if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                    {
                        // see if it's a number
                        if (Items[SelectedIndex] is LayoutNumberSelect)
                        {
                            editingIndex = SelectedIndex;
                            ((LayoutNumberSelect)Items[editingIndex]).ValueColor = EditingColor;
                            ButtonPrompt = ((LayoutNumberSelect )Items[editingIndex]).ButtonPrompt;
                        }
                        else if (Items[SelectedIndex] is ILayoutInteractable)
                        {
                            GridInfo.Echo("run menu: interactable");
                            editingIndex = SelectedIndex;
                            ButtonPrompt = ((ILayoutInteractable)Items[editingIndex]).ButtonPrompt;
                            Items[SelectedIndex].Color = EditingColor;
                        }
                        else
                        {
                            return Items[SelectedIndex].Text;
                        }
                        //return Items[SelectedIndex].Text;
                    }

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
