using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Collections.Generic;

namespace Alfapet
{
    /*
     * Klass för dina bokstäver (din hand)
     */
    class Hand : Game
    {
        public static Tile[] Tiles = new Tile[Alfapet_Config.HandAmount];

        public static float TilesMargin = 5f;
        public static float TilesWidth = (Alfapet._graphics.GraphicsDevice.Viewport.Width - ((Tiles.Length + 1) * TilesMargin)) / Tiles.Length;
        public static float TilesHeight = TilesWidth;

        public static Action<dynamic, Vector2, Tile, Tile> DragCallback;

        public static void SetPositions()
        {
            float w = 5f;
            foreach (Tile tile in Tiles)
            {
                if (tile.Letter == '\0')
                    continue;

                tile.SetPos(w, Alfapet._graphics.GraphicsDevice.Viewport.Height - TilesHeight + 5);

                w += TilesWidth + TilesMargin;
            }
        }

        public static void Initialize() // Körs i Initialize()
        {
            for (int i = 0; i < Tiles.Length; i++) // Populera arrayen med nya objekt
            {
                Tiles[i] = new Tile();
                Tiles[i].Letter = Alfapet_Util.GenerateRandomLetter();
            }
            SetPositions();
        }

        public static void Draw()
        {
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(0, (int)(Alfapet._graphics.GraphicsDevice.Viewport.Height - TilesHeight), Alfapet._graphics.GraphicsDevice.Viewport.Width, (int)TilesHeight), Color.Black * 0.5f);

            for (int i = 0; i < Tiles.Length; i++)
            {
                if(Tiles[i].Letter == '\0')
                    continue;

                if (!Tiles[i].Dragging)
                {
                    Tiles[i].SetSize(TilesWidth, TilesHeight - 10);
                    Tiles[i].SetFont(Fonts.Montserrat_Bold);
                }
                else
                {
                    Tiles[i].SetSize(Board.TilesWidth, Board.TilesHeight);
                    Tiles[i].SetFont(Fonts.Montserrat_Bold_Smaller);
                }

                UI.StylishRectangle(new Rectangle((int)Tiles[i].X, (int)Tiles[i].Y, (int)Tiles[i].W, (int)Tiles[i].H));

                UI.DrawCenterText(Tiles[i].Font, Tiles[i].Letter.ToString(), new Vector2(Tiles[i].X, Tiles[i].Y), Color.White, (int)Tiles[i].W, (int)Tiles[i].H);
            }
        }
    }
}
