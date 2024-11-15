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
        // for:Ints.i,start,end,step;
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
                GridInfo.Echo("GameActionForLoop: " + command);
                string[] parts = command.Split(':');
                string[] vars = parts[1].Split(',');
                loopVar = new GameActionVariable(vars[0].Trim(), gameData, action);
                startValue = int.Parse(vars[1].Trim());
                endValue = int.Parse(vars[2].Trim());
                stepValue = int.Parse(vars[3].Trim());
            }
            public override void Execute()
            {
                GridInfo.Echo("GameActionForLoop.Execute");
                // loop through the actions
                for (int i = startValue; i <= endValue; i += stepValue)
                {
                    loopVar.Value = i.ToString();
                    foreach (GameActionCommand action in actions)
                    {
                        action.Execute();
                    }
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
