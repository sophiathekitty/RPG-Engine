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
        // game ui menu
        //-----------------------------------------------------------------------
        public class GameUIMenu : LayoutMenu
        {
            Dictionary<string,string> actions = new Dictionary<string, string>();
            GameData gameData;
            NPC npc;
            //-----------------------------------------------------------------------
            // constructor
            //-----------------------------------------------------------------------
            public GameUIMenu(string header, Vector2 position, Vector2 size, Vector2 padding, GameInput input, Color backGroundColor, Color borderColor, float borderWidth, GameData gameData, NPC npc) : base(position, size, padding, input, backGroundColor, borderColor, borderWidth)
            {
                if (!string.IsNullOrEmpty(header))
                {
                    LayoutText title = new LayoutText(header, Color.White, 0.5f);
                    title.Alignment = TextAlignment.CENTER;
                    title.Padding = new Vector2(0, -15);
                    extras.Add(title);
                }
                this.gameData = gameData;
                this.npc = npc;
            }
            //-----------------------------------------------------------------------
            // add a menu item with its action
            //-----------------------------------------------------------------------
            public void Add(string displayText, string action)
            {
                actions.Add(displayText, action);
                Add(displayText);
            }
            //-----------------------------------------------------------------------
            // handle input
            //-----------------------------------------------------------------------
            public override string Run()
            {
                string action = base.Run();
                if (actions.ContainsKey(action))
                {
                    if(gameData == null) return "done"; // no game data, so just return done
                    if (gameData.map.Actions.ContainsKey(actions[action])) gameData.map.Actions[actions[action]].Execute(npc);
                    else if (gameData.Actions.ContainsKey(actions[action])) gameData.Actions[actions[action]].Execute(npc);
                    return "done";
                }
                return action;
            }
        }
        //-----------------------------------------------------------------------
    }
}
