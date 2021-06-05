using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alfapet
{
    class Button : Game
    {
        public float X, Y;
        public float W, H;
        private Action DrawFunc;
        public Action ClickEvent;
        public static List<Button> List = new List<Button>();
        private bool pressed = false;
        private string DrawText;
        private static int i = 0;
        public int Id;
        public Button()
        {
            Id = i + 1;
            List.Add(this);
            i++;
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

        /*
         * Funktion för att en knapp som av någon anleding inte ska tryckas
         * Gör knappen röd som lerpar genomskinlighet, och ändrar texten på knappen
        */
        public async void InvalidClick(string text = "Invalid")
        {
            if (DrawFunc != null) // Om draw functionen redan finns har klickat nyligen
                return;

            var tempDrawFunc = DrawFunc;
            var tempClickEvent = ClickEvent;

            var lerpStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var delay = 1500; // Hur länge det tar tills man får klicka igen

            DrawFunc = delegate ()
            {
                var lerpValue = MathHelper.Lerp(1f, 0.25f, (float)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lerpStart) / delay);
                // Sätter färgen till röd och lerpar färgens transparitet 
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.Red * lerpValue);
                UI.DrawCenterText(UI.MontserratBoldSmaller, text, GetPos(), Color.White, (int)W, (int)H);
            };
            ClickEvent = null;

            await Task.Delay(delay); // Efter x sekunder, sätt tillbaka funktionerna till deras tidigare läge
            DrawFunc = tempDrawFunc;
            ClickEvent = tempClickEvent;
        }
        private void DefaultDraw()
        {
            var isHovering = AlfapetUtil.IsHovering(GetPos(), GetSize());

            if (Mouse.GetState(Alfapet.Window).LeftButton == ButtonState.Pressed && isHovering)
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.85f);
            else if (isHovering)
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.5f);
            else
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H));

            UI.DrawCenterText(UI.MontserratBoldSmaller, DrawText ?? "Button", GetPos(), Color.White, (int)W, (int)H);
        }

        public static void Draw()
        {
            foreach (var button in List)
            {
                if (button.DrawFunc == null)
                    button.DefaultDraw();
                else
                    button.DrawFunc();
            }
        }

        public static void ListenForPresses()
        {
            if (!Alfapet.IsActive)
                return;

            foreach (var button in List.Where(button => button.ClickEvent != null).ToList())
            {
                if (AlfapetUtil.IsHovering(button.GetPos(), button.GetSize()))
                {
                    var mouse = Mouse.GetState(Alfapet.Window);

                    if (!button.pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        button.pressed = true;
                    }
                    if (button.pressed && mouse.LeftButton == ButtonState.Released)
                    {
                        button.ClickEvent();
                        button.pressed = false;
                    }
                }
                else if (button.pressed)
                {
                    button.pressed = false;
                }
            }
        }
    }
}
