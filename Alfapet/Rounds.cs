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

            for (int x = 0; x < Board.XTiles; x++)
            {
                string _word = "";
                for (int y = 0; y < Board.YTiles; y++)
                {
                    if (Board.Tiles[y, x].Letter == '\0')
                    {
                        if (_word.Length == 1)
                        {
                            continue;
                        }
                        else if (_word.Length > 0)
                        {
                            words.Add(_word);
                        }
                        _word = "";
                    }
                    else
                        _word += Board.Tiles[y, x].Letter;
                }
            }
            for (int y = 0; y < Board.YTiles; y++)
            {
                string _word = "";
                for (int x = 0; x < Board.XTiles; x++)
                {
                    if (Board.Tiles[y, x].Letter == '\0')
                    {
                        // TODO: FIX length
                        if (_word.Length == 1)
                        {
                            continue;
                        }
                        else if (_word.Length > 0)
                        {
                            words.Add(_word);
                        }
                        _word = "";
                    }
                    else
                        _word += Board.Tiles[y, x].Letter;
                }
            }

            foreach (string word in words)
            {
                if (!Dictionaries.IsWord(word))
                    return false;
            }
            return true;
        }
        public async static void DoMove()
        {
            if (Board.TilesPlaced <= 0)
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
            }

            System.Diagnostics.Debug.WriteLine(PlayerPoints);
        }
    }
}
