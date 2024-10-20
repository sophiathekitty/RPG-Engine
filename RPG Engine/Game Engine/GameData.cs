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
        // holds the data for the game
        //-----------------------------------------------------------------------
        public class GameData
        {
            public int saveIndex = 0;
            public Dictionary<string, bool> Bools = new Dictionary<string, bool>();
            public Dictionary<string, int> Ints = new Dictionary<string, int>();
            public Dictionary<string, int> Inventory = new Dictionary<string, int>();
            public List<PlayerCharacter> CharacterList = new List<PlayerCharacter>();
        }
        //-----------------------------------------------------------------------
    }
}
