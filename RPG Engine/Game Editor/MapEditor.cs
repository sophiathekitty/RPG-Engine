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
            GameData gameData;
            TileMap tileMap;
            ScreenSprite tilePreview;
            ScreenSprite tileLayer;
            ScreenSprite inputPrompt;
            MapCursor cursor;
            List<MapCursor> doors = new List<MapCursor>();
            Vector2 cursorPosition = Vector2.Zero;
            int tileIndex = 0;
            char currentTile = '0';
            LayoutArea mapInfoDisplay;
            LayoutText mapSavedStatus;
            LayoutText mapIndex;
            LayoutText mapInfoSize;
            LayoutText mapInfoCursorPosition;
            MapEditorMainMenu mainMenu;
            LoadMapSelecter loadMapSelecter;
            CreateMapForm createMapForm;
            MapOptionsForm mapOptionsForm;
            DoorInfoForm doorInfoForm;
            CharacterSpriteLoader spriteSheet;
            NPCOptions npcOptions;
            string focused = "menu";
            string game = "FinalFantasy";
            public MapEditor(IMyTextSurface drawingSurface, GameInput gameInput, IMySoundBlock musicBlock, IMySoundBlock fxBlock) : base(drawingSurface, gameInput, musicBlock, fxBlock)
            {
                GridInfo.Echo("Map Editor");
                BackgroundColor = new Color(0, 10, 20);
                spriteSheet = new CharacterSpriteLoader(GridDB.Get(game + ".Sprites.0.CustomData"));
                gameData = new GameData(game, null);
                GridInfo.Echo("Creating TileMap");
                tileMap = new TileMap(new Vector2(60,30), new Vector2(24,24), game, spriteSheet, gameData);
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
                AddSprite(cursor,4);
                // mape info
                GridInfo.Echo("Creating Map Info");
                mapInfoDisplay = new LayoutArea(new Vector2(0, 200), new Vector2(60, 30), new Vector2(5, 5));
                mapInfoSize = new LayoutText("Map: 32x24", Color.White, 0.25f);
                mapInfoCursorPosition = new LayoutText("Cur: 0,0", Color.White, 0.25f);
                mapIndex = new LayoutText("Id: "+tileMap.index, Color.White, 0.25f);
                mapSavedStatus = new LayoutText("New", Color.Green, 0.25f);
                mapInfoDisplay.Items.Add(mapSavedStatus);
                mapInfoDisplay.Items.Add(mapIndex);
                mapInfoDisplay.Items.Add(mapInfoSize);
                mapInfoDisplay.Items.Add(mapInfoCursorPosition);
                mapInfoDisplay.Position = new Vector2(0, Size.Y - mapInfoDisplay.ContentSize.Y -mapInfoDisplay.Padding.Y - 20);
                mapInfoDisplay.ApplyLayout();
                AddSprite(mapInfoDisplay);
                // map menu
                GridInfo.Echo("Creating Main Menu");
                mainMenu = new MapEditorMainMenu(new Vector2(0, 80), new Vector2(60, 120), new Vector2(5, 5), input);
                mainMenu.ApplyLayout();
                mainMenu.SelectedIndex = 0;
                AddSprite(mainMenu);
                // input prompt
                GridInfo.Echo("Creating Input Prompt");
                inputPrompt = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopRight, new Vector2(-5, 5), 0.45f, new Vector2(60, 30), Color.White, "Monospace", mainMenu.ButtonPrompt, TextAlignment.RIGHT, SpriteType.TEXT);
                AddSprite(inputPrompt);
                // load map selecter
                GridInfo.Echo("Creating Load Map Selecter");
                loadMapSelecter = new LoadMapSelecter(new Vector2(100, 100), new Vector2(100, 120), new Vector2(5, 5), input, 10);
                loadMapSelecter.ApplyLayout();
                // create map form
                GridInfo.Echo("Creating Create Map Form");
                createMapForm = new CreateMapForm(input, game);
                createMapForm.ApplyLayout();
                // map options form
                GridInfo.Echo("Creating Map Options Form");
                mapOptionsForm = new MapOptionsForm(input, game);
                mapOptionsForm.ApplyLayout();
                // door info form
                GridInfo.Echo("Creating Door Info Form");
                doorInfoForm = new DoorInfoForm(input, game);
                doorInfoForm.ApplyLayout();
                // npc info form
                GridInfo.Echo("Creating NPC Sprite Sheet");
                GridInfo.Echo("Creating NPC Options");
                npcOptions = new NPCOptions(input, spriteSheet, gameData);
                npcOptions.ApplyLayout();
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
                else if (focused == "ceiling")
                {
                    MapEditing(true);
                }
                else if (focused == "load")
                {
                    LoadingMap();
                }
                else if (focused == "create")
                {
                    CreatingMap();
                }
                else if (focused == "options")
                {
                    MapOptions();
                }
                else if (focused == "doors")
                {
                    DoorEditing();
                }
                else if (focused == "doorOptions")
                {
                    DoorOptionsEditing();
                }
                else if (focused == "npc")
                {
                    NPCEditing();
                }
                else if (focused == "npcOptions")
                {
                    NPCOptionsEditing();
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
                    inputPrompt.Data = "(M) E/C: tile, W/A/S/D: move, Space: set, Q: back";
                }
                else if(result == "Ceiling")
                {
                    focused = "ceiling";
                    mainMenu.SelectedIndex = -1;
                    cursor.Visible = true;
                    inputPrompt.Data = "(C) E/C: tile, W/A/S/D: move, Space: set, Q: back";
                }
                else if (result == "Save")
                {
                    tileMap.Save();
                    mapSavedStatus.Text = "Saved";
                }
                else if (result == "Load")
                {
                    focused = "load";
                    mainMenu.SelectedIndex = -1;
                    inputPrompt.Data = loadMapSelecter.ButtonPrompt;
                    loadMapSelecter.MaxMapIndex = TileMap.GetMapCount(game) - 1;
                    loadMapSelecter.Reset();
                    loadMapSelecter = new LoadMapSelecter(new Vector2(100, 100), new Vector2(100, 120), new Vector2(5, 5), input, TileMap.GetMapCount(game) - 1);
                    loadMapSelecter.ApplyLayout();
                    AddSprite(loadMapSelecter,4);
                }
                else if (result == "New")
                {
                    focused = "create";
                    mainMenu.SelectedIndex = -1;
                    AddSprite(createMapForm, 4);
                }
                else if (result == "Options")
                {
                    focused = "options";
                    mainMenu.SelectedIndex = -1;
                    mapOptionsForm.MapSize = tileMap.Size;
                    mapOptionsForm.TileSetIndex = tileMap.tileSetIndex;
                    mapOptionsForm.Exit = tileMap.DefaultExit;
                    AddSprite(mapOptionsForm, 4);
                }
                else if (result == "Doors")
                {
                    focused = "doors";
                    mainMenu.SelectedIndex = -1;
                    cursor.Visible = true;
                    ApplyDoorsOverlay();
                }
                else if (result == "NPCs")
                {
                    focused = "npc";
                    mainMenu.SelectedIndex = -1;
                    cursor.Visible = true;
                }
            }
            //-----------------------------------------------------------------------
            // map loading
            //-----------------------------------------------------------------------
            void LoadingMap()
            {
                string result = loadMapSelecter.Run();
                if (result == "done")
                {
                    tileMap.Load(loadMapSelecter.MapIndex);
                    mapIndex.Text = "Id: " + tileMap.index;
                    mapInfoSize.Text = "Map: " + tileMap.Size.X + "x" + tileMap.Size.Y;
                    mapSavedStatus.Text = "Loaded";
                    tileIndex = 0;
                    cursorPosition = Vector2.Zero;
                    currentTile = tileMap.tilesSet.tiles.Keys.ToArray()[tileIndex];
                    tileMap.CenterOn(0,0);
                    tilePreview.Data = tileMap.tilesSet.tiles[currentTile];
                    foreach (MapCursor door in doors) RemoveSprite(door);
                    doors.Clear();
                    foreach (MapDoor door in tileMap.Doors)
                    {
                        MapCursor cursor = new MapCursor();
                        cursor.Size = tileMap.TileSize;
                        cursor.Position = tileMap.TilePosition((int)door.X, (int)door.Y);
                        cursor.Color = Color.Red;
                        cursor.Visible = false;
                        doors.Add(cursor);
                        AddSprite(cursor, 3);
                    }
                }
                else if (result == "") return;
                focused = "menu";
                mainMenu.SelectedIndex = 1;
                inputPrompt.Data = mainMenu.ButtonPrompt;
                RemoveSprite(loadMapSelecter);
            }
            //-----------------------------------------------------------------------
            // map creation
            //-----------------------------------------------------------------------
            void CreatingMap()
            {
                string result = createMapForm.Run();
                inputPrompt.Data = createMapForm.ButtonPrompt;
                if (result == "Create")
                {
                    tileMap.CreateMap((int)createMapForm.MapSize.X,(int)createMapForm.MapSize.Y, createMapForm.TileSetIndex);
                    mapIndex.Text = "Id: " + tileMap.index;
                    mapInfoSize.Text = "Map: " + tileMap.Size.X + "x" + tileMap.Size.Y;
                    mapSavedStatus.Text = "New";
                    tileIndex = 0;
                    currentTile = tileMap.tilesSet.tiles.Keys.ToArray()[tileIndex];
                    tilePreview.Data = tileMap.tilesSet.tiles[currentTile];
                }
                else if (result == "") return;
                focused = "menu";
                mainMenu.SelectedIndex = 3;
                inputPrompt.Data = mainMenu.ButtonPrompt;
                RemoveSprite(createMapForm);
            }
            //-----------------------------------------------------------------------
            // map options
            //-----------------------------------------------------------------------
            void MapOptions()
            {
                string result = mapOptionsForm.Run();
                inputPrompt.Data = mapOptionsForm.ButtonPrompt;
                if (result == "Apply")
                {
                    tileMap.ResizeMap(mapOptionsForm.MapSize);
                    tileMap.tileSetIndex = mapOptionsForm.TileSetIndex;
                    tileMap.DefaultExit = mapOptionsForm.Exit;
                    mapIndex.Text = "Id: " + tileMap.index;
                    mapInfoSize.Text = "Map: " + tileMap.Size.X + "x" + tileMap.Size.Y;
                    mapSavedStatus.Text = "Unsaved";
                }
                else if (result == "") return;
                focused = "menu";
                mainMenu.SelectedIndex = 3;
                inputPrompt.Data = mainMenu.ButtonPrompt;
                RemoveSprite(mapOptionsForm);
            }
            //-----------------------------------------------------------------------
            // door overlay
            //-----------------------------------------------------------------------
            void ApplyDoorsOverlay()
            {
                for (int i = 0; i < tileMap.Doors.Count; i++)
                {
                    doors[i].Visible = tileMap.IsWithinViewport((int)tileMap.Doors[i].X, (int)tileMap.Doors[i].Y);
                    doors[i].Position = tileMap.TilePosition((int)tileMap.Doors[i].X, (int)tileMap.Doors[i].Y);
                    inputPrompt.Data = (cursorPosition == new Vector2(tileMap.Doors[i].X, tileMap.Doors[i].Y)) ? "W/A/S/D: move, Space: edit, Q: back" : "W/A/S/D: move, Space: add, Q: back";
                }
            }
            void HideDoorsOverlay()
            {
                foreach (MapCursor door in doors) door.Visible = false;
                GridInfo.Echo("doors hidden???");
            }
            //-----------------------------------------------------------------------
            // door editing
            //-----------------------------------------------------------------------
            void DoorEditing()
            {
                if (MoveCursor()) ApplyDoorsOverlay();
                if (input.SpacePressed)
                {
                    doorInfoForm = new DoorInfoForm(input, game);
                    doorInfoForm.ApplyLayout();
                    MapDoor door = tileMap.IsDoor((int)cursorPosition.X, (int)cursorPosition.Y);
                    if (door != null)
                    {
                        // edit door
                        doorInfoForm.Exit = door.exit;
                    }
                    focused = "doorOptions";
                    AddSprite(doorInfoForm, 4);
                    inputPrompt.Data = doorInfoForm.ButtonPrompt;
                }
            }
            void DoorOptionsEditing()
            {
                string result = doorInfoForm.Run();
                inputPrompt.Data = doorInfoForm.ButtonPrompt;
                if (result == "Save")
                {
                    MapDoor door = tileMap.IsDoor((int)cursorPosition.X, (int)cursorPosition.Y);
                    if (door != null)
                    {
                        door.exit = doorInfoForm.Exit;
                    }
                    else
                    {
                        MapDoor newDoor = new MapDoor((int)cursorPosition.X, (int)cursorPosition.Y, doorInfoForm.Exit);
                        tileMap.Doors.Add(newDoor);
                        MapCursor cursor = new MapCursor();
                        cursor.Size = tileMap.TileSize;
                        cursor.Position = tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y);
                        cursor.Color = Color.Red;
                        cursor.Visible = true;
                        doors.Add(cursor);
                        AddSprite(cursor, 3);
                    }
                }
                else if (result == "Remove")
                {
                    MapDoor door = tileMap.IsDoor((int)cursorPosition.X, (int)cursorPosition.Y);
                    if (door != null)
                    {
                        tileMap.Doors.Remove(door);
                        MapCursor cursor = doors.Find(d => d.Position == tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y));
                        if (cursor != null)
                        {
                            doors.Remove(cursor);
                            RemoveSprite(cursor);
                        }
                    }
                }
                else if (result == "") return;
                focused = "doors";
                RemoveSprite(doorInfoForm);
            }
            //-----------------------------------------------------------------------
            // npc editing
            //-----------------------------------------------------------------------
            void NPCEditing()
            {
                MoveCursor();
                if(input.SpacePressed)
                {
                    // see if there is an npc at this location
                    NPC npc = tileMap.GetNPC((int)cursorPosition.X, (int)cursorPosition.Y);
                    if (npc != null)
                    {
                        // edit npc
                        npcOptions.SetNPC(ref npc);
                    }
                    else
                    {
                        // create new npc
                        npc = new NPC(cursorPosition, 1, spriteSheet.LoadSpriteSet(0));
                        npc.MapPosition = cursorPosition;
                        npcOptions.SetNPC(ref npc);
                    }
                    focused = "npcOptions";
                    AddSprite(npcOptions, 4);
                    inputPrompt.Data = npcOptions.ButtonPrompt;
                }
            }
            void NPCOptionsEditing()
            {
                string result = npcOptions.Run();
                inputPrompt.Data = npcOptions.ButtonPrompt;
                if (result == "Apply")
                {
                    NPC npc = npcOptions.GetNPC();
                    tileMap.SetNPC(npc);
                }
                else if (result == "Remove")
                {
                    tileMap.RemoveNPC((int)cursorPosition.X, (int)cursorPosition.Y);
                }
                else if (result == "") return;
                focused = "npc";
                RemoveSprite(npcOptions);
            }
            //-----------------------------------------------------------------------
            // map editing
            //-----------------------------------------------------------------------
            void MapEditing(bool ceiling = false)
            {
                MoveCursor();
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
                else if (input.SpacePressed)
                {
                    if (ceiling) tileMap.SetCeilingTile((int)cursorPosition.X, (int)cursorPosition.Y, currentTile);
                    else tileMap.SetTile((int)cursorPosition.X, (int)cursorPosition.Y, currentTile);
                    mapSavedStatus.Text = "Unsaved";
                }

            }
            //-----------------------------------------------------------------------
            // move cursor
            //-----------------------------------------------------------------------
            public bool MoveCursor()
            {
                bool moved = false;
                if (input.WPressed)
                {
                    cursorPosition += new Vector2(0, -1);
                    if (cursorPosition.Y < 0) cursorPosition.Y = 0;
                    tileMap.CenterOn(cursorPosition);
                    cursor.Position = tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y);
                    moved = true;
                }
                else if (input.APressed)
                {
                    cursorPosition += new Vector2(-1, 0);
                    if (cursorPosition.X < 0) cursorPosition.X = 0;
                    tileMap.CenterOn(cursorPosition);
                    cursor.Position = tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y);
                    moved = true;
                }
                else if (input.SPressed)
                {
                    cursorPosition += new Vector2(0, 1);
                    if (cursorPosition.Y >= tileMap.Size.Y) cursorPosition.Y = tileMap.Size.Y - 1;
                    tileMap.CenterOn(cursorPosition);
                    cursor.Position = tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y);
                    moved = true;

                }
                else if (input.DPressed)
                {
                    cursorPosition += new Vector2(1, 0);
                    if (cursorPosition.X >= tileMap.Size.X) cursorPosition.X = tileMap.Size.X - 1;
                    tileMap.CenterOn(cursorPosition);
                    cursor.Position = tileMap.TilePosition((int)cursorPosition.X, (int)cursorPosition.Y);
                    moved = true;
                }
                else if (input.QPressed)
                {
                    focused = "menu";
                    mainMenu.SelectedIndex = 3;
                    cursor.Visible = false;
                    inputPrompt.Data = mainMenu.ButtonPrompt;
                    HideDoorsOverlay();
                }
                mapInfoCursorPosition.Text = "Cur: " + cursorPosition.X + "," + cursorPosition.Y;
                return moved;
            }
        }
        //-----------------------------------------------------------------------
    }
}
