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
        // a game action for loop
        // for:@Ints.i,start,end,step;
        // action1;action2;...;actionN;
        // endfor;
        //-----------------------------------------------------------------------
        public class GameActionForLoop : GameActionIfBlock
        {
            GameActionVariable endValue;
            GameActionVariable startValue;
            GameActionVariable stepValue;
            GameActionVariable loopVar;
            NPC npc;
            public GameActionForLoop(string command, GameData gameData, GameUILayoutBuilder uiBuilder, GameAction action) : base("", gameData, uiBuilder,action)
            {
                //GridInfo.Echo("GameActionForLoop: " + command);
                string[] parts = command.Split(':');
                string[] vars = parts[1].Split(',');
                loopVar = new GameActionVariable(vars[0].Trim(), gameData, action);
                startValue = new GameActionVariable(vars[1].Trim(), gameData, action);
                endValue = new GameActionVariable(vars[2].Trim(), gameData, action);
                stepValue = new GameActionVariable(vars[3].Trim(), gameData, action);
            }
            public override bool Execute()
            {
                // loop through the actions
                int _startValue = startValue.As<int>();
                int _endValue = endValue.As<int>();
                int _stepValue = stepValue.As<int>();
                for (int i = _startValue; i < _endValue; i += _stepValue)
                {
                    loopVar.Set(i);
                    foreach (GameActionCommand action in actions)
                    {
                        if (!action.Execute()) return false;
                    }
                }
                return true;
            }
        }
        //-----------------------------------------------------------------------
    }
}
