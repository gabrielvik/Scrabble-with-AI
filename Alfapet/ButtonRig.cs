using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Alfapet
{
    class ButtonRig : Game
    {
        public static Dictionary<string, Button> Buttons;
        public static float ButtonHeight = 47.5f;
        public new static void Initialize()
        {
            Buttons = new Dictionary<string, Button>();
            Buttons["move"] = new Button();
            Buttons["sort"] = new Button();
            Buttons["newHand"] = new Button();
            // Buttons[2] = new Button();

            float x = 5;
            var w = (Alfapet.Graphics.GraphicsDevice.Viewport.Width - ((Buttons.Count + 1) * x)) / Buttons.Count;
            foreach (var button in Buttons.Values)
            {
                button.SetPos(x, Alfapet.Graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight - ButtonHeight - 2.5f);
                button.SetSize(w, ButtonHeight);

                x += w + 5;
            }
            
            Buttons["move"].SetText("Skip");
            Buttons["move"].ClickEvent = delegate ()
            {
                if (Ai.Playing)
                    return;
                
                Rounds.DoMove(Board.TilesPlaced <= 0);
            };
            
            Buttons["sort"].SetText("Sort Hand");
            Buttons["sort"].ClickEvent = Hand.Sort;
            
            Buttons["newHand"].SetText("New Hand (skip)");
            Buttons["newHand"].ClickEvent = delegate ()
            {
                if (Ai.Playing)
                {
                    Buttons["newHand"].InvalidClick("Opponent Playing!");
                    return;
                }

                foreach (var tile in Hand.Tiles)
                {
                    tile.Letter = Util.GenerateRandomLetter();
                }
                Hand.SetPositions();
                Rounds.DoMove(true);
            };
        }
    }
}
