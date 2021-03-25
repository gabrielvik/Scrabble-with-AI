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

        static private void DoDrop(int index, Vector2 pos, GameWindow window) // Kallas när användaren släppt
        {
            Debug.WriteLine("Dropped " + index + " at position (" + pos.X + ", " + pos.Y + ')');

            Tile tile = Hand.Tiles[index];

            for(int i = 0; i < Board.Tiles.Length; i++)
            {
                if(Alfapet_Util.IsHovering(pos, new Vector2(Hand.TilesWidth, Hand.TilesHeight), window))
                {
                    Debug.WriteLine(i);
                }
            }
            
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
                       // Debug.WriteLine(mouse.X - Hand.Tiles[i].X);
                       Hand.Tiles[i].SetPos(mouse.X - Hand.TilesWidth / 2, mouse.Y - Hand.TilesHeight / 2);
                    }
                    else
                    {
                        DoDrop(i, Hand.Tiles[i].GetPos(), window); // Kalla funktionen när användaren släppt 
                        Hand.BeingDragged = false;
                        Hand.Tiles[i].Dragging = false;
                    }
                    continue;
                }
                else if (Hand.BeingDragged) // Om detta objekt inte blir draggen fast någon annan blir det, fortsätt till nästa
                    continue;

                if (mouse.LeftButton == ButtonState.Pressed && Alfapet_Util.IsHovering(Hand.Tiles[i].GetPos(), new Vector2(Hand.TilesWidth, Hand.TilesHeight), window))
                {
                    Hand.Tiles[i].Dragging = true;
                    Hand.BeingDragged = true;
                }
            }
        }
    }
}
