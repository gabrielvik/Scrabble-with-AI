using Microsoft.Xna.Framework;
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

            List<char> boardWords = new List<char>();

            foreach(Tile tile in Hand.Tiles)
            {
                if (tile.Letter == '\0')
                    continue;

                hand += tile.Letter.ToString().ToLower();
            }

            for(int y = 0; y < YTiles; y++)
            {
                for (int x = 0; x < XTiles; x++)
                {
                    char letterUp = Tiles[Math.Max(y - 1, 0), x].Letter;
                    char letterDown = Tiles[Math.Min(y + 1, Board.YTiles - 1), x].Letter;
                    char letterLeft = Tiles[y, Math.Max(x - 1, 0)].Letter;
                    char letterRight = Tiles[y, Math.Max(x - 1, 0)].Letter;

                    if (letterUp != '\0' || letterDown != '\0' || letterLeft != '\0' || letterRight != '\0')
                        boardWords.Add(Tiles[y, x].Letter);
                }
            }
           
            System.Diagnostics.Debug.WriteLine(hand);

            List<string> wordList = new List<string>();

            foreach(var words in Dictionaries.Current)
            {
                string word = words.Key;
                if(word.Length <= 1)
                    continue;

                foreach (var boardWord in boardWords)
                {
                    bool found = true;
                    string _hand = hand + boardWord.ToString().ToLower();

                    for (int i = 0; i < word.Length; i++)
                    {
                        //System.Diagnostics.Debug.WriteLine(hand[i]);
                        int index = _hand.IndexOf(word[i]);

                        if (index == -1)
                        {
                            found = false;
                            break;
                        }
                        else
                            _hand = _hand.Remove(index);
                    }
                    if (found && !wordList.Contains(word))
                    {
                        wordList.Add(word);
                        System.Diagnostics.Debug.WriteLine(word);
                        continue;
                    }
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
