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
        public async static void DoMove()
        {
            // Sätter texten av "move" knappen och väntar tills man gjort det innan går vidare
            await Task.Run(() =>
            {
                ButtonRig.Buttons[0].SetText("Opponent Thinking");
            });

            List<List<Tuple<char, int, int>>> words = GetBestWords(); // Möjliga ord man kan placera

            // Väntar en random tid mellan 0.35s-1s innan man går vidare för att göra det mer verkligt för användaren
            await Task.Delay(new Random().Next(1000, 3000));
            ButtonRig.Buttons[0].SetText("Opponent Playing..."); // Har "tänkt" klart

            List<Tuple<char, int, int>> bestWord = new List<Tuple<char, int, int>>();
            int mostPoint = 0;

            foreach (var word in words) // Räkna ut vilket ord som är värt mest poäng
            {
                int tempScore = 0;
                foreach (var characther in word)
                {
                    tempScore += Alfapet_Config.CharactherPoints[characther.Item1]; // Lägger till hur mycket karaktären är värd i poäng
                }
                if (tempScore > mostPoint)
                {
                    bestWord = word;
                    mostPoint = tempScore;
                }
            }

            foreach (var t in bestWord) // Lägg ut det bästa ordet man hittade på brädan
            {
                Board.Tiles[t.Item2, t.Item3].Letter = t.Item1;
                Board.Tiles[t.Item2, t.Item3].TempPlaced = true;
                Board.Tiles[t.Item2, t.Item3].PlayerPlaced = false; // Användaren ska inte kunna röra brickan

                Rounds.AIPoints += Alfapet_Config.CharactherPoints[t.Item1];

                await Task.Delay(new Random().Next(500, 1250)); // Väntar 0.5-1.25s innan man lägger nästa karaktär
            }

            foreach (Tile tile in Board.Tiles)
            {
                if (!tile.TempPlaced || tile.Letter == '\0')
                    continue;

                tile.TempPlaced = false;
                await Task.Delay(150); // Vänta 0.15s innan nästa loop så användaren kan se allting hända
            }

            ButtonRig.Buttons[0].SetText("Skip");
        }

        /*
         * Returnerar en lista av alla ord på brädan i en Word struktur
         */
        private static List<Word> GetBoardWords()
        {
            List<Word> boardWords = new List<Word>();
            for (int y = 0; y < Board.YTiles; y++)
            {
                string xWord = "";
                string yWord = "";

                int xStart = -1;
                int yStart = -1;

                for (int x = 0; x < Board.XTiles; x++)
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
         * Item2 är Y axeln vart den ska placeras, Item3 är X axeln där den sks placeras
         */
        public static List<List<Tuple<char, int, int>>> GetBestWords()
        {
            string hand = Hand.GetHandString();
            List<Word> boardWords = GetBoardWords();

            List<List<Tuple<char, int, int>>> wordPlacements = new List<List<Tuple<char, int, int>>>();

            List<string> dictionaryList = Dictionaries.GetWordList();

            foreach (string word in dictionaryList)
            {
                bool foundWord = false;
                foreach (var boardWord in boardWords.Where(boardWord => word.Contains(boardWord.Value))) // Loop där ordet innehåller ett ord på brädan
                {
                    if (foundWord) // Gör igen skillnad om man hittar samma ord flera gånger, fortsätt till nästa istället
                        break;

                    string tempHand = hand + boardWord.Value;
                    string tempWord = word;
                    int match = 0;

                    for (int i = 0; i < tempHand.Length; i++)
                    {
                        int index = tempWord.IndexOf(tempHand[i]);

                        if (index != -1)
                        {
                            tempWord = tempWord.Remove(index, 1); // Ta bort karaktären från strängen så den inte kan användas igen
                            match++;
                        }

                        if (match >= word.Length) // Är en match, ordet på brädan kan bli ordet i ordboken
                        {
                            string[] splittedWord = word.Split(boardWord.Value);
                            if (splittedWord.Length != 2) // Borde bara finnas två, innan och/eller efter ordet på brädan 
                                continue;

                            // Kollar om ordet går utanför brädan på X eller Y axeln
                            if (boardWord.XEnd - (splittedWord[0].Length + boardWord.Value.Length) < 0 || boardWord.XEnd + (splittedWord[1].Length + boardWord.Value.Length) > Board.XTiles - 1)
                                continue;
                            if (boardWord.YEnd - (splittedWord[0].Length + boardWord.Value.Length) < 0 || boardWord.YEnd + (splittedWord[1].Length + boardWord.Value.Length) > Board.YTiles - 1)
                                continue;

                            var wordPlacement = new List<Tuple<char, int, int>>();

                            bool invalid = false;

                            if (boardWord.Axis) // På X axeln
                            {
                                for (int x = 0; x < splittedWord[0].Length; x++)
                                {
                                    var boardX = boardWord.XStart - splittedWord[0].Length + x; // X Positionen där bokstaven ska placeras

                                    // Kolla om det finns ett ord över karaktären
                                    string upWord = boardWords.Where(wordObj => wordObj.XEnd == boardX && wordObj.YEnd == boardWord.YEnd - 1)
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
                                    string downWord = boardWords.Where(wordObj => wordObj.XEnd == boardX && wordObj.YEnd == boardWord.YEnd + 1)
                                        .Select(wordObj => wordObj.Value)
                                        .FirstOrDefault();

                                    if (downWord != null)
                                    {
                                        if (!Dictionaries.IsWord(downWord + Board.Tiles[boardWord.YEnd, boardX].Letter))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    if (Board.Tiles[boardWord.YEnd, Math.Max(boardX - 1, 0)].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }

                                    wordPlacement.Add(new Tuple<char, int, int>(char.ToUpper(splittedWord[0][x]), boardWord.YEnd, boardX));
                                }
                                for (int x = 0; x < splittedWord[1].Length; x++) // Karaktärerna efter ordet på brädan slutar
                                {
                                    var boardX = boardWord.XEnd + 1 + x;

                                    // Ordet över karaktärerna
                                    string upWord = boardWords.Where(wordObj => wordObj.XEnd == boardX && wordObj.YEnd == boardWord.YEnd - 1)
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

                                    // Ordet under karaktärerna
                                    string downWord = boardWords.Where(wordObj => wordObj.XEnd == boardX && wordObj.YStart == boardWord.YEnd + 1)
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

                                    if (Board.Tiles[boardWord.YEnd, Math.Min(boardX + 1, Board.XTiles - 1)].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }
                                    wordPlacement.Add(new Tuple<char, int, int>(char.ToUpper(splittedWord[1][x]), boardWord.YEnd, boardX));
                                }
                            }
                            else // Y axeln
                            {
                                for (int y = 0; y < splittedWord[0].Length; y++) // Bokstäverna innan ordet börjar
                                {
                                    var boardY = boardWord.YStart - splittedWord[0].Length + y; // Y positionen där karaktären ska placeras

                                    // Kolla om det finns ett ord vänster till karaktären
                                    string leftWord = boardWords.Where((wordObj) => wordObj.XEnd == boardWord.XEnd - 1 && wordObj.YEnd == boardY)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (leftWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Board.Tiles[boardY, boardWord.XEnd].Letter + leftWord))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    // Kolla om det finns ett ord till höger av karaktären
                                    string rightWord = boardWords.Where((wordObj) => wordObj.XStart == boardWord.XEnd + 1 && wordObj.YEnd == boardY)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (rightWord != null)
                                    {
                                        if (!Dictionaries.IsWord(rightWord + Board.Tiles[boardY, boardWord.XEnd].Letter))
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

                                    wordPlacement.Add(new Tuple<char, int, int>(char.ToUpper(splittedWord[0][y]), boardY, boardWord.XEnd));
                                    //System.Diagnostics.Debug.WriteLine("Adds here 0" + word);
                                }
                                for (int y = 0; y < splittedWord[1].Length; y++) // Bokstäverna efter ordet börjar
                                {
                                    //System.Diagnostics.Debug.WriteLine("Adds here 1"+word);
                                    var boardY = boardWord.YEnd + 1 + y; // Y position där karaktären ska placeras

                                    // Kolla om det finns ett ord till vänster om karaktären
                                    string leftWord = boardWords.Where((wordObj) => wordObj.XEnd == boardWord.XEnd - 1 && wordObj.YEnd == boardY)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (leftWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Board.Tiles[boardY, boardWord.XEnd].Letter + leftWord))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    // Kolla om det finns ett ord till höger om karaktären
                                    string rightWord = boardWords.Where((wordObj) => wordObj.XStart == boardWord.XEnd + 1 && wordObj.YEnd == boardY)
                                        .Select((wordObj) => wordObj.Value)
                                        .FirstOrDefault();

                                    if (rightWord != null)
                                    {
                                        if (!Dictionaries.IsWord(Board.Tiles[boardY, boardWord.XEnd].Letter + rightWord))
                                        {
                                            invalid = true;
                                            break;
                                        }
                                    }

                                    if (Board.Tiles[Math.Min(boardY + 1, Board.YTiles - 1), boardWord.XEnd].Letter != '\0')
                                    {
                                        invalid = true;
                                        break;
                                    }

                                    wordPlacement.Add(new Tuple<char, int, int>(char.ToUpper(splittedWord[1][y]), boardWord.YEnd + 1 + y, boardWord.XEnd));
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
            }

            return wordPlacements;
        }
    }
}
