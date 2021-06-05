﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public bool PlayerPlaced = true;

        public Vector2 OriginalPos = default(Vector2);

        public Tile()
        {

        }

        public void SetPos(float x, float y)
        {
            this.X = x;
            this.Y = y;
            if (OriginalPos == Vector2.Zero)
                OriginalPos = new Vector2(x, y);
        }

        public void ResetPos()
        {
            SetPos(OriginalPos.X, OriginalPos.Y);
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