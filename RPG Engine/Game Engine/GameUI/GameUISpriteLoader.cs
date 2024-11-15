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
        // load sprites for the game ui
        //-----------------------------------------------------------------------
        public class GameUISpriteLoader : RasterSprite
        {
            //-----------------------------------------------------------------------
            // static
            //-----------------------------------------------------------------------
            public static string GetTileSetFromGridDB(string game, int index)
            {
                // 0: game.Map.0.CustomData
                // 1: game.Map.0.Text
                int i = index / 2;
                bool isData = index % 2 == 0;
                return GridDB.Get(game + ".Sprites." + i + "." + (isData ? "CustomData" : "Text"));
            }
            public static int GetTileSetCount(string game)
            {
                int i = 0;
                while (GetTileSetFromGridDB(game, i) != "") i++;
                return i;
            }
            string game;
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public GameUISpriteLoader(string game, int index) : base(Vector2.Zero, 1, Vector2.One, "")
            {
                _index = index;
                this.game = game;
                LoadSpriteSheet(index);
            }
            public void LoadSpriteSheet(int index)
            {
                LoadImage(GetTileSetFromGridDB(game, index));
            }
            int _index = 0;
            public int Index
            {
                get
                {
                    return _index;
                }
                set
                {
                    _index = value;
                    LoadSpriteSheet(value);
                }
            }
            //-----------------------------------------------------------------------
            public RasterSprite GetSprite(int screenX, int screenY, float scale, int spriteX, int spriteY, int spriteWidth, int spriteHeight)
            {
                return new RasterSprite(new Vector2(screenX, screenY), scale, new Vector2(spriteWidth, spriteHeight),getPixels(spriteX, spriteY, spriteWidth, spriteHeight));
            }
        }
        //-----------------------------------------------------------------------
    }
}
