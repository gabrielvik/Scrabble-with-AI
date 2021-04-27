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

        public static int ColumnIndex(int index) => (index % YTiles);

        private static Tile[] GetRow(int index)
        {
            Tile[] temp = new Tile[XTiles];

            for (int i = 0; i < XTiles; i++)
            {
                temp[i] = Tiles[index, i];
            }

            return temp;
        }

        private static Tile[] GetColumn(int index)
        {
            Tile[] temp = new Tile[XTiles];

            for (int i = 0; i < XTiles; i++)
            {
                temp[i] = Tiles[i, index];
            }

            return temp;
        }

        public static void CacheWordPlacement()
        {
            // TODO:::
            List<string> words = new List<string>();

            for (int x = 0; x < XTiles; x++)
            {
                string _word = "";
                for (int y = 0; y < YTiles; y++)
                {
                    if (Tiles[y, x].Letter == '\0')
                    {
                        if (_word.Length == 1)
                        {
                            if (Tiles[y, x + 1].Letter == '\0' || Tiles[y, x - 1].Letter == '\0')
                                continue;
                        }
                        else if (_word.Length > 0)
                        {
                            words.Add(_word);
                        }
                        _word = "";
                    }
                    else
                        _word += Tiles[y, x].Letter;
                }
            }
            for(int y = 0; y < YTiles; y++)
            {
                string _word = "";
                for(int x = 0; x < XTiles; x++)
                {
                    if (Tiles[y, x].Letter == '\0')
                    {
                        if (_word.Length == 1)
                        {
                            if (Tiles[y + 1, x].Letter == '\0' || Tiles[y - 1, x].Letter == '\0')
                                continue;
                        }
                        else if (_word.Length > 0)
                        {
                            words.Add(_word);
                        }
                        _word = "";
                    }
                    else
                        _word += Tiles[y, x].Letter;
                }
            }

            foreach(string word in words)
            {
                System.Diagnostics.Debug.WriteLine(word);
            }
        }
    }
}
