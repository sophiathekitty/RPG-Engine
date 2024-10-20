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
    partial class Program : MyGridProgram
    {
        //=======================================================================
        // RPG Engine
        //=======================================================================
        List<GameSeat> gameSeats = new List<GameSeat>();
        //TileSet tileSet;
        public Program()
        {
            Echo("RPG Engine\nbooting...");
            GridInfo.Init("RPG Engine", this);
            Echo("GridInfo\ninitialized!");
            GridBlocks.Init();
            Echo("GridBlocks\ninitialized!");
            GridDB.Init();
            Echo("GridDB\ninitialized!");
            gameSeats.Add(MapEditor.FindMainEditor());
            Echo("GameSeat\ninitialized!");
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            Echo("RPG Engine\nbooted!");
        }

        public void Save()
        {
            GridInfo.Save();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            //Echo("RPG Engine\nrunning... "+GridInfo.RunCount++);
            foreach (GameSeat seat in gameSeats)
            {
                seat.Main(argument);
            }
        }
    }
}
