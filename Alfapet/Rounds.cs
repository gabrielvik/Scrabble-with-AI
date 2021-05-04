using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Alfapet
{
    class Rounds : Game
    {
        public static int PlayerPoints = 0;
        public static bool PlacedValidWords()
        {
            List<string> words = new List<string>();

            // TODO: organisera till en funktion


            for (int x = 0; x < Board.XTiles; x++) // Kollar ord på Y axeln
            {
                string _word = ""; // Temp variabel som lagrar ord
                for (int y = 0; y < Board.YTiles; y++)
                {
                    if (_word.Length == 1) // Om ordet är bara en karaktär, kolla så den inte är isolerad från andra karaktärer och om den är, returna false
                    {
                        char letterUp = Board.Tiles[Math.Max(y - 1, 0), Math.Max(x - 1, 0)].Letter;
                        char letterDown = Board.Tiles[Math.Min(y + 1, Board.YTiles - 1), Math.Max(x - 1, 0)].Letter;
                        if (letterUp == '\0' && letterDown == '\0')
                            return false;
                    }
                    else
                        words.Add(_word); // Annars om det är ett komplett ord, lägg till i listan

                    _word = ""; // töm temp variabeln
                }
            }
            for (int y = 0; y < Board.YTiles; y++) // Samma som förra men kollar på X axeln
            {
                string _word = "";
                for (int x = 0; x < Board.XTiles; x++)
                {

                    if (Board.Tiles[y, x].Letter == '\0')
                    {
                        if (_word.Length == 1)
                        {
                            char letterUp = Board.Tiles[Math.Max(y - 1, 0), Math.Max(x - 1, 0)].Letter;
                            char letterDown = Board.Tiles[Math.Min(y + 1, Board.YTiles - 1), Math.Max(x - 1, 0)].Letter;
                            if (letterUp == '\0' && letterDown == '\0')
                                return false;
                        }
                        else
                            words.Add(_word);

                        _word = "";
                    }
                    else
                        _word += Board.Tiles[y, x].Letter;
                }
            }

            foreach (string word in words)
            {
                if (!Dictionaries.IsWord(word))
                    return false; // Så fort man hittar ett ogilitigt ord returna false, behöver inte kolla längre
            }
            return true;
        }
        public async static void DoMove()
        {
            if (Board.TilesPlaced <= 0) // Måste ha placerat minst en bokstav
                return;

            else if (!PlacedValidWords())
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
                    UI.DrawCenterChar(Fonts.Montserrat_Bold_Smaller, "Invalid Words", moveBtn.GetPos(), Color.White, (int)moveBtn.W, (int)moveBtn.H);
                };
                await Task.Delay(delay * 1000); // Efter x sekunder, sätt draw funktionen till null och återgå till normalt
                moveBtn.DrawFunc = null;
                return;
            }

            Board.TilesPlaced = 0;

            foreach(Tile tile in Board.Tiles)
            {
                if (!tile.TempPlaced || tile.Letter == '\0')
                    continue;

                tile.TempPlaced = false;
                PlayerPoints += Alfapet_Config.CharactherPoints[tile.Letter];
                await Task.Delay(100); // vänta 0.1s innan nästa loop så användaren kan se allting hända
            }

            System.Diagnostics.Debug.WriteLine(PlayerPoints);
        }
    }
}
