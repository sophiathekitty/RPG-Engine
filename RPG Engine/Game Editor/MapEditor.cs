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
        // This is the main game editor class.
        //-----------------------------------------------------------------------
        public class MapEditor : GameSeat
        {
            //-----------------------------------------------------------------------
            // static methods
            //-----------------------------------------------------------------------
            public static MapEditor FindMainEditor()
            {
                IMyTextSurfaceProvider mainSurface = null;
                IMyShipController mainSeat = null;
                IMySoundBlock musicBlock = null;
                IMySoundBlock fxBlock = null;
                foreach (IMyTextSurfaceProvider privider in GridBlocks.surfaceProviders)
                {
                    IMyTerminalBlock block = privider as IMyTerminalBlock;
                    if (block.CustomName.Contains("RPG.Editor"))
                    {
                        mainSurface = privider;
                        break;
                    }
                }
                foreach (IMyShipController seat in GridBlocks.seats)
                {
                    if (seat.CustomName.Contains("RPG.Editor"))
                    {
                        mainSeat = seat;
                        break;
                    }
                }
                foreach (IMySoundBlock sound in GridBlocks.soundBlocks)
                {
                    if (sound.CustomName.Contains("RPG.EditorMusic"))
                    {
                        musicBlock = sound;
                    }
                    if (sound.CustomName.Contains("RPG.EditorFX"))
                    {
                        fxBlock = sound;
                    }
                }
                if(mainSurface != null && mainSeat != null) return new MapEditor(mainSurface.GetSurface(0), new GameInput(mainSeat), musicBlock, fxBlock);
                GameSeat gameSeat = GameSeat.FindMain();
                if (gameSeat != null) return new MapEditor(gameSeat.DrawingSurface, gameSeat.input, gameSeat.music, gameSeat.soundFX);
                return null;
            }
            //-----------------------------------------------------------------------
            TileMap tileMap;
            ScreenSprite tilePreview;
            ScreenSprite tileLayer;
            MapCursor cursor;
            Vector2 cursorPosition = Vector2.Zero;
            int tileIndex = 0;
            char currentTile = '0';
            LayoutArea mapInfoDisplay;
            LayoutText mapIndex;
            LayoutText mapInfoSize;
            LayoutText mapInfoCursorPosition;
            LayoutMenu mainMenu;
            string focused = "menu";
            public MapEditor(IMyTextSurface drawingSurface, GameInput gameInput, IMySoundBlock musicBlock, IMySoundBlock fxBlock) : base(drawingSurface, gameInput, musicBlock, fxBlock)
            {
                BackgroundColor = new Color(0, 10, 20);
                tileMap = new TileMap(new Vector2(60,30), new Vector2(24,24), "FinalFantasy");
                tilePreview = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, new Vector2(10, 10), 0.1f, Vector2.Zero, Color.White, "Monospace", "",TextAlignment.LEFT, SpriteType.TEXT);
                tileLayer = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, new Vector2(56, 58), 0.5f, Vector2.Zero, Color.White, "Monospace", "", TextAlignment.RIGHT, SpriteType.TEXT);
                currentTile = tileMap.tilesSet.tiles.Keys.ToArray()[tileIndex];
                tilePreview.Data = tileMap.tilesSet.tiles[currentTile];
                tileLayer.Data = tileMap.tilesSet.GetLayer(currentTile).ToString();
                AddSprite(tilePreview);
                AddSprite(tileLayer);
                AddSprite(tileMap);
                tileMap.CreateMap(32, 24);
                cursor = new MapCursor();
                cursor.Size = tileMap.TileSize;
                cursor.Position = tileMap.ViewportPosition;
                cursor.Visible = false;
                AddSprite(cursor);
                // mape info
                mapInfoDisplay = new LayoutArea(new Vector2(0, 200), new Vector2(60, 30), new Vector2(5, 5));
                mapInfoSize = new LayoutText("Map: 32x24", Color.White, 0.25f);
                mapInfoCursorPosition = new LayoutText("Cur: 0,0", Color.White, 0.25f);
                mapIndex = new LayoutText("Id: "+tileMap.index, Color.White, 0.25f);
                mapInfoDisplay.Items.Add(mapIndex);
                mapInfoDisplay.Items.Add(mapInfoSize);
                mapInfoDisplay.Items.Add(mapInfoCursorPosition);
                mapInfoDisplay.Position = new Vector2(0, Size.Y - mapInfoDisplay.ContentSize.Y -mapInfoDisplay.Padding.Y - 20);
                mapInfoDisplay.ApplyLayout();
                AddSprite(mapInfoDisplay);
                // map menu
                mainMenu = new LayoutMenu(new Vector2(0, 80), new Vector2(60, 120), new Vector2(5, 5), input);
                mainMenu.FontSize = 0.32f;
                mainMenu.Add("Save");
                mainMenu.Add("Load");
                mainMenu.Add("New");
                mainMenu.Add("Map");
                mainMenu.Add("Ceiling");
                mainMenu.Add("NPCs");
                mainMenu.Add("Doors");
                mainMenu.Add("Options");
                mainMenu.ApplyLayout();
                mainMenu.SelectedIndex = 0;
                AddSprite(mainMenu);
            }
            //-----------------------------------------------------------------------
            // main loop
            //-----------------------------------------------------------------------
            public override void Main(string argument)
            {
                if (focused == "menu")
                {
                    MainMenu();
                }
                else if (focused == "map")
                {
                    MapEditing();
                }
                base.Main(argument);
            }
            //-----------------------------------------------------------------------
            // main menu
            //-----------------------------------------------------------------------
            void MainMenu()
            {
                string result = mainMenu.Run();
                if (result == "Map")
                {
                    focused = "map";
                    mainMenu.SelectedIndex = -1;
                    cursor.Visible = true;
                }
            }
            //-----------------------------------------------------------------------
            // map editing
            //-----------------------------------------------------------------------
            void MapEditing()
            {
                if (input.EPressed)
                {
                    tileIndex--;
                    if (tileIndex < 0) tileIndex = tileMap.tilesSet.tiles.Count - 1;
                    currentTile = tileMap.tilesSet.tiles.Keys.ToArray()[tileIndex];
                    tilePreview.Data = tileMap.tilesSet.tiles[currentTile];
                    tileLayer.Data = tileMap.tilesSet.GetLayer(currentTile).ToString();
                }
                else if (input.CPressed)
                {
                    tileIndex++;
                    if (tileIndex >= tileMap.tilesSet.tiles.Count) tileIndex = 0;
                    currentTile = tileMap.tilesSet.tiles.Keys.ToArray()[tileIndex];
                    tilePreview.Data = tileMap.tilesSet.tiles[currentTile];
                    tileLayer.Data = tileMap.tilesSet.GetLayer(currentTile).ToString();
                }
                else if (input.WPressed)
                {
                    cursorPosition += new Vector2(0, -1);
                    tileMap.CenterOn(cursorPosition);
                    cursor.Position = tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y);
                }
                else if (input.APressed)
                {
                    cursorPosition += new Vector2(-1, 0);
                    tileMap.CenterOn(cursorPosition);
                    cursor.Position = tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y);
                }
                else if (input.SPressed)
                {
                    cursorPosition += new Vector2(0, 1);
                    tileMap.CenterOn(cursorPosition);
                    cursor.Position = tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y);

                }
                else if (input.DPressed)
                {
                    cursorPosition += new Vector2(1, 0);
                    tileMap.CenterOn(cursorPosition);
                    cursor.Position = tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y);
                }
                else if (input.SpacePressed)
                {
                    tileMap.SetTile((int)cursorPosition.X, (int)cursorPosition.Y, currentTile);
                }
                else if (input.QPressed)
                {
                    focused = "menu";
                    mainMenu.SelectedIndex = 3;
                    cursor.Visible = false;
                }
                mapInfoCursorPosition.Text = "Cur: " + cursorPosition.X + "," + cursorPosition.Y;

            }
        }
        //-----------------------------------------------------------------------
    }
}
