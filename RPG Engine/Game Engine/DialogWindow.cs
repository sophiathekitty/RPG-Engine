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
        // a multi line (and scrolling) dialog window
        //-----------------------------------------------------------------------
        public class DialogWindow : LayoutArea, ILayoutInteractable
        {
            string message;
            string word;
            GameInput input;
            int lineMaxChars;
            int currentLine = 0;
            int scroll = 0;
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public DialogWindow(string message, float fontSize, Vector2 position, Vector2 size, GameInput input) : base(position, size, new Vector2(5, 5), Color.Black, Color.White, 1)
            {
                this.message = message;
                this.input = input;
                Vector2 charSize = LayoutText.MonospaceCharSize;
                charSize.X += LayoutText.MonospaceCharSpace;
                charSize *= fontSize;
                lineMaxChars = (int)(size.X / charSize.X);
                int lines = (int)(size.Y / charSize.Y) - 1;
                for (int i = 0; i < lines; i++)
                {
                    Items.Add(new LayoutText("", Color.White, fontSize));
                }
            }
            //-----------------------------------------------------------------------
            // ILayoutInteractable
            //-----------------------------------------------------------------------
            public string ButtonPrompt { get; set; } = "";

            public Color ValueColor
            {
                get
                {
                    foreach(ILayoutItem item in Items)
                    {
                        return item.Color;
                    }
                    return Color.White;
                }

                set
                {
                    foreach (ILayoutItem item in Items)
                    {
                        item.Color = value;
                    }
                }
            }
            public string Run()
            {
                // get the next word? or done?
                //GridInfo.Echo("Running dialog");
                if (string.IsNullOrEmpty(word))
                {
                    //GridInfo.Echo("Getting next word");
                    if (string.IsNullOrEmpty(message)) // message is done rendering now waiting to close
                    {
                        //GridInfo.Echo("Message is done rendering");
                        if (input.SpacePressed) return "done"; // press space to close
                    }
                    else
                    {
                        // get the next word
                        string[] words = message.Split(' ');
                        //GridInfo.Echo("Words: " + words.Length);
                        word = words[0];
                        //GridInfo.Echo("Word:" + word);
                        //GridInfo.Echo("Current line: " + currentLine);
                        if (!string.IsNullOrEmpty(Items[currentLine].Text)) word = " " + word;
                        //GridInfo.Echo("Word:" + word);
                        if(word.Length < message.Length) message = message.Substring(word.Length).Trim();
                        else message = "";
                        if (Items[currentLine].Text.Length + word.Length > lineMaxChars)
                        {
                            //GridInfo.Echo("Word too long... line wrap");
                            currentLine++;
                            if (currentLine >= Items.Count) currentLine = Items.Count; // be one line too far for now...
                        }
                    }
                }
                else if (currentLine < Items.Count) // render one letter at a time
                {
                    //GridInfo.Echo("Rendering word");
                    if (word[0] == '\n') // do a new line if needed
                    {
                        //GridInfo.Echo("New line");
                        currentLine++;
                        if (currentLine >= Items.Count) currentLine = Items.Count; // be one line too far for now...
                        word = word.Substring(1);
                        //GridInfo.Echo("Word:" + word);
                    }
                    else
                    {
                        //GridInfo.Echo("Current line: " + currentLine + "(" + Items.Count + ")");
                        Items[currentLine].Text += word[0];
                        word = word.Substring(1);
                    }
                }
                else if(scroll > 0) // if we can scroll
                {
                    for(int i = 0; i < Items.Count - 1; i++)
                    {
                        Items[i].Text = Items[i + 1].Text;
                    }
                    currentLine = Items.Count - 1;
                    Items[currentLine].Text = "";
                    scroll--;
                }
                else
                {
                    // waiting to page
                    if (input.SpacePressed)
                    {
                        scroll = Items.Count - 1;
                    }
                }
                return "";
            }
        }
        //-----------------------------------------------------------------------
    }
}
