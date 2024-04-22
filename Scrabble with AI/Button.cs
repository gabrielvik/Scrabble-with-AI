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
         * Function for when a button should not be clickable for some reason
         * Turns the button red with lerping opacity, and changes the button text
        */
        public async void InvalidClick(string text = "Invalid")
        {
            if (DrawFunc != null) // If the draw function already exists, a click has occurred recently
                return;

            var tempDrawFunc = DrawFunc;
            var tempClickEvent = ClickEvent;

            var lerpStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var delay = 1500; // How long until you can click again

            DrawFunc = delegate ()
            {
                var lerpValue = MathHelper.Lerp(1f, 0.25f, (float)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lerpStart) / delay);
                Ui.OutlinedRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.Red * lerpValue); // Sets opacity to lerp value
                Ui.DrawCenterText(Ui.MontserratBoldSmaller, text, GetPos(), GetSize(), Color.White);
            };
            ClickEvent = null;

            await Task.Delay(delay); // After x seconds, revert the functions to their previous state
            DrawFunc = tempDrawFunc;
            ClickEvent = tempClickEvent;
        }

        /*
         * If the button does not have a draw function, this will become it 
        */
        private void DefaultDraw()
        {
            var isHovering = Util.IsHovering(GetPos(), GetSize());

            if (Mouse.GetState(Alfapet.Window).LeftButton == ButtonState.Pressed && isHovering) // Holding left click on the button
                Ui.OutlinedRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.85f);
            else if (isHovering)
                Ui.OutlinedRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H), Color.White * 0.5f);
            else
                Ui.OutlinedRectangle(new Rectangle((int)X, (int)Y, (int)W, (int)H));

            Ui.DrawCenterText(Ui.MontserratBoldSmaller, DrawText ?? "Button", GetPos(), GetSize(), Color.White);
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
            // No point in checking if not in the program
            if (!Alfapet.IsActive)
                return;

            // Convert to list, so changes made don't interfere in the middle of the update function (attempting to index outside etc)
            foreach (var button in List.Where(button => button.ClickEvent != null).ToList())
            {
                if (Util.IsHovering(button.GetPos(), button.GetSize()))
                {
                    var mouse = Mouse.GetState(Alfapet.Window);

                    // Starts holding down the button
                    if (!button.Pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        button.Pressed = true;
                    }
                    // Has been holding down the button and has now released, the user wants to click
                    if (button.Pressed && mouse.LeftButton == ButtonState.Released)
                    {
                        button.ClickEvent();
                        button.Pressed = false;
                    }
                }
                // Has been holding down but moved the mouse pointer away from the button, user wants to cancel
                else if (button.Pressed)
                {
                    button.Pressed = false;
                }
            }
        }
    }
}
