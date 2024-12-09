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
        // a game action if block
        //-----------------------------------------------------------------------
        public class GameActionIfBlock : GameActionCommand
        {
            GameActionVariable VarA;
            GameActionVariable VarB;
            string Operator;
            public List<GameActionCommand> actions = new List<GameActionCommand>();
            public List<GameActionCommand> elseActions = new List<GameActionCommand>();
            public bool AddingElseActions = false;
            NPC npc;

            // if:Numbers.Var1>Numbers.Var2; (we're just getting the if condition here... then the parser that created this will add the actions)
            public GameActionIfBlock(string command, GameData gameData, GameUILayoutBuilder uiBuilder, GameAction action) : base("",gameData,uiBuilder,action)
            {
                //GridInfo.Echo("GameActionIfBlock: " + command);
                if (command == "") return;
                string[] parts = command.Split(':');
                if (parts[1].Contains(">=")) Operator = ">=";
                else if (parts[1].Contains("<=")) Operator = "<=";
                else if (parts[1].Contains("==")) Operator = "==";
                else if (parts[1].Contains("!=")) Operator = "!=";
                else if (parts[1].Contains(">")) Operator = ">";
                else if (parts[1].Contains("<")) Operator = "<";
                else throw new Exception("Invalid operator in if block: " + parts[1]);
                string[] vars = parts[1].Split(Operator.ToCharArray());
                //GridInfo.Echo("vars length: "+ vars.Length);
                //GridInfo.Echo("Vars: " + vars[0] + " " + Operator + " " + vars[vars.Length-1]);
                VarA = new GameActionVariable(vars[0].Trim(), gameData,action);
                VarB = new GameActionVariable(vars[vars.Length-1].Trim(), gameData,action);
            }
            public override bool Execute()
            {
                //GridInfo.Echo("GameActionIfBlock.Execute");
                // compare the variables
                string a = VarA.Value;
                string b = VarB.Value;
                bool result = false;
                //GridInfo.Echo("Comparing: " + a + " " + Operator + " " + b);
                switch (Operator)
                {
                    case ">=":
                        result = Convert.ToDouble(a) >= Convert.ToDouble(b);
                        break;
                    case "<=":
                        result = Convert.ToDouble(a) <= Convert.ToDouble(b);
                        break;
                    case "==":
                        result = a == b;
                        break;
                    case "!=":
                        result = a != b;
                        break;
                    case ">":
                        result = Convert.ToDouble(a) > Convert.ToDouble(b);
                        break;
                    case "<":
                        result = Convert.ToDouble(a) < Convert.ToDouble(b);
                        break;
                }
                //GridInfo.Echo("Result: " + result);
                // execute the actions
                if (result)
                {
                    foreach (GameActionCommand action in actions)
                    {
                        if (!action.Execute()) return false;
                    }
                }
                else
                {
                    foreach (GameActionCommand action in elseActions)
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
