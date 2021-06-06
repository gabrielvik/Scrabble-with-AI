using Microsoft.Xna.Framework;
using System;

namespace Alfapet
{
    /*
     * Klass för dina bokstäver (din hand)
     */
    class Hand : Game
    {
        public static Tile[] Tiles = new Tile[Config.HandAmount];

        public static float TilesMargin = 5f;
        public static float TilesWidth = (Alfapet.Graphics.GraphicsDevice.Viewport.Width - (Tiles.Length + 1) * TilesMargin) / Tiles.Length;
        public static float TilesHeight = (Alfapet.Graphics.GraphicsDevice.Viewport.Height - (Tiles.Length + 1) * TilesMargin) / Tiles.Length;

        public static void SetPositions()
        {
            var w = 5f;
            foreach (var tile in Tiles)
            {
                if (tile.Letter == '\0')
                    continue;

                tile.SetPos(w, Alfapet.Graphics.GraphicsDevice.Viewport.Height - TilesHeight + 5);

                w += TilesWidth + TilesMargin;
            }
        }

        public new static void Initialize() // Körs i Initialize()
        {
            for (var i = 0; i < Tiles.Length; i++) // Populera arrayen med nya objekt
            {
                Tiles[i] = new Tile
                {
                    Letter = Util.GenerateRandomLetter()
                };
            }
            SetPositions();
        }

        public static void Sort()
        {
            Array.Sort(Hand.Tiles, (tile1, tile2) => tile1.Letter.CompareTo(tile2.Letter));
            SetPositions();
        }
        
        public static void InsertLetter(char letter)
        {
            foreach (var tile in Tiles)
            {
                if (tile.Letter == '\0')
                {
                    tile.Letter = letter; // Sätt hand brickans bokstav till vad bokstaven på bordet var
                    break;
                }
            }
        }

        public static void GiveNewLetters()
        {
            foreach (var tile in Hand.Tiles)
            {
                if (tile.Letter == '\0')
                    tile.Letter = Util.GenerateRandomLetter();
            }
            SetPositions();
        }
        
        public static void Draw()
        {
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack,
                new Rectangle(0, (int) (Alfapet.Graphics.GraphicsDevice.Viewport.Height - TilesHeight),
                    Alfapet.Graphics.GraphicsDevice.Viewport.Width, (int) TilesHeight), Color.Black * 0.5f);

            foreach (var tile in Tiles)
            {
                if (tile.Letter == '\0')
                    continue;

                if (!tile.Dragging)
                {
                    tile.SetSize(TilesWidth, TilesHeight - 10);
                    tile.SetFont(UI.MontserratBold);
                }
                else
                {
                    tile.SetSize(Board.TilesWidth, Board.TilesHeight);
                    tile.SetFont(UI.MontserratBoldSmaller);
                }

                UI.StylishRectangle(new Rectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H));
                UI.DrawCenterText(tile.Font, tile.Letter.ToString(), tile.GetPos(), tile.GetSize(), Color.White);
            }
        }
    }
}