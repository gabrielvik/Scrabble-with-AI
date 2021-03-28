using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Alfapet
{
    /*
     * Klass för dina bokstäver (din hand)
     */
    class Hand : Game
    {
        public static Tile[] Tiles = new Tile[Alfapet_Config.HandAmount];

        public static float TilesMargin = 5f;
        public static float TilesWidth = ((Alfapet._graphics.GraphicsDevice.Viewport.Width - (TilesMargin * (Tiles.Length + 1))) / Tiles.Length);
        public static float TilesHeight = TilesWidth;

        public static bool BeingDragged = false;

        public static void Build() // Körs i Initialize()
        {
            for (int i = 0; i < Tiles.Length; i++) // Populera arrayen med nya objekt
            {
                Tiles[i] = new Tile();
                Tiles[i].Letter = Alfapet_Util.GenerateRandomLetter();
            }
        }

        public static void Draw()
        {
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(0, (int)(Alfapet._graphics.GraphicsDevice.Viewport.Height - TilesHeight), Alfapet._graphics.GraphicsDevice.Viewport.Width, (int)TilesHeight), Color.Black * 0.5f);

            float _w = 5f;

            for (int i = 0; i < Tiles.Length; i++)
            {
                if(Tiles[i] == null)
                    continue;

                if (!Tiles[i].Dragging)
                {
                    Tiles[i].SetPos(_w, Alfapet._graphics.GraphicsDevice.Viewport.Height - TilesHeight + 5);
                    Tiles[i].SetSize(TilesWidth, TilesHeight - 10);
                    Tiles[i].SetFont(Fonts.Montserrat_Bold);
                }
                else
                {
                    Tiles[i].SetSize(Board.TilesWidth, Board.TilesHeight);
                    Tiles[i].SetFont(Fonts.Montserrat_Bold_Smaller);
                }

                UI.StylishRectangle(new Rectangle((int)Tiles[i].X, (int)Tiles[i].Y, (int)Tiles[i].W, (int)Tiles[i].H));

                UI.DrawCenterChar(Tiles[i].Font, Tiles[i].Letter.ToString(), new Vector2(Tiles[i].X, Tiles[i].Y), Color.White, (int)Tiles[i].W, (int)Tiles[i].H);

                _w += TilesWidth + TilesMargin;
            }
        }
    }
}
