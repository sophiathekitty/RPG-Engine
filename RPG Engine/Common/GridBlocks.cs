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
        // This class is used to store references to blocks on the grid.
        //-----------------------------------------------------------------------
        public class GridBlocks
        {
            public static List<IMyShipController> seats = new List<IMyShipController>();
            public static List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
            public static List<IMySoundBlock> soundBlocks = new List<IMySoundBlock>();
            public static List<IMyButtonPanel> buttonPanels = new List<IMyButtonPanel>();
            public static List<IMyTextSurfaceProvider> surfaceProviders = new List<IMyTextSurfaceProvider>();
            public static void Init()
            {
                seats.Clear();
                textPanels.Clear();
                soundBlocks.Clear();
                buttonPanels.Clear();
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyShipController>(seats, x => x.IsSameConstructAs(GridInfo.Me));
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(textPanels, x => x.IsSameConstructAs(GridInfo.Me));
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(soundBlocks, x => x.IsSameConstructAs(GridInfo.Me));
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyButtonPanel>(buttonPanels, x => x.IsSameConstructAs(GridInfo.Me));
                foreach (IMyTextPanel panel in textPanels)
                {
                    if (panel.CustomName.Contains("TV") || panel.CustomName.Contains("Screen") || panel.CustomName.Contains("RPG"))
                    {
                        surfaceProviders.Add(panel as IMyTextSurfaceProvider);
                    }
                }
                foreach (IMySoundBlock sound in soundBlocks)
                {
                    if (sound is IMyTextSurfaceProvider && (sound.CustomName.Contains("TV") || sound.CustomName.Contains("Screen") || sound.CustomName.Contains("RPG")))
                    {
                        surfaceProviders.Add(sound as IMyTextSurfaceProvider);
                    }
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
