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
        // a game action command
        //-----------------------------------------------------------------------
        public class GameActionCommand
        {
            public string cmd { get; private set; }
            GameActionVariable Destination;
            List<GameActionVariable> Source = new List<GameActionVariable>();
            GameUILayoutBuilder UIbuilder;
            NPC npc;
            GameData gamedata;

            public GameActionCommand(string command, GameData gameData, GameUILayoutBuilder uiBuilder, GameAction action)
            {
                this.gamedata = gameData;
                this.npc = action.npc;
                UIbuilder = uiBuilder;
                // split the command on ':' limit to 2 parts
                string[] parts = command.Split(new[] { ':' }, 2);
                cmd = parts[0].Trim().ToLower();
                if (parts.Length > 1)
                {
                    if (parts[1].Contains("="))
                    {
                        string[] props = parts[1].Split(new[] { '=' }, 2);
                        if (props[0].Contains("~"))
                        {
                            string[] d = props[0].Split('~');
                            Destination = new GameActionVariable(d[0].Trim(), gameData, action, d[1].Trim());
                        }
                        else Destination = new GameActionVariable(props[0].Trim(), gameData, action);
                        foreach (string s in props[1].Split(','))
                        {
                            if (s.Contains("~"))
                            {
                                string[] d = s.Split('~');
                                Source.Add(new GameActionVariable(d[0].Trim(), gameData, action, d[1].Trim()));
                            }
                            else Source.Add(new GameActionVariable(s.Trim(), gameData, action));
                        }
                    }
                    else
                    {
                        Destination = new GameActionVariable(parts[1].Trim(), gameData, action);
                    }
                }
                if (cmd.Contains('@')) throw new Exception("Invalid command: " + command);
            }
            public virtual bool Execute()
            {
                try
                {
                    if (string.IsNullOrEmpty(cmd)) return true;
                    //GridInfo.Echo("cmd:"+cmd);
                    if (cmd == "end") return false;
                    if (cmd == "echo")
                    {
                        string echo = Destination.Value;
                        foreach (GameActionVariable v in Source)
                        {
                            echo += " " + v.Value;
                        }
                        echo = gamedata.ParseString(echo);
                        GridInfo.Echo(echo);
                    }
                    // run another action
                    else if (cmd == "run")
                    {
                        GridInfo.Echo("Running: " + Destination.Value);
                        if(gamedata.map.Actions.ContainsKey(Destination.Value)) gamedata.map.Actions[Destination.Value].Execute();
                        else if (gamedata.Actions.ContainsKey(Destination.Value)) gamedata.Actions[Destination.Value].Execute();
                        else throw new Exception("Invalid run command: "+Destination.Value);
                    }
                    // set command
                    else if (cmd == "set")
                    {
                        Destination.Value = Source[0].Value;
                    }
                    // str command
                    else if (cmd == "str")
                    {
                        if (Source.Count > 1)
                        {
                            string str = "";
                            foreach (GameActionVariable v in Source)
                            {
                                str += v.Value;
                            }
                            //GridInfo.Echo("Adding: " + total);
                            Destination.Value = str;
                        }
                        else if (Source.Count == 1)
                        {
                            //GridInfo.Echo("Adding: " + Destination.As<int>() + " + " + Source[0].As<int>());
                            Destination.Set<int>(Destination.As<int>() + Source[0].As<int>());
                        }
                        else throw new Exception("Invalid add command");
                    }
                    // add command
                    else if (cmd == "add")
                    {
                        if (Source.Count > 1)
                        {
                            int total = 0;
                            foreach (GameActionVariable v in Source)
                            {
                                total += v.As<int>();
                            }
                            //GridInfo.Echo("Adding: " + total);
                            Destination.Set<int>(total);
                        }
                        else if (Source.Count == 1)
                        {
                            //GridInfo.Echo("Adding: " + Destination.As<int>() + " + " + Source[0].As<int>());
                            Destination.Set<int>(Destination.As<int>() + Source[0].As<int>());
                        }
                        else throw new Exception("Invalid add command");
                    }
                    // subtract command
                    else if (cmd == "sub")
                    {
                        if (Source.Count > 1)
                        {
                            int total = Source[0].As<int>();
                            for (int i = 1; i < Source.Count; i++)
                            {
                                total -= Source[i].As<int>();
                            }
                            Destination.Set<int>(total);
                        }
                        else if (Source.Count == 1)
                        {
                            Destination.Set<int>(Destination.As<int>() - Source[0].As<int>());
                        }
                        else throw new Exception("Invalid sub command");
                    }
                    // multiply command
                    else if (cmd == "mul")
                    {
                        if (Source.Count > 1)
                        {
                            int total = Source[0].As<int>();
                            for (int i = 1; i < Source.Count; i++)
                            {
                                total *= Source[i].As<int>();
                            }
                            Destination.Set<int>(total);
                        }
                        else if (Source.Count == 1)
                        {
                            Destination.Set<int>(Destination.As<int>() * Source[0].As<int>());
                        }
                        else throw new Exception("Invalid mul command");
                    }
                    // divide command
                    else if (cmd == "div")
                    {
                        if (Source.Count > 1)
                        {
                            int total = Source[0].As<int>();
                            for (int i = 1; i < Source.Count; i++)
                            {
                                total /= Source[i].As<int>();
                            }
                            Destination.Set<int>(total);
                        }
                        else if (Source.Count == 1)
                        {
                            Destination.Set<int>(Destination.As<int>() / Source[0].As<int>());
                        }
                        else throw new Exception("Invalid div command");
                    }
                    // say command
                    else if (cmd == "say")
                    {
                        float fontSize = 0.5f;
                        Vector2 size = new Vector2(200, 100);
                        if (Source.Count > 0)
                        {
                            // get dialog settings
                            fontSize = Source[0].As<float>() / 100f;
                            if (Source.Count > 3)
                            {
                                size.X = Source[1].As<int>();
                                size.Y = Source[2].As<int>();
                            }
                        }
                        if (UIbuilder == null) GridInfo.Echo("UIbuilder is null");
                        UIbuilder.ShowDialog(Destination.Value, fontSize, size);
                    }
                    // start a UI Scene
                    else if (cmd == "startscene")
                    {
                        UIbuilder.StartScene(Destination.Value, npc);
                    }
                    // end a UI Scene
                    else if (cmd == "endscene")
                    {
                        UIbuilder.EndScene();
                    }
                    // add a menu
                    else if (cmd == "addmenu")
                    {
                        if (Source.Count > 3)
                        {
                            UIbuilder.AddMenu(Destination.Value, Source[0].As<int>(), Source[1].As<int>(), Source[2].As<int>(), Source[3].As<int>(), npc);
                        }
                        else throw new Exception("Invalid addmenu command");
                    }
                    // add a menu item
                    else if (cmd == "addmenuitem")
                    {
                        if (Source.Count > 0)
                        {
                            string label = "";
                            bool first = true;
                            foreach (GameActionVariable v in Source)
                            {
                                if (first) first = false;
                                else label += " ";
                                label += v.Value;
                            }
                            UIbuilder.AddMenuItem(label, Destination.Value);
                        }
                        else throw new Exception("Invalid addMenuItem command");
                    }
                    // finalize the menu
                    else if (cmd == "showmenu")
                    {
                        UIbuilder.ShowMenu();
                    }
                    // add sprite
                    else if (cmd == "addsprite")
                    {
                        if (Source.Count > 6)
                        {
                            UIbuilder.AddSprite(Destination.Value, Source[0].As<int>(), Source[1].As<int>(), Source[2].As<int>(), Source[3].As<int>(), Source[4].As<int>(), Source[5].As<int>(), Source[6].As<int>());
                        }
                        else throw new Exception("Invalid addsprite command");
                    }
                    // move sprite
                    else if (cmd == "movesprite")
                    {
                        if (Source.Count > 1)
                        {
                            UIbuilder.MoveSprite(Destination.Value, Source[0].As<int>(), Source[1].As<int>());
                        }
                        else throw new Exception("Invalid movesprite command");
                    }
                    // replace sprite
                    else if (cmd == "replacesprite")
                    {
                        if (Source.Count > 4)
                        {
                            UIbuilder.ReplaceSprite(Destination.Value, Source[0].As<int>(), Source[1].As<int>(), Source[2].As<int>(), Source[3].As<int>(), Source[4].As<int>());
                        }
                        else throw new Exception("Invalid replacesprite command");
                    }
                    // remove sprite
                    else if (cmd == "removesprite")
                    {
                        UIbuilder.RemoveSprite(Destination.Value);
                    }
                    // add area
                    else if (cmd == "addarea")
                    {
                        if (Source.Count > 5) UIbuilder.AddArea(Source[0].As<int>(), Source[1].As<int>(), Source[2].As<int>(), Source[3].As<int>(), Source[4].As<bool>(), Source[5].As<bool>());
                        else if (Source.Count > 4) UIbuilder.AddArea(Source[0].As<int>(), Source[1].As<int>(), Source[2].As<int>(), Source[3].As<int>(), Source[4].As<bool>());
                        else if (Source.Count > 3) UIbuilder.AddArea(Source[0].As<int>(), Source[1].As<int>(), Source[2].As<int>(), Source[3].As<int>());
                        else throw new Exception("Invalid addarea command");
                        if (!string.IsNullOrEmpty(Destination.Value))
                        {
                            UIbuilder.AddAreaHeader(Destination.Value);
                        }
                    }
                    // add area text
                    else if (cmd == "addareatext")
                    {
                        if (Source.Count > 0) UIbuilder.AddText(Destination.Value, Source[0].As<float>(), Color.White);
                        else UIbuilder.AddText(Destination.Value, 0.5f, Color.White);
                    }
                    // add area variable
                    else if (cmd == "addareavar")
                    {
                        if (Source.Count > 0) UIbuilder.AddVarDisplay(Destination.Value, Source[0]);
                        else throw new Exception("Invalid addAreaVar command");
                    }
                    // show area
                    else if (cmd == "showarea" || cmd == "endarea")
                    {
                        UIbuilder.ShowArea();
                    }
                    // remove area
                    else if (cmd == "removearea")
                    {
                        if (Destination == null || string.IsNullOrEmpty(Destination.Value)) UIbuilder.RemoveArea();
                        else UIbuilder.RemoveArea(Destination.As<int>());
                    }
                    // clear party
                    else if (cmd == "clearparty")
                    {
                        gamedata.CharacterList.Clear();
                    }
                    // add party member
                    else if (cmd == "addparty")
                    {
                        if (Source.Count > 0)
                        {
                            if (gamedata.Jobs.ContainsKey(Source[0].Value)) gamedata.CharacterList.Add(new PlayerCharacter(Destination.Value, gamedata.Jobs[Source[0].Value]));
                            else throw new Exception("Invalid job: " + Source[0].Value);
                        }
                        else throw new Exception("Invalid addparty command");
                    }
                    // remove last party member
                    else if (cmd == "removeparty")
                    {
                        if (string.IsNullOrEmpty(Destination.Value))
                        {
                            if (gamedata.CharacterList.Count > 0) gamedata.CharacterList.RemoveAt(gamedata.CharacterList.Count - 1);
                        }
                        else
                        {
                            gamedata.CharacterList.RemoveAt(Destination.As<int>());
                        }
                    }
                    // clear enemies
                    else if (cmd == "clearenemies")
                    {
                        gamedata.EnemyList.Clear();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("CMD:"+cmd+ ": " + ex.Message);
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
