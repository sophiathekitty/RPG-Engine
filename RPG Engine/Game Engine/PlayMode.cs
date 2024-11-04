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
            GameInput input;
            PlayerSprite player;
            string game = "FinalFantasy";
            //---------------------------------------------------------------------------
            // constructor
            //---------------------------------------------------------------------------
            public PlayMode(IMyTextSurface drawingSurface, GameInput gameInput, IMySoundBlock musicBlock, IMySoundBlock fxBlock) : base(drawingSurface, gameInput, musicBlock, fxBlock)
            {
                GridInfo.Echo("Play Mode");
                BackgroundColor = new Color(0, 10, 20);
                spriteSheet = new CharacterSpriteLoader(GridDB.Get(game + ".Sprites.0.CustomData"));
                gameData = new GameData(game);
                GridInfo.Echo("Creating TileMap");
                map = new TileMap(game, ref spriteSheet, ref gameData);
                AddSprite(map);
                input = gameInput;
                player = new PlayerSprite(Vector2.Zero, map.TileScale, spriteSheet.LoadSpriteSet(0));
                AddSprite(player,1);
            }
            public void LoadGame(string game)
            {
                RemoveSprite(map);
                RemoveSprite(player);
                this.game = game;
                spriteSheet = new CharacterSpriteLoader(GridDB.Get(game + ".Sprites.0.CustomData"));
                gameData = new GameData(game);
                map = new TileMap(game, ref spriteSheet, ref gameData);
                AddSprite(map);
                player = new PlayerSprite(Vector2.Zero, map.TileScale, spriteSheet.LoadSpriteSet(0));
                player.MapPosition = gameData.playerPos;
                map.Load(gameData.mapIndex);
                map.CenterOn(player.MapPosition);
                player.Position = map.TilePosition((int)player.MapPosition.X, (int)player.MapPosition.Y);
                AddSprite(player, 1);
            }
            //---------------------------------------------------------------------------
            // main loop
            //---------------------------------------------------------------------------
            public override void Main(string argument)
            {
                Vector2 move = Vector2.Zero;
                if (input.WPressed)
                {
                    if(map.IsGround((int)player.MapPosition.X, (int)player.MapPosition.Y - 1)) move.Y = -1;
                    player.Direction = 'u';
                }
                else if (input.SPressed)
                {
                    if (map.IsGround((int)player.MapPosition.X, (int)player.MapPosition.Y + 1)) move.Y = 1;
                    player.Direction = 'd';
                }
                else if (input.APressed)
                {
                    if (map.IsGround((int)player.MapPosition.X - 1, (int)player.MapPosition.Y)) move.X = -1;
                    player.Direction = 'l';
                }
                else if (input.DPressed)
                {
                    if (map.IsGround((int)player.MapPosition.X + 1, (int)player.MapPosition.Y)) move.X = 1;
                    player.Direction = 'r';
                }
                if (move != Vector2.Zero)
                {
                    player.MapPosition += move;
                    GridInfo.Echo("Player Position: " + player.MapPosition + ", Map Size: " + map.Size);
                    if (player.MapPosition.X < 0 || player.MapPosition.X >= map.Size.X || player.MapPosition.Y < 0 || player.MapPosition.Y >= map.Size.Y)
                    {
                        if (map.DefaultExit != null && map.DefaultExit.IsValid)
                        {
                            GridInfo.Echo("Player out of bounds, loading default exit");
                            map.Load(map.DefaultExit);
                            player.MapPosition = new Vector2(map.DefaultExit.X, map.DefaultExit.Y);
                        }
                    }
                    else
                    {
                        GridInfo.Echo("Checking for door");
                        MapDoor door = map.IsDoor((int)player.MapPosition.X, (int)player.MapPosition.Y);
                        if (door != null)
                        {
                            GridInfo.Echo("Player at door, loading exit");
                            map.Load(door.exit);
                            player.MapPosition = new Vector2(door.exit.X, door.exit.Y);
                        }
                    }
                    GridInfo.Echo("Centering on player");
                    map.CenterOn(player.MapPosition);
                    player.Position = map.TilePosition((int)player.MapPosition.X, (int)player.MapPosition.Y);
                }
                base.Main(argument);
            }
        }
        //---------------------------------------------------------------------------
    }
}
