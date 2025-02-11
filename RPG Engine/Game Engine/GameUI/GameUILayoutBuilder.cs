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
            public GameData _gameData;
            // a stack of ILayoutInteractable
            Stack<ILayoutInteractable> _interactables = new Stack<ILayoutInteractable>();
            Dictionary<string, RasterSprite> Sprites = new Dictionary<string, RasterSprite>();
            GameUIScene _scene;
            GameUIMenu _menu;
            int _layoutLayer = 4;
            Stack<LayoutArea> _areas = new Stack<LayoutArea>();
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
                _interactables.Clear();
                Sprites.Clear();
            }
            void IScreenSpriteProvider.RemoveFromScreen(Screen screen)
            {
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
                //GridInfo.Echo("Showing dialog: "+message);
                // calculate position based on size and screen size
                Vector2 position = new Vector2((_screen.Size.X - size.X) / 2, (_screen.Size.Y - size.Y - 10));
                foreach(string key in _gameData.Ints.Keys)
                {
                    //GridInfo.Echo("key: " + key + " value: " + _gameData.Ints[key]);
                    message = message.Replace("#" + key, _gameData.Ints[key].ToString());
                }
                foreach (string key in _gameData.Strings.Keys)
                {
                    //GridInfo.Echo("key: ${" + key + "}");
                    message = message.Replace("$" + key, _gameData.Strings[key]);
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
            //-----------------------------------------------------------------------
            // add a layout area
            //-----------------------------------------------------------------------
            public void AddArea(int x, int y, int width, int height, bool vertical = true, bool background = true)
            {
                LayoutArea area;
                if(background)
                {
                    area = new LayoutArea(new Vector2(x, y), new Vector2(width, height), new Vector2(5, 5), Color.Black, Color.White, 2);
                }
                else
                {
                    area = new LayoutArea(new Vector2(x, y), new Vector2(width, height), new Vector2(5, 5));
                }
                area.Vertical = vertical;
                if(_areas.Count > 0)
                {
                    _areas.Peek().Items.Add(area);
                }
                _areas.Push(area);
            }
            public void AddAreaHeader(string text)
            {
                LayoutText header = new LayoutText(text, Color.White, 0.5f);
                _areas.Peek().extras.Add(header);
            }
            public void AddText(string text, float fontSize, Color color)
            {
                LayoutText layoutText = new LayoutText(text, color, fontSize);
                _areas.Peek().Items.Add(layoutText);
            }
            public void AddVarDisplay(string label, GameActionVariable variable, float fontSize = 0.5f, bool vertical = false)
            {
                GameUIVarDisplay varDisplay = new GameUIVarDisplay(label, variable, fontSize, vertical);
                _areas.Peek().Items.Add(varDisplay);
                if(_scene != null) _scene.Add(varDisplay);
            }
            public void ShowArea()
            {
                LayoutArea area = _areas.Pop();
                if(_areas.Count == 0)
                {
                    area.ApplyLayout();
                    if (_scene != null)
                    {
                        _scene.Add(area);
                        _scene.layoutAreas.Push(area);
                    }
                    _screen.AddSprite(area, _layoutLayer);
                }
            }
            public void RemoveArea(int count = 1)
            {
                GridInfo.Echo("Removing " + count + " areas");
                for (int i = 0; i < count; i++)
                {
                    GridInfo.Echo("Removing area " + i);
                    if (_scene != null)
                    {
                        GridInfo.Echo("Removing from scene");
                        if(_scene.layoutAreas == null) GridInfo.Echo("LayoutAreas is null?");
                        if (_scene.layoutAreas.Count > 0)
                        {
                            LayoutArea area = _scene.layoutAreas.Pop();
                            if(area != null)
                            {
                                _screen.RemoveSprite(area);
                            } else GridInfo.Echo("Area is null");
                        }
                    }
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
