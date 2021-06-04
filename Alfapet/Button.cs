using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alfapet
{
    class Button : Game
    {
        public float X, Y;
        public float W, H;
        public Action DrawFunc;
        public Action ClickEvent;
        public static List<Button> List = new List<Button>();
        private bool pressed = false;
        private string DrawText;
        public Button()
        {
            List.Add(this);
        }
        ~Button()
        {
            System.Diagnostics.Debug.WriteLine("aasd");
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

        async public void InvalidClick(string text = null)
        {
            if (DrawFunc != null) // Om draw functionen redan finns (man har klickat nyligen), returna
                return;

            var tempDrawFunc = DrawFunc;
            var tempClickEvent = ClickEvent;

            long lerpStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            int delay = 4;
            DrawFunc = delegate ()
            {
                // Sätter färgen till röd och lerpar färgens transparitet 
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.Red * MathHelper.Lerp(1f, 0.95f, (DateTimeOffset.Now.ToUnixTimeMilliseconds() - lerpStart) * delay / 1000));
                UI.DrawCenterText(UI.Montserrat_Bold_Smaller, text == null ? "Invalid" : text, GetPos(), Color.White, (int)W, (int)H);
            };

            await Task.Delay(delay * 1000); // Efter x sekunder, sätt draw funktionen till null och återgå till normalt
            DrawFunc = tempDrawFunc;
            ClickEvent = tempClickEvent;
        }
        private void DefaultDraw()
        {
            bool hovering = Alfapet_Util.IsHovering(GetPos(), GetSize()); // För att inte behöva köra funktionen 2 gånger

            if (Mouse.GetState(Alfapet._window).LeftButton == ButtonState.Pressed && hovering)
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.85f);
            else if (hovering)
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.5f);
            else
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H));

            UI.DrawCenterText(UI.Montserrat_Bold_Smaller, (DrawText == null) ? "Button" : DrawText, GetPos(), Color.White, (int)W, (int)H);
        }

        public static void Draw()
        {
            for (int i = 0; i < List.Count; i++)
                if (List[i].DrawFunc == null)
                    List[i].DefaultDraw();
                else
                    List[i].DrawFunc();
        }

        public static void Think()
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].ClickEvent == null || List[i] == null) // Om man inte har en click funktion är det onödigt att äns kolla
                    continue;

                if (Alfapet_Util.IsHovering(List[i].GetPos(), List[i].GetSize()))
                {
                    MouseState mouse = Mouse.GetState(Alfapet._window);

                    if (!List[i].pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        List[i].pressed = true;
                    }
                    if (List[i].pressed && mouse.LeftButton == ButtonState.Released)
                    {
                        List[i].ClickEvent();
                        List[i].pressed = false;
                    }
                }
                else if (List[i].pressed)
                {
                    List[i].pressed = false;
                }
            }
        }
    }
}
