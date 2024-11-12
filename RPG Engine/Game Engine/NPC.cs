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
        //---------------------------------------------------------------------------
        // NPC - display a non-player character (or object) on the map and hold info for interaction
        //---------------------------------------------------------------------------
        public class NPC : RasterSprite
        {
            CharacterSpriteLoader spriteLoader;
            Dictionary<char, List<string>> sprites;
            public Vector2 MapPosition;
            char direction = 'd'; // default direction is down
            int frame = 0;
            int animationDelay = 10;
            int animationStep = 0;
            int spriteID = 0;
            bool isEnabled = true;
            public string EnabledBool = ""; // name of bool to store if npc is enabled
            public bool randomWalk = false;
            public bool guardedSpace = false; // if true, player cannot walk through this npc (but it will do the NPCs action)
            public bool Enabled { get { return isEnabled; } set { Visible = isEnabled = value; } }
            public string InteractAction;
            public int X { get { return (int)MapPosition.X; } set { MapPosition.X = value; } }
            public int Y { get { return (int)MapPosition.Y; } set { MapPosition.Y = value; } }
            public int SpriteIndex { get { return spriteID; } set { spriteID = value; sprites = spriteLoader.LoadSpriteSet(spriteID); Data = sprites[direction][frame].Replace(IGNORE.ToString(), INVISIBLE); } }
            //---------------------------------------------------------------------------
            // constructor
            //---------------------------------------------------------------------------
            public NPC(Vector2 position, float scale, Dictionary<char, List<string>> sprites) : base(position, scale, Vector2.Zero, sprites['d'][0])
            {
                this.sprites = sprites;
            }
            public NPC(string data, CharacterSpriteLoader spriteSheet, GameData gd) : base(Vector2.Zero, 1, Vector2.Zero, "")
            {
                GridInfo.Echo("Loading NPC: " + data);
                string[] parts = data.Split(';');
                string[] pos = parts[1].Split(',');
                GridInfo.Echo("parts: " + parts.Length + " id: " + parts[0] + " pos: " + pos[0] + "," + pos[1] + " dir: " + pos[2]);
                spriteID = int.Parse(parts[0]);
                GridInfo.Echo("spriteID: " + spriteID);
                sprites = spriteSheet.LoadSpriteSet(spriteID);
                spriteLoader = spriteSheet;
                GridInfo.Echo("sprites: " + sprites.Count);
                MapPosition = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
                GridInfo.Echo("MapPosition: " + MapPosition);
                Direction = pos[2][0];
                GridInfo.Echo("Direction: " + Direction);
                randomWalk = bool.Parse(parts[2]);
                guardedSpace = bool.Parse(parts[3]);
                EnabledBool = parts[4];
                GridInfo.Echo("EnabledBool: " + EnabledBool);
                if(gd == null) GridInfo.Echo("GameData is null");
                if (EnabledBool != "" && gd.Bools.ContainsKey(EnabledBool))
                {
                    GridInfo.Echo("Setting Enabled?");
                    Enabled = gd.Bools[EnabledBool];
                }
                //else Enabled = true;
                GridInfo.Echo("Enabled: " + Enabled);
                InteractAction = parts[5];
            }
            //---------------------------------------------------------------------------
            // methods
            //---------------------------------------------------------------------------
            public char Direction
            {
                get { return direction; }
                set 
                { 
                    direction = value;
                    Data = sprites[direction][frame].Replace(IGNORE.ToString(), INVISIBLE);

                }
            }
            public int SpriteID 
            { 
                get 
                { 
                    return spriteID; 
                }
            }
            public void SetSpriteID(int id, CharacterSpriteLoader spriteSheet)
            {
                spriteID = id;
                sprites = spriteSheet.LoadSpriteSet(spriteID);
                frame = 0;
                Data = sprites[direction][frame].Replace(IGNORE.ToString(), INVISIBLE);
            }
            public override MySprite ToMySprite(RectangleF _viewport)
            {
                if(randomWalk && animationStep++ > animationDelay)
                {
                    animationStep = 0;
                    frame = (frame + 1) % sprites[direction].Count;
                    Data = sprites[direction][frame].Replace(IGNORE.ToString(), INVISIBLE);
                }
                return base.ToMySprite(_viewport);
            }
            public string ToSaveString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(spriteID).Append(';');
                sb.Append(MapPosition.X).Append(',').Append(MapPosition.Y).Append(',').Append(Direction).Append(';');
                sb.Append(randomWalk).Append(';');
                sb.Append(guardedSpace).Append(';');
                sb.Append(EnabledBool).Append(';');
                sb.Append(InteractAction);
                return sb.ToString();
            }
        }
        //---------------------------------------------------------------------------
    }
}
