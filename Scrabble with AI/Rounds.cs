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
         * Returns true if the tiles are placed legally and all placed words exist in the dictionary
         */
        private static bool PlacedValidWords(bool ignoreIsolated = false)
        {
            var words = new List<string>();
            var xWord = "";
            var yWord = "";

            /*
             * Used to count all words added to the board,
             * doesn't need to be real words, just counts placed words
             */
            bool CorrectWordPlacement(int y, int x, bool axis)
            {
                // Reference to x or y word variable depending on axis
                ref var tempWord = ref axis ? ref xWord : ref yWord;
                if (Board.Tiles[y, x].Letter == '\0')
                {
                    // Move back one tile since the word ended on the previous tile
                    if (axis)
                        x--;
                    else
                        y--;

                    // If not going off the board, check that the letter has at least another placed beside it
                    if (!ignoreIsolated && tempWord.Length == 1 && x > 0 && x < Board.XTiles - 1 && y > 0 &&
                        y < Board.YTiles - 1)
                    {
                        var letterUp = Board.Tiles[y - 1, x];
                        var letterDown = Board.Tiles[y + 1, x];
                        var letterLeft = Board.Tiles[y, x - 1];
                        var letterRight = Board.Tiles[y, x + 1];

                        // The letter must also not be temporarily placed
                        if ((letterUp.Letter == '\0' || letterUp.TempPlaced) &&
                            (letterDown.Letter == '\0' || letterDown.TempPlaced) &&
                            (letterLeft.Letter == '\0' || letterLeft.TempPlaced) &&
                            (letterRight.Letter == '\0' || letterRight.TempPlaced))
                            return false;
                    }
                    else if (tempWord.Length > 1) // Is a valid word
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

            // Run the function for each character on the X and Y axis
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

            return words.All(Dictionaries.IsWord); // Returns if all words in the list are in the dictionary
        }

        /*
         * Called when the user wants to end the round
         */
        public static void DoMove(bool skip = false)
        {
            if (skip)
            {
                if (RoundNum > 0) // User must place the first word
                {
                    Notifications.AddMessage("You skipped this round");
                    Ai.DoMove();
                    RoundNum++;
                }

                return;
            }

            // Don't check if the word is isolated in the first round
            if (!PlacedValidWords(RoundNum <= 0))
            {
                ButtonRig.Buttons["move"].InvalidClick("Invalid Words");
                return;
            }

            RoundNum++;

            var notificationString = "You placed the letters (";
            var score = 0;

            // Add points for each letter and add the letter to the notification string
            Board.ResetTempTiles(tile =>
            {
                if (score != 0) // Don't add a comma before the first letter
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
