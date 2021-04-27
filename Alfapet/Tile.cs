using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;

namespace Alfapet
{
    public class Tile : Game
    {
        public float X, Y;
        public float W, H;
        public char Letter = '\0';
        public SpriteFont Font;
        public bool TempPlaced = false;

        public bool Dragging = false;
        public bool Changed = false;

        public Vector2 originalPos = default(Vector2);

        public Tile()
        {

        }

        public void SetPos(float x, float y)
        {
            this.X = x;
            this.Y = y;
            if (originalPos == Vector2.Zero)
                originalPos = new Vector2(x, y);
        }

        public void SetFont(SpriteFont font)
        {
            this.Font = font;
        }

        public void SetSize(float w, float h)
        {
            this.W = w;
            this.H = h;
        }

        public Vector2 GetPos()
        {
            return new Vector2(X, Y);
        }
    }
}