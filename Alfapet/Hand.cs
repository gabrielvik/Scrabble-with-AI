﻿using Microsoft.Xna.Framework;
using System;

namespace Alfapet
{
    class Hand : Game
    {
        public static int Amount = 8; // Antalet brickor man har i handen
        public static Tile[] Tiles = new Tile[Amount];

        public static float TilesMargin = 5f; // Mellanrum mellan brickorna 
        // Anpassar sig automatiskt till skärmens storlek
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

        /*
         * Byter alla bokstäver i handen till nya random
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

        /*
         * Populerar handen med brickor med random karaktärer 
        */
        public new static void Initialize()
        {
            GenerateNew();
        }

        /*
         * Sorterar handen alfabetiskt
        */
        public static void Sort()
        {
            Array.Sort(Tiles, (tile1, tile2) => tile1.Letter.CompareTo(tile2.Letter));
            SetPositions();
        }
        
        /*
         * Hittar automatiskt en tom plats och sätter in karaktären
        */
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

        /*
         * Ändrar alla toma bokstäver till nya random bokstäver
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
                new Rectangle(0, (int) (Alfapet.Graphics.GraphicsDevice.Viewport.Height - TilesHeight),
                    Alfapet.Graphics.GraphicsDevice.Viewport.Width, (int) TilesHeight), Color.Black * 0.5f);

            foreach (var tile in Tiles)
            {
                if (tile.Letter == '\0')
                    continue;

                if (!tile.Dragging)
                {
                    tile.SetSize(TilesWidth, TilesHeight - 10);
                    tile.SetFont(Ui.MontserratBold);
                }
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