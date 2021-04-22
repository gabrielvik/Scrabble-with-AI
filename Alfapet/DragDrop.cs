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
        public static bool Dragging = false;
        public enum MOVE 
        { 
            PLACE,
            CHANGE
        }

        static private void DoDrop(dynamic index, Tile tile, MOVE moveType) // Kallas när användaren släppt
        {
            bool foundReceiver = false;
            for(int y = 0; y < Board.YTiles; y++)
            {
                for (int x = 0; x < Board.XTiles; x++)
                {
                    Tile _tile = Board.Tiles[y, x];
                    if (Alfapet_Util.IsHovering(_tile.GetPos(), new Vector2(_tile.W, _tile.H)))
                    {
                        if (_tile.Letter != '\0')
                        { // Om platsen inte är tom returnar man
                            // TODO: positionsf ick
                            continue;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("gets here");
                            foundReceiver = true;
                            switch (moveType)
                            {
                                case MOVE.PLACE:
                                    Board.CacheWordPlacement((int)x, (int)y, tile.Letter);

                                    _tile.Letter = tile.Letter;
                                    _tile.TempPlaced = true;

                                    Hand.Tiles[index] = null;

                                    break;
                                case MOVE.CHANGE:
                                    _tile.Letter = tile.Letter;
                                    _tile.TempPlaced = true;

                                    tile.SetPos(tile.originalPos.X, tile.originalPos.Y);
                                    tile.TempPlaced = false;

                                    tile.Letter = '\0';

                                    Board.CacheWordPlacement((int)x, (int)y, tile.Letter);

                                    break;
                            }
                            Hand.SetPositions();
                        }
                    }
                }
            }
            if (!foundReceiver)
            {
                tile.SetPos(tile.originalPos.X,tile.originalPos.Y);
            }

        }

        public static void CheckDrag(dynamic index, Tile tile, MOVE moveType)
        {
            MouseState mouse = Mouse.GetState(Alfapet._window);
                if (tile == null)
                    return;

                if (tile.Dragging)
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        // Debug.WriteLine(mouse.X - Hand.Tiles[i].X);
                        tile.SetPos(mouse.X - tile.W / 2, mouse.Y - tile.H / 2);
                    }
                    else
                    {
                        Dragging = false;
                        tile.Dragging = false;
                        DoDrop(index, tile, moveType);
                    }
                    return;
                }
                else if (Dragging) // Om detta objekt inte blir draggen fast någon annan blir det, fortsätt till nästa
                    return;

                if (mouse.LeftButton == ButtonState.Pressed && Alfapet_Util.IsHovering(tile.GetPos(), new Vector2(Hand.TilesWidth, Hand.TilesHeight)))
                {
                    tile.Dragging = true;
                    Dragging = true;
                }
        }

        static public void Think()
        {
            for(int i = 0; i < Hand.Tiles.Length; i++)
            {
                CheckDrag(i, Hand.Tiles[i], MOVE.PLACE);
            }
            for(int y = 0; y < Board.YTiles; y++)
            {
                for(int x = 0; x < Board.XTiles; x++)
                {
                    if (!Board.Tiles[y, x].TempPlaced)
                        continue;
                    else
                        CheckDrag(new Vector2(x, y), Board.Tiles[y, x], MOVE.CHANGE);
                }
            }
        }
    }
}