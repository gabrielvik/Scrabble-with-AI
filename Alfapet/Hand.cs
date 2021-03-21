using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Alfapet
{
    /*
     * Klass för dina bokstäver (din hand)
     */
    class Hand : Game
    {
        public static GameObject[] HandTiles = new GameObject[Alfapet_Config.HandAmount];

        public static float TilesMargin = 5f;
        public static float TilesWidth = ((Alfapet._graphics.GraphicsDevice.Viewport.Width - TilesMargin * HandTiles.Length) / HandTiles.Length);
        public static float TilesHeight = Alfapet._graphics.GraphicsDevice.Viewport.Height / 8;
        public static void Init() // Körs i Initialize()
        {
            for (int i = 0; i < HandTiles.Length; i++) // Populera arrayen med nya objekt
            {
                HandTiles[i] = new GameObject();
                HandTiles[i].Letter = Alfapet_Util.GenerateRandomLetter();
            }
        }

        public static void Draw()
        {
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(0, (int)(Alfapet._graphics.GraphicsDevice.Viewport.Height - TilesHeight), Alfapet._graphics.GraphicsDevice.Viewport.Width, (int)TilesHeight), Color.Black * 0.5f);

            float _w = 5f;

            for (int i = 0; i < HandTiles.Length; i++)
            {
                if(HandTiles[i] == null)
                    continue;

                if (!HandTiles[i].BeingDragged)
                {
                   HandTiles[i].SetPos(_w, Alfapet._graphics.GraphicsDevice.Viewport.Height - TilesHeight + 5);
                }

                UI.StylishRectangle(new Rectangle((int)HandTiles[i].X, (int)HandTiles[i].Y, (int)TilesWidth, (int)TilesHeight - 10));

                UI.DrawCenterChar(Fonts.Montserrat_Bold, HandTiles[i].Letter.ToString(), new Vector2(HandTiles[i].X, HandTiles[i].Y - 5), Color.White, (int)TilesWidth, (int)TilesHeight);

                _w += TilesWidth + TilesMargin;
            }
        }
    }
}
