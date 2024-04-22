using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Alfapet
{
    class Ai : Game
    {
        public static bool Playing;
        private static readonly char[] Letters = new char[Hand.Amount];
        private struct Word
        {
            public int YStart { get; set; } // Where the word starts on the Y axis
            public int YEnd { get; set; } // Where the word ends on the Y axis
            public int XStart { get; set; } // Where the word starts on the X axis
            public int XEnd { get; set; } // Where the word ends on the X axis
            public bool Axis { get; set; } // If it's on X or Y. True for X and false for Y
            public string Value { get; set; } // What the word is
        }

        /*
         * Sets all characters in the hand to new random letters
        */
        private static void GenerateHand()
        {
            for (var i = 0; i < Letters.Length; i++)
            {
                Letters[i] = Util.GenerateRandomLetter();
            }
        }

        public new static void Initialize()
        {
            GenerateHand();
        }

        /*
        * Called when AI is making its move, places tiles and provides points
        */
        public static async void DoMove()
        {
            Playing = true;
            // Wait until the text changes before transitioning to finding words
            await Task.Run(() =>
            {
                ButtonRig.Buttons["move"].SetText("Opponent Thinking");
            });

            var words = GetPlacableWords();

            // Wait for a random time between 1s-3s before proceeding to make it more realistic for the user
            await Task.Delay(new Random().Next(1000, 3000));

            ButtonRig.Buttons["move"].SetText("Opponent Playing"); // "Thinking" done

            var bestWord = new List<Tuple<char, int, int>>();
            var mostPoint = 0;
            foreach (var word in words) // Calculate which word is worth the most points
            {
                // Sum of all letters in the word based on points in config
                var tempScore = word.Sum(character => Config.CharacterPoints[character.Item1]);

                if (tempScore > mostPoint)
                {
                    bestWord = word;
                    mostPoint = tempScore;
                }
            }

            var score = 0;
            var notificationString = "Opponent placed the letters (";

            // Place the best word found on the board while adding points and letters
            foreach (var (letter, y, x) in bestWord)
            {
                // Replace with a new random character
                Letters[Array.IndexOf(Letters, char.ToUpper(letter))] = Util.GenerateRandomLetter();

                if (score != 0) // Do not add a comma for the first letter
                    notificationString += ", ";

                notificationString += letter;

                Board.Tiles[y, x].Letter = letter;
                Board.Tiles[y, x].TempPlaced = true;

                score += Config.CharacterPoints[letter];
                await Task.Delay(new Random().Next(500, 1250)); // Wait 0.5-1.25s before placing the next character
            }
            if (score <= 0)
            {
                Notifications.AddMessage("Opponent skipped this round");
                // Give Ai a new hand so it might place a word next round
                GenerateHand();
            }
            else
            {
                Rounds.AIPoints += score;

                notificationString += ") for " + score + " points - Total score " + Rounds.AIPoints;
                Notifications.AddMessage(notificationString);
            }

            Board.ResetTempTiles();
            Board.CheckTilesPlaced();

            Playing = false;
        }

        /*
         * Returns a string of Ai's hand
        */
        private static string GetHandString()
        {
            var handString = "";
            foreach (var tile in Letters)
            {
                if (tile == '\0')
                    continue;

                handString += tile;
            }
            return handString;
        }

        /*
         * Returns a list of all words on the board in a Word structure
        */
        private static List<Word> GetBoardWords()
        {
            var boardWords = new List<Word>();
            for (var y = 0; y < Board.YTiles; y++)
            {
                var xWord = "";
                var yWord = "";

                var xStart = -1;
                var yStart = -1;

                for (var x = 0; x < Board.XTiles; x++)
                {
                    // An existing word has ended because the tile is empty on the X axis
                    if (xWord.Length > 0 && Board.Tiles[y, x].Letter == '\0')
                    {
                        var xWordObj = new Word
                        {
                            YStart = y,
                            YEnd = y,
                            XStart = xStart,
                            XEnd = x - 1, // Minus one because the word ended on the previous tile
                            Axis = true,
                            Value = xWord.ToLower()
                        };
                        boardWords.Add(xWordObj);
                        xWord = "";
                    }

                    // An existing word has ended because the tile is empty on the Y axis
                    if (yWord.Length > 0 && Board.Tiles[x, y].Letter == '\0')
                    {
                        var yWordObj = new Word
                        {
                            YStart = yStart,
                            YEnd = x - 1, // Minus one because the word ended on the previous tile
                            XStart = y,
                            XEnd = y,
                            Axis = false,
                            Value = yWord.ToLower()
                        };
                        boardWords.Add(yWordObj);

                        yWord = "";
                    }
                    if (Board.Tiles[y, x].Letter != '\0')
                    {
                        if (xWord.Length <= 0) // Start of the word
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
            return boardWords;
        }

        /*
         * Finds all words that can be placed on the board.
         * Returns a list with a list containing information of all characters that can be placed
         * Tuple<char, int, int> - Item1 is the character to be placed,
         * Item2 is the Y axis where it should be placed, Item3 is the X axis where it should be placed
        */
        private static List<List<Tuple<char, int, int>>> GetPlacableWords()
        {
            var hand = GetHandString().ToLower();
            var boardWords = GetBoardWords();
            var dictionaryList = Dictionaries.GetWordList();
            var wordPlacements = new List<List<Tuple<char, int, int>>>();

            foreach (var word in dictionaryList)
            {
                var foundWord = false;
                // Loop through all words on the board that the word string contains
                foreach (var boardWord in boardWords.Where(boardWord => word.Contains(boardWord.Value)))
                {
                    if (foundWord) // Make a difference if the same word is found multiple times
                        break;

                    var tempHand = hand + boardWord.Value;
                    var tempWord = word;

                    var match = 0;

                    var boardWordIndex = word.IndexOf(boardWord.Value, StringComparison.Ordinal);
                    var firstHalf = word[..boardWordIndex]; // Range from the beginning to the position of the board word in the word
                    var secondHalf = word[boardWordIndex..]; // Range from the position of the board word in the word to the end

                    // Check if the word has enough space on the board
                    if (boardWord.Axis)
                    {
                        if (boardWord.XStart - firstHalf.Length < 0)
                            break;

                        // Minus the length of boardWord because boardWord is included in the range
                        if (boardWord.XEnd + secondHalf.Length - boardWord.Value.Length > Board.XTiles - 1)
                            break;
                    }
                    else
                    {
                        if (boardWord.YStart - firstHalf.Length < 0)
                            break;

                        if (boardWord.YEnd + secondHalf.Length - boardWord.Value.Length > Board.YTiles - 1)
                            break;
                    }

                    foreach (var index in tempHand.Select(handChar => tempWord.IndexOf(handChar))) // Position of the character in the hand in the word 
                    {
                        if (index != -1) // The character is in the word
                        {
                            tempWord = tempWord.Remove(index, 1); // Remove the character from the string so it can't be used again
                            match++;
                        }

                        if (match < word.Length) continue; // Not a match yet

                        var wordPlacement = new List<Tuple<char, int, int>>();
                        var invalid = false;

                        var separated = false;
                        var separatedIterator = 0;
                        if (boardWord.Axis)
                        {
                            for (var x = 0; x < word.Length; x++)
                            {
                                // Reached the position of the boardWord, separate the word
                                if (boardWordIndex == x)
                                {
                                    // Skip all characters in boardWord (minus one to continue to the next)
                                    x += boardWord.Value.Length - 1;

                                    separated = true;
                                    continue;
                                }

                                int boardX; // X position where the letter should be placed
                                if (separated)
                                {
                                    // Add with a new iterator that starts to add up when the word is separated
                                    boardX = boardWord.XEnd + 1 + separatedIterator;

                                    // To avoid collision with another word coming after
                                    if (Board.Tiles[boardWord.YEnd, Math.Min(boardX + 1, Board.XTiles - 1)].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                    separatedIterator++;
                                }
                                else
                                {
                                    boardX = boardWord.XStart - firstHalf.Length + x;

                                    // To avoid collision with another word coming before
                                    if (Board.Tiles[boardWord.YEnd, Math.Max(boardX - 1, 0)].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                // Check if there is a word above the character
                                var upWord = boardWords.Where(wordObj =>
                                        wordObj.XStart >= boardX &&
                                        wordObj.XEnd <= boardX &&
                                        wordObj.YEnd == boardWord.YEnd - 1)
                                    .Select(wordObj => wordObj.Value)
                                    .FirstOrDefault();

                                if (upWord != null)
                                {
                                    // Check if the combined word is in the dictionary
                                    if (!Dictionaries.IsWord(Board.Tiles[boardWord.YEnd, boardX].Letter + upWord))
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                // Word below the character
                                var downWord = boardWords.Where(wordObj =>
                                        wordObj.XStart >= boardX &&
                                        wordObj.XEnd <= boardX &&
                                        wordObj.YStart == boardWord.YEnd + 1)
                                    .Select(wordObj => wordObj.Value)
                                    .FirstOrDefault();

                                if (downWord != null)
                                {
                                    if (!Dictionaries.IsWord(Board.Tiles[boardWord.YEnd, boardX].Letter + downWord))
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                wordPlacement.Add(new Tuple<char, int, int>(char.ToUpper(word[x]), boardWord.YEnd, boardX));
                            }
                        }
                        else
                        {
                            for (var y = 0; y < word.Length; y++)
                            {
                                int boardY; // Y position where the character should be placed

                                if (boardWordIndex == y)
                                {
                                    y += boardWord.Value.Length - 1;

                                    separated = true;
                                    continue;
                                }

                                if (separated)
                                {
                                    boardY = boardWord.YEnd + 1 + separatedIterator;
                                    // To avoid collision with another word coming after
                                    if (Board.Tiles[Math.Min(boardY + 1, Board.YTiles - 1), boardWord.XEnd].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                    separatedIterator++;
                                }
                                else
                                {
                                    boardY = boardWord.YStart - word[..boardWordIndex].Length + y;
                                    // To avoid collision with another word coming before
                                    if (Board.Tiles[Math.Max(boardY - 1, 0), boardWord.XEnd].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                // Check if there is a word to the left of the character
                                var leftWord = boardWords.Where(wordObj =>
                                        wordObj.XEnd == boardWord.XEnd - 1 &&
                                        wordObj.YStart >= boardY &&
                                        wordObj.YEnd <= boardY)
                                    .Select(wordObj => wordObj.Value)
                                    .FirstOrDefault();

                                if (leftWord != null)
                                {
                                    if (!Dictionaries.IsWord(leftWord + Board.Tiles[boardY, boardWord.XEnd].Letter))
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                // Check if there is a word to the right of the character
                                var rightWord = boardWords.Where(wordObj =>
                                        wordObj.XStart == boardWord.XEnd + 1 &&
                                        wordObj.YStart >= boardY &&
                                        wordObj.YEnd <= boardY)
                                    .Select(wordObj => wordObj.Value)
                                    .FirstOrDefault();

                                if (rightWord != null)
                                {
                                    if (!Dictionaries.IsWord(Board.Tiles[boardY, boardWord.XEnd].Letter + rightWord))
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                wordPlacement.Add(new Tuple<char, int, int>(char.ToUpper(word[y]), boardY, boardWord.XEnd));
                            }
                        }
                        if (!invalid)
                        {
                            wordPlacements.Add(wordPlacement);
                            foundWord = true;
                        }
                        break;
                    }
                }
            }
            return wordPlacements;
        }
    }
}
