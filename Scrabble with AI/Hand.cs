using Microsoft.Xna.Framework;
using System;

namespace Alfapet
{
    class Hand : Game
    {
        public static int Amount = 8; // The number of tiles one has in hand
        public static Tile[] Tiles = new Tile[Amount];

        public static float TilesMargin = 5f; // Spacing between the tiles 

        // Automatically adjusts to the screen size
        public static float TilesWidth = (Alfapet.Graphics.GraphicsDevice.Viewport.Width - (Tiles.Length + 1) * TilesMargin) / Tiles.Length;
        public static float TilesHeight = (Alfapet.Graphics.GraphicsDevice.Viewport.Height - (Tiles.Length + 1) * TilesMargin) / Tiles.Length;

        /*
         * Sets the positions of the tiles in the hand 
        */
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

        /*
         * Changes all letters in the hand to new random tiles
        */
        public static void GenerateNew()
        {
            for (var i = 0; i < Tiles.Length; i++)
            {
                Tiles[i] = new Tile
                {
                    Letter = Util.GenerateRandomLetter()
                };
            }
            SetPositions();
        }

        public new static void Initialize()
        {
            GenerateNew();
        }

        /*
         * Sorts the hand alphabetically
        */
        public static void Sort()
        {
            Array.Sort(Tiles, (tile1, tile2) => tile1.Letter.CompareTo(tile2.Letter));
            SetPositions();
        }

        /*
         * Automatically finds an empty slot and inserts the character
        */
        public static void InsertLetter(char letter)
        {
            foreach (var tile in Tiles)
            {
                if (tile.Letter == '\0')
                {
                    tile.Letter = letter; // Set hand tile's letter to the letter on the board
                    break;
                }
            }
        }

        /*
         * Changes all empty letters to new random letters
        */
        public static void GiveNewLetters()
        {
            foreach (var tile in Tiles)
            {
                if (tile.Letter == '\0')
                    tile.Letter = Util.GenerateRandomLetter();
            }
            SetPositions();
        }

        public static void Draw()
        {
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack,
                new Rectangle(0, (int)(Alfapet.Graphics.GraphicsDevice.Viewport.Height - TilesHeight),
                    Alfapet.Graphics.GraphicsDevice.Viewport.Width, (int)TilesHeight), Color.Black * 0.5f);

            foreach (var tile in Tiles)
            {
                if (tile.Letter == '\0')
                    continue;

                if (!tile.Dragging)
                {
                    tile.SetSize(TilesWidth, TilesHeight - 10);
                    tile.SetFont(Ui.MontserratBold);
                }
                // If dragging a tile, set the size to match the tiles on the board
                else
                {
                    tile.SetSize(Board.TilesWidth, Board.TilesHeight);
                    tile.SetFont(Ui.MontserratBoldSmaller);
                }

                Ui.OutlinedRectangle(new Rectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H));
                Ui.DrawCenterText(tile.Font, tile.Letter.ToString(), tile.GetPos(), tile.GetSize(), Color.White);
            }
        }
    }
}
