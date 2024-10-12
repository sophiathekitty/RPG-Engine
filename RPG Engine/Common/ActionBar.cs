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
        // ScreenActionBar
        //----------------------------------------------------------------------
        public class ScreenActionBar : IScreenSpriteProvider
        {
            private string[] actions;
            List<ScreenSprite> sprites = new List<ScreenSprite>();
            //ScreenSprite back;
            bool vertical = false;
            public int Count { get { return actions.Length; } }
            public bool Visible { get { return sprites[0].Visible; } set { foreach (ScreenSprite sprite in sprites) sprite.Visible = value; } }
            float width;
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public ScreenActionBar(int actionCount, float width, Color color, string actionString = "", bool vertical = false)
            {
                actions = new string[actionCount];
                this.width = width;
                if (actionString != "")
                {
                    SetActions(actionString);
                }
                SetupSprites(color, ScreenSprite.DEFAULT_FONT_SIZE);
                this.vertical = vertical;
            }
            //----------------------------------------------------------------------
            // set actions from a string separated by spaces
            //----------------------------------------------------------------------
            public void SetActions(string actionString)
            {
                string[] _actions = actionString.Split(' ');
                for (int i = 0; i < actions.Length; i++)
                {
                    if (i < _actions.Length)
                    {
                        actions[i] = _actions[i];
                    }
                    else
                    {
                        actions[i] = "";
                    }
                    if (sprites.Count > i)
                    {
                        sprites[i].Data = actions[i];
                    }

                }
            }
            //----------------------------------------------------------------------
            // setup the sprites so they can be added to a screen later
            //----------------------------------------------------------------------
            public void SetupSprites(Color color, float scale = 2f, string font = "White")
            {
                if (vertical)
                {
                    SetupVerticalSprites(color, scale, font);
                    return;
                }
                sprites.Clear();
                //back = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.BottomLeft, new Vector2(0, -30 * scale), 0f, new Vector2(width, 30 * scale * 2), new Color(Color.Black, 0.1f), "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                float awidth = width / actions.Length;
                Vector2 position = new Vector2(awidth / 2, (-30 * scale) - 10);
                Vector2 actionWidth = new Vector2(awidth, 0);
                for (int i = 0; i < actions.Length; i++)
                {
                    ScreenSprite sprite = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.BottomLeft, position, scale, Vector2.Zero, color, font, actions[i], TextAlignment.CENTER, SpriteType.TEXT);
                    position += actionWidth;
                    sprites.Add(sprite);
                }
            }
            public void SetupVerticalSprites(Color color, float scale = 2f, string font = "White")
            {
                sprites.Clear();
                // so it's going to be right aligned and the anchor is going to be the center right
                // figure out the first position by taking the height of a line of text and multiply it by half the number of actions
                Vector2 position = new Vector2(-10, (-35 * scale) * ((actions.Length / 2) + 1));
                Vector2 actionHeight = new Vector2(0, 46 * scale);
                for (int i = 0; i < actions.Length; i++)
                {
                    ScreenSprite sprite = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.CenterRight, position, scale, Vector2.Zero, color, font, actions[i], TextAlignment.RIGHT, SpriteType.TEXT);
                    position += actionHeight;
                    sprites.Add(sprite);
                }
            }
            //----------------------------------------------------------------------
            // implement IScreenSpriteProvider
            // add the sprites to a screen
            //----------------------------------------------------------------------
            public void AddToScreen(Screen screen)
            {
                //screen.AddSprite(back);
                foreach (ScreenSprite sprite in sprites)
                {
                    screen.AddSprite(sprite);
                }
            }
            // remove the sprites from a screen
            public void RemoveToScreen(Screen screen)
            {
                //screen.RemoveSprite(back);
                foreach (ScreenSprite sprite in sprites)
                {
                    screen.RemoveSprite(sprite);
                }
            }
            //----------------------------------------------------------------------
            // handle action bar input takes an int and returns a string
            //----------------------------------------------------------------------
            public string HandleInput(int input)
            {
                if (input >= 0 && input < actions.Length)
                {
                    return actions[input].ToLower();
                }
                return "";
            }
            public string HandleInput(string input)
            {
                string[] cmd = input.Split(' ');
                int index = -1;
                for (int i = 0; i < cmd.Length; i++)
                {
                    if (int.TryParse(cmd[i], out index))
                    {
                        break;
                    }
                }
                index--;
                return HandleInput(index);
            }
        }
        //----------------------------------------------------------------------
    }
}
