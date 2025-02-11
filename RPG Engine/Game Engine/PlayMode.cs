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
using static IngameScript.Program;

namespace IngameScript
{
    partial class Program
    {
        //---------------------------------------------------------------------------
        // PlayMode - the main game mode where the player can move around the map
        //---------------------------------------------------------------------------
        public class PlayMode : GameSeat
        {
            //---------------------------------------------------------------------------
            // FindMainPlayer - find the main player seat and drawing surface
            //---------------------------------------------------------------------------
            public static PlayMode FindMainPlayer()
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
                if (mainSurface != null && mainSeat != null) return new PlayMode(mainSurface.GetSurface(0), new GameInput(mainSeat), musicBlock, fxBlock);
                GameSeat gameSeat = GameSeat.FindMain();
                if (gameSeat != null) return new PlayMode(gameSeat.DrawingSurface, gameSeat.input, gameSeat.music, gameSeat.soundFX);
                return null;
            }
            //---------------------------------------------------------------------------
            // members
            //---------------------------------------------------------------------------
            TileMap map;
            GameData gameData;
            CharacterSpriteLoader spriteSheet;
            //GameInput input;
            PlayerSprite player;
            string game = "FinalFantasy";
            GameUILayoutBuilder uiBuilder;
            //Stack<MapExit> mapExits = new Stack<MapExit>();
            Vector2 InteractPos 
            { 
                get 
                { 
                    switch(player.Direction)
                    {
                        case 'u': return player.MapPosition + new Vector2(0, -1);
                        case 'd': return player.MapPosition + new Vector2(0, 1);
                        case 'l': return player.MapPosition + new Vector2(-1, 0);
                        case 'r': return player.MapPosition + new Vector2(1, 0);
                        default: return player.MapPosition;
                    }
                }
            }
            Vector2 CounterInteractPos // one extra step away
            {
                get
                {
                    switch (player.Direction)
                    {
                        case 'u': return player.MapPosition + new Vector2(0, -2);
                        case 'd': return player.MapPosition + new Vector2(0, 2);
                        case 'l': return player.MapPosition + new Vector2(-2, 0);
                        case 'r': return player.MapPosition + new Vector2(2, 0);
                        default: return player.MapPosition;
                    }
                }
            }
            //---------------------------------------------------------------------------
            // constructor
            //---------------------------------------------------------------------------
            public PlayMode(IMyTextSurface drawingSurface, GameInput gameInput, IMySoundBlock musicBlock, IMySoundBlock fxBlock) : base(drawingSurface, gameInput, musicBlock, fxBlock)
            {
                GridInfo.Echo("PlayMode Constructor");
                BackgroundColor = Color.Black;
                spriteSheet = new CharacterSpriteLoader(GridDB.Get(game + ".Sprites.0.CustomData"));
                uiBuilder = new GameUILayoutBuilder(gameInput,game);
                
                //gameData = new GameData(game, uiBuilder);
                //map = new TileMap(game, spriteSheet, gameData);
                //AddSprite(map);
                //player = new PlayerSprite(Vector2.Zero, map.TileScale, spriteSheet.LoadSpriteSet(0), spriteSheet);
                //AddSprite(player,1);
                //AddSprite(uiBuilder);
                GridInfo.Echo("PlayMode Constructor Done");
            }
            public void LoadGame(string game)
            {
                GridInfo.Echo("Loading Game: " + game);
                if (map != null)
                {
                    RemoveSprite(map);
                    RemoveSprite(player);
                    RemoveSprite(uiBuilder);
                }
                this.game = game;
                spriteSheet = new CharacterSpriteLoader(GridDB.Get(game + ".Sprites.0.CustomData"));
                gameData = new GameData(game,uiBuilder);
                map = new TileMap(game, spriteSheet, gameData);
                AddSprite(map);
                player = new PlayerSprite(Vector2.Zero, map.TileScale, spriteSheet.LoadSpriteSet(0),spriteSheet);
                player.MapPosition = gameData.playerPos;
                map.Load(gameData.mapIndex);
                map.CenterOn(player.MapPosition);
                player.Position = map.TilePosition((int)player.MapPosition.X, (int)player.MapPosition.Y);
                AddSprite(player, 1);
                gameData.playerSprite = player;
                gameData.map = map;
                AddSprite(uiBuilder);
                GridInfo.Echo("Loading Game Done");
                if(gameData.Actions.ContainsKey("GameStart")) gameData.Actions["GameStart"].Execute();
            }
            //---------------------------------------------------------------------------
            // main loop
            //---------------------------------------------------------------------------
            public override void Main(string argument)
            {
                string cmd = uiBuilder.Run();
                if(cmd == "go") // go around map
                {
                    Vector2 move = Vector2.Zero;
                    if (input.WPressed)
                    {
                        move.Y = -1;
                        player.Direction = 'u';
                    }
                    else if (input.SPressed)
                    {
                        move.Y = 1;
                        player.Direction = 'd';
                    }
                    else if (input.APressed)
                    {
                        move.X = -1;
                        player.Direction = 'l';
                    }
                    else if (input.DPressed)
                    {
                        move.X = 1;
                        player.Direction = 'r';
                    }
                    if (move != Vector2.Zero && map.IsGround(player.MapPosition + move))
                    {
                        player.MapPosition += move;
                        //GridInfo.Echo("Player Position: " + player.MapPosition + ", Map Size: " + map.Size);
                        if (player.MapPosition.X < 0 || player.MapPosition.X >= map.Size.X || player.MapPosition.Y < 0 || player.MapPosition.Y >= map.Size.Y)
                        {
                            if (map.DefaultExit != null && map.DefaultExit.IsValid)
                            {
                                //GridInfo.Echo("Player out of bounds, loading default exit");
                                MapExit exit = map.DefaultExit;
                                map.Load(exit);
                                player.MapPosition = new Vector2(exit.X, exit.Y);
                            }
                        }
                        else
                        {
                            //GridInfo.Echo("Checking for door");
                            MapDoor door = map.IsDoor((int)player.MapPosition.X, (int)player.MapPosition.Y);
                            NPC guardedTile = map.GetNPC(player.MapPosition);
                            if (door != null)
                            {
                                //GridInfo.Echo("Player at door, loading exit");
                                map.Load(door.exit);
                                player.MapPosition = new Vector2(door.exit.X, door.exit.Y);
                            }
                            else if (guardedTile != null && guardedTile.guardedSpace)
                            {
                                GridInfo.Echo("Player at guarded space");
                                map.CenterOn(player.MapPosition);
                                player.Position = map.TilePosition((int)player.MapPosition.X, (int)player.MapPosition.Y);
                                if (gameData.Actions.ContainsKey(guardedTile.InteractAction)) gameData.Actions[guardedTile.InteractAction].Execute(guardedTile);
                            }
                            else
                            {
                                if (gameData.Actions.ContainsKey("PlayerStep")) gameData.Actions["PlayerStep"].Execute();
                            }
                        }
                        //GridInfo.Echo("Centering on player");
                        map.CenterOn(player.MapPosition);
                        player.Position = map.TilePosition((int)player.MapPosition.X, (int)player.MapPosition.Y);
                    }
                    else
                    {
                        if(move!= Vector2.Zero)
                        {
                            // see if there's an npc on the tile
                            NPC npc = map.GetNPC(player.MapPosition + move);
                            if(npc != null) map.PushNPC(npc);
                        }
                    }
                    // interact
                    if (input.SpacePressed)
                    {
                        NPC npc = map.GetNPC(InteractPos);
                        if (npc != null)
                        {
                            if (gameData.Actions.ContainsKey(npc.InteractAction)) gameData.Actions[npc.InteractAction].Execute(npc);
                        }
                        else if (map.IsCounter(InteractPos))
                        {
                            NPC counter = map.GetNPC(CounterInteractPos);
                            if (counter != null)
                            {
                                if (gameData.Actions.ContainsKey(counter.InteractAction)) gameData.Actions[counter.InteractAction].Execute(counter);
                            }
                        }
                    }
                    // game menu
                    else if (input.EPressed)
                    {
                        if (gameData.Actions.ContainsKey("GameMenu")) gameData.Actions["GameMenu"].Execute();
                    }
                    // pause menu
                    else if (input.QPressed)
                    {
                        if (gameData.Actions.ContainsKey("PauseMenu")) gameData.Actions["PauseMenu"].Execute();
                    }
                    map.RandomWalkNPCs();
                }
                // go done
                base.Main(argument);
            }
        }
        //---------------------------------------------------------------------------
    }
}
