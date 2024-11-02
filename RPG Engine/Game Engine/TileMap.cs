using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Policy;
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
using static IngameScript.Program;

namespace IngameScript
{
    partial class Program
    {
        //-----------------------------------------------------------------------
        // this is a tile map class
        //-----------------------------------------------------------------------
        public class TileMap : IScreenSpriteProvider
        {
            //-----------------------------------------------------------------------
            // static methods
            //-----------------------------------------------------------------------
            public static string GetMapDB(string game, int index)
            {
                // 0: game.Map.0.CustomData
                // 1: game.Map.0.Text
                int i = index/2;
                bool isData = index % 2 == 0;
                return GridDB.Get(game + ".Map." + i + "." + (isData ? "CustomData" : "Text"));
            }
            public static void SetMapDB(string game, int index, string data)
            {
                int i = index / 2;
                bool isData = index % 2 == 0;
                GridDB.Set(game + ".Map." + i + "." + (isData ? "CustomData" : "Text"), data,true);
            }
            public static int GetMapCount(string game)
            {
                int i = 0;
                while (GetMapDB(game, i) != "") i++;
                return i;
            }
            //-----------------------------------------------------------------------
            // properties
            //-----------------------------------------------------------------------
            public TileSet tilesSet;
            public MapExit DefaultExit = new MapExit();
            public List<MapDoor> Doors = new List<MapDoor>();
            public CharacterSpriteLoader spriteSheet;
            public GameData gameData;
            public List<NPC> NPCs = new List<NPC>();
            //string tileSetAddress;
            public int tileSetIndex = 0;
            string[] map;
            int mapHeight { get { return map.Length; } }
            int mapWidth { get { return map[0].Length; } }
            string[] ceilingMap; // overlay that switches visible area when you're under it
            public bool IsWall(int x, int y) { return tilesSet.layers.ContainsKey('w') && tilesSet.layers['w'].Contains(map[y][x]); }
            public MapDoor IsDoor(int x, int y)
            {
                foreach (MapDoor door in Doors)
                {
                    if (door.X == x && door.Y == y) return door;
                }
                return null;
            }
            public NPC GetNPC(int x, int y)
            {
                foreach (NPC npc in NPCs)
                {
                    if (npc.MapPosition.X == x && npc.MapPosition.Y == y) return npc;
                }
                return null;
            }
            public void SetNPC(NPC npc)
            {
                // see if it exists
                npc.RotationOrScale = ground[0].RotationOrScale;
                for (int i = 0; i < NPCs.Count; i++)
                {
                    if (NPCs[i].MapPosition == npc.MapPosition)
                    {
                        NPCs[i] = npc;
                        return;
                    }
                }
                NPCs.Add(npc);
                _Screen.AddSprite(npc);
                ApplyViewportTiles();   
            }
            public void RemoveNPC(int x, int y)
            {
                for (int i = 0; i < NPCs.Count; i++)
                {
                    if (NPCs[i].MapPosition.X == x && NPCs[i].MapPosition.Y == y)
                    {
                        _Screen.RemoveSprite(NPCs[i]);
                        NPCs.RemoveAt(i);
                        return;
                    }
                }
            }
            public bool IsWithinViewport(int x, int y)
            {
                return x >= MapPosition.X && x < MapPosition.X + ViewportSize.X && y >= MapPosition.Y && y < MapPosition.Y + ViewportSize.Y;
            }
            public bool IsUnderCeiling(int x, int y) { return ceilingMap[y][x] != ' '; }
            bool underCeiling = false;
            public string GetViewportTileData(int x, int y) 
            { 
                char tile = GetViewportTile(x, y);
                if(tile != ' ') return tilesSet.tiles[tile];
                return "";
            }
            public char GetViewportTile(int x, int y)
            { 
                x += (int)MapPosition.X;
                y += (int)MapPosition.Y;
                if (x < 0 || y < 0 || x >= map[0].Length || y >= map.Length) return ' ';
                return map[y][x];
            }
            public char GetCeilingTile(int x, int y)
            {
                x += (int)MapPosition.X;
                y += (int)MapPosition.Y;
                if (x < 0 || y < 0 || x >= map[0].Length || y >= map.Length) return ' ';
                return ceilingMap[y][x];
            }
            public void SetTile(int x, int y, char tile)
            {
                // fit x,y into the map range
                x %= map[0].Length;
                y %= map.Length;
                map[y] = map[y].Substring(0, x) + tile + map[y].Substring(x + 1); 
                ApplyViewportTiles(); 
            }
            public void SetCeilingTile(int x, int y, char tile)
            {
                x %= map[0].Length;
                y %= map.Length;
                if (tile == (char)ceilingMap[y][x]) tile = ' ';
                ceilingMap[y] = ceilingMap[y].Substring(0, x) + tile + ceilingMap[y].Substring(x + 1); 
                ApplyViewportTiles();
            }
            public Vector2 TilePosition(int x, int y) 
            {
                GridInfo.Echo("TileMap.TilePosition: "+x+","+y);
                // fit x,y into the map range
                x %= map[0].Length;
                y %= map.Length;
                return new Vector2((x - MapPosition.X) * TileSize.X, (y - MapPosition.Y) * TileSize.Y) + ViewportPosition; 
            }
            Screen _Screen;
            string game;
            public int index = 0;
            //-----------------------------------------------------------------------
            // Create Map
            //-----------------------------------------------------------------------
            public void CreateMap(int width, int height, int tileSetIndex = 0)
            {
                // fill the tile map with the default tile '\uE100'
                map = new string[height];
                ceilingMap = new string[height];
                for (int i = 0; i < height; i++)
                {
                    map[i] = new string('\uE100', width);
                    ceilingMap[i] = new string(' ', width);
                }
                MapPosition = new Vector2(0, 0);
                this.tileSetIndex = tileSetIndex;
                tilesSet = new TileSet(TileSet.GetTileSetFromGridDB(game, tileSetIndex));
                ApplyViewportTiles();
                index = GetMapCount(game);
            }
            public void Save()
            {
                string data = tileSetIndex.ToString() + '║';
                // map data
                bool first = true;
                foreach (string line in map)
                {
                    if (first) first = false;
                    else data += '\n';
                    data += line;
                }
                // ceiling data
                data += '║';
                first = true;
                foreach (string line in ceilingMap)
                {
                    if (first) first = false;
                    else data += '\n';
                    data += line;
                }
                data += '║';
                first = true;
                foreach (MapDoor door in Doors)
                {
                    if(first) first = false;
                    else data += '\n';
                    data += door.ToString();
                }
                data += '║';
                data += DefaultExit.ToString();
                data += '║';
                first = true;
                foreach (NPC npc in NPCs)
                {
                    if (first) first = false;
                    else data += '\n';
                    data += npc.ToSaveString();
                }
                SetMapDB(game, index, data);

            }
            public void Load(int index)
            {
                GridInfo.Echo("TileMap.Load: " + index);
                this.index = index;
                string data = GetMapDB(game, index);
                string[] parts = data.Split('║');
                //tileSetAddress = parts[0];
                tileSetIndex = int.Parse(parts[0]);
                tilesSet = new TileSet(TileSet.GetTileSetFromGridDB(game, tileSetIndex));
                map = parts[1].Split('\n');
                ceilingMap = parts[2].Split('\n');
                GridInfo.Echo("TileMap.Load: Doors?");
                Doors.Clear();
                if (parts.Length > 3)
                {
                    GridInfo.Echo("TileMap.Load: Doors");
                    foreach (string door in parts[3].Split('\n'))
                    {
                        if(door != "") Doors.Add(new MapDoor(door));
                    }
                }
                GridInfo.Echo("TileMap.Load: DefaultExit?");
                if (parts.Length > 4) DefaultExit = parts[4] != "" ? new MapExit(parts[4]) : new MapExit();
                GridInfo.Echo("TileMap.Load: NPCs?");
                foreach (NPC npc in NPCs) _Screen.RemoveSprite(npc);
                NPCs.Clear();
                if (parts.Length > 5)
                {
                    GridInfo.Echo("TileMap.Load: NPCs");
                    foreach (string npc in parts[5].Split('\n'))
                    {
                        if(npc != "") NPCs.Add(new NPC(npc, ref spriteSheet, ref gameData));
                    }
                }
                foreach (NPC npc in NPCs){ _Screen.AddSprite(npc); npc.RotationOrScale = ground[0].RotationOrScale; }
                ApplyViewportTiles();
            }
            public void Load(MapExit exit)
            {
                Load(exit.Id);
            }
            //-----------------------------------------------------------------------
            // Tile Map Viewport (visible area)
            //-----------------------------------------------------------------------
            public Vector2 ViewportSize { get; private set; } = new Vector2(16, 16);
            public Vector2 ViewportPosition { get; private set; }
            Vector2 MapPosition = new Vector2(0, 0);
            List<ScreenSprite> ground = new List<ScreenSprite>(); // the ground tiles
            List<ScreenSprite> overlay = new List<ScreenSprite>(); // the overlay tiles
            List<ScreenSprite> ceiling = new List<ScreenSprite>(); // area that toggles the visible area when you're under it
            public Vector2 TileSize
            {
                get
                {
                    float width = (_Screen.Size.X-ViewportPosition.X) / ViewportSize.X;
                    float height = (_Screen.Size.Y - ViewportPosition.Y) / ViewportSize.Y;
                    if (width < height) return new Vector2(width, width);
                    return new Vector2(height, height);
                }
            }
            //-----------------------------------------------------------------------
            // methods
            //-----------------------------------------------------------------------
            // apply tiles to the viewport
            public void ApplyViewportTiles()
            {
                if(ground.Count == 0) return;
                int i = 0;
                for (int y = 0; y < ViewportSize.Y; y++)
                {
                    for (int x = 0; x < ViewportSize.X; x++)
                    {
                        char tile = GetViewportTile(x, y);
                        if(tilesSet.tiles.ContainsKey(tile)) ground[i].Data = tilesSet.tiles[tile];
                        else ground[i].Data = "";
                        if (tilesSet.layers.ContainsKey('1') && tilesSet.layers['1'].Contains(tile) && tilesSet.tiles.ContainsKey(tile)) overlay[i].Data = tilesSet.tiles[tile]; // secrets
                        else if (tilesSet.layers.ContainsKey('f') && tilesSet.layers.ContainsKey('F') && tilesSet.layers['f'].Contains(tile)) overlay[i].Data = tilesSet.tiles[tilesSet.layers['F'].First()]; // secret overlay 1
                        else if (tilesSet.layers.ContainsKey('g') && tilesSet.layers.ContainsKey('G') && tilesSet.layers['g'].Contains(tile)) overlay[i].Data = tilesSet.tiles[tilesSet.layers['G'].First()]; // secret overlay 2
                        else if (tilesSet.layers.ContainsKey('h') && tilesSet.layers.ContainsKey('H') && tilesSet.layers['h'].Contains(tile)) overlay[i].Data = tilesSet.tiles[tilesSet.layers['H'].First()]; // secret overlay 3
                        else overlay[i].Data = "";
                        tile = GetCeilingTile(x, y);
                        if (tilesSet.tiles.ContainsKey(tile)) ceiling[i].Data = tilesSet.tiles[tile];
                        else ceiling[i].Data = "";
                        i++;
                    }
                }
                foreach (NPC npc in NPCs)
                {
                    npc.Visible = IsWithinViewport((int)npc.MapPosition.X, (int)npc.MapPosition.Y);
                    npc.Position = TilePosition((int)npc.MapPosition.X, (int)npc.MapPosition.Y);
                }
            }
            // center the viewport on point x,y
            public void CenterOn(int x, int y)
            {
                // make sure it's within the map
                MapPosition = new Vector2(x - ViewportSize.X / 2, y - ViewportSize.Y / 2);
                if (MapPosition.X < 0) MapPosition.X = 0;
                if (MapPosition.Y < 0) MapPosition.Y = 0;
                if (MapPosition.X + ViewportSize.X >= map[0].Length) MapPosition.X = map[0].Length - ViewportSize.X;
                if (MapPosition.Y + ViewportSize.Y >= map.Length) MapPosition.Y = map.Length - ViewportSize.Y;
                ApplyViewportTiles();
            }
            public void CenterOn(Vector2 position)
            {
                CenterOn((int)position.X, (int)position.Y);
            }
            public void ResizeMap(Vector2 newSize)
            {
                string[] newMap = new string[(int)newSize.Y];
                string[] newCeilingMap = new string[(int)newSize.Y];
                for (int i = 0; i < newMap.Length; i++)
                {
                    // crop the map if it's too big (or fill with empty tiles if it's too small)
                    if (map.Length > i)
                    {
                        if (map[i].Length < newSize.X)
                        {
                            newMap[i] = map[i].PadRight((int)newSize.X, '\uE100');
                            newCeilingMap[i] = ceilingMap[i].PadRight((int)newSize.X, ' ');
                        }
                        else
                        {
                            newMap[i] = map[i].Substring(0, (int)newSize.X);
                            newCeilingMap[i] = ceilingMap[i].Substring(0, (int)newSize.X);
                        }
                    }
                    else
                    {
                        newMap[i] = new string('\uE100', (int)newSize.X);
                        newCeilingMap[i] = new string(' ', (int)newSize.X);
                    }
                }
                map = newMap;
                ceilingMap = newCeilingMap;
                ApplyViewportTiles();
            }
            //float fontSize { get { return TileSize.X / (RasterSprite.PIXEL_TO_SCREEN_RATIO * TileSet.tileSize.X); } }
            //-----------------------------------------------------------------------
            // IScreenSpriteProvider
            //-----------------------------------------------------------------------
            public void AddToScreen(Screen screen, int layer = 0)
            {
                GridInfo.Echo("TileMap.AddToScreen");
                if (_Screen != screen) _Screen = screen;
                else RemoveFromScreen(screen); // clear the tilemap from the screen if it's already there
                ground.Clear();
                overlay.Clear();
                ceiling.Clear();
                Vector2 tileSize = TileSize;
                Vector2 tilePos = ViewportPosition;
                RasterSprite darkTile = new RasterSprite(Vector2.Zero, 1, TileSet.tileSize, "");
                darkTile.fillRGB(Color.Black);

                // screen = pixel* PIXEL_TO_SCREEN_RATIO *RotationOrScale
                // 

                float fontSize = tileSize.X / (RasterSprite.PIXEL_TO_SCREEN_RATIO * TileSet.tileSize.X);

                string[] tiles = tilesSet.tiles.Values.ToArray();
                string map = "";
                for (int y = 0; y < ViewportSize.Y; y++)
                {
                    for (int x = 0; x < ViewportSize.X; x++)
                    {
                        ScreenSprite sprite = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, tilePos, fontSize, tileSize, Color.White, "Monospace", tiles[0], TextAlignment.LEFT, SpriteType.TEXT);
                        ScreenSprite overlaySprite = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, tilePos, fontSize, tileSize, Color.White, "Monospace", "", TextAlignment.LEFT, SpriteType.TEXT);
                        ScreenSprite ceilingSprite = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, tilePos, fontSize, tileSize, Color.White, "Monospace", "", TextAlignment.LEFT, SpriteType.TEXT);
                        ScreenSprite darknessSprite = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, tilePos, fontSize, tileSize, Color.White, "Monospace", darkTile.Data, TextAlignment.LEFT, SpriteType.TEXT);
                        ground.Add(sprite);
                        overlay.Add(overlaySprite);
                        ceiling.Add(ceilingSprite);
                        darknessSprite.Visible = false;
                        screen.AddSprite(sprite);
                        screen.AddSprite(overlaySprite, 1);
                        screen.AddSprite(ceilingSprite, 1);
                        screen.AddSprite(darknessSprite, 1);
                        tilePos.X += tileSize.X;
                        map += '\uE100';
                    }
                    tilePos.X = ViewportPosition.X;
                    tilePos.Y += tileSize.Y;
                    map += '\n';
                }
                this.map = map.Trim().Split('\n');
                foreach (NPC npc in NPCs)
                {
                    npc.Visible = IsWithinViewport((int)npc.MapPosition.X, (int)npc.MapPosition.Y);
                    npc.Position = TilePosition((int)npc.MapPosition.X, (int)npc.MapPosition.Y);
                    screen.AddSprite(npc);
                }
            }
            public void AddOveralyToScreen(Screen screen)
            {
                for (int i = 0; i < overlay.Count; i++)
                {
                    screen.AddSprite(overlay[i]);
                    screen.AddSprite(ceiling[i]);
                }
            }
            public void RemoveFromScreen(Screen screen)
            {
                // remove the ground from the screen
                foreach (ScreenSprite sprite in ground)
                {
                    screen.RemoveSprite(sprite);
                }
                // remove npcs and player here
                // - todo
                // remove the overlays from the screen
                foreach (ScreenSprite sprite in overlay)
                {
                    screen.RemoveSprite(sprite);
                }
                foreach (ScreenSprite sprite in ceiling)
                {
                    screen.RemoveSprite(sprite);
                }
            }
            public Vector2 Size { get { return new Vector2(map[0].Length, map.Length); } }
            public bool Visible
            {
                get
                {
                    return ground[0].Visible || ceiling[0].Visible;
                }

                set
                {
                    foreach (ScreenSprite sprite in ground)
                    {
                        sprite.Visible = value;
                    }
                    foreach (ScreenSprite sprite in overlay)
                    {
                        sprite.Visible = value;
                    }
                    foreach (ScreenSprite sprite in ceiling)
                    {
                        sprite.Visible = value;
                    }
                }
            }
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public TileMap(string game, ref CharacterSpriteLoader spriteSheet, ref GameData gameData, int index = 0) : this(Vector2.Zero, game, ref spriteSheet, ref gameData, index) { }
            public TileMap(Vector2 viewportPosition, string game, ref CharacterSpriteLoader spriteSheet, ref GameData gameData, int index = 0)
            {
                ViewportPosition = viewportPosition;
                this.game = game;
                this.index = index;
                this.spriteSheet = spriteSheet;
                // load the tile set
                //tileSetAddress = game + ".TileSet.0.CustomData";
                tilesSet = new TileSet(TileSet.GetTileSetFromGridDB(game, 0));
            }
            public TileMap(Vector2 viewportPosition, Vector2 viewportSize, string game, ref CharacterSpriteLoader spriteSheet, ref GameData gameData, int index = 0) : this(viewportPosition, game, ref spriteSheet, ref gameData, index)
            {
                ViewportSize = viewportSize;
            }
        }
        //-----------------------------------------------------------------------
    }
}
