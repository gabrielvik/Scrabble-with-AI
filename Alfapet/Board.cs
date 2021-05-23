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

        public static List<List<Tuple<char, int, int>>> GetBestWords()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            string hand = "";

            var boardWords = new Dictionary<Tuple<int, int, bool>, string>();

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
                        boardWords[new Tuple<int, int, bool>(y, x - 1, true)] = xWord.ToLower();
                        xWord = "";
                    }
                    if (yWord.Length > 0 && Tiles[x, y].Letter == '\0')
                    {
                        System.Diagnostics.Debug.WriteLine(yWord);
                        boardWords[new Tuple<int, int, bool>(x - 1, y, false)] = yWord.ToLower();
                        yWord = "";
                    }
                    if (Tiles[y, x].Letter != '\0')
                    {
                        xWord += Tiles[y, x].Letter;

                        
                        // TODO: säkerställ att omringande bokstäver räknas som ord i både X och Y axlen
                    }
                    if (Tiles[x, y].Letter != '\0')
                    {
                        yWord += Tiles[x, y].Letter;
                    }
                }
            }
           

            // TODO: ord på en axel stämmer inte överens med den andra TEX AL på y axeln blir WL på x axeln, tror att bokstäver också kan sättas mitt i ord

            var t = new List<List<Tuple<char, int, int>>>();
            List<string> wordList = new List<string>();

            foreach (var words in Dictionaries.Current)
            {
                string word = words.Key;
                if(word.Length <= 1)
                    continue;

                var _t_ = new List<Tuple<char, int, int>>();

                foreach (var boardWord in boardWords)
                {
                    if (!word.Contains(boardWord.Value) || word == boardWord.Value)
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
                            string[] splittedWord = word.Split(boardWord.Value);
                            if (splittedWord[0].Length - boardWord.Key.Item2 + boardWord.Value.Length > 1 || boardWord.Key.Item2 + splittedWord[1].Length > XTiles)
                                continue;
                            if (splittedWord[0].Length - boardWord.Key.Item1 + boardWord.Value.Length > 1 || boardWord.Key.Item1 + splittedWord[1].Length > YTiles)
                                continue;



                            if (boardWord.Key.Item3)
                            {
                                int left = splittedWord[0].Length;
  
                                for (int x = 0; x < splittedWord[0].Length; x++)
                                {
                                    _t_.Add(new Tuple<char, int, int>(splittedWord[0][x], boardWord.Key.Item1, boardWord.Key.Item2 - boardWord.Value.Length + x));
                                }
                                for (int x = 0; x < splittedWord[1].Length; x++)
                                {
                                    _t_.Add(new Tuple<char, int, int>(splittedWord[1][x], boardWord.Key.Item1, boardWord.Key.Item2 + 1 + x));
                                }


                                if (t.Count <= 1)
                                {
                                    foreach (var _t in _t_)
                                    {
                                        //System.Diagnostics.Debug.WriteLine(_t.Item1 + ", Y: " + _t.Item2 + ", X: " + _t.Item3);
                                    }
                                    //System.Diagnostics.Debug.WriteLine(word + ":" + boardWord + (boardWord.Value.Item3 ? " - From X" : " - From Y"));
                                }
                            }
                            else
                            {
                                int left = splittedWord[0].Length;
                                for (int x = 0; x < splittedWord[0].Length; x++)
                                {
                                    _t_.Add(new Tuple<char, int, int>(splittedWord[0][x], boardWord.Key.Item1 + x - 1, boardWord.Key.Item2));
                                }
                                for (int x = 0; x < splittedWord[1].Length; x++)
                                {
                                    _t_.Add(new Tuple<char, int, int>(splittedWord[1][x], boardWord.Key.Item1 + 1 + x, boardWord.Key.Item2));
                                }

                                if (t.Count <= 1)
                                {
                                    foreach (var _t in _t_)
                                    {
                                        //System.Diagnostics.Debug.WriteLine(_t.Item1 + ", Y: " + _t.Item2 + ", X: " + _t.Item3);
                                    }
                                    //System.Diagnostics.Debug.WriteLine(word + ":" + boardWord + (boardWord.Value.Item3 ? " - From X" : " - From Y"));
                                }
                            }

                            wordList.Add(word);
                            t.Add(_t_);
                        }
                    }
                }
            }

            sw.Stop();
            System.Diagnostics.Debug.WriteLine("ELAPSED: " + (sw.ElapsedMilliseconds / 1000).ToString());
            return t;
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
