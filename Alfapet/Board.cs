using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;

namespace Alfapet
{
    class Board : Game
    {
        public static Tile[] Tiles;

        static public float TilesMargin = 5;

        static public float TilesWidth;
        static public float TilesHeight;

        public static void Build() // Bygger brädan, kallas i Initalize()
        {
            int x_tiles = 15;
            int y_tiles = 15;
            int tiles = x_tiles * y_tiles;

            Tiles = new Tile[tiles];

            TilesWidth  = (Alfapet._graphics.GraphicsDevice.Viewport.Width - (TilesMargin * (x_tiles + 1))) / x_tiles;
            TilesHeight = (Alfapet._graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight - (TilesMargin * (y_tiles))) / y_tiles;

            float x = 5, y = 5;

            for (int i = 0; i < tiles; i++)
            {
                if (x + TilesWidth > Alfapet._graphics.GraphicsDevice.Viewport.Width)
                {
                    y += TilesHeight + TilesMargin;
                    x = 5;
                }

                Tiles[i] = new Tile();
                Tiles[i].SetSize(TilesWidth, TilesHeight);
                Tiles[i].SetPos(x, y);

                x += TilesWidth + TilesMargin;
            }

            /*int x_tiles = Alfapet._graphics.GraphicsDevice.Viewport.TilesWidth / 100;
            int y_tiles = (Alfapet._graphics.GraphicsDevice.Viewport.TilesHeight - (int)Hand.TilesTilesHeight - 5) / 100;

            int TilesMargin = 5;

            int TilesWidth = x_tiles;
            int TilesHeight = y_tiles;

            int x = 5, y = 5;

            for (int i = 0; i < x_tiles * y_tiles; i++)
            {
                if(i % y_tiles == 0)
                {
                    y += TilesHeight + TilesMargin;
                }
                x += TilesWidth + TilesMargin;

                Debug.WriteLine("x: " + x + " y:" + y);
            }*/
        }

        public static void Draw()
        {
            for(int i = 0; i < Tiles.Length; i++)
            {
                UI.StylishRectangle(new Rectangle((int)Tiles[i].X, (int)Tiles[i].Y, (int)Tiles[i].W, (int)Tiles[i].H));
            }
        }
    }
}
