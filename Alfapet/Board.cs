using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Alfapet
{
    class Board : Game
    {
        public static Tile[] Tiles;

        static public float TilesMargin = 5;

        static public float TilesWidth;
        static public float TilesHeight;

        static public int XTiles = 16;
        static public int YTiles = 16;

        public static void Build() // Bygger brädan, kallas i Initalize()
        {
            int tiles = XTiles * YTiles;

            Tiles = new Tile[tiles];

            TilesWidth = (Alfapet._graphics.GraphicsDevice.Viewport.Width - (TilesMargin * (XTiles + 1))) / XTiles;
            TilesHeight = (Alfapet._graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight - (TilesMargin * (YTiles + 1))) / YTiles;

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
            for (int i = 0; i < Tiles.Length; i++)
            {
                UI.StylishRectangle(new Rectangle((int)Tiles[i].X, (int)Tiles[i].Y, (int)Tiles[i].W, (int)Tiles[i].H));

                if (Tiles[i].Letter != '\0')
                {
                    UI.DrawCenterChar(Fonts.Montserrat_Bold_Smaller, Tiles[i].Letter.ToString(), new Vector2(Tiles[i].X, Tiles[i].Y), Color.White, (int)Tiles[i].W, (int)Tiles[i].H);
                }
            }
        }

        public static int RowIndex(int index)
        {
            float test = index / XTiles;
            return (int)(Math.Round(test));
        }
        public static int ColumnIndex(int index) => (index % YTiles);

        private static Dictionary<int, Tile> GetRow(int index)
        {
            Dictionary<int, Tile> temp = new Dictionary<int, Tile>();

            int startingPoint = index - (index % XTiles);
            for(int i = startingPoint; i < startingPoint + XTiles; i++)
            {
                temp[i] = Tiles[i];
            }

            return temp;
        }

        private static Dictionary<int, Tile> GetColumn(int index)
        {
            Dictionary<int, Tile> temp = new Dictionary<int, Tile>();

            for(int i = 0; i < XTiles * YTiles; i += XTiles)
            {
                if (Tiles[i].Letter == '\0')
                    continue;

                temp[i] = Tiles[i];
            }

            return temp;
        }

        public static bool IsValidWord(int index, char letter)
        {
            Dictionary<int, Tile> row = GetRow(index);
            Dictionary<int, Tile> column = GetColumn(index);

            string _word = "";

            if(Tiles[index - XTiles].Letter != '\0')
            {
                System.Diagnostics.Debug.WriteLine("gets here1");
                foreach (var tile in column)
                {
                    _word += tile.Value.Letter;
                }
                _word += letter;
                if (Dictionaries.IsWord(_word))
                    return true;
            }
            else if(Tiles[index + XTiles].Letter != '\0')
            {
                System.Diagnostics.Debug.WriteLine("gets here2");
                _word = letter.ToString();
                foreach (var tile in column)
                {
                    _word += tile.Value.Letter;
                }
                if (Dictionaries.IsWord(_word))
                    return true;
            }
            
                


            /*for(int i = 0; i < column.Count; i++)
            {
            //    System.Diagnostics.Debug.WriteLine(i);
            }



           // for (int i = 0; column[i].Letter != '\0'; i++) { }
            if (ColumnIndex(column.First().Key) == ColumnIndex(index)) // 
            {
                string _word = letter.ToString();

                foreach(var tile in column)
                {
                    if (tile.Value.Letter == '\0') { continue; }

                    _word += tile.Value.Letter.ToString();
                }
                System.Diagnostics.Debug.WriteLine(_word);
            }*/

            return true;
        }
    }
}
