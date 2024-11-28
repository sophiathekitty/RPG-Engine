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
            int endValue;
            int startValue;
            int stepValue;
            GameActionVariable loopVar;
            NPC npc;
            public GameActionForLoop(string command, GameData gameData, GameUILayoutBuilder uiBuilder, GameAction action) : base("", gameData, uiBuilder,action)
            {
                //GridInfo.Echo("GameActionForLoop: " + command);
                string[] parts = command.Split(':');
                string[] vars = parts[1].Split(',');
                loopVar = new GameActionVariable(vars[0].Trim(), gameData, action);
                GameActionVariable _startValue = new GameActionVariable(vars[1].Trim(), gameData, action);
                GameActionVariable _endValue = new GameActionVariable(vars[2].Trim(), gameData, action);
                GameActionVariable _stepValue = new GameActionVariable(vars[3].Trim(), gameData, action);
                startValue = _startValue.As<int>();
                endValue = _endValue.As<int>();
                stepValue = _stepValue.As<int>();
            }
            public override void Execute()
            {
                GridInfo.Echo("GameActionForLoop.Execute "+ startValue + " to " + endValue + " step " + stepValue);
                // loop through the actions
                for (int i = startValue; i <= endValue; i += stepValue)
                {
                    loopVar.Set(i);
                    GridInfo.Echo("Looping: " + i + " - " + actions.Count + " actions");
                    foreach (GameActionCommand action in actions)
                    {
                        GridInfo.Echo("Executing action: " + action.cmd);
                        action.Execute();
                        GridInfo.Echo("Action executed");
                    }
                    GridInfo.Echo("End loop");
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
