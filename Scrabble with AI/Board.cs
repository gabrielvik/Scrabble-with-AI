using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Alfapet
{
    class Board : Game
    {
        public static Tile[,] Tiles;

        private const float TilesMargin = 5;

        public static float TilesWidth;
        public static float TilesHeight;

        public static int XTiles = 15;
        public static int YTiles = 15;

        public static int TilesPlaced = 0;

        public new static void Initialize()
        {
            Tiles = new Tile[YTiles, XTiles];

            // Automatically adjusts to the screen size
            TilesWidth = (Alfapet.Graphics.GraphicsDevice.Viewport.Width - (XTiles + 1) * TilesMargin) / XTiles;
            TilesHeight = (Alfapet.Graphics.GraphicsDevice.Viewport.Height - (Hand.TilesHeight + ButtonRig.ButtonHeight) - (YTiles + 1) * TilesMargin) / YTiles;

            float xPos = 5, yPos = 5;
            for (var y = 0; y < YTiles; y++)
            {
                if (xPos + TilesWidth > Alfapet.Graphics.GraphicsDevice.Viewport.Width) // No more room, go to the next row
                {
                    yPos += TilesHeight + TilesMargin;
                    xPos = 5;
                }

                for (var x = 0; x < XTiles; x++)
                {
                    Tiles[y, x] = new Tile();
                    Tiles[y, x].SetSize(TilesWidth, TilesHeight);
                    Tiles[y, x].SetPos(xPos, yPos);

                    xPos += TilesWidth + TilesMargin;
                }
            }
        }

        public static void Draw()
        {
            foreach (var tile in Tiles)
            {
                if (tile.TempPlaced)
                    Ui.OutlinedRectangle(new Rectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H), Color.White * 0.5f);
                else
                    Ui.OutlinedRectangle(new Rectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H), null);

                if (tile.Letter != '\0')
                    Ui.DrawCenterText(Ui.MontserratBoldSmaller, tile.Letter.ToString(), tile.GetPos(), tile.GetSize(), Color.White);
            }
        }

        /*
         * Changes the text on the move button to Skip or Move depending on how many tiles have been placed
        */
        public static void CheckTilesPlaced()
        {
            ButtonRig.Buttons["move"].SetText(TilesPlaced <= 0 ? "Skip" : "Move");
        }

        /*
         * Resets all temporarily placed tiles
         * noDelay indicates whether there should be a delay between each tile or not
        */
        public static async void ResetTempTiles(Action<Tile> callback = null, bool noDelay = false)
        {
            foreach (var tile in Tiles)
            {
                if (!tile.TempPlaced || tile.Letter == '\0')
                    continue;

                callback?.Invoke(tile);
                tile.TempPlaced = false;

                if (!noDelay)
                    await Task.Delay(150); // Wait 0.15s before next loop so the user can see everything happen
            }
        }
    }
}
