using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Emit;
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
        // a display for a game action variable (binding)
        //-----------------------------------------------------------------------
        public class GameUIVarDisplay : LayoutArea
        {
            GameActionVariable _variable;
            public GameUIVarDisplay(string label, GameActionVariable variable, float fontSize = 0.5f, bool vertical = false)
            {
                Vertical = vertical;
                _variable = variable.ApplyAddressIndexes();
                Items.Add(new LayoutText(label, Color.White, fontSize));
                Items.Add(new LayoutText(_variable.Value, Color.White, fontSize));
                GetSizeFromParent = true;
            }
            public void Update()
            {
                Items[1].Text = _variable.Value;
            }
        }
        //-----------------------------------------------------------------------
    }
}
