using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace Alfapet
{
    /*
     * Klass för dina bokstäver (din hand)
     */
    class Hand : Game
    {
        private Texture2D hand_background;
        private Texture2D tile_background;

        private GameObject[] hand_tiles = new GameObject[Alfapet_Config.HandAmount];

        public void Load() // Körs i LoadContent()
        {
            hand_background = new Texture2D(Alfapet._graphics.GraphicsDevice, 1, 1);
            hand_background.SetData(new Color[] { Color.Green });

            tile_background = new Texture2D(Alfapet._graphics.GraphicsDevice, 1, 1);
            tile_background.SetData(new Color[] { Color.White });
        }

        public void Draw()
        {
            int height = Alfapet._graphics.GraphicsDevice.Viewport.Height / 6;
            Alfapet._spriteBatch.Draw(hand_background, new Rectangle(0, Alfapet._graphics.GraphicsDevice.Viewport.Height - height, Alfapet._graphics.GraphicsDevice.Viewport.Width, height), Color.White);

            int _margin = 5;
            int _w = _margin + _margin / 2;
            int width = (Alfapet._graphics.GraphicsDevice.Viewport.Width) / hand_tiles.Length - _margin;

            for (int i = 0; i < hand_tiles.Length; i++)
            {
                Alfapet._spriteBatch.Draw(tile_background, new Rectangle(_w, Alfapet._graphics.GraphicsDevice.Viewport.Height - height - 5, width, height), Color.White);
                _w += width + _margin;
            }
        }
    }
}
