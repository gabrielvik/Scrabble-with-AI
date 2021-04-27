using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alfapet
{
    class Rounds : Game
    {
        public static bool PlacedValidWords()
        {
            // TODO:::
            List<string> words = new List<string>();

            for (int x = 0; x < Board.XTiles; x++)
            {
                string _word = "";
                for (int y = 0; y < Board.YTiles; y++)
                {
                    if (Board.Tiles[y, x].Letter == '\0')
                    {
                        if (_word.Length == 1)
                        {
                            if (Board.Tiles[y, x + 1].Letter == '\0' || Board.Tiles[y, x - 1].Letter == '\0')
                                continue;
                        }
                        else if (_word.Length > 0)
                        {
                            words.Add(_word);
                        }
                        _word = "";
                    }
                    else
                        _word += Board.Tiles[y, x].Letter;
                }
            }
            for (int y = 0; y < Board.YTiles; y++)
            {
                string _word = "";
                for (int x = 0; x < Board.XTiles; x++)
                {
                    if (Board.Tiles[y, x].Letter == '\0')
                    {
                        if (_word.Length == 1)
                        {
                            if (Board.Tiles[y + 1, x].Letter == '\0' || Board.Tiles[y - 1, x].Letter == '\0')
                                continue;
                        }
                        else if (_word.Length > 0)
                        {
                            words.Add(_word);
                        }
                        _word = "";
                    }
                    else
                        _word += Board.Tiles[y, x].Letter;
                }
            }

            foreach (string word in words)
            {
                if (!Dictionaries.IsWord(word))
                    return false;
            }
            return true;
        }
        public static void DoMove()
        {
            if (!PlacedValidWords())
            {
                System.Diagnostics.Debug.WriteLine("didnt place valid");
                return;
            }

            foreach(Tile tile in Board.Tiles)
            {
                if (tile.TempPlaced)
                    tile.TempPlaced = false;
            }
        }
    }
}
