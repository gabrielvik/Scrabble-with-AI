using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Alfapet
{
    class Rounds : Game
    {
        public static int PlayerPoints;
        public static int AIPoints;
        public static int RoundNum;

        /*
         * Returnerar sant om brickorna är lagligt placerade och att alla ord placerade finns i ordboken
        */
        private static bool PlacedValidWords(bool ignoreIsolated = false)
        {
            var words = new List<string>();
            var xWord = "";
            var yWord = "";

            /*
             * Används för att räkna alla ord tillagda på brädan,
             * behöver inte vara riktiga ord utan räknar bara ord som är placerade
            */
            bool CorrectWordPlacement(int y, int x, bool axis)
            {
                // Referens till x eller y word variabeln beroende på axis
                ref var tempWord = ref axis ? ref xWord : ref yWord;
                if (Board.Tiles[y, x].Letter == '\0')
                {
                    // Går bak en bricka eftersom ordet slutade på förra brickan
                    if (axis)
                        x--;
                    else
                        y--;

                    // Om man inte går utanför brädan, kolla att bokstaven har åtmistonde en annan placerad brevid sig
                    if (!ignoreIsolated && tempWord.Length == 1 && x > 0 && x < Board.XTiles - 1 && y > 0 &&
                        y < Board.YTiles - 1)
                    {
                        var letterUp = Board.Tiles[y - 1, x];
                        var letterDown = Board.Tiles[y + 1, x];
                        var letterLeft = Board.Tiles[y, x - 1];
                        var letterRight = Board.Tiles[y, x + 1];

                        // Bokstaven får inte heller vara temporerat placerad
                        if ((letterUp.Letter == '\0' || letterUp.TempPlaced) &&
                            (letterDown.Letter == '\0' || letterDown.TempPlaced) &&
                            (letterLeft.Letter == '\0' || letterLeft.TempPlaced) &&
                            (letterRight.Letter == '\0' || letterRight.TempPlaced))
                            return false;
                    }
                    else if (tempWord.Length > 1) // Är ett lagligt ord
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

            // Kör funktionen för varje karaktär på X och Y axeln
            for (var y = 0; y < Board.XTiles; y++)
            {
                for (var x = 0; x < Board.YTiles; x++)
                {
                    if (!CorrectWordPlacement(y, x, true))
                        return false;

                    if (!CorrectWordPlacement(x, y, false))
                        return false;
                }
            }

            return words.All(Dictionaries.IsWord); // Returnerar om alla ord i listan finns i ordboken
        }

        /*
         * Kallas när användaren vill avsluta rundan
        */
        public static void DoMove(bool skip = false)
        {
            if (skip)
            {
                if (RoundNum > 0) // Användaren måste placera första ordet
                {
                    Notifications.AddMessage("You skipped this round");
                    Ai.DoMove();
                    RoundNum++;
                }

                return;
            }

            // Kolla inte om ordet är isolerat första rundan
            if (!PlacedValidWords(RoundNum <= 0))
            {
                ButtonRig.Buttons["move"].InvalidClick("Invalid Words");
                return;
            }

            RoundNum++;

            var notificationString = "You placed the letters (";
            var score = 0;

            // Lägg till poäng för varje bokstav och lägg till bokstaven i notifikation strängen
            Board.ResetTempTiles(tile =>
            {
                if (score != 0) // Lägg inte ett komma innan första bokstaven
                    notificationString += ", ";

                notificationString += tile.Letter;
                score += Config.CharacterPoints[tile.Letter];
            }, true);

            PlayerPoints += score;
            notificationString += ") for " + score + " points - Total score " + PlayerPoints;
            Notifications.AddMessage(notificationString);

            Board.TilesPlaced = 0;
            Hand.GiveNewLetters();

            Ai.DoMove();
        }
    }
}