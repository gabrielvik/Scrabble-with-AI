using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alfapet
{
    public class Tile : Game
    {
        public float X, Y, W, H;

        public char Letter = '\0';

        public SpriteFont Font;

        public bool TempPlaced = false;
        public bool Dragging = false;

        private Vector2 OriginalPos;

        public void SetPos(float x, float y)
        {
            X = x;
            Y = y;

            // If the original position does not exist, set it
            if (OriginalPos == Vector2.Zero)
                OriginalPos = new Vector2(x, y);
        }

        /*
         * Sets the position back to where it was initially 
        */
        public void ResetPos()
        {
            SetPos(OriginalPos.X, OriginalPos.Y);
        }

        public void SetFont(SpriteFont font)
        {
            Font = font;
        }

        public void SetSize(float w, float h)
        {
            W = w;
            H = h;
        }

        public Vector2 GetSize()
        {
            return new Vector2(W, H);
        }

        public Vector2 GetPos()
        {
            return new Vector2(X, Y);
        }
    }
}
