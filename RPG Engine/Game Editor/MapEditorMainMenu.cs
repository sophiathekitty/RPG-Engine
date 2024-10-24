using EmptyKeys.UserInterface.Generated;
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
        //----------------------------------------------------------------------
        // main menu for the map editor
        public class MapEditorMainMenu : LayoutMenu
        {
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public MapEditorMainMenu(Vector2 position, Vector2 size, Vector2 padding, GameInput input) : base(position, size, padding, input)
            {
                FontSize = 0.32f;
                Add("Save");
                Add("Load");
                Add("New");
                Add("Map");
                Add("Ceiling");
                Add("NPCs");
                Add("Doors");
                Add("Options");
            }
            //----------------------------------------------------------------------
            // run
            //----------------------------------------------------------------------
            public override string Run()
            {
                string cmd = base.Run();
                return cmd;
            }
        }
    }
}
