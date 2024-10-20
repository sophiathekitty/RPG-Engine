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
        // cursor for the map editor
        //-----------------------------------------------------------------------
        public class MapCursor : IScreenSpriteProvider
        {
            ScreenSprite top;
            ScreenSprite bottom;
            ScreenSprite left;
            ScreenSprite right;
            Vector2 size = new Vector2(1, 1);
            const float lineThickness = 2f;
            public MapCursor()
            {
                top = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, new Vector2(0, 0), 0f, new Vector2(2, 2f), Color.White,"", "SquareSimple",TextAlignment.LEFT,SpriteType.TEXTURE);
                bottom = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, new Vector2(0, size.Y), 0f, new Vector2(1, 0.1f), Color.White, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                left = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, new Vector2(0, 0), 0f, new Vector2(0.1f, 1), Color.White, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                right = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, new Vector2(size.X, 0), 0f, new Vector2(0.1f, 1), Color.White, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
            }
            public Vector2 Position
            {
                get
                {
                    return top.Position;
                }
                set
                {
                    top.Position = value;
                    bottom.Position = value + new Vector2(0, size.Y);
                    left.Position = value + new Vector2(0, size.Y / 2);
                    right.Position = value + new Vector2(size.X, size.Y/2);
                }
            }
            public Vector2 Size
            {
                get
                {
                    return size;
                }
                set
                {
                    size = value;
                    bottom.Position = top.Position + new Vector2(0, size.Y);
                    right.Position = top.Position + new Vector2(size.X, size.Y/2);
                    left.Position = top.Position + new Vector2(0, size.Y / 2);
                    top.Size = new Vector2(size.X, lineThickness);
                    left.Size = new Vector2(lineThickness, size.Y);
                    bottom.Size = new Vector2(size.X, lineThickness);
                    right.Size = new Vector2(lineThickness, size.Y);
                }
            }
            public bool Visible
            {
                get
                {
                    return top.Visible;
                }

                set
                {
                    top.Visible = value;
                    bottom.Visible = value;
                    left.Visible = value;
                    right.Visible = value;
                }
            }

            public void AddToScreen(Screen screen, int layer = 2)
            {
                screen.AddSprite(top,layer);
                screen.AddSprite(bottom,layer);
                screen.AddSprite(left, layer);
                screen.AddSprite(right, layer);
            }

            public void RemoveFromScreen(Screen screen)
            {
                screen.RemoveSprite(top);
                screen.RemoveSprite(bottom);
                screen.RemoveSprite(left);
                screen.RemoveSprite(right);
            }
        }
        //-----------------------------------------------------------------------
    }
}
