using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alfapet
{
    class Rounds : Game
    {
        public static int PlayerPoints = 0;
        /*
         * Kollar att orden man placerat är kopplade till de andra orden
         * Om övre är sant, kolla också att orden är faktiska ord i nuvarande ordboken
         */
        public static bool PlacedValidWords()
        {
            List<string> words = new List<string>();
            string xWord = ""; // Temp variabel som lagrar ord
            string yWord = "";

            /*
             * Privat funktion till PlacedValidWords() 
             * Används för att kalkylera alla ord tillagda på brädan,
             * behöver inte vara riktiga ord utan räknar bara ord som är placerade
             */
            bool CorrectWordPlacement(int y, int x, bool axis)
            {
                ref string word = ref (axis ? ref xWord : ref yWord); 
                if (Board.Tiles[y, x].Letter == '\0')
                {
                    if (axis)
                        x--;
                    else
                        y--;

                    if (word.Length == 1)
                    {
                        char letterUp = Board.Tiles[Math.Max(y - 1, 0), x].Letter;
                        char letterDown = Board.Tiles[Math.Min(y + 1, Board.YTiles - 1), x].Letter;
                        char letterLeft = Board.Tiles[y, Math.Max(x - 1, 0)].Letter;
                        char letterRight = Board.Tiles[y, Math.Min(x + 1, Board.XTiles - 1)].Letter;
                        if (letterUp == '\0' && letterDown == '\0' && letterLeft == '\0' && letterRight == '\0')
                            return false;
                    }
                    else if(word.Length > 1)
                    {
                        words.Add(word);
                    }
                    word = "";
                }
                else
                {
                    word += Board.Tiles[y, x].Letter;
                }
                return true; 
            }

            for (int y = 0; y < Board.XTiles; y++) // Kollar ord på Y axeln
            {
                for (int x = 0; x < Board.YTiles; x++)
                {
                    if (!CorrectWordPlacement(y, x, true))
                        return false;

                    if (!CorrectWordPlacement(x, y, false))
                        return false;
                }
            }

            foreach (string _word in words)
            {
                if (!Dictionaries.IsWord(_word))
                    return false; // Så fort man hittar ett ogilitigt ord returna false, behöver inte kolla längre
            }
            return true;
        }

        /*
         * Kallas när användaren vill avsluta rundan
         * Säkerställer att man placerat riktiga ord
         * Om övre är sant, ger poäng or återställer variabler som behövs
         */
        public static async void DoMove(bool skip = false)
        {
            if (skip) // Måste ha placerat minst en bokstav
                return;

            else if (!PlacedValidWords())
            {
                ButtonRig.Buttons[0].InvalidClick("Invalid Words");
                return;
            }

            Board.TilesPlaced = 0;
            var words = Board.GetBestWords();

            if (words.Count <= 0)
            {
                for (int i = 0; i < Hand.Tiles.Length; i++) // Populera arrayen med nya objekt
                {
                    Hand.Tiles[i] = new Tile();
                    Hand.Tiles[i].Letter = Alfapet_Util.GenerateRandomLetter();
                }
            }
            var bestWord = new List<Tuple<char, int, int>>();
            int mostPoint = -1;
            foreach (var t in words)
            {
                bool placed = false;
                int score = 0;
                foreach (var _t in t)
                {
                    score += Alfapet_Config.CharactherPoints[char.ToUpper(_t.Item1)];
                }
                if (score > mostPoint)
                {
                    bestWord = t;
                    mostPoint = score;
                }
            }

            foreach (Tile tile in Board.Tiles)
            {
                if (!tile.TempPlaced || tile.Letter == '\0')
                    continue;

                tile.TempPlaced = false;
                await Task.Delay(150); // vänta 0.15s innan nästa loop så användaren kan se allting hända
            }

            await Task.Delay(new Random().Next(350, 1000));
            foreach (var t in bestWord)
            {
                Board.Tiles[t.Item2, t.Item3].Letter = char.ToUpper(t.Item1);
                Board.Tiles[t.Item2, t.Item3].TempPlaced = true;
                await Task.Delay(new Random().Next(500,1000));
            }

            /*for(int i = 0; i < 10; i++)
            {
                var ai = Board.GetBestWords();
                foreach (var t in ai)
                {
                    foreach (var _t in t)
                    {
                        Board.Tiles[_t.Item2, _t.Item3].Letter = char.ToUpper(_t.Item1);
                    }
                    break;
                }
                var player = Board.GetBestWords();
                foreach (var t in player)
                {
                    foreach (var _t in t)
                    {
                        Board.Tiles[_t.Item2, _t.Item3].Letter = char.ToUpper(_t.Item1);
                    }
                    break;
                }
            }*/

            int n = 0;
            foreach (Tile tile in Board.Tiles)
            {
                if (!tile.TempPlaced || tile.Letter == '\0')
                    continue;

                tile.TempPlaced = false;
                PlayerPoints += Alfapet_Config.CharactherPoints[tile.Letter];
                n += Alfapet_Config.CharactherPoints[tile.Letter];
                await Task.Delay(150); // vänta 0.15s innan nästa loop så användaren kan se allting hända
            }

            foreach(var tile in Hand.Tiles)
            {
                if (tile.Letter == '\0')
                    tile.Letter = Alfapet_Util.GenerateRandomLetter();
            }

            Hand.SetPositions();
            ButtonRig.Buttons[0].SetText("Skip");


            System.Diagnostics.Debug.WriteLine("Player placed score for: " + n);
            System.Diagnostics.Debug.WriteLine("AI placed score for: " + mostPoint);
        }
    }
}
