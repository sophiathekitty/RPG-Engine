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
        public class LayoutToggle : LayoutArea, ILayoutInteractable
        {
            //----------------------------------------------------------------------
            // properties
            //----------------------------------------------------------------------
            public string ButtonPrompt { get; set; } = "A/D: change, W/S: change more, Q: reset, Space: done";
            bool _value;
            bool previousValue;
            public bool StartValue
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                    previousValue = value;
                }
            }
            public bool Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                    ((LayoutText)Items[1]).Text = _value.ToString();
                }
            }
            public override string Text
            {
                get
                {
                    return ((LayoutText)Items[0]).Text;
                }
                set
                {
                    ((LayoutText)Items[0]).Text = value;
                }
            }
            public override Color Color
            {
                get
                {
                    return ((LayoutText)Items[0]).Color;
                }
                set
                {
                    ((LayoutText)Items[0]).Color = value;
                }
            }
            public Color ValueColor
            {
                get
                {
                    return ((LayoutText)Items[1]).Color;
                }
                set
                {
                    ((LayoutText)Items[1]).Color = value;
                }
            }
            GameInput input;
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public LayoutToggle(string label, bool value, ref GameInput input) : base(Vector2.Zero, Vector2.Zero, Vector2.Zero)
            {
                this.input = input;
                Items.Add(new LayoutText(label, Color.White, 0.5f));
                Items.Add(new LayoutText(value.ToString(), Color.White, 0.5f));
                Vertical = false;
                GetSizeFromParent = true;
                StartValue = value;
            }
            //----------------------------------------------------------------------
            // run the number select
            //----------------------------------------------------------------------
            public string Run()
            {
                Value = !Value;
                return "done";
            }
        }
    }
}
