using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Alfapet
{
    class Ai : Game
    {
        private struct Word
        {
            public int? YEnd { get; set; }
            public int? XStart { get; set; }
            public int? YStart { get; set; }
            public int? XEnd { get; set; }

            public bool Axis { get; set; }

            public string Value { get; set; }
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

            for (int y = 0; y < Board.YTiles; y++)
            {
                string xWord = "";
                string yWord = "";

                int xStart = -1;
                int yStart = -1;

                for (int x = 0; x < Board.XTiles; x++)
                {
                    if (xWord.Length > 0 && Board.Tiles[y, x].Letter == '\0')
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
                    if (yWord.Length > 0 && Board.Tiles[x, y].Letter == '\0')
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
                    if (Board.Tiles[y, x].Letter != '\0')
                    {
                        if (xWord.Length <= 0)
                            xStart = x;

                        xWord += Board.Tiles[y, x].Letter;
                    }
                    if (Board.Tiles[x, y].Letter != '\0')
                    {
                        if (yWord.Length <= 0)
                            yStart = x;

                        yWord += Board.Tiles[x, y].Letter;
                    }
                }
            }

            var wordPlacements = new List<List<Tuple<char, int, int>>>();
            List<string> wordList = new List<string>();

            sw.Stop();

            System.Diagnostics.Debug.WriteLine("ELAPSED0: " + (sw.ElapsedMilliseconds).ToString());
            sw = System.Diagnostics.Stopwatch.StartNew();


            foreach (var word in Dictionaries.GetWordList())
            {
                bool test = false;
                foreach (var boardWord in boardWords)
                {
                    if (test)
                        break;

                    if (!word.Contains(boardWord.Value) || word == boardWord.Value)
                        continue;

                    string tempHand = hand + boardWord.Value;
                    string tempWord = word;
                    int l = 0;

                    for (int i = 0; i < tempHand.Length; i++)
                    {
                        int index = tempWord.IndexOf(tempHand[i]);

                        if (index != -1)
                        {
                            tempWord = tempWord.Remove(index, 1);
                            l++;
                        }

                        if (l >= word.Length)
                        {
                            string[] splittedWord = word.Split(boardWord.Value);
                            if (boardWord.XEnd - (splittedWord[0].Length + boardWord.Value.Length) < 0 || boardWord.XEnd + (splittedWord[1].Length + boardWord.Value.Length) > Board.XTiles - 1)
                                continue;
                            if (boardWord.YEnd - (splittedWord[0].Length + boardWord.Value.Length) < 0 || boardWord.YEnd + (splittedWord[1].Length + boardWord.Value.Length) > Board.YTiles - 1)
                                continue;

                            var wordPlacement = new List<Tuple<char, int, int>>();

                            bool invalid = false;

                            if (boardWord.Axis)
                            {
                                for (int x = 0; x < splittedWord[0].Length; x++)
                                {
                                    var _x = boardWord.XStart - splittedWord[0].Length + x;

                                    string upWord = boardWords.Where(wordObj => wordObj.XEnd == _x && wordObj.YEnd == boardWord.YEnd - 1)
                                        .Select(wordObj => wordObj.Value)
                                        .FirstOrDefault();

                                    if (upWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Board.Tiles[(int)boardWord.YEnd, (int)_x].Letter + upWord))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    string downWord = boardWords.Where(wordObj => wordObj.XEnd == _x && wordObj.YEnd == boardWord.YEnd + 1)
                                        .Select(wordObj => wordObj.Value)
                                        .FirstOrDefault();

                                    if (downWord != null)
                                    {
                                        if (!Dictionaries.IsWord(downWord + Board.Tiles[(int)boardWord.YEnd, (int)_x].Letter))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    if (Board.Tiles[(int)boardWord.YEnd, Math.Max((int)_x - 1, 0)].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }

                                    wordPlacement.Add(new Tuple<char, int, int>(splittedWord[0][x], (int)boardWord.YEnd, (int)_x));
                                }
                                for (int x = 0; x < splittedWord[1].Length; x++)
                                {
                                    var _x = boardWord.XEnd + 1 + x;

                                    string upWord = boardWords.Where(wordObj => wordObj.XEnd == _x && wordObj.YEnd == boardWord.YEnd - 1)
                                        .Select(wordObj => wordObj.Value)
                                        .FirstOrDefault();

                                    if (upWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Board.Tiles[(int)boardWord.YEnd, (int)_x].Letter + upWord))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    string downWord = boardWords.Where(wordObj => wordObj.XEnd == _x && wordObj.YStart == boardWord.YEnd + 1)
                                        .Select(wordObj => wordObj.Value)
                                        .FirstOrDefault();

                                    if (downWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Board.Tiles[(int)boardWord.YEnd, (int)_x].Letter + downWord))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    if (Board.Tiles[(int)boardWord.YEnd, Math.Min((int)_x + 1, Board.XTiles - 1)].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                    wordPlacement.Add(new Tuple<char, int, int>(splittedWord[1][x], (int)boardWord.YEnd, (int)_x));
                                }
                            }
                            else
                            {
                                for (int x = 0; x < splittedWord[0].Length; x++)
                                {
                                    var y = (int)boardWord.YStart - splittedWord[0].Length + x;

                                    string leftWord = boardWords.Where((wordObj) => wordObj.XEnd == boardWord.XEnd - 1 && wordObj.YEnd == y)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (leftWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Board.Tiles[(int)y, (int)boardWord.XEnd].Letter + leftWord))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    string rightWord = boardWords.Where((wordObj) => wordObj.XStart == boardWord.XEnd + 1 && wordObj.YEnd == y)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (rightWord != null)
                                    {
                                        if (!Dictionaries.IsWord(rightWord + Board.Tiles[(int)y, (int)boardWord.XEnd].Letter))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }
                                    if (Board.Tiles[(int)Math.Max((int)y - 1, 0), (int)boardWord.XEnd].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }

                                    wordPlacement.Add(new Tuple<char, int, int>(splittedWord[0][x], (int)y, (int)boardWord.XEnd));
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
                                        if (!Dictionaries.IsWord(Board.Tiles[(int)y, (int)boardWord.XEnd].Letter + leftWord))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    string rightWord = boardWords.Where((wordObj) => wordObj.XStart == boardWord.XEnd + 1 && wordObj.YEnd == y)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (rightWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Board.Tiles[(int)y, (int)boardWord.XEnd].Letter + rightWord))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    if (Board.Tiles[(int)Math.Min((int)y + 1, (int)Board.YTiles - 1), (int)boardWord.XEnd].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }

                                    wordPlacement.Add(new Tuple<char, int, int>(splittedWord[1][x], (int)boardWord.YEnd + 1 + x, (int)boardWord.XEnd));
                                }
                            }

                            if (!invalid)
                            {
                                wordList.Add(word);
                                wordPlacements.Add(wordPlacement);

                                sw.Stop();
                                test = true;
                                break;
                            }

                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("ELAPSED1: " + (sw.ElapsedMilliseconds).ToString());

            return wordPlacements;
        }
    }
}
