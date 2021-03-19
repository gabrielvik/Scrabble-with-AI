using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Alfapet
{
    /*
     * Klass för dina bokstäver (din hand)
     */
    class Hand : Game
    {
        private GameObject[] hand_tiles = new GameObject[Alfapet_Config.HandAmount];

        public void Init() // Körs i Initialize()
        {
            for (int i = 0; i < hand_tiles.Length; i++) // Populera arrayen med nya objekt
            {
                hand_tiles[i] = new GameObject();
                hand_tiles[i].Letter = Alfapet_Util.GenerateRandomLetter();
            }
        }

        public void Draw()
        {
            int height = Alfapet._graphics.GraphicsDevice.Viewport.Height / 8;
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(0, Alfapet._graphics.GraphicsDevice.Viewport.Height - height, Alfapet._graphics.GraphicsDevice.Viewport.Width, height), Color.Black * 0.5f);

            float _margin = 5;
            float _w = 5;
            float width = ((Alfapet._graphics.GraphicsDevice.Viewport.Width - _margin * hand_tiles.Length - _w / 2) / hand_tiles.Length);

            for (int i = 0; i < hand_tiles.Length; i++)
            {
                UI.StylishRectangle(new Rectangle((int)_w, Alfapet._graphics.GraphicsDevice.Viewport.Height - height + 5, (int)width, height - 10));

                Alfapet._spriteBatch.DrawString(Fonts.Montserrat_Bold, hand_tiles[i].Letter.ToString(), new Vector2(_w + 30, Alfapet._graphics.GraphicsDevice.Viewport.Height - height + 18), Color.White); ;
                // TODO: CENTER CHARS WITH UI.DRAWSTRING
                _w += width + _margin;
            }
        }
    }
}
