using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Alfapet
{
    class Board : Game
    {
        public static Tile[,] Tiles;

        static public float TilesMargin = 5;

        static public float TilesWidth;
        static public float TilesHeight;

        static public int XTiles = 16;
        static public int YTiles = 16;

        public static void Build() // Bygger brädan, kallas i Initalize()
        {
            int tiles = XTiles * YTiles;

            Tiles = new Tile[YTiles, XTiles];

            TilesWidth = (Alfapet._graphics.GraphicsDevice.Viewport.Width - (TilesMargin * (XTiles + 1))) / XTiles;
            TilesHeight = (Alfapet._graphics.GraphicsDevice.Viewport.Height - Hand.TilesHeight - (TilesMargin * (YTiles + 1))) / YTiles;

            float x = 5, y = 5;

            for (int i = 0; i < YTiles; i++)
            {
                if (x + TilesWidth > Alfapet._graphics.GraphicsDevice.Viewport.Width)
                {
                    y += TilesHeight + TilesMargin;
                    x = 5;
                }

                for(int z = 0; z < XTiles; z++)
                {
                    Tiles[i, z] = new Tile();
                    Tiles[i, z].SetSize(TilesWidth, TilesHeight);
                    Tiles[i, z].SetPos(x, y);

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
            foreach(var tile in Tiles)
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
            
            for(int i = 0; i < XTiles; i++)
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

        public static bool IsValidWord(int x, int y, char letter)
        {
            Tile[] row = GetRow(y);
            Tile[] column = GetColumn(ColumnIndex(x));

            string word = "";

            bool columnUp = (y - 1  < 0) ? false : (Tiles[y - 1, x].Letter != '\0'); // Om bokstaven över Y har en bokstav
            bool columnDown = (y + 1 > YTiles) ? false : (Tiles[y + 1, x].Letter != '\0'); // Om bokstaven under Y har en bokstav

            if (columnUp && columnDown) // Om Y är emellan två bokstaver
            {
                for(int i = 0; i < column.Length; i++) // Loopa egenom kolumnen
                {
                    if (i == y) // Om I är Y (tomma bokstaven i mellan), lägg till bokstaven som ska placeras, i ordet och gå vidare
                    {
                        word += letter;
                        continue;
                    }
                    else if (Tiles[i, x].Letter != '\0') // Annars om brickan inte är tom, lägg till karaktären i ordet
                        word += Tiles[i, x].Letter;
                }
            }
            else if(columnUp || columnDown) // Annars om det finns en bokstav över eller under Y
            {
                if (columnDown) // Om man lägger bokstaven över en annan, lägg till karaktären i början av ordet
                    word += letter;

                for (int i = 0; i < column.Length; i++) // Loopa igenom kolumnen och lägg till bokstäverna i ordet
                {
                    if (Tiles[i, x].Letter == '\0')
                    {
                        // Om I är mindre än tillagda bokstavens plats och ordets längd är över 1 (man har lagt till en bokstav från ett annat ord på brädan), sätt tillbaka ordet till placerade bokstaven
                        if (i < y && word.Length > 1)
                            word = letter.ToString();
                        else if (i > y) // Annars om man är på en tom bokstav och i är större än y, är ordet slut
                            break;
                        else
                            continue;
                    }
                    word += Tiles[i, x].Letter;
                }

                if(columnUp) // Om man lägger bokstaven under en annan, lägg till karaktären i slutet på ordet
                    word += letter;
            }
            

            System.Diagnostics.Debug.WriteLine("WORD: " + word);

            return true;
        }
    }
}
