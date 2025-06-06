﻿using Sandbox.Game.EntityComponents;
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
        //List<GameSeat> gameSeats = new List<GameSeat>();
        MapEditor mapEditor;
        PlayMode playMode;
        //TileSet tileSet;
        bool playModeActive = true;
        bool needsToLoad = true;
        public Program()
        {
            Echo("RPG Engine: booting...");
            GridInfo.Init("RPG Engine", GridTerminalSystem, IGC, Me, Echo);
            Echo("GridInfo: initialized!");
            GridBlocks.Init();
            Echo("GridBlocks: initialized!");
            GridDB.Init();
            Echo("GridDB: initialized!");
            //gameSeats.Add(MapEditor.FindMainEditor());
            mapEditor = MapEditor.FindMainEditor();
            Echo("MapEditor: initialized!");
            playMode = PlayMode.FindMainPlayer();
            Echo("PlayMode: initialized!");
            if(playMode == null)
            {
                Echo("PlayMode: not found!");
            }
            //playMode.LoadGame("FinalFantasy");
            Echo("PlayMode: loaded game!");
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            Echo("RPG Engine: booted!");
        }

        public void Save()
        {
            GridInfo.Save();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            //Echo("RPG Engine\nrunning... "+GridInfo.RunCount++);
            /*
            foreach (GameSeat seat in gameSeats)
            {
                seat.Main(argument);
            }
            */
            if (needsToLoad)
            {
                needsToLoad = false;
                playMode.LoadGame("FinalFantasy");
                return;
            }
            if (argument == "PlayMode")
            {
                if(playModeActive)
                {
                    playModeActive = false;
                    // see if this makes it reload nicely
                    mapEditor = MapEditor.FindMainEditor();
                }
                else
                {
                    playModeActive = true;
                    // maybe just reload the playmode...
                    playMode = PlayMode.FindMainPlayer();
                    // reset game....
                    playMode.LoadGame("FinalFantasy");

                }
            }
            if(playModeActive)
            {
                playMode.Main(argument);
            }
            else
            {
                mapEditor.Main(argument);
            }
        }
    }
}
