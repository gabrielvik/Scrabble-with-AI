using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;

namespace Alfapet
{
    public class GameObject : Game
    {
        private Texture2D texture;
        public float X, Y;
        private static List<GameObject> GameObjects = new List<GameObject>();

        private dynamic draw_object; // dynamisk så man kan sätta till vilket objekt som helst 

        public int Index;

        private static int index_i = 0;

        public char Letter;

        public bool BeingDragged = false;

        public GameObject()
        {
            GameObjects.Add(this); // lägg till objektet i listan
            Index = index_i; // 
            index_i++;
        }

        public void SetTexture(Texture2D texture)
        {
            this.texture = texture;
            GameObjects[Index].texture = texture;
        }

        public void SetDrawObject(dynamic draw_object)
        {
            this.draw_object = draw_object;
        }

        /*public void SetSize(int w, int h)
        {
            this.w = w;
            this.h = h;
            GameObjects[Index].w = w;
            GameObjects[Index].h = h;
        }
        */
        public void SetPos(float x, float y)
        {
            this.X = x;
            this.Y = y;
            //GameObjects[Index].x = x;
            //GameObjects[Index].y = y;
        }

        public Vector2 GetPos()
        {
            return new Vector2(X, Y);
        }

        public static List<GameObject> GetAllObjects()
        {
            return GameObjects;
        }
    }
}