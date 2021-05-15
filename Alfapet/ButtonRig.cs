using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alfapet
{
    class ButtonRig : Game
    {
        public static Button[] Buttons;
        public static float ButtonHeight = 47.5f;
        public static void Initialize()
        {
            Buttons = new Button[2];
            Buttons[0] = new Button();
            Buttons[1] = new Button();
            // Buttons[2] = new Button();

            float x = 5;
            float w = (Alfapet._graphics.GraphicsDevice.Viewport.Width - ((Buttons.Length + 1) * x)) / Buttons.Length;
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].SetPos(x, Alfapet._graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight * 1.5f - 5f);
                Buttons[i].SetSize(w, ButtonHeight);

                x += w + 5;
            }
            Buttons[0].SetText("Skip");
            Buttons[0].ClickEvent = delegate ()
            {
                Rounds.DoMove();
            };
            Buttons[1].SetText("Sort Alphabeticely");
            Buttons[1].ClickEvent = delegate ()
            {
                Alfapet_Util.SortHand();
            };
        }
    }
}
