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
        // loads tile sets from the GridDB
        //-----------------------------------------------------------------------
        public class TileSet : RasterSprite
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
                return GridDB.Get(game + ".TileSet." + i + "." + (isData ? "CustomData" : "Text"));
            }
            public static int GetTileSetCount(string game)
            {
                int i = 0;
                while (GetTileSetFromGridDB(game, i) != "") i++;
                return i;
            }
            //-----------------------------------------------------------------------
            // properties
            //-----------------------------------------------------------------------
            public static Vector2 tileSize = new Vector2(16, 16);
            public Dictionary<char,string> tiles {get; private set; } = new Dictionary<char, string>();
            public Dictionary<char,string> layers { get; private set; } = new Dictionary<char, string>();
            public TileSet(string data) : base(Vector2.Zero, 1, Vector2.Zero, data)
            {
                string[] parts = data.Split('║');
                string[] lines = parts[0].Split('\n');
                string[] mask = parts[1].Split('\n');
                Size = new Vector2(lines[0].Length, lines.Length);
                // based on the size of the tilemap and tile size split it into tiles
                char i = '\uE100'; char max = '\uE2FF';
                for (int y = 0; y < Size.Y; y += (int)tileSize.Y)
                {
                    for (int x = 0; x < Size.X; x += (int)tileSize.X)
                    {
                        // get the tile data and add it to the dictionary
                        if (i > max) break;
                        string sprite = getPixels(x, y, (int)tileSize.X, (int)tileSize.Y);
                        sprite = sprite.Replace(RasterSprite.IGNORE.ToString(), RasterSprite.INVISIBLE);
                        tiles.Add(i, sprite);
                        char m = mask[y / (int)tileSize.Y][x / (int)tileSize.X];
                        if(!layers.ContainsKey(m)) layers.Add(m, "");
                        layers[m] += i;
                        i++;
                    }
                }
                Data = "";
            }
            // find which layer contains the tile
            public char GetLayer(char layer) 
            {
                foreach (KeyValuePair<char, string> kvp in layers)
                {
                    if (kvp.Value.Contains(layer)) return kvp.Key;
                }
                return ' ';
            }
        }
        //-----------------------------------------------------------------------
    }
}
