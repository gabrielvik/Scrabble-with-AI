using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Alfapet
{
    public class DragDrop : Game
    {
        public DragDrop(GameObject obj)
        {
            
        }
        static public void Think(GameWindow window)
        {
            MouseState mouse = Mouse.GetState(window);

            Debug.WriteLine(mouse.X);

            for (int i = 0; i < Hand.HandTiles.Length; i++)
            {
                if (Hand.HandTiles[i] == null)
                    continue;

                if (mouse.LeftButton != ButtonState.Pressed)
                {
                    if (Hand.HandTiles[i].BeingDragged)
                    {
                        Hand.HandTiles[i].BeingDragged = false;
                        // släppt 
                    }

                    Debug.WriteLine("EXIT");
                    return;
                }

                if (mouse.X >= Hand.HandTiles[i].X && mouse.X <= Hand.HandTiles[i].X + Hand.TilesWidth && mouse.Y >= Hand.HandTiles[i].Y && mouse.Y <= Hand.HandTiles[i].Y + Hand.TilesHeight)
                {
                    Hand.HandTiles[i].BeingDragged = true;
                    Hand.HandTiles[i].SetPos(mouse.X - Hand.TilesWidth / 2, mouse.Y - Hand.TilesHeight / 2);

                }
            }
        }
    }
}
