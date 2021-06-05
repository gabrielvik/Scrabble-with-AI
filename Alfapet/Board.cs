using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alfapet
{
    class Board : Game
    {
        public static Tile[,] Tiles;

        private const float TilesMargin = 5;

        public static float TilesWidth;
        public static float TilesHeight;

        public const int XTiles = 15;
        public const int YTiles = 15;

        public static int TilesPlaced = 0;

        public new static void Initialize() // Bygger brädan, kallas i Initalize()
        {
            Tiles = new Tile[YTiles, XTiles];

            TilesWidth = (Alfapet.Graphics.GraphicsDevice.Viewport.Width - (XTiles + 1) * TilesMargin) / XTiles;
            TilesHeight = (Alfapet.Graphics.GraphicsDevice.Viewport.Height - (Hand.TilesHeight + ButtonRig.ButtonHeight) - ((YTiles + 1) * TilesMargin)) / YTiles;

            float x = 5, y = 5;

            for (var i = 0; i < YTiles; i++)
            {
                if (x + TilesWidth > Alfapet.Graphics.GraphicsDevice.Viewport.Width)
                {
                    y += TilesHeight + TilesMargin;
                    x = 5;
                }

                for (var z = 0; z < XTiles; z++)
                {
                    Tiles[i, z] = new Tile();
                    Tiles[i, z].SetSize(TilesWidth, TilesHeight);
                    Tiles[i, z].SetPos(x, y);
                    
                    if (i == 4 && z == 7)
                        Tiles[i, z].Letter = 'P';
                    else if (i == 5 && z == 7)
                        Tiles[i, z].Letter = 'A';
                    else if (i == 6 && z == 7)
                        Tiles[i, z].Letter = 'Y';

                    x += TilesWidth + TilesMargin;
                }
            }
        }

        public static void Draw()
        {
            foreach (var tile in Tiles)
            {
                if (tile.TempPlaced)
                    UI.StylishRectangle(new Rectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H), Color.White * 0.5f);
                else
                    UI.StylishRectangle(new Rectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H), null);

                if (tile.Letter != '\0')
                    UI.DrawCenterText(UI.MontserratBoldSmaller, tile.Letter.ToString(), new Vector2(tile.X, tile.Y), Color.White, (int)tile.W, (int)tile.H);
            }
        }

        /*
         * Ändrar texten på move knappen beroende på hur många brickor man placerat
        */
        public static void CheckTilesPlaced()
        {
            ButtonRig.Buttons[0].SetText(TilesPlaced <= 0 ? "Skip" : "Move");
        }

        public static async void ResetTempTiles(Action<Tile> callback = null)
        {
            foreach (var tile in Tiles)
            {
                if (!tile.TempPlaced || tile.Letter == '\0')
                    continue;

                callback?.Invoke(tile);
                
                tile.TempPlaced = false;
                await Task.Delay(150); // Vänta 0.15s innan nästa loop så användaren kan se allting hända
            }
        }
    }
}
