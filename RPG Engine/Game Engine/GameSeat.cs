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
        // This class is used to store references to the blocks on the grid that are used to play the game.
        //-----------------------------------------------------------------------
        public class GameSeat : Screen
        {
            //-----------------------------------------------------------------------
            // static methods
            //-----------------------------------------------------------------------
            public static GameSeat FindMain()
            {
                IMyTextSurfaceProvider mainSurface = null;
                IMyShipController mainSeat = null;
                IMySoundBlock musicBlock = null;
                IMySoundBlock fxBlock = null;
                foreach(IMyTextSurfaceProvider privider in GridBlocks.surfaceProviders)
                {
                    IMyTerminalBlock block = privider as IMyTerminalBlock;
                    if (block.CustomName.Contains("RPG.Game"))
                    {
                        mainSurface = privider;
                        musicBlock = privider as IMySoundBlock;
                        break;
                    }
                }
                foreach (IMyShipController seat in GridBlocks.seats)
                {
                    if (seat.CustomName.Contains("RPG.Game"))
                    {
                        mainSeat = seat;
                        break;
                    }
                }
                foreach (IMySoundBlock sound in GridBlocks.soundBlocks)
                {
                    if (sound.CustomName.Contains("RPG.Music") && musicBlock == null)
                    {
                        musicBlock = sound;
                    }
                    if (sound.CustomName.Contains("RPG.FX") && fxBlock == null)
                    {
                        fxBlock = sound;
                    }
                }
                // if we didn't get a main seat or surface return null
                if (mainSurface == null || mainSeat == null) return null;
                return new GameSeat(mainSurface.GetSurface(0), new GameInput(mainSeat), musicBlock, fxBlock);
            }
            //-----------------------------------------------------------------------
            public GameInput input;
            public IMySoundBlock music;
            public IMySoundBlock soundFX;

            public GameSeat(IMyTextSurface drawingSurface, GameInput gameInput, IMySoundBlock musicBlock, IMySoundBlock fxBlock) : base(drawingSurface)
            {
                input = gameInput;
                music = musicBlock;
                soundFX = fxBlock;
            }
        }
        //-----------------------------------------------------------------------
    }
}
