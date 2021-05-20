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

            var boardWords = new Dictionary<string, Tuple<int, int, bool>>();

            foreach(Tile tile in Hand.Tiles)    
            {
                if (tile.Letter == '\0')
                    continue;

                hand += tile.Letter.ToString().ToLower();
            }

            for(int y = 0; y < YTiles; y++)
            {
                string xWord = "";
                string yWord = "";

                for (int x = 0; x < XTiles; x++)
                {
                    if(xWord.Length > 0 && Tiles[y, x].Letter == '\0')
                    {
                        System.Diagnostics.Debug.WriteLine(xWord);
                        boardWords[xWord.ToLower()] = new Tuple<int, int, bool>(y, x-1, true);
                        xWord = "";
                    }
                    if (yWord.Length > 0 && Tiles[x, y].Letter == '\0')
                    {
                        System.Diagnostics.Debug.WriteLine(yWord);
                        boardWords[yWord.ToLower()] = new Tuple<int, int, bool>(x-1, y, false);
                        yWord = "";
                    }
                    if (Tiles[y, x].Letter != '\0')
                    {
                        xWord += Tiles[y, x].Letter;

                    }
                    if (Tiles[x, y].Letter != '\0')
                    {
                        yWord += Tiles[x, y].Letter;
                    }
                }
            }
           

            // TODO: kolla så att ordens längd stämmer, till exempel att om ordet börjar på 0 kan man inte lägga en bokstav innan

            List<string> wordList = new List<string>();

            foreach(var words in Dictionaries.Current)
            {
                string word = words.Key;
                if(word.Length <= 1)
                    continue;

                foreach (var boardWord in boardWords)
                {
                    if (!word.Contains(boardWord.Key) || word == boardWord.Key)
                        continue;

                    string _hand = hand + boardWord.Key;
                    string _word = word;
                    int l = 0;

                    for (int i = 0; i < _hand.Length; i++)
                    {
                        int index = _word.IndexOf(_hand[i]);
                        
                        if (index != -1)
                        {
                            _word = _word.Remove(index, 1);
                            l++;
                        }

                        if (l >= word.Length && !wordList.Contains(word))
                        {
                            string[] splittedWord = word.Split(boardWord.Key);
                            if (boardWord.Value.Item1 - splittedWord[0].Length < 0 || boardWord.Value.Item1 + splittedWord[1].Length > XTiles)
                                continue;
                            if (boardWord.Value.Item1 - splittedWord[0].Length < 0 || boardWord.Value.Item1 + splittedWord[1].Length > YTiles)
                                continue;

                            var t = new List<Tuple<char, int, int>>();

                            if (boardWord.Value.Item3)
                            {
                                int left = splittedWord[0].Length - boardWord.Key.Length;
  
                                for (int x = 0; x < splittedWord[0].Length; x++)
                                {
                                    //if(left >= 0)
                                       // t.Add(new Tuple<char, int, int>(splittedWord[0][x], boardWord.Value.Item1, boardWord.Value.Item2 - splittedWord[0].Length + x));
                                    //else
                                     //   t.Add(new Tuple<char, int, int>(splittedWord[0][x], boardWord.Value.Item1, boardWord.Value.Item2 + 1 + x));
                                }
                                for (int x = 0; x < splittedWord[1].Length; x++)
                                {
                                   // t.Add(new Tuple<char, int, int>(splittedWord[1][x], boardWord.Value.Item1, boardWord.Value.Item2 + 1 + x));
                                }

                                //System.Diagnostics.Debug.WriteLine(word + ":" + boardWord + (boardWord.Value.Item3 ? " - From X" : " - From Y"));
                            }
                            else
                            {
                                // Funkar inte för y axeln, bara X, kommenterad X för testa Y
                                
                            }

                            wordList.Add(word);
                            
                        }
                    }
                }
            }

            sw.Stop();
            System.Diagnostics.Debug.WriteLine("ELAPSED: " + (sw.ElapsedMilliseconds / 1000).ToString());
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
