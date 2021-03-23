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
        public DragDrop(Tile obj)
        {
            
        }

        static private void DoDrop(int index, Vector2 pos) // Kallas när användaren släppt
        {
            Debug.WriteLine("Dropped " + index + " at position (" + pos.X + ", " + pos.Y + ')');

            Tile tile = Hand.Tiles[index];

            // TODO: Sök för en receiver 
        }

        static public void Think(GameWindow window)
        {
            MouseState mouse = Mouse.GetState(window);

            for (int i = 0; i < Hand.Tiles.Length; i++)
            {
                if (Hand.Tiles[i] == null)
                    continue;

                if (Hand.Tiles[i].Dragging)
                {
                    if(mouse.LeftButton == ButtonState.Pressed)
                    {
                        Hand.Tiles[i].SetPos(mouse.X + mouse.X - Hand.Tiles[i].X - Hand.TilesWidth, mouse.Y - Hand.TilesHeight / 2);
                    }
                    else
                    {
                        DoDrop(i, Hand.Tiles[i].GetPos()); // Kalla funktionen när användaren släppt 
                        Hand.BeingDragged = false;
                        Hand.Tiles[i].Dragging = false;
                    }
                    continue;
                }
                else if (Hand.BeingDragged) // Om detta objekt inte blir draggen fast någon annan blir det, fortsätt till nästa
                    continue;

                if (mouse.LeftButton == ButtonState.Pressed && mouse.X >= Hand.Tiles[i].X && mouse.X <= Hand.Tiles[i].X + Hand.TilesWidth && mouse.Y >= Hand.Tiles[i].Y && mouse.Y <= Hand.Tiles[i].Y + Hand.TilesHeight)
                {
                    Hand.Tiles[i].Dragging = true;
                    Hand.BeingDragged = true;
                    Hand.Tiles[i].SetPos(mouse.X + Hand.Tiles[i].X - mouse.X, mouse.Y);
                }
            }
        }
    }
}
