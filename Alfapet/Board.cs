using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private struct Word
        {
            public int? YEnd { get; set; }
            public int? XStart { get; set; }
            public int? YStart { get; set; }
            public int? XEnd { get; set; }

            public bool Axis { get; set; }

            public string Value { get; set; }
        }
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
                    if (i == 4 && z == 7)
                        Tiles[i, z].Letter = 'P';
                    else if (i == 5 && z == 7)
                        Tiles[i, z].Letter = 'A';
                    else if (i == 6 && z == 7)
                        Tiles[i, z].Letter = 'Y';

                    x += TilesWidth + TilesMargin;
                }
            }
        }

        public static List<List<Tuple<char, int, int>>> GetBestWords()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            string hand = "";

            var boardWords = new List<Word>();

            foreach (Tile tile in Hand.Tiles)
            {
                if (tile.Letter == '\0')
                    continue;

                hand += tile.Letter.ToString().ToLower();
            }

            for (int y = 0; y < YTiles; y++)
            {
                string xWord = "";
                string yWord = "";

                int xStart = -1;
                int yStart = -1;

                for (int x = 0; x < XTiles; x++)
                {
                    if (xWord.Length > 0 && Tiles[y, x].Letter == '\0')
                    {
                        //System.Diagnostics.Debug.WriteLine(xWord);
                        var xWordObj = new Word()
                        {
                            YEnd = y,
                            XEnd = x - 1,
                            XStart = xStart,
                            Axis = true,
                            Value = xWord.ToLower()
                        };
                        boardWords.Add(xWordObj);
                        xWord = "";
                    }
                    if (yWord.Length > 0 && Tiles[x, y].Letter == '\0')
                    {
                        //System.Diagnostics.Debug.WriteLine(yWord);
                        var yWordObj = new Word()
                        {
                            YEnd = x - 1,
                            YStart = yStart,
                            XEnd = y,
                            Axis = false,
                            Value = yWord.ToLower()
                        };
                        boardWords.Add(yWordObj);

                        yWord = "";
                    }
                    if (Tiles[y, x].Letter != '\0')
                    {
                        if (xWord.Length <= 0)
                            xStart = x;

                        xWord += Tiles[y, x].Letter;
                    }
                    if (Tiles[x, y].Letter != '\0')
                    {
                        if (yWord.Length <= 0)
                            yStart = x;

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
                if (word.Length <= 1)
                    continue;

                foreach (var boardWord in boardWords)
                {
                    if (!word.Contains(boardWord.Value) || word == boardWord.Value)
                        continue;

                    string _hand = hand + boardWord.Value;
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
                            if (boardWord.XEnd - (splittedWord[0].Length + boardWord.Value.Length) < 0 || boardWord.XEnd + (splittedWord[1].Length + boardWord.Value.Length) > XTiles-1)
                                continue;
                            if (boardWord.YEnd - (splittedWord[0].Length + boardWord.Value.Length) < 0 || boardWord.YEnd + (splittedWord[1].Length + boardWord.Value.Length) > YTiles-1)
                                continue;

                            var _t_ = new List<Tuple<char, int, int>>();

                            bool fuck = false;

                            if (boardWord.Axis)
                            {
                                for (int x = 0; x < splittedWord[0].Length; x++)
                                {
                                    var _x = boardWord.XEnd - boardWord.Value.Length + x;

                                    string leftWord = boardWords.Where((wordObj) => wordObj.XEnd == _x && wordObj.YEnd == boardWord.YEnd - 1)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (leftWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Tiles[(int)boardWord.YEnd, (int)_x].Letter + leftWord))
                                        {
                                            fuck = true;
                                            break;
                                        }
                                    }

                                    string rightWord = boardWords.Where((wordObj) => wordObj.XStart == _x && wordObj.YStart == boardWord.YEnd + 1)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (rightWord != null)
                                    {
                                        if (!Dictionaries.IsWord(rightWord + Tiles[(int)boardWord.YEnd, (int)_x].Letter))
                                        {
                                            fuck = true;
                                            break;
                                        }
                                    }

                                    if (Tiles[(int)boardWord.YEnd, (int)Math.Min((int)boardWord.XEnd - splittedWord[0].Length + x + 1, XTiles - 1)].Letter != '\0')
                                    {
                                        fuck = true;
                                        break;
                                    }

                                    _t_.Add(new Tuple<char, int, int>(splittedWord[0][x], (int)boardWord.YEnd, (int)boardWord.XEnd - splittedWord[0].Length + x));
                                }
                                for (int x = 0; x < splittedWord[1].Length; x++)
                                {
                                    var _x = boardWord.XEnd + 1 + x;

                                    string leftWord = boardWords.Where((wordObj) => wordObj.XEnd == _x && wordObj.YEnd == boardWord.YEnd - 1)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (leftWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Tiles[(int)boardWord.YEnd, (int)_x].Letter + leftWord))
                                        {
                                            fuck = true;
                                            break;
                                        }
                                    }

                                    string rightWord = boardWords.Where((wordObj) => wordObj.XEnd == _x && wordObj.YStart == boardWord.YEnd + 1)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (rightWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Tiles[(int)boardWord.YEnd, (int)_x].Letter + rightWord))
                                        {
                                            fuck = true;
                                            break;
                                        }
                                    }

                                    if (Tiles[(int)boardWord.YEnd, (int)Math.Min((int)boardWord.XEnd + 1 + x + 1, XTiles - 1)].Letter != '\0')
                                    {
                                        fuck = true;
                                        break;
                                    }
                                    _t_.Add(new Tuple<char, int, int>(splittedWord[1][x], (int)boardWord.YEnd, (int)boardWord.XEnd + 1 + x));
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
                                for (int x = 0; x < splittedWord[0].Length; x++)
                                {
                                    var y = (int)boardWord.YEnd - boardWord.Value.Length + x;

                                    string leftWord = boardWords.Where((wordObj) => wordObj.XEnd == boardWord.XEnd - 1 && wordObj.YEnd == y)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (leftWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Tiles[(int)y, (int)boardWord.XEnd].Letter + leftWord))
                                        {
                                            fuck = true;
                                            break;
                                        }
                                    }

                                    string rightWord = boardWords.Where((wordObj) => wordObj.XStart == boardWord.XEnd + 1 && wordObj.YEnd == y)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (rightWord != null)
                                    {
                                        if (!Dictionaries.IsWord(rightWord + Tiles[(int)y, (int)boardWord.XEnd].Letter))
                                        {
                                            fuck = true;
                                            break;
                                        }
                                    }
                                    if (Tiles[(int)Math.Min((int)y + 1, (int)YTiles - 1), (int)boardWord.XEnd].Letter != '\0')
                                    {
                                        fuck = true;
                                        break;
                                    }

                                    _t_.Add(new Tuple<char, int, int>(splittedWord[0][x], (int)y, (int)boardWord.XEnd));
                                    //System.Diagnostics.Debug.WriteLine("Adds here 0" + word);
                                }
                                for (int x = 0; x < splittedWord[1].Length; x++)
                                {
                                    //System.Diagnostics.Debug.WriteLine("Adds here 1"+word);
                                    var y = boardWord.YEnd + 1 + x;
                                    string leftWord = boardWords.Where((wordObj) => wordObj.XEnd == boardWord.XEnd - 1 && wordObj.YEnd == y)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (leftWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Tiles[(int)y, (int)boardWord.XEnd].Letter + leftWord))
                                        {
                                            fuck = true;
                                            break;
                                        }
                                    }

                                    string rightWord = boardWords.Where((wordObj) => wordObj.XStart == boardWord.XEnd + 1 && wordObj.YEnd == y)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (rightWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Tiles[(int)y, (int)boardWord.XEnd].Letter + rightWord))
                                        {
                                            fuck = true;
                                            break;
                                        }
                                    }

                                    if (Tiles[(int)Math.Min((int)y + 1, (int)YTiles - 1), (int)boardWord.XEnd].Letter != '\0')
                                    {
                                        fuck = true;
                                        break;
                                    }

                                    _t_.Add(new Tuple<char, int, int>(splittedWord[1][x], (int)boardWord.YEnd + 1 + x, (int)boardWord.XEnd));
                                }
                            }

                            if (!fuck)
                            {
                                wordList.Add(word);
                                t.Add(_t_);
                            }
                        }
                    }
                }
            }

            sw.Stop();
            System.Diagnostics.Debug.WriteLine("ELAPSED: " + (sw.ElapsedMilliseconds).ToString());
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
