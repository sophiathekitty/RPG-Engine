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
using VRage.Render.Scene;
using VRageMath;
using static IngameScript.Program;

namespace IngameScript
{
    partial class Program
    {
        //-----------------------------------------------------------------------
        // a game ui scene
        //-----------------------------------------------------------------------
        public class GameUIScene : ILayoutInteractable
        {
            //-----------------------------------------------------------------------
            // properties
            //-----------------------------------------------------------------------
            GameData _gameData;
            NPC _npc;
            Screen _screen;
            string MainAction;
            List<ScreenSprite> sprites = new List<ScreenSprite>();
            List<IScreenSpriteProvider> _sprites = new List<IScreenSpriteProvider>();
            Stack<ILayoutInteractable> _interactables = new Stack<ILayoutInteractable>();
            List<GameUIVarDisplay> _varDisplays = new List<GameUIVarDisplay>();
            public string ButtonPrompt { get; set; }

            public Color ValueColor { get; set; }
            public GameUIScene(GameData gameData, string mainAction, NPC npc, Screen screen)
            {
                _gameData = gameData;
                MainAction = mainAction;
                _npc = npc;
                _screen = screen;
            }
            //-----------------------------------------------------------------------
            // run the scene
            //-----------------------------------------------------------------------
            public string Run()
            {
                foreach (GameUIVarDisplay varDisplay in _varDisplays)
                {
                    varDisplay.Update();
                }
                if (_interactables.Count > 0)
                {
                    ILayoutInteractable interactable = _interactables.Peek();
                    string result = interactable.Run();
                    if (result == "done")
                    {
                        interactable.RemoveFromScreen(_screen);
                        _interactables.Pop();
                        return "";
                    }
                    return result;

                }
                else if (_gameData.Actions.ContainsKey(MainAction))
                {
                    _gameData.Actions[MainAction].Execute(_npc);
                }
                return "";
            }
            //-----------------------------------------------------------------------
            // ILayoutInteractable
            //-----------------------------------------------------------------------
            public void RemoveFromScreen(Screen screen)
            {
                foreach (IScreenSpriteProvider sprite in _sprites)
                {
                    sprite.RemoveFromScreen(screen);
                }
                foreach (ScreenSprite sprite in sprites)
                {
                    screen.RemoveSprite(sprite);
                }
            }
            //-----------------------------------------------------------------------
            // add an element
            //-----------------------------------------------------------------------
            public void Add(IScreenSpriteProvider sprite)
            {
                _sprites.Add(sprite);
                if (sprite is ILayoutInteractable)
                {
                    _interactables.Push((ILayoutInteractable)sprite);
                }
                else if (sprite is GameUIVarDisplay)
                {
                    _varDisplays.Add((GameUIVarDisplay)sprite);
                }
            }
            public void Add(ScreenSprite sprite)
            {
                sprites.Add(sprite);
            }
            //-----------------------------------------------------------------------
        }
        //-----------------------------------------------------------------------
    }
}
