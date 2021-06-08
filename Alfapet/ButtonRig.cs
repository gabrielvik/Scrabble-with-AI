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
            Buttons = new Dictionary<string, Button>
            {
                { "move", new Button() },
                { "sort", new Button() },
                { "newHand", new Button() }
            };

            var x = 5f;
            var w = (Alfapet.Graphics.GraphicsDevice.Viewport.Width - (Buttons.Count + 1) * x) / Buttons.Count;
            foreach (var button in Buttons.Values)
            {
                button.SetPos(x, Alfapet.Graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight - ButtonHeight - 2.5f);
                button.SetSize(w, ButtonHeight);

                x += w + 5;
            }
            
            Buttons["move"].SetText("Skip");
            Buttons["move"].ClickEvent = delegate
            {
                if (Ai.Playing)
                    return;

                Rounds.DoMove(Board.TilesPlaced <= 0);
            };
            
            Buttons["sort"].SetText("Sort Hand");
            Buttons["sort"].ClickEvent = Hand.Sort;
            
            Buttons["newHand"].SetText("New Hand (skip)");
            Buttons["newHand"].ClickEvent = delegate
            {
                // Låt inte skippa om inte användaren spelar
                if (Ai.Playing)
                {
                    Buttons["newHand"].InvalidClick("Opponent Playing!");
                    return;
                }
                // Låt inte skippa om man har brickor på brädan
                if (Board.TilesPlaced > 0)
                {
                    Buttons["newHand"].InvalidClick("Remove Tiles!");
                    return;
                }

                Hand.GenerateNew();
                Rounds.DoMove(true);
            };
        }
    }
}
