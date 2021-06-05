using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Alfapet
{
    class Rounds : Game
    {
        public static int PlayerPoints = 0;
        public static int AIPoints = 0;
        /*
         * Kollar att orden man placerat är kopplade till de andra orden
         * Om övre är sant, kolla också att orden är faktiska ord i nuvarande ordboken
        */
        private static bool PlacedValidWords()
        {
            var words = new List<string>();
            var xWord = "";
            var yWord = "";

            /*
             * Privat funktion till PlacedValidWords() 
             * Används för att kalkylera alla ord tillagda på brädan,
             * behöver inte vara riktiga ord utan räknar bara ord som är placerade
            */
            bool CorrectWordPlacement(int y, int x, bool axis)
            {
                ref var tempWord = ref axis ? ref xWord : ref yWord;
                if (Board.Tiles[y, x].Letter == '\0')
                {
                    if (axis)
                        x--;
                    else
                        y--;

                    if (tempWord.Length == 1)
                    {
                        var letterUp = Board.Tiles[Math.Max(y - 1, 0), x].Letter;
                        var letterDown = Board.Tiles[Math.Min(y + 1, Board.YTiles - 1), x].Letter;
                        var letterLeft = Board.Tiles[y, Math.Max(x - 1, 0)].Letter;
                        var letterRight = Board.Tiles[y, Math.Min(x + 1, Board.XTiles - 1)].Letter;
                    
                        if (letterUp == '\0' && letterDown == '\0' && letterLeft == '\0' && letterRight == '\0')
                            return false;
                    }
                    else if (tempWord.Length > 1)
                    {
                        words.Add(tempWord);
                    }
                    tempWord = "";
                }
                else
                {
                    tempWord += Board.Tiles[y, x].Letter;
                }
                return true;
            }

            for (var y = 0; y < Board.XTiles; y++) // Kollar ord på Y axeln
            {
                for (var x = 0; x < Board.YTiles; x++)
                {
                    if (!CorrectWordPlacement(y, x, true))
                        return false;

                    if (!CorrectWordPlacement(x, y, false))
                        return false;
                }
            }
            return words.All(Dictionaries.IsWord);
        }

        /*
         * Kallas när användaren vill avsluta rundan
         * Säkerställer att man placerat riktiga ord
         * Om övre är sant, ger poäng och återställer variabler som behövs
         */
        public static void DoMove(bool skip = false)
        {
            if (skip) // Måste ha placerat minst en bokstav
                return;

            if (!PlacedValidWords())
            {
                ButtonRig.Buttons[0].InvalidClick("Invalid Words");
                return;
            }

            var score = 0;

            Board.ResetTempTiles(tile =>
            {
                score += AlfapetConfig.CharacterPoints[tile.Letter];
            });
            PlayerPoints += score;

            Hand.GiveNewLetters();
            Board.TilesPlaced = 0;

            Ai.DoMove();

            Debug.WriteLine("Player placed score for: " + score);
        }
    }
}
