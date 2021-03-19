using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace Alfapet
{
    public class GameObject : Game
    {
        private Texture2D texture;
        //private int x, y, w, h;
        private static List<GameObject> GameObjects = new List<GameObject>();

        private dynamic draw_object; // dynamisk så man kan sätta till vilket objekt som helst 

        public int Index;

        private static int index_i = 0;

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

        public void SetPos(int x, int y)
        {
            this.x = x;
            this.y = y;
            GameObjects[Index].x = x;
            GameObjects[Index].y = y;
        }*/

        public static List<GameObject> GetAllObjects()
        {
            return GameObjects;
        }

        public void Draw()
        {
            for (int i = 0; i < GameObjects.Count i++)
            {
                Alfapet._spriteBatch.Draw(this.texture, this.draw_object, Color.White);
            }
        }
    }
}