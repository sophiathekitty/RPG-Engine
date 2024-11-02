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
        // parses a simple script
        // ActionName@Script
        // commands:
        //      set:[object.]property=value
        //      if:[object.]property=value
        //      ifnot:[object.]property=value
        //      else
        //      endif
        //      add:[object.]property=value
        //      sub:[object.]property=value
        //      mul:[object.]property=value
        //      div:[object.]property=value
        //      say:some string of text to say
        //      savegame
        //-----------------------------------------------------------------------
        public class GameAction
        {
            public GameAction(string data)
            {

            }
        }
        //-----------------------------------------------------------------------
    }
}
