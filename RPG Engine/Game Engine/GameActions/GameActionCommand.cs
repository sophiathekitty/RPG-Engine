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
            string cmd;
            GameActionVariable Destination;
            List<GameActionVariable> Source = new List<GameActionVariable>();
            GameUILayoutBuilder UIbuilder;
            NPC npc;

            public GameActionCommand(string command, GameData gameData, GameUILayoutBuilder uiBuilder, GameAction action)
            {
                this.npc = action.npc;
                UIbuilder = uiBuilder;
                string[] parts = command.Split(':');
                cmd = parts[0].Trim();
                if (parts.Length > 1)
                {
                    if (parts[1].Contains("="))
                    {
                        string[] props = parts[1].Split('=');
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
            }
            public virtual void Execute() 
            {
                GridInfo.Echo("cmd:"+cmd);
                // set command
                if (cmd.ToLower() == "set")
                {
                    Destination.Value = Source[0].Value;
                }
                // add command
                else if (cmd.ToLower() == "add")
                {
                    if(Source.Count > 1)
                    {
                        int total = 0;
                        foreach(GameActionVariable v in Source)
                        {
                            total += Convert.ToInt32(v.Value);
                        }
                        Destination.Value = total.ToString();
                    }
                    else if (Source.Count == 1)
                    {
                        Destination.Value = (Convert.ToInt32(Destination.Value) + Convert.ToInt32(Source[0].Value)).ToString();
                    }
                    else throw new Exception("Invalid add command");
                }
                // subtract command
                else if (cmd.ToLower() == "sub")
                {
                    if (Source.Count > 1)
                    {
                        int total = Convert.ToInt32(Source[0].Value);
                        for (int i = 1; i < Source.Count; i++)
                        {
                            total -= Convert.ToInt32(Source[i].Value);
                        }
                        Destination.Value = total.ToString();
                    }
                    else if (Source.Count == 1)
                    {
                        Destination.Value = (Convert.ToInt32(Destination.Value) - Convert.ToInt32(Source[0].Value)).ToString();
                    }
                    else throw new Exception("Invalid sub command");
                }
                // multiply command
                else if (cmd.ToLower() == "mul")
                {
                    if (Source.Count > 1)
                    {
                        int total = Convert.ToInt32(Source[0].Value);
                        for (int i = 1; i < Source.Count; i++)
                        {
                            total *= Convert.ToInt32(Source[i].Value);
                        }
                        Destination.Value = total.ToString();
                    }
                    else if (Source.Count == 1)
                    {
                        Destination.Value = (Convert.ToInt32(Destination.Value) * Convert.ToInt32(Source[0].Value)).ToString();
                    }
                    else throw new Exception("Invalid mul command");
                }
                // divide command
                else if (cmd.ToLower() == "div")
                {
                    if (Source.Count > 1)
                    {
                        int total = Convert.ToInt32(Source[0].Value);
                        for (int i = 1; i < Source.Count; i++)
                        {
                            total /= Convert.ToInt32(Source[i].Value);
                        }
                        Destination.Value = total.ToString();
                    }
                    else if (Source.Count == 1)
                    {
                        Destination.Value = (Convert.ToInt32(Destination.Value) / Convert.ToInt32(Source[0].Value)).ToString();
                    }
                    else throw new Exception("Invalid div command");
                }
                // say command
                else if (cmd.ToLower() == "say")
                {
                    float fontSize = 0.5f;
                    Vector2 size = new Vector2(200, 100);
                    if (Source.Count > 0)
                    {
                        // get dialog settings
                        fontSize = Convert.ToSingle(Source[0].Value) / 100f;
                        if (Source.Count > 3)
                        {
                            size.X = Convert.ToInt32(Source[1].Value);
                            size.Y = Convert.ToInt32(Source[2].Value);
                        }
                    }
                    if(UIbuilder == null) GridInfo.Echo("UIbuilder is null");
                    UIbuilder.ShowDialog(Destination.Value, fontSize,size);
                }
                // start a UI Scene
                else if (cmd.ToLower() == "startscene")
                {
                    UIbuilder.StartScene(Destination.Value, npc);
                }
                // end a UI Scene
                else if (cmd.ToLower() == "endscene")
                {
                    UIbuilder.EndScene();
                }
                // add a menu
                else if (cmd.ToLower() == "addmenu")
                {
                    if (Source.Count > 3)
                    {
                        UIbuilder.AddMenu(Destination.Value,Convert.ToInt32(Source[0].Value), Convert.ToInt32(Source[1].Value), Convert.ToInt32(Source[2].Value), Convert.ToInt32(Source[3].Value), npc);
                    }
                    else throw new Exception("Invalid addmenu command");
                }
                // add a menu item
                else if (cmd.ToLower() == "addmenuitem")
                {
                    if (Source.Count > 0)
                    {
                        string label = "";
                        foreach (GameActionVariable v in Source)
                        {
                            label += v.Value;
                        }
                        UIbuilder.AddMenuItem(label, Destination.Value);
                    }
                    else throw new Exception("Invalid addMenuItem command");
                }
                // finalize the menu
                else if (cmd.ToLower() == "showmenu")
                {
                    UIbuilder.ShowMenu();
                }
                // add sprite
                else if (cmd.ToLower() == "addsprite")
                {
                    if (Source.Count > 6)
                    {
                        UIbuilder.AddSprite(Destination.Value, Convert.ToInt32(Source[0].Value), Convert.ToInt32(Source[1].Value), Convert.ToInt32(Source[2].Value), Convert.ToInt32(Source[3].Value), Convert.ToInt32(Source[4].Value), Convert.ToInt32(Source[5].Value), Convert.ToInt32(Source[6].Value));
                    }
                    else throw new Exception("Invalid addsprite command");
                }
                // move sprite
                else if (cmd.ToLower() == "movesprite")
                {
                    if (Source.Count > 1)
                    {
                        UIbuilder.MoveSprite(Destination.Value, Convert.ToInt32(Source[0].Value), Convert.ToInt32(Source[1].Value));
                    }
                    else throw new Exception("Invalid movesprite command");
                }
                // replace sprite
                else if (cmd.ToLower() == "replacesprite")
                {
                    if (Source.Count > 4)
                    {
                        UIbuilder.ReplaceSprite(Destination.Value, Convert.ToInt32(Source[0].Value), Convert.ToInt32(Source[1].Value), Convert.ToInt32(Source[2].Value), Convert.ToInt32(Source[3].Value), Convert.ToInt32(Source[4].Value));
                    }
                    else throw new Exception("Invalid replacesprite command");
                }
                // remove sprite
                else if (cmd.ToLower() == "removesprite")
                {
                    UIbuilder.RemoveSprite(Destination.Value);
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
