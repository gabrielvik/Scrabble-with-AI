using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Alfapet
{
    class Board : Game
    {
        public static Tile[,] Tiles;

        static public float TilesMargin = 5;

        static public float TilesWidth;
        static public float TilesHeight;

        static public int XTiles = 15;
        static public int YTiles = 15;

        static public Action<dynamic, Vector2, Tile, Tile> TempDragCallback;
        static public Action<dynamic, Tile> FailDragCallback;

        static private bool PlacedValidWord = true;

        public static void Build() // Bygger brädan, kallas i Initalize()
        {
            Tiles = new Tile[YTiles, XTiles];

            TilesWidth = (Alfapet._graphics.GraphicsDevice.Viewport.Width - (TilesMargin * (XTiles + 1))) / XTiles;
            TilesHeight = (Alfapet._graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight - (TilesMargin * (YTiles + 1))) / YTiles - 3;

            float x = 5, y = 5;

            for (int i = 0; i < YTiles; i++)
            {
                if (x + TilesWidth > Alfapet._graphics.GraphicsDevice.Viewport.Width)
                {
                    y += TilesHeight + TilesMargin;
                    x = 5;
                }

                for (int z = 0; z < XTiles; z++)
                {
                    Tiles[i, z] = new Tile();
                    Tiles[i, z].SetSize(TilesWidth, TilesHeight);
                    Tiles[i, z].SetPos(x, y);
                    if (i == 1 && z == 3)
                    {
                        Tiles[i, z].Letter = 'P';
                    }
                    else if (i == 2 && z == 3)
                    {
                        Tiles[i, z].Letter = 'A';
                    }

                    x += TilesWidth + TilesMargin;
                }
            }

            /*int XTiles = Alfapet._graphics.GraphicsDevice.Viewport.TilesWidth / 100;
            int YTiles = (Alfapet._graphics.GraphicsDevice.Viewport.TilesHeight - (int)Hand.TilesTilesHeight - 5) / 100;

            int TilesMargin = 5;

            int TilesWidth = XTiles;
            int TilesHeight = YTiles;

            int x = 5, y = 5;

            for (int i = 0; i < XTiles * YTiles; i++)
            {
                if(i % YTiles == 0)
                {
                    y += TilesHeight + TilesMargin;
                }
                x += TilesWidth + TilesMargin;

                Debug.WriteLine("x: " + x + " y:" + y);
            }*/
        }

        public static void Draw()
        {
            foreach (var tile in Tiles)
            {
                UI.StylishRectangle(new Rectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H));

                if (tile.Letter != '\0')
                {
                    UI.DrawCenterChar(Fonts.Montserrat_Bold_Smaller, tile.Letter.ToString(), new Vector2(tile.X, tile.Y), Color.White, (int)tile.W, (int)tile.H);
                }
            }
        }
    }
}
