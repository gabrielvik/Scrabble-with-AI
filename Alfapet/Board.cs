﻿using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Alfapet
{
    class Board : Game
    {
        public static Tile[,] Tiles;

        public static float TilesMargin = 5;

        public static float TilesWidth;
        public static float TilesHeight;

        public static int XTiles = 15;
        public static int YTiles = 15;

        public static int TilesPlaced = 0;

        public static async void Initialize() // Bygger brädan, kallas i Initalize()
        {
            Tiles = new Tile[YTiles, XTiles];

            TilesWidth = (Alfapet._graphics.GraphicsDevice.Viewport.Width - ((XTiles + 1) * TilesMargin)) / XTiles;
            TilesHeight = (Alfapet._graphics.GraphicsDevice.Viewport.Height - (Hand.TilesHeight + ButtonRig.ButtonHeight) - ((YTiles + 1) * TilesMargin)) / YTiles;

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
                        Tiles[i, z].Letter = 'P';
                    else if (i == 2 && z == 3)
                        Tiles[i, z].Letter = 'A';

                    x += TilesWidth + TilesMargin;
                }
            }
        }

        public static void GetBestWords()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            string hand = "";

            foreach(Tile tile in Hand.Tiles)
            {
                if (tile.Letter == '\0')
                    continue;

                hand += tile.Letter.ToString().ToLower();
            }
           /* for(int y = 0; y < YTiles; y++)
            {

                for(int x = 0; x < XTiles; x++)
                {
                    if (Tiles[y, x].Letter != '\0')
                    {
                        if (Tiles[y, Math.Max(x - 1, 0)].Letter == '\0')
                        {
                            hand += Tiles[y, x].Letter;
                        }
                        if (Tiles[y, Math.Min(x - 1, XTiles - 1)].Letter == '\0')
                        {
                            hand += Tiles[y, x].Letter;
                        }
                        if (Tiles[Math.Max(y - 1, 0), x].Letter == '\0')
                        {
                            hand += Tiles[y, x].Letter;
                        }
                        if (Tiles[Math.Min(y + 1, YTiles - 1), x].Letter == '\0')
                        {
                            hand += Tiles[y, x].Letter;
                        }
                    }
                }
            }
           */
            System.Diagnostics.Debug.WriteLine(hand);

            foreach(var words in Dictionaries.Current)
            {
                string word = words.Key;
                if(word.Length <= 1)
                    continue;

                bool found = true;
                // TODO: gör att man jämför hand strängen men ordet istället för tvärtom
                for(int i = 0; i < hand.Length; i++)
                {
                    //System.Diagnostics.Debug.WriteLine(hand[i]);
                    if (!word.ToLower().Contains(hand[i]))
                        found = false;
                }
                if(found)
                {
                    System.Diagnostics.Debug.WriteLine(word);
                }
            }



            sw.Stop();
            System.Diagnostics.Debug.WriteLine("ELAPSED: " + (sw.ElapsedMilliseconds).ToString());
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
                    UI.DrawCenterText(Fonts.Montserrat_Bold_Smaller, tile.Letter.ToString(), new Vector2(tile.X, tile.Y), Color.White, (int)tile.W, (int)tile.H);
            }
        }



    }
}
