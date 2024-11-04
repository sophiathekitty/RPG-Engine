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
        // the player's avatar
        //-----------------------------------------------------------------------
        public class PlayerSprite : RasterSprite
        {
            //-----------------------------------------------------------------------
            // fields
            //-----------------------------------------------------------------------
            Dictionary<char, List<string>> sprites;
            Vector2 mapPosition;
            char direction = 'd'; // default direction is down
            int frame = 0;
            int animationDelay = 10;
            int animationStep = 0;
            int animationLoop = 0; // when move animaate for a few frames (so if we're moving, we can animate the movement as it hops from tile to tile)
            public char Direction 
            { 
                get { return direction; } 
                set 
                { 
                    direction = value;
                    Data = sprites[direction][frame].Replace(IGNORE.ToString(), INVISIBLE);
                } 
            }
            public Vector2 MapPosition 
            { 
                get { return mapPosition; } 
                set 
                { 
                    mapPosition = value;
                    animationLoop = 3;
                } 
            }
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public PlayerSprite(Vector2 position, float scale, Dictionary<char, List<string>> sprites) : base(position, scale, Vector2.Zero, sprites['d'][0])
            {
                this.sprites = sprites;
            }
            //-----------------------------------------------------------------------
            // render the sprite
            //-----------------------------------------------------------------------
            public override MySprite ToMySprite(RectangleF _viewport)
            {
                if (animationLoop > 0)
                {
                    if (animationStep++ > animationDelay)
                    {
                        animationStep = 0;
                        frame = (frame + 1) % sprites[direction].Count;
                        Data = sprites[direction][frame].Replace(IGNORE.ToString(), INVISIBLE);
                        animationLoop--;
                    }
                }
                return base.ToMySprite(_viewport);
            }
        }
        //-----------------------------------------------------------------------
    }
}
