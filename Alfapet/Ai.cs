using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Alfapet
{
    class Ai : Game
    {
        private struct Word
        {
            public int YStart { get; set; } // Där ordet börjar på Y axeln
            public int YEnd { get; set; } // Där ordet slutar på Y axeln
            public int XStart { get; set; } // Där ordet börjar på X axeln
            public int XEnd { get; set; } // Där Ordet slutar på X axeln
            public bool Axis { get; set; } // Om man är på X eller Y. sant för X och falskt för Y
            public string Value { get; set; } // Sträng av vad ordet är
        }

        /*
        * Kallas när AI ska göra sitt drag
        */
        public static async void DoMove()
        {
            // Vänta tills texten byts innan man gör över till att hitta ord eftersom funktionen kan vara kostsam
            await Task.Run(() =>
            {
                ButtonRig.Buttons[0].SetText("Opponent Thinking");
            });

            var words = GetPlacableWords();

            // Väntar en random tid mellan 0.35s-1s innan man går vidare för att göra det mer verkligt för användaren
            await Task.Delay(new Random().Next(1000, 3000));
            
            ButtonRig.Buttons[0].SetText("Opponent Playing..."); // Har "tänkt" klart

            var bestWord = new List<Tuple<char, int, int>>();
            var mostPoint = 0;

            foreach (var word in words) // Räkna ut vilket ord som är värt mest poäng
            {
                // Summan av alla bokstäver i ordet beroende på poäng i config
                var tempScore = word.Sum(character => AlfapetConfig.CharacterPoints[character.Item1]);
                
                if (tempScore > mostPoint)
                {
                    bestWord = word;
                    mostPoint = tempScore;
                }
            }

            var score = 0;
            // Lägg ut det bästa ordet man hittade på brädan
            foreach (var (letter, y, x) in bestWord)
            {
                Board.Tiles[y, x].Letter = letter;
                Board.Tiles[y, x].TempPlaced = true;
                Board.Tiles[y, x].PlayerPlaced = false; // Användaren ska inte kunna röra brickan

                score += AlfapetConfig.CharacterPoints[letter];

                await Task.Delay(new Random().Next(500, 1250)); // Väntar 0.5-1.25s innan man lägger nästa karaktär
            }

            Rounds.AIPoints += score;
            System.Diagnostics.Debug.WriteLine("Ai scored: " + score);

            Board.ResetTempTiles();
            ButtonRig.Buttons[0].SetText("Skip");
        }

        /*
         * Returnerar en lista av alla ord på brädan i en Word struktur
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
                    // Finns ett ord som nu har tagit slut eftersom brickan är tom på X axeln
                    if (xWord.Length > 0 && Board.Tiles[y, x].Letter == '\0')
                    {
                        var xWordObj = new Word()
                        {
                            YEnd = y,
                            XEnd = x - 1, // Minus ett eftersom ordet slutate på förra brickan
                            XStart = xStart,
                            Axis = true,
                            Value = xWord.ToLower()
                        };
                        boardWords.Add(xWordObj);
                        xWord = "";
                    }
                    
                    // Finns ett ord som nu har tagit slut eftersom brickan är tom på Y axeln
                    if (yWord.Length > 0 && Board.Tiles[x, y].Letter == '\0')
                    {
                        //System.Diagnostics.Debug.WriteLine(yWord);
                        var yWordObj = new Word()
                        {
                            YEnd = x - 1, // Minus ett eftersom ordet slutate på förra brickan
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
            return boardWords;
        }

        /*
         * Hittar alla ord som kan placeras på brädan.
         * Returnerar en lista med en lista som har information om alla karaktärer som ska placeras
         * Tuple<char, int, int> - Item1 är karaktären som ska placeras,
         * Item2 är Y axeln vart den ska placeras, Item3 är X axeln där den ska placeras
        */
        private static List<List<Tuple<char, int, int>>> GetPlacableWords()
        {
            var hand = Hand.GetHandString().ToLower();
            var boardWords = GetBoardWords();
            var wordPlacements = new List<List<Tuple<char, int, int>>>();
            var dictionaryList = Dictionaries.GetWordList();

            foreach (var word in dictionaryList)
            {
                var foundWord = false;
                foreach (var boardWord in boardWords.Where(boardWord => word.Contains(boardWord.Value)))
                {
                    if (foundWord) // Gör igen skillnad om man hittar samma ord flera gånger
                        break;

                    var tempHand = hand + boardWord.Value;
                    var tempWord = word;
                    var match = 0;
                    
                    var boardWordIndex = word.IndexOf(boardWord.Value, StringComparison.Ordinal);

                    if (boardWord.Axis)
                    {
                        if (boardWord.XStart - word[..boardWordIndex].Length < 0)
                            break;

                        if (boardWord.XEnd + word[boardWordIndex..].Length - 1 > Board.XTiles - 1)
                            break;
                    }
                    else
                    {
                        if (boardWord.YEnd + word[boardWordIndex..].Length - 1 > Board.YTiles - 1)
                            break;

                        if (boardWord.YStart - word[..boardWordIndex].Length < 0)
                            break;
                    }

                    foreach (var index in tempHand.Select(handChar => tempWord.IndexOf(handChar)))
                    {
                        if (index != -1)
                        {
                            tempWord = tempWord.Remove(index, 1); // Ta bort karaktären från strängen så den inte kan användas igen
                            match++;
                        }
                        if (match < word.Length) continue;

                        var wordPlacement = new List<Tuple<char, int, int>>();
                        var invalid = false;
                        
                        var separated = false;
                        var separatedIterator = 0;
                        
                        if (boardWord.Axis) // X axel
                        {
                            for (var x = 0; x < word.Length; x++)
                            {
                                if (boardWordIndex == x)
                                {
                                    x += boardWord.Value.Length - 1;
                                    
                                    separated = true;
                                    continue;
                                }

                                int boardX; // X Positionen där bokstaven ska placeras

                                if (separated)
                                {
                                    boardX = boardWord.XEnd + 1 + separatedIterator;
                                    
                                    if (Board.Tiles[boardWord.YEnd, Math.Min(boardX + 1, Board.XTiles - 1)].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                    separatedIterator++;
                                }
                                else
                                {
                                    boardX = boardWord.XStart - word[..boardWordIndex].Length + x;
                                    
                                    if (Board.Tiles[boardWord.YEnd, Math.Max(boardX - 1, 0)].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }
                                
                                // Kolla om det finns ett ord över karaktären
                                var upWord = boardWords.Where(wordObj => wordObj.XEnd == boardX && wordObj.YEnd == boardWord.YEnd - 1)
                                    .Select(wordObj => wordObj.Value)
                                    .FirstOrDefault();

                                if (upWord != null)
                                {
                                    if (!Dictionaries.IsWord(Board.Tiles[boardWord.YEnd, boardX].Letter + upWord))
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                // Ordet under karaktären
                                var downWord = boardWords.Where(wordObj => wordObj.XEnd == boardX && wordObj.YStart == boardWord.YEnd + 1)
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
                        else // Y axeln
                        {
                            for (var y = 0; y < word.Length; y++) // Bokstäverna innan ordet börjar
                            {
                                var boardY = -1; // Y positionen där karaktären ska placeras

                                if (boardWordIndex == y)
                                {
                                    y += boardWord.Value.Length - 1;
                                    
                                    separated = true;
                                    continue;
                                }
                                
                                if (separated)
                                {
                                    boardY = boardWord.YEnd + 1 + separatedIterator;
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
                                    if (Board.Tiles[Math.Max(boardY - 1, 0), boardWord.XEnd].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                // Kolla om det finns ett ord vänster till karaktären
                                var leftWord = boardWords.Where(wordObj => wordObj.XEnd == boardWord.XEnd - 1 && wordObj.YEnd == boardY)
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

                                // Kolla om det finns ett ord till höger av karaktären
                                var rightWord = boardWords.Where(wordObj => wordObj.XStart == boardWord.XEnd + 1 && wordObj.YEnd == boardY)
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
                                if (Board.Tiles[Math.Max(boardY - 1, 0), boardWord.XEnd].Letter != '\0')
                                {
                                    invalid = true;
                                    break;
                                }

                                wordPlacement.Add(new Tuple<char, int, int>(char.ToUpper(word[y]), boardY, boardWord.XEnd));
                                //System.Diagnostics.Debug.WriteLine("Adds here 0" + word);
                            }
                        }

                        if (!invalid)
                        {
                            wordPlacements.Add(wordPlacement);

                            foundWord = true;
                            break;
                        }
                    }
                }
            }

            return wordPlacements;
        }
    }
}
