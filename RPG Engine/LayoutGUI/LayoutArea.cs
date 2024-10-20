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
        // LayoutArea
        //----------------------------------------------------------------------
        public class LayoutArea : ILayoutItem
        {
            //----------------------------------------------------------------------
            // ILayoutItem
            //----------------------------------------------------------------------
            public Vector2 Position { get; set; } = new Vector2(0, 0);
            public Vector2 Size { get; set; }
            public Vector2 Padding { get; set; } = new Vector2(5, 5);
            public bool GetSizeFromParent { get; set; } = false;
            public Vector2 ContentSize 
            { 
                get
                {
                    if (!GetSizeFromParent) return Size;
                    Vector2 contentSize = new Vector2(0, 0);
                    bool first = true;
                    foreach (ILayoutItem item in Items)
                    {
                        Vector2 itemSize = item.ContentSize;
                        if (Vertical)
                        {
                            contentSize.X = Math.Max(contentSize.X, itemSize.X);
                            if (first) { first = false; contentSize = itemSize; }
                            else contentSize.Y += itemSize.Y + Spacing;
                        }
                        else
                        {
                            if (first) { first = false; contentSize = itemSize; }
                            else contentSize.X += itemSize.X + Spacing;
                            contentSize.Y = Math.Max(contentSize.Y, itemSize.Y);
                        }
                    }
                    return contentSize + (Padding * 2);
                }
            }
            public bool Visible 
            { 
                get
                {
                    if (Items.Count > 0) return Items[0].Visible;
                    if (background != null) return background.Visible;
                    if (extras.Count > 0) return extras[0].Visible;
                    return false;
                }
                set
                {
                    if (background != null) background.Visible = value;
                    foreach (ILayoutItem sprite in extras) sprite.Visible = value;
                    foreach (ILayoutItem item in Items) item.Visible = value;
                }
            }
            public void AddToScreen(Screen screen, int layer = 0)
            {
                if(background != null) screen.AddSprite(background,layer);
                foreach (ILayoutItem sprite in extras)
                {
                    screen.AddSprite(sprite, layer);
                }
                foreach (ILayoutItem item in Items)
                {
                    item.AddToScreen(screen, layer);
                }
            }
            public void RemoveFromScreen(Screen screen)
            {
                if (background != null) screen.RemoveSprite(background);
                foreach (ILayoutItem sprite in extras)
                {
                    screen.RemoveSprite(sprite);
                }
                foreach (ILayoutItem item in Items)
                {
                    item.RemoveFromScreen(screen);
                }
            }
            public void ApplyLayout()
            {
                if (background != null)
                {
                    background.Position = new Vector2(Position.X, Position.Y + Size.Y/2);
                    background.Size = Size;
                }
                foreach (ILayoutItem sprite in extras)
                {
                    sprite.Position = Position;
                    sprite.Size = Size;
                    sprite.ApplyLayout();
                }
                Vector2 pos = Position + Padding;
                Vector2 spacing = new Vector2(Spacing, Spacing);
                Vector2 contentSize = ContentSize;
                if (Justify)
                {
                    GridInfo.Echo("Justify");
                    GridInfo.Echo("Size: " + Size);
                    GridInfo.Echo("Spacing before: " + spacing); 
                    GridInfo.Echo("ContentSize: " + contentSize);
                    if (Vertical) spacing.Y = (Size.Y - contentSize.Y) / (Items.Count - 1);
                    else spacing.X = (Size.X - contentSize.X) / (Items.Count - 1);
                    GridInfo.Echo("Spacing after: " + spacing);
                }
                Vector2 contentsArea = Size - (Padding * 2);
                foreach (ILayoutItem item in Items)
                {
                    item.Position = pos;
                    if (item.GetSizeFromParent)
                    {
                        if (Justify)
                        {
                            if (Vertical) item.Size = new Vector2(contentsArea.X, (contentsArea.Y / Items.Count) - spacing.Y);
                            else item.Size = new Vector2((contentsArea.X / Items.Count) - spacing.X, contentsArea.Y);
                        }
                        else
                        {
                            if (Vertical) item.Size = new Vector2(contentsArea.X, item.ContentSize.Y);
                            else item.Size = new Vector2(item.ContentSize.X, contentsArea.Y);
                        }
                    }
                    item.ApplyLayout();
                    if (Vertical) pos.Y += item.ContentSize.Y + spacing.Y;
                    else pos.X += item.ContentSize.X + spacing.X;
                }
            }
            //----------------------------------------------------------------------
            // properties
            //----------------------------------------------------------------------
            public List<ILayoutItem> Items { get; set; } = new List<ILayoutItem>();
            ScreenSprite background;
            List<ILayoutItem> extras = new List<ILayoutItem>();
            public bool Vertical { get; set; } = true;
            public bool Justify { get; set; } = false;
            public float Spacing { get; set; } = 5;
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public LayoutArea() { }
            public LayoutArea(Vector2 position, Vector2 size, Vector2 padding)
            {
                Position = position;
                Size = size;
                Padding = padding;
            }
            public LayoutArea(Vector2 position, Vector2 size, Vector2 padding, Color backgroundColor)
            {
                Position = position;
                Size = size;
                Padding = padding;
                background = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, position, 0f, size, backgroundColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
            }
            public LayoutArea(Vector2 position, Vector2 size, Vector2 padding, Color backgroundColor, Color borderColor, float borderThickness)
            {
                Position = position;
                Size = size;
                Padding = padding;
                background = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, position, 0f, size, backgroundColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                LayoutBorder border = new LayoutBorder(position, size, borderThickness, borderColor);
                extras.Add(border);
            }
        }
        //----------------------------------------------------------------------
    }
}
