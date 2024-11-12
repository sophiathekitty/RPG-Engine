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
        public class CharacterSpriteSelector : ILayoutItem, ILayoutInteractable
        {
            RasterSprite sprite;
            MapCursor border;
            CharacterSpriteLoader spriteLoader;
            public string ButtonPrompt { get; set; } = "W/D: Sprite, A/D: Direction, Space: Select, Q: Cancel";
            public Vector2 Position { get; set; }
            public Vector2 Size { get { return ContentSize; } set { } }
            public Vector2 Padding { get; set; }
            public Vector2 ContentSize
            {
                get
                {
                    return sprite.PixelToScreen(sprite.Size);
                }
            }
            public Color Color
            {
                get
                {
                    return border.Color;
                }

                set
                {
                    border.Color = value;
                }
            }
            public Color ValueColor { get; set; }
            public string Text
            {
                get
                {
                    return "Sprite";
                }

                set
                {
                    // do nothing
                }
            }
            public bool GetSizeFromParent { get; set; }
            public bool Visible
            {
                get
                {
                    return sprite.Visible;
                }

                set
                {
                    sprite.Visible = value;
                    border.Visible = value;
                }
            }
            int spriteID = 0;
            public int SpriteID
            {
                get { return spriteID; }
                set
                {
                    spriteID = value;
                    sprite.Data = spriteLoader.LoadSprite(spriteID, dirs[dirIndex]).Replace(RasterSprite.IGNORE.ToString(), RasterSprite.INVISIBLE);
                }
            }
            public char Direction
            {
                get { return dirs[dirIndex]; }
                set
                {
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        if (dirs[i] == value)
                        {
                            dirIndex = i;
                            sprite.Data = spriteLoader.LoadSprite(spriteID, dirs[dirIndex]).Replace(RasterSprite.IGNORE.ToString(), RasterSprite.INVISIBLE);
                            break;
                        }
                    }
                }
            }
            GameInput input;
            //---------------------------------------------------------------------------
            // constructor
            //---------------------------------------------------------------------------
            public CharacterSpriteSelector(Vector2 position, float scale, Vector2 size, CharacterSpriteLoader spriteLoader, GameInput input)
            {
                GridInfo.Echo("CharacterSpriteSelector");
                this.spriteLoader = spriteLoader;
                sprite = new RasterSprite(position, scale, size, spriteLoader.LoadSprite(0, 'd'));
                border = new MapCursor();
                border.Size = sprite.PixelToScreen(sprite.Size);
                this.input = input;
            }
            public void ApplyLayout()
            {
                border.Position = Position + Padding;
                sprite.Position = Position + Padding;
            }
            string dirs = "dlru";
            int dirIndex = 0;
            public string Run()
            {
                bool changed = false;
                if(input.WPressed)
                {
                    spriteID--;
                    changed = true;
                }
                else if (input.SPressed)
                {
                    spriteID++;
                    changed = true;
                }
                else if (input.APressed)
                {
                    dirIndex--;
                    if (dirIndex < 0) dirIndex = 3;
                    changed = true;
                }
                else if (input.DPressed)
                {
                    dirIndex++;
                    if (dirIndex > 3) dirIndex = 0;
                    changed = true;
                }
                else if (input.SpacePressed)
                {
                    return "done";
                }
                else if (input.QPressed)
                {
                    return "reset";
                }
                if (changed)
                {
                    spriteID = MathHelper.Clamp(spriteID, 0, spriteLoader.SpriteCount - 1);
                    dirIndex = MathHelper.Clamp(dirIndex, 0, 3);
                    sprite.Data = spriteLoader.LoadSprite(spriteID, dirs[dirIndex]).Replace(RasterSprite.IGNORE.ToString(), RasterSprite.INVISIBLE);
                }
                return "";
            }

            void IScreenSpriteProvider.AddToScreen(Screen screen, int layer)
            {
                screen.AddSprite(sprite, layer);
                screen.AddSprite(border, layer);
            }

            public void RemoveFromScreen(Screen screen)
            {
                screen.RemoveSprite(sprite);
                screen.RemoveSprite(border);
            }
        }
    }
}
