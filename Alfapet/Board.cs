using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;

namespace Alfapet
{
    class Board : Game
    {
        public static Tile[] Tiles = new Tile[40];
        public static void BuildBoard() // Bygger brädan, kallas i Initalize()
        {
            int x_tiles = Alfapet._graphics.GraphicsDevice.Viewport.Width / 100;
            int y_tiles = (Alfapet._graphics.GraphicsDevice.Viewport.Height - (int)Hand.TilesHeight - 5) / 100;

            int margin = 5;

            int width = x_tiles;
            int height = y_tiles;

            int x = 5, y = 5;

            for (int i = 0; i < x_tiles * y_tiles; i++)
            {
                if(i % y_tiles == 0)
                {
                    y += height + margin;
                }
                x += width + margin;

                Debug.WriteLine("x: " + x + " y:" + y);
            }
        }
    }
}
