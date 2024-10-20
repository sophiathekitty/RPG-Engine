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
        // LayoutText - a simple text item for a layout
        //----------------------------------------------------------------------
        public class LayoutText : ILayoutItem
        {
            public static Vector2 MonospaceCharSize = new Vector2(18.68108f, 28.8f);
            public static float MonospaceCharSpace = 0.77838f;
            //----------------------------------------------------------------------
            // ILayoutItem
            //----------------------------------------------------------------------
            public Vector2 Position { get; set; } = new Vector2(0, 0);
            public Vector2 Size { get; set; }
            public Vector2 Padding { get; set; } = new Vector2(0,0);
            public bool GetSizeFromParent { get; set; } = true;
            public Vector2 ContentSize { get { return new Vector2(Text.Length * MonospaceCharSize.X * text.RotationOrScale + Text.Length-1 * MonospaceCharSpace, MonospaceCharSize.Y * text.RotationOrScale)  + (Padding * 2); } }
            public bool Visible { get { return text.Visible; } set { text.Visible = value; } }
            public void AddToScreen(Screen screen, int layer = 0) { screen.AddSprite(text, layer); }
            public void RemoveFromScreen(Screen screen) { screen.RemoveSprite(text); }
            public void ApplyLayout()
            {
                // position text based on Position, Size, and Alignment
                Vector2 pos = Position + Padding;
                switch (text.Alignment)
                {
                    case TextAlignment.LEFT:
                        break;
                    case TextAlignment.CENTER:
                        pos.X += (Size.X - (Padding.X * 2)) / 2;
                        break;
                    case TextAlignment.RIGHT:
                        pos.X += Size.X - (Padding.X * 2);
                        break;
                }
                text.Position = pos;
            }
            //----------------------------------------------------------------------
            // properties
            //----------------------------------------------------------------------
            ScreenSprite text;
            public string Text { get { return text.Data;  } set { text.Data = value; } }
            public Color Color { get { return text.Color; } set { text.Color = value; } }
            public float FontSize { get { return text.RotationOrScale; } set { text.RotationOrScale = value; } }
            public TextAlignment Alignment { get { return text.Alignment; } set { text.Alignment = value; } }
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public LayoutText(string text, Color color, TextAlignment alignment)
            {
                this.text = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, Vector2.Zero, 1, Vector2.Zero, color, "Monospace", text, alignment, SpriteType.TEXT);
            }
            public LayoutText(string text, Color color, float fontSize) : this(text, color, TextAlignment.LEFT) 
            {
                this.text.RotationOrScale = fontSize;
            }
        }
        //----------------------------------------------------------------------
    }
}
