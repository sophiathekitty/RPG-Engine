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
        // number select layout
        //----------------------------------------------------------------------
        public class LayoutNumberSelect : LayoutArea, ILayoutInteractable
        {
            //----------------------------------------------------------------------
            // properties
            //----------------------------------------------------------------------
            public string ButtonPrompt { get; set; } = "A/D: change, W/S: change more, Q: reset, Space: done";
            public float MinValue { get; set; } = 0;
            public virtual float MaxValue { get; set; } = 100;
            public float Step { get; set; } = 1;
            public float BigStep { get; set; } = 10;
            float _value;
            public float previousValue;
            public float StartValue
            {
                get
                {
                    return _value;
                }
                set
                {
                    //_value = value;
                    previousValue = value;
                    Value = value;
                }
            }
            public virtual float Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                    if (_value < MinValue) _value = MinValue;
                    if (_value > MaxValue) _value = MaxValue;
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
            public LayoutNumberSelect(string label, float value, GameInput input) : base(Vector2.Zero,Vector2.Zero, Vector2.Zero)
            {
                this.input = input;
                Items.Add(new LayoutText(label,Color.White, 0.5f));
                Items.Add(new LayoutText(value.ToString(), Color.White, 0.5f));
                Vertical = false;
                GetSizeFromParent = true;
                StartValue = value;
            }
            public LayoutNumberSelect(string label, float value, float minValue, float maxValue, float step, float bigStep, GameInput input) : this(label, value, input)
            {
                MinValue = minValue;
                MaxValue = maxValue;
                Step = step;
                BigStep = bigStep;
            }
            //----------------------------------------------------------------------
            // run the number select
            //----------------------------------------------------------------------
            public string Run()
            {
                if (input.APressed)
                {
                    Value -= Step;
                }
                else if (input.DPressed)
                {
                    Value += Step;
                }
                else if(input.WPressed)
                {
                    Value += BigStep;
                }
                else if (input.SPressed)
                {
                    Value -= BigStep;
                }
                else if (input.QPressed)
                {
                    Value = previousValue;
                    return "reset";
                }
                else if (input.SpacePressed)
                {
                    previousValue = Value;
                    return "done";
                }
                return "";
            }
        }
    }
}
