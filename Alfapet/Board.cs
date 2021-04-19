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

        static public int XTiles = 15;
        static public int YTiles = 15;

        static public bool PlacedValidWord = false;

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

                for(int z = 0; z < XTiles; z++)
                {
                    Tiles[i, z] = new Tile();
                    Tiles[i, z].SetSize(TilesWidth, TilesHeight);
                    Tiles[i, z].SetPos(x, y);
                    if(i == 1 && z == 3)
                    {
                        Tiles[i, z].Letter = 'P';
                    }
                    else if(i == 2 && z == 3)
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

        public static void CacheWordPlacement(int x, int y, char letter)
        {
            Tile[] row = GetRow(y);
            Tile[] column = GetColumn(ColumnIndex(x));

            string columnWord = null;

            bool columnUp = (y - 1  < 0) ? false : (Tiles[y - 1, x].Letter != '\0'); // Om man är på första raden returna false, annars returna om övre raden har bokstav
            bool columnDown = (y + 1 >= YTiles) ? false : (Tiles[y + 1, x].Letter != '\0'); // Om man är på sista raden returna false, annars returna om undre raden har bokstav

            if (columnUp && columnDown) // Om Y är emellan två bokstaver
            {
                columnWord = "";
                for(int i = 0; i < column.Length; i++) // Loopa egenom kolumnen
                {
                    if (i == y) // Om I är Y (tomma bokstaven i mellan), lägg till bokstaven som ska placeras, i ordet och gå vidare
                    {
                        columnWord += letter;
                        continue;
                    }
                    else if (Tiles[i, x].Letter != '\0') // Annars om brickan inte är tom, lägg till karaktären i ordet
                        columnWord += Tiles[i, x].Letter;
                }
            }
            else if(columnUp || columnDown) // Annars om det finns en bokstav över eller under Y
            {
                columnWord = "";
                if (columnDown) // Om man lägger bokstaven över en annan, lägg till karaktären i början av ordet
                    columnWord += letter;

                for (int i = 0; i < column.Length; i++) // Loopa igenom kolumnen och lägg till bokstäverna i ordet
                {
                    if (Tiles[i, x].Letter == '\0')
                    {
                        // Om I är mindre än tillagda bokstavens plats och ordets längd är över 1 (man har lagt till en bokstav från ett annat ord på brädan), sätt tillbaka ordet till placerade bokstaven
                        if (i < y && columnWord.Length > 1)
                            columnWord = letter.ToString();
                        else if (i > y) // Annars om man är på en tom bokstav och i är större än y, är ordet slut
                            break;
                        else
                            continue;
                    }
                    columnWord += Tiles[i, x].Letter;
                }

                if(columnUp) // Om man lägger bokstaven under en annan, lägg till karaktären i slutet på ordet
                    columnWord += letter;
            }

            bool rowLeft = (x - 1 < 0) ? false : (Tiles[y, x - 1].Letter != '\0');
            bool rowRight = (x + 1 >= XTiles) ? false : (Tiles[y, x + 1].Letter != '\0');
            string rowWord = null;

            if(rowLeft && rowRight)
            {
                rowWord = "";
                for(int i = 0; i < row.Length; i++)
                {
                    if(i == x)
                    {
                        rowWord += letter;
                        continue;
                    }
                    else if(Tiles[y, i].Letter != '\0')
                    {
                        rowWord += Tiles[y, i].Letter;
                    }
                }
            }
            else if(rowLeft || rowRight)
            {
                rowWord = "";
                if (rowRight)
                    rowWord += letter; 

                for(int i = 0; i < row.Length; i++)
                {
                    if (Tiles[y, i].Letter == '\0')
                    {
                        if (i < x && rowWord.Length > 1)
                            rowWord = letter.ToString();
                        else if (i > x)
                            break;
                        else
                            continue;
                    }
                    rowWord += Tiles[y, i].Letter;
                }

                if (rowLeft)
                    rowWord += letter;
            }
            System.Diagnostics.Debug.WriteLine("rowWord: " + rowWord + ";\n");
            System.Diagnostics.Debug.WriteLine("columnWord: " + columnWord + ";\n");

            if (columnWord != null && rowWord != null)
                PlacedValidWord = Dictionaries.IsWord(columnWord) && Dictionaries.IsWord(rowWord);
            else if (columnWord != null)
                PlacedValidWord = Dictionaries.IsWord(columnWord);
            else if (rowWord != null)
                PlacedValidWord = Dictionaries.IsWord(rowWord);
        }
    }
}
