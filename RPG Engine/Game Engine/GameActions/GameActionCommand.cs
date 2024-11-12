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

            public GameActionCommand(string command, GameData gameData, GameUILayoutBuilder uiBuilder)
            {
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
                            Destination = new GameActionVariable(d[0].Trim(), gameData, d[1].Trim());
                        }
                        else Destination = new GameActionVariable(props[0].Trim(), gameData);
                        foreach (string s in props[1].Split(','))
                        {
                            if (s.Contains("~"))
                            {
                                string[] d = s.Split('~');
                                Source.Add(new GameActionVariable(d[0].Trim(), gameData, d[1].Trim()));
                            }
                            else Source.Add(new GameActionVariable(s.Trim(), gameData));
                        }
                    }
                    else
                    {
                        Destination = new GameActionVariable(parts[1].Trim(), gameData);
                    }
                }
            }
            public virtual void Execute() 
            {
                GridInfo.Echo(cmd);
                // add command
                if (cmd.ToLower() == "add")
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
                if (cmd.ToLower() == "sub")
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
                if (cmd.ToLower() == "mul")
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
                if (cmd.ToLower() == "div")
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
                // set command
                if (cmd.ToLower() == "set")
                {
                    Destination.Value = Source[0].Value;
                }
                // say command
                if (cmd.ToLower() == "say")
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
            }
        }
        //-----------------------------------------------------------------------
    }
}
