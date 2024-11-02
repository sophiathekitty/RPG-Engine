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
        // NPCOptions - options for an NPC
        //---------------------------------------------------------------------------
        public class NPCOptions : LayoutMenu
        {
            CharacterSpriteLoader spriteSheet;
            NPC npc;
            CharacterSpriteSelector spriteSelector;
            public NPCOptions(GameInput input, ref CharacterSpriteLoader spriteSheet) : base(new Vector2(100, 100), new Vector2(200, 200), new Vector2(5, 5), input, Color.Black, Color.White, 1f)
            {
                this.spriteSheet = spriteSheet;
                // title
                LayoutText title = new LayoutText("NPC Options", Color.White, 0.5f);
                title.Alignment = TextAlignment.CENTER;
                title.Padding = new Vector2(0, -15);
                extras.Add(title);
                // sprite selector
                spriteSelector = new CharacterSpriteSelector(Position, 0.1f, Vector2.Zero, ref spriteSheet, ref input);
                Items.Add(spriteSelector);
                // random walk (layout toggle)
                Items.Add(new LayoutToggle("Random Walk",true, ref input));
                //---------------------------------------------------------------------------
                // TODO - stuff like EnabledBool and GameAction stuff
                //---------------------------------------------------------------------------
                // apply button
                Add("Apply");
                Add("Remove");
            }
            public void SetNPC(ref NPC npc)
            {
                this.npc = npc;
                spriteSelector.SpriteID = npc.SpriteID;
                spriteSelector.Direction = npc.Direction;
                ((LayoutToggle)Items[1]).Value = npc.randomWalk;
            }
            public NPC GetNPC()
            {
                npc.SetSpriteID(spriteSelector.SpriteID,spriteSheet);
                npc.Direction = spriteSelector.Direction;
                npc.randomWalk = ((LayoutToggle)Items[1]).Value;
                return npc;
            }
        }
    }
}
