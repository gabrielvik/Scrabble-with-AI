using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Alfapet
{
    class Ai : Game
    {
        public static bool Playing = false;
        private static readonly char[] Letters = new char[Hand.Amount];
        private struct Word
        {
            public int YStart { get; set; } // Där ordet börjar på Y axeln
            public int YEnd { get; set; } // Där ordet slutar på Y axeln
            public int XStart { get; set; } // Där ordet börjar på X axeln
            public int XEnd { get; set; } // Där Ordet slutar på X axeln
            public bool Axis { get; set; } // Om man är på X eller Y. Sant för X och falskt för Y
            public string Value { get; set; } // Vad ordet är
        }

        /*
         * Sätter alla karakträrer i handen till nya random bokstäver
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
        * Kallas när AI ska göra sitt drag, sätter ut brickor och ger poäng
        */
        public static async void DoMove()
        {
            Playing = true;
            // Vänta tills texten byts innan man gör över till att hitta ord
            await Task.Run(() =>
            {
                ButtonRig.Buttons["move"].SetText("Opponent Thinking");
            });

            var words = GetPlacableWords();

            // Väntar en random tid mellan 1s-3s innan man går vidare för att göra det mer verkligt för användaren
            await Task.Delay(new Random().Next(1000, 3000));
            
            ButtonRig.Buttons["move"].SetText("Opponent Playing"); // Har "tänkt" klart

            var bestWord = new List<Tuple<char, int, int>>();
            var mostPoint = 0;
            foreach (var word in words) // Räkna ut vilket ord som är värt mest poäng
            {
                // Summan av alla bokstäver i ordet beroende på poäng i config
                var tempScore = word.Sum(character => Config.CharacterPoints[character.Item1]);
                
                if (tempScore > mostPoint)
                {
                    bestWord = word;
                    mostPoint = tempScore;
                }
            }

            // TODO: 
            if (Rounds.RoundNum == 2 || Rounds.RoundNum == 3)
            {
                Rounds.RoundNum++;
                Notifications.AddMessage("Opponent skipped this round");
                Board.CheckTilesPlaced();
                Playing = false;
                return;
            }
            
            var score = 0;
            var notificationString = "Opponent placed the letters (";
            // Lägg ut det bästa ordet man hittade på brädan
            foreach (var (letter, y, x) in bestWord)
            {
                // Ersätt med en ny random karaktär
                Letters[Array.IndexOf(Letters, char.ToUpper(letter))] = Util.GenerateRandomLetter();
                
                if(score != 0) // Lägg inte till ett komma första bokstaven
                    notificationString += ", ";
                
                notificationString += letter;
                
                Board.Tiles[y, x].Letter = letter;
                Board.Tiles[y, x].TempPlaced = true;

                score += Config.CharacterPoints[letter];
                await Task.Delay(new Random().Next(500, 1250)); // Väntar 0.5-1.25s innan man lägger nästa karaktär
            }
            if (score <= 0)
            {
                Notifications.AddMessage("Opponent skipped this round");
                // Ge Ai en ny hand så den kanske kan lägga ett ord nästa runda
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
         * Returnerar en sträng av Ai handen
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
                            YStart = y,
                            YEnd = y,
                            XStart = xStart,
                            XEnd = x - 1, // Minus ett eftersom ordet slutate på förra brickan
                            Axis = true,
                            Value = xWord.ToLower()
                        };
                        boardWords.Add(xWordObj);
                        xWord = "";
                    }
                    
                    // Finns ett ord som nu har tagit slut eftersom brickan är tom på Y axeln
                    if (yWord.Length > 0 && Board.Tiles[x, y].Letter == '\0')
                    {
                        var yWordObj = new Word()
                        {
                            YStart = yStart,
                            YEnd = x - 1, // Minus ett eftersom ordet slutate på förra brickan
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
                        if (xWord.Length <= 0) // Är start på ordet
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
         * Returnerar en lista med en lista som har information om alla karaktärer som kan placeras
         * Tuple<char, int, int> - Item1 är karaktären som ska placeras,
         * Item2 är Y axeln vart den ska placeras, Item3 är X axeln där den ska placeras
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
                // Loopa genom alla ord på brädan som word strängen innehåller
                foreach (var boardWord in boardWords.Where(boardWord => word.Contains(boardWord.Value)))
                {
                    if (foundWord) // Gör igen skillnad om man hittar samma ord flera gånger
                        break;

                    var tempHand = hand + boardWord.Value;
                    var tempWord = word;

                    var match = 0;
                    
                    var boardWordIndex = word.IndexOf(boardWord.Value, StringComparison.Ordinal);
                    var firstHalf = word[..boardWordIndex];
                    var secondHalf = word[boardWordIndex..];
                    
                    // Kollar om ordet har tillräckligt plats på brädan
                    if (boardWord.Axis)
                    {
                        if (boardWord.XStart - firstHalf.Length < 0)
                            break;
                        // Minus längden på boardWord eftersom boardWord tas med i rangen
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

                    foreach (var index in tempHand.Select(handChar => tempWord.IndexOf(handChar))) // Ordets position av karaktären i handen 
                    {
                        if (index != -1) // Karaktären finns i ordet
                        {
                            tempWord = tempWord.Remove(index, 1); // Ta bort karaktären från strängen så den inte kan användas igen
                            match++;
                        }
                        
                        if (match < word.Length) continue; // Inte en match än

                        var wordPlacement = new List<Tuple<char, int, int>>();
                        var invalid = false;
                        
                        var separated = false;
                        var separatedIterator = 0;
                        if (boardWord.Axis)
                        {
                            for (var x = 0; x < word.Length; x++)
                            {
                                // Man har kommit till positionen av boardWord, separera ordet
                                if (boardWordIndex == x)
                                {
                                    // Skippa alla karaktärer i boardWord (minus ett för att man fortsätter till nästa)
                                    x += boardWord.Value.Length - 1;
                                    
                                    separated = true;
                                    continue;
                                }

                                int boardX; // X Positionen där bokstaven ska placeras
                                if (separated)
                                {
                                    // Plussa med nya iteratorn som börjar plussas när ordet är separerad
                                    boardX = boardWord.XEnd + 1 + separatedIterator;
                                    
                                    // För att inte krocka med ett annat ord som kommer efter
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
                                    
                                    // För att inte krocka med ett annat ord som kommer innan
                                    if (Board.Tiles[boardWord.YEnd, Math.Max(boardX - 1, 0)].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }
                                
                                // Kolla om det finns ett ord över karaktären
                                var upWord = boardWords.Where(wordObj =>
                                        wordObj.XStart >= boardX &&
                                        wordObj.XEnd <= boardX &&
                                        wordObj.YEnd == boardWord.YEnd - 1)
                                    .Select(wordObj => wordObj.Value)
                                    .FirstOrDefault();

                                if (upWord != null)
                                {
                                    // Kolla det kollektiva ordet finns i ordboken
                                    if (!Dictionaries.IsWord(Board.Tiles[boardWord.YEnd, boardX].Letter + upWord))
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                // Ordet under karaktären
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
                                int boardY; // Y positionen där karaktären ska placeras

                                if (boardWordIndex == y)
                                {
                                    y += boardWord.Value.Length - 1;
                                    
                                    separated = true;
                                    continue;
                                }
                                
                                if (separated)
                                {
                                    boardY = boardWord.YEnd + 1 + separatedIterator;
                                    // För att inte krocka med ett annat ord som kommer efter
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
                                    // För att inte krocka med ett annat ord som kommer innan
                                    if (Board.Tiles[Math.Max(boardY - 1, 0), boardWord.XEnd].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                }

                                // Kolla om det finns ett ord vänster till karaktären
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

                                // Kolla om det finns ett ord till höger av karaktären
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