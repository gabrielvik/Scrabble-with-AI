using Microsoft.Xna.Framework;

namespace Alfapet
{
    class ButtonRig : Game
    {
        public static Button[] Buttons;
        public static float ButtonHeight = 47.5f;
        new public static void Initialize()
        {
            Buttons = new Button[3];
            Buttons[0] = new Button();
            Buttons[1] = new Button();
            Buttons[2] = new Button();
            // Buttons[2] = new Button();

            float x = 5;
            float w = (Alfapet._graphics.GraphicsDevice.Viewport.Width - ((Buttons.Length + 1) * x)) / Buttons.Length;
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].SetPos(x, Alfapet._graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight - ButtonHeight - 2.5f);
                Buttons[i].SetSize(w, ButtonHeight);

                x += w + 5;
            }
            Buttons[0].SetText("Skip");
            Buttons[0].ClickEvent = delegate ()
            {
                Rounds.DoMove(Board.TilesPlaced <= 0);
            };
            Buttons[1].SetText("Sort Alphabeticely");
            Buttons[1].ClickEvent = delegate ()
            {
                Alfapet_Util.SortHand();
            };
            Buttons[2].SetText("New Hand (skip)");
            Buttons[2].ClickEvent = delegate ()
            {
                foreach(var tile in Hand.Tiles)
                {
                    tile.Letter = Alfapet_Util.GenerateRandomLetter();
                }
                Hand.SetPositions();
            };
        }
    }
}
