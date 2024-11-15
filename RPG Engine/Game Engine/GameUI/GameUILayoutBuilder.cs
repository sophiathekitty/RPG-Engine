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
            GameUISpriteLoader spriteSheet;
            bool _visible = false;
            Screen _screen;
            GameInput _input;
            List<LayoutArea> _items = new List<LayoutArea>();
            public GameData _gameData;
            // a stack of ILayoutInteractable
            Stack<ILayoutInteractable> _interactables = new Stack<ILayoutInteractable>();
            Dictionary<string, RasterSprite> Sprites = new Dictionary<string, RasterSprite>();
            GameUIScene _scene;
            GameUIMenu _menu;
            int _layoutLayer = 4;
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
                /*
                foreach (ILayoutItem item in _items)
                {
                    item.AddToScreen(screen, layer);
                }
                */
                _interactables.Clear();
                _items.Clear();
                Sprites.Clear();
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
                if (_scene != null)
                {
                    _scene.RemoveFromScreen(screen);
                }
                if (_menu != null)
                {
                    _menu.RemoveFromScreen(screen);
                }
                foreach (RasterSprite sprite in Sprites.Values)
                {
                    screen.RemoveSprite(sprite);
                }
                // clear stack
                _interactables.Clear();
                _items.Clear();
            }
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public GameUILayoutBuilder(GameInput input, string game)
            {
                _input = input;
                spriteSheet = new GameUISpriteLoader(game,1);
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
                        if(_scene == interactable) _scene = null;
                        if (_menu == interactable) _menu = null;
                        interactable.RemoveFromScreen(_screen);
                        _interactables.Pop();
                    }
                    return result;

                }
                return "go";
            }
            //-----------------------------------------------------------------------
            //-----------------------------------------------------------------------
            // add layout elements
            //-----------------------------------------------------------------------
            //-----------------------------------------------------------------------
            // dialog window
            //-----------------------------------------------------------------------
            public void ShowDialog(string message, float fontSize, Vector2 size)
            {
                GridInfo.Echo("Showing dialog");
                // calculate position based on size and screen size
                Vector2 position = new Vector2((_screen.Size.X - size.X) / 2, (_screen.Size.Y - size.Y - 10));
                foreach(string key in _gameData.Ints.Keys)
                {
                    message = message.Replace("#{" + key + "}", _gameData.Ints[key].ToString());
                }
                foreach (string key in _gameData.Strings.Keys)
                {
                    message = message.Replace("${" + key + "}", _gameData.Strings[key]);
                }
                DialogWindow dialog = new DialogWindow(message, fontSize, position, size, _input);
                dialog.ApplyLayout();
                //_items.Add(dialog);
                _screen.AddSprite(dialog,_layoutLayer);
                if(_scene != null)
                {
                    _scene.Add(dialog);
                }
                else _interactables.Push(dialog);
            }
            //-----------------------------------------------------------------------
            // add a menu
            //-----------------------------------------------------------------------
            public void AddMenu(string header, int x, int y, int width, int height, NPC npc)
            {
                _menu = new GameUIMenu(header, new Vector2(x, y), new Vector2(width, height), new Vector2(5, 5), _input, Color.Black, Color.White, 2, _gameData, npc);
                
            }
            public void AddMenuItem(string displayText, string action)
            {
                _menu.Add(displayText, action);
            }
            public void ShowMenu()
            {
                _menu.ApplyLayout();
                _screen.AddSprite(_menu, _layoutLayer);
                if(_scene != null)
                {
                    _scene.Add(_menu);
                }
                else _interactables.Push(_menu);
                //_interactables.Push(_menu);
            }
            //-----------------------------------------------------------------------
            // add a sprite
            //-----------------------------------------------------------------------
            public void AddSprite(string name, int screenX, int screenY, int sheetIndex, int spriteX, int spriteY, int spriteWidth, int spriteHeight)

            {
                if(spriteSheet.Index != sheetIndex) spriteSheet.Index = sheetIndex;
                RasterSprite sprite = spriteSheet.GetSprite(screenX, screenY, 1, spriteX, spriteY, spriteWidth, spriteHeight);
                sprite.RotationOrScale = _gameData.playerSprite.RotationOrScale;
                Sprites.Add(name, sprite);
                if(_scene != null)
                {
                    _scene.Add(sprite);
                }
                _screen.AddSprite(sprite, _layoutLayer);
            }
            public void MoveSprite(string name, int screenX, int screenY)
            {
                Sprites[name].Position = new Vector2(screenX, screenY);
            }
            public void ReplaceSprite(string name, int sheetIndex, int spriteX, int spriteY, int spriteWidth, int spriteHeight)
            {
                if (spriteSheet.Index != sheetIndex) spriteSheet.Index = sheetIndex;
                Sprites[name].Data = spriteSheet.getPixels(spriteX, spriteY, spriteWidth, spriteHeight);
            }
            public void RemoveSprite(string name)
            {
                _screen.RemoveSprite(Sprites[name]);
                Sprites.Remove(name);
            }
            //-----------------------------------------------------------------------
            // scene management
            //-----------------------------------------------------------------------
            public void StartScene(string mainAction, NPC npc = null)
            {
                _scene = new GameUIScene(_gameData, mainAction, npc,_screen);
                _interactables.Push(_scene);
            }
            public void EndScene()
            {
                _scene.RemoveFromScreen(_screen);
                _scene = null;
                _interactables.Pop();
                Sprites.Clear();
            }
        }
        //-----------------------------------------------------------------------
    }
}
