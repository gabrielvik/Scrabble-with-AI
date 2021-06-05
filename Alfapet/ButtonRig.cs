using Microsoft.Xna.Framework;

namespace Alfapet
{
    class ButtonRig : Game
    {
        public static Button[] Buttons;
        public static float ButtonHeight = 47.5f;
        public new static void Initialize()
        {
            Buttons = new Button[3];
            Buttons[0] = new Button();
            Buttons[1] = new Button();
            Buttons[2] = new Button();
            // Buttons[2] = new Button();

            float x = 5;
            var w = (Alfapet.Graphics.GraphicsDevice.Viewport.Width - ((Buttons.Length + 1) * x)) / Buttons.Length;
            foreach (var button in Buttons)
            {
                button.SetPos(x, Alfapet.Graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight - ButtonHeight - 2.5f);
                button.SetSize(w, ButtonHeight);

                x += w + 5;
            }
            
            Buttons[0].SetText("Skip");
            Buttons[0].ClickEvent = delegate ()
            {
                Rounds.DoMove(Board.TilesPlaced <= 0);
            };
            
            Buttons[1].SetText("Sort Alphabetically");
            Buttons[1].ClickEvent = AlfapetUtil.SortHand;
            
            Buttons[2].SetText("New Hand (skip)");
            Buttons[2].ClickEvent = delegate ()
            {
                foreach (var tile in Hand.Tiles)
                {
                    tile.Letter = AlfapetUtil.GenerateRandomLetter();
                }
                Hand.SetPositions();
                Rounds.DoMove(true);
            };
        }
    }
}
