using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alfapet
{
    class Button : Game
    {
        public float X, Y;
        public float W, H;
        public Action DrawFunc;
        public Action ClickEvent;
        private static List<Button> buttons = new List<Button>();
        private bool pressed = false;
        private string DrawText;
        public Button()
        {
            buttons.Add(this);
        }

        public void SetPos(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2 GetPos()
        {
            return new Vector2(X, Y);
        }

        public void SetSize(float w, float h)
        {
            W = w;
            H = h;
        }

        public Vector2 GetSize()
        {
            return new Vector2(W, H);
        }

        public void SetText(string text)
        {
            DrawText = text;
        }

        private void DefaultDraw()
        {
            bool hovering = Alfapet_Util.IsHovering(GetPos(), GetSize()); // För att inte behöva köra funktionen 2 gånger

            if (Mouse.GetState(Alfapet._window).LeftButton == ButtonState.Pressed && hovering)
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.85f);
            else if(hovering)
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.5f);
            else
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H));

            UI.DrawCenterChar(Fonts.Montserrat_Bold_Smaller, (DrawText == null) ? "Button" : DrawText, GetPos(), Color.White, (int)W, (int)H);
        }

        public static void Draw()
        {
            for (int i = 0; i < buttons.Count; i++)
                if(buttons[i].DrawFunc == null)
                    buttons[i].DefaultDraw();
                else
                    buttons[i].DrawFunc();
        }

        public static void Think()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].ClickEvent == null) // Om man inte har en click funktion är det onödigt att äns kolla
                    continue;

                if (Alfapet_Util.IsHovering(buttons[i].GetPos(), buttons[i].GetSize()))
                {
                    MouseState mouse = Mouse.GetState(Alfapet._window);

                    if(!buttons[i].pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        buttons[i].pressed = true;
                    }
                    if(buttons[i].pressed && mouse.LeftButton == ButtonState.Released)
                    {
                        buttons[i].ClickEvent();
                        buttons[i].pressed = false;
                    }
                }
                else if (buttons[i].pressed)
                {
                    buttons[i].pressed = false;
                }
            }
        }
    }
}
