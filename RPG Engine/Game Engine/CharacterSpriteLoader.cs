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
        public class CharacterSpriteLoader : RasterSprite
        {
            //---------------------------------------------------------------------------
            // constructor
            //---------------------------------------------------------------------------
            public CharacterSpriteLoader(string spriteSheet) : base(Vector2.Zero,0.1f,Vector2.Zero, spriteSheet)
            {
                GridInfo.Echo("Loading sprite sheet");
            }
            public int SpriteCount { get { return (int)(Size.Y / 16); } }
            //---------------------------------------------------------------------------
            // LoadSpriteSet - load a sprite set from a sprite sheet
            //---------------------------------------------------------------------------
            public Dictionary<char,List<string>> LoadSpriteSet(int id)
            {
                Dictionary<char, List<string>> sprites = new Dictionary<char, List<string>>();
                int spriteY = id * 16;
                // sheet has sprites in ddlluu
                sprites.Add('d', new List<string> { getPixels(0,spriteY,16,16),getPixels(16, spriteY, 16, 16) });
                sprites.Add('l', new List<string> { getPixels(32, spriteY, 16, 16), getPixels(48, spriteY, 16, 16) });
                RasterSprite sprite = new RasterSprite(Vector2.Zero, 0.1f, Vector2.Zero, sprites['l'][0]);
                RasterSprite sprite2 = new RasterSprite(Vector2.Zero, 0.1f, Vector2.Zero, sprites['l'][1]);
                sprite.FlipHorizontal();
                sprite2.FlipHorizontal();
                sprites.Add('r', new List<string> { sprite.Data, sprite2.Data });
                sprites.Add('u', new List<string> { getPixels(64, spriteY, 16, 16), getPixels(80, spriteY, 16, 16) });
                return sprites;
            }
            public string LoadSprite(int id, char direction)
            {
                int spriteY = id * 16;
                switch (direction)
                {
                    case 'd':
                        return getPixels(0, spriteY, 16, 16);
                    case 'l':
                        return getPixels(32, spriteY, 16, 16);
                    case 'r':
                        RasterSprite sprite = new RasterSprite(Vector2.Zero, 0.1f, Vector2.Zero, getPixels(32, spriteY, 16, 16));
                        sprite.FlipHorizontal();
                        return sprite.Data;
                    case 'u':
                        return getPixels(64, spriteY, 16, 16);
                    default:
                        return getPixels(0, spriteY, 16, 16);
                }
            }
        }
    }
}
