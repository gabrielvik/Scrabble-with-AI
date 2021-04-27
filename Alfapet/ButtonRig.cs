using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alfapet
{
    class ButtonRig : Game
    {
        public static Button[] Buttons;
        public static void Initialize()
        {
            Buttons = new Button[2];
            Buttons[0] = new Button();
            Buttons[1] = new Button();

            int x = 5;
            int w = Alfapet._graphics.GraphicsDevice.Viewport.Width / Buttons.Length - 10;
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].SetPos(x, Alfapet._graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight * 1.5f - 5f);
                Buttons[i].SetSize(w, 47.5f);

                x += w + 5;
            }
            Buttons[0].SetText("DoMove()");
            Buttons[0].ClickEvent = delegate ()
            {
                Rounds.DoMove();
            };

            Buttons[1].ClickEvent = delegate ()
            {
                Alfapet_Util.SortHand();
            };
        }
    }
}
