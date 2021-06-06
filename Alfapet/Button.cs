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
        public float X, Y, W, H;
        private Action DrawFunc;
        public Action ClickEvent;
        public static List<Button> List = new List<Button>();
        private bool Pressed = false;
        private string DrawText;

        public Button()
        {
            List.Add(this);
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
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.Red * lerpValue); // Sätter genomskinligheten till lerp värdet
                UI.DrawCenterText(UI.MontserratBoldSmaller, text, GetPos(), GetSize(), Color.White);
            };
            ClickEvent = null;

            await Task.Delay(delay); // Efter x sekunder, sätt tillbaka funktionerna till deras tidigare läge
            DrawFunc = tempDrawFunc;
            ClickEvent = tempClickEvent;
        }
        private void DefaultDraw()
        {
            var isHovering = Util.IsHovering(GetPos(), GetSize());

            if (Mouse.GetState(Alfapet.Window).LeftButton == ButtonState.Pressed && isHovering) // Håller leftclick på knappen
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.85f);
            else if (isHovering)
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.5f);
            else
                UI.StylishRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H));

            UI.DrawCenterText(UI.MontserratBoldSmaller, DrawText ?? "Button", GetPos(), GetSize(), Color.White);
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
            // Ingen idé att kolla om man inte är inne i programmet
            if (!Alfapet.IsActive)
                return;

            // Gör om till lista, så ändringar som görs inte interfererar mitt i update funktionen (försöker indexa utanför etc)
            foreach (var button in List.Where(button => button.ClickEvent != null).ToList())
            {
                if (Util.IsHovering(button.GetPos(), button.GetSize()))
                {
                    var mouse = Mouse.GetState(Alfapet.Window);

                    // Man börjar hålla nere knappen
                    if (!button.Pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        button.Pressed = true;
                    }
                    // Man har hållt nere knappen och har nu släppt, användaren vill klicka
                    if (button.Pressed && mouse.LeftButton == ButtonState.Released)
                    {
                        button.ClickEvent();
                        button.Pressed = false;
                    }
                }
                // Man har hållt nere men tagit bort muspekaren från knappen, användaren vill avbryta
                else if (button.Pressed)
                {
                    button.Pressed = false;
                }
            }
        }
    }
}
