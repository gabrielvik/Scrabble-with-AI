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
            string word = ""; // Temp variabel som lagrar ord

            /*
             * Privat funktion till PlacedValidWords() 
             * Används för att kalkylera alla ord tillagda på brädan,
             * behöver inte vara riktiga ord utan räknar bara ord som är placerade
             */
            void CorrectWordPlacement(int y, int x, out bool ok)
            {
                ok = true;
                if (Board.Tiles[y, x].Letter == '\0')
                {
                    if (word.Length == 1) // Om ordet är bara en karaktär, kolla så den inte är isolerad från andra karaktärer och om den är, returna false
                    {
                        char letterUp = Board.Tiles[Math.Max(y - 1, 0), x].Letter;
                        char letterDown = Board.Tiles[Math.Min(y + 1, Board.YTiles - 1), x].Letter;
                        char letterLeft = Board.Tiles[y, Math.Max(x - 1, 0)].Letter;
                        char letterRight = Board.Tiles[y, Math.Max(x - 1, 0)].Letter;
                        if (letterUp == '\0' && letterDown == '\0' && letterLeft == '\0' && letterRight == '\0')
                            ok = false;
                    }
                    else if (word.Length > 1)
                    {
                        words.Add(word); // Annars om det är ett komplett ord (2 eller mer karaktärer), lägg till i listan
                    }
                    word = ""; // töm temp variabeln
                }
                else
                {
                    word += Board.Tiles[y, x].Letter;
                }
            }

            for (int x = 0; x < Board.XTiles; x++) // Kollar ord på Y axeln
            {
                for (int y = 0; y < Board.YTiles; y++)
                {
                    CorrectWordPlacement(y, x, out bool ok); // Gör en lokal bool till loopen, om inkorrekt placering, returna false
                    if (!ok)
                    {
                        System.Diagnostics.Debug.WriteLine("here");
                        return false;
                    }
                }
            }

            for (int y = 0; y < Board.YTiles; y++) // Samma som förra men kollar på X axeln
            {
                for (int x = 0; x < Board.XTiles; x++)
                {
                    CorrectWordPlacement(y, x, out bool ok);
                    if (!ok)
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

            else if (true)
            {
                ButtonRig.Buttons[0].InvalidClick("Invalid Words");
                return;
            }

            Board.TilesPlaced = 0;

            foreach (var t in Board.GetBestWords())
            {
                bool placed = false;
                foreach (var _t in t)
                {
                    Board.Tiles[_t.Item2, _t.Item3].Letter = char.ToUpper(_t.Item1);
                    placed = true;
                }
                if(placed)
                 break;
            }

            foreach (Tile tile in Board.Tiles)
            {
                if (!tile.TempPlaced || tile.Letter == '\0')
                    continue;

                tile.TempPlaced = false;
                PlayerPoints += Alfapet_Config.CharactherPoints[tile.Letter];
                await Task.Delay(150); // vänta 0.15s innan nästa loop så användaren kan se allting hända
            }

            foreach(var tile in Hand.Tiles)
            {
                if (tile.Letter == '\0')
                    tile.Letter = Alfapet_Util.GenerateRandomLetter();
            }

            Hand.SetPositions();
            ButtonRig.Buttons[0].SetText("Skip");


            System.Diagnostics.Debug.WriteLine(PlayerPoints);
        }
    }
}
