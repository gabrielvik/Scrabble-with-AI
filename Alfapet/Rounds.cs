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
        public static async void DoMove()
        {
            if (Board.TilesPlaced <= 0) // Måste ha placerat minst en bokstav
                return;
            else if (false)
            {
                Button moveBtn = ButtonRig.Buttons[0];
                if (moveBtn.DrawFunc != null) // Om draw functionen redan finns (man har klickat nyligen), returna
                    return;

                long lerpStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int delay = 4;
                ButtonRig.Buttons[0].DrawFunc = delegate ()
                {
                    // Sätter färgen till röd och lerpar färgens transparitet 
                    UI.StylishRectangle(new Rectangle((int)moveBtn.X, (int)moveBtn.Y, (int)moveBtn.W, (int)moveBtn.H), Color.Red * MathHelper.Lerp(1f, 0.95f, (DateTimeOffset.Now.ToUnixTimeMilliseconds() - lerpStart) * delay / 1000));
                    UI.DrawCenterText(Fonts.Montserrat_Bold_Smaller, "Invalid Words", moveBtn.GetPos(), Color.White, (int)moveBtn.W, (int)moveBtn.H);
                };
                await Task.Delay(delay * 1000); // Efter x sekunder, sätt draw funktionen till null och återgå till normalt
                moveBtn.DrawFunc = null;
                return;
            }

            Board.TilesPlaced = 0;
            foreach (var t in Board.GetBestWords())
            {
                foreach(var _t in t)
                {
                    Board.Tiles[_t.Item2, _t.Item3].Letter = char.ToUpper(_t.Item1);
                }
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

            System.Diagnostics.Debug.WriteLine(PlayerPoints);
        }
    }
}
