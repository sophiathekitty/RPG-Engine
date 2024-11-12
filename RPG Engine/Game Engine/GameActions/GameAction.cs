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
            List<GameActionCommand> commands = new List<GameActionCommand>();
            public GameAction(string data, GameData gameData, GameUILayoutBuilder uiBuilder)
            {
                Stack<GameActionIfBlock> ifBlocks = new Stack<GameActionIfBlock>();
                string[] cmds = data.Split(';');
                //bool elseBlock = false;
                foreach (string command in cmds)
                {
                    string cmd = command.Trim();
                    if (cmd.StartsWith("if:"))
                    {
                        GridInfo.Echo("Creating if block");
                        GameActionIfBlock ifBlock = new GameActionIfBlock(cmd, gameData, uiBuilder);
                        if (ifBlocks.Count > 0)
                        {
                            GridInfo.Echo("Adding if block to parent");
                            if (ifBlocks.Peek().AddingElseActions)  ifBlocks.Peek().elseActions.Add(ifBlock);
                            else ifBlocks.Peek().actions.Add(ifBlock);
                        }
                        else
                        {
                            GridInfo.Echo("1-Adding if block to commands");
                            commands.Add(ifBlock);
                        }
                        ifBlocks.Push(ifBlock);
                    }
                    else if(cmd.StartsWith("else"))
                    {
                        GridInfo.Echo("3-Switching to Esle commands");
                        ifBlocks.Peek().AddingElseActions = true;
                    }
                    else if(cmd.StartsWith("endif"))
                    {
                        GridInfo.Echo("4-Ending if block");
                        ifBlocks.Pop();
                        //elseBlock = false;
                    }
                    else if (cmd.StartsWith("for:")) // for loop start
                    {
                        GridInfo.Echo("Creating for block");
                        GameActionForLoop forBlock = new GameActionForLoop(cmd, gameData, uiBuilder);
                        if (ifBlocks.Count > 0)
                        {
                            GridInfo.Echo("Adding for block to parent");
                            if (ifBlocks.Peek().AddingElseActions) ifBlocks.Peek().elseActions.Add(forBlock);
                            else ifBlocks.Peek().actions.Add(forBlock);
                        }
                        else
                        {
                            GridInfo.Echo("1-Adding for block to commands");
                            commands.Add(forBlock);
                        }
                    }
                    else if (cmd.StartsWith("endfor;"))
                    {
                        ifBlocks.Pop();
                    }
                    else if (ifBlocks.Count > 0)
                    {
                        GridInfo.Echo("2-Adding action to if block");
                        if (ifBlocks.Peek().AddingElseActions) ifBlocks.Peek().elseActions.Add(new GameActionCommand(cmd, gameData, uiBuilder));
                        else ifBlocks.Peek().actions.Add(new GameActionCommand(cmd, gameData, uiBuilder));
                    } 
                    else commands.Add(new GameActionCommand(cmd, gameData, uiBuilder));
                }
            }
            public void Execute()
            {
                GridInfo.Echo("Executing action");
                foreach (GameActionCommand cmd in commands)
                {
                    cmd.Execute();
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
