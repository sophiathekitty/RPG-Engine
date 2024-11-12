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
        // a game ui layout builder for game actions to build UIs
        //-----------------------------------------------------------------------
        public class GameUILayoutBuilder : IScreenSpriteProvider
        {
            bool _visible = false;
            Screen _screen;
            GameInput _input;
            List<LayoutArea> _items = new List<LayoutArea>();
            // a stack of ILayoutInteractable
            Stack<ILayoutInteractable> _interactables = new Stack<ILayoutInteractable>();
            //-----------------------------------------------------------------------
            // IScreenSpriteProvider
            //-----------------------------------------------------------------------
            public bool Visible
            {
                get
                {
                    return _visible;
                }

                set
                {
                    _visible = value;
                }
            }
            void IScreenSpriteProvider.AddToScreen(Screen screen, int layer)
            {
                _screen = screen;
                foreach (ILayoutItem item in _items)
                {
                    item.AddToScreen(screen, layer);
                }
            }
            void IScreenSpriteProvider.RemoveFromScreen(Screen screen)
            {
                foreach (ILayoutItem item in _items)
                {
                    item.RemoveFromScreen(screen);
                }
                while (_interactables.Count > 0)
                {
                    _interactables.Pop().RemoveFromScreen(screen);
                }
                // clear stack
                _interactables.Clear();
                _items.Clear();
            }
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public GameUILayoutBuilder(GameInput input)
            {
                _input = input;
            }
            public string Run()
            {
                // run _interactables
                if(_interactables.Count > 0)
                {
                    ILayoutInteractable interactable = _interactables.Peek();
                    string result = interactable.Run();
                    if(result == "done")
                    {
                        interactable.RemoveFromScreen(_screen);
                        _interactables.Pop();
                    }
                    return result;

                }
                return "go";
            }
            //-----------------------------------------------------------------------
            // add layout elements
            //-----------------------------------------------------------------------
            public void ShowDialog(string message, float fontSize, Vector2 size)
            {
                //GridInfo.Echo("Showing dialog");
                // calculate position based on size and screen size
                Vector2 position = new Vector2((_screen.Size.X - size.X) / 2, (_screen.Size.Y - size.Y - 10));
                DialogWindow dialog = new DialogWindow(message, fontSize, position, size, _input);
                dialog.ApplyLayout();
                //_items.Add(dialog);
                _screen.AddSprite(dialog,4);
                _interactables.Push(dialog);
            }
        }
        //-----------------------------------------------------------------------
    }
}
