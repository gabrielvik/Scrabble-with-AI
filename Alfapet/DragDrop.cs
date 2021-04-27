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
            CHANGE,
            REMOVE
        }

        static private void DoDrop(dynamic index, Tile tile, MOVE moveType) // Kallas när användaren släppt
        {
            if(moveType == MOVE.REMOVE)
            {
                tile.SetPos(tile.originalPos.X, tile.originalPos.Y);
                foreach(Tile _tile in Hand.Tiles)
                {
                    if (_tile.Letter == '\0')
                    {
                        _tile.Letter = tile.Letter;
                        break;
                    }
                }
                Hand.SetPositions();
                tile.Letter = '\0';
                tile.TempPlaced = false;
                return;
            }
            for(int y = 0; y < Board.YTiles; y++)
            {
                for (int x = 0; x < Board.XTiles; x++)
                {
                    Tile _tile = Board.Tiles[y, x];
                    if (Alfapet_Util.IsHovering(_tile.GetPos(), new Vector2(_tile.W, _tile.H)))
                    {
                        if (_tile.Letter != '\0')
                        { // Om platsen inte är tom returnar man
                            Hand.SetPositions();
                            break;
                        }
                        else
                        {
                            //System.Diagnostics.Debug.WriteLine("gets here");
                            switch (moveType)
                            {
                                case MOVE.PLACE:
                                   

                                    _tile.Letter = tile.Letter;
                                    _tile.TempPlaced = true;

                                    Hand.Tiles[index].Letter = '\0';

                                    break;
                                case MOVE.CHANGE:
                                   

                                    _tile.Letter = tile.Letter;
                                    _tile.TempPlaced = true;

                                    tile.SetPos(tile.originalPos.X, tile.originalPos.Y);
                                    tile.TempPlaced = false;

                                    tile.Letter = '\0';

                                    break;
                                case MOVE.REMOVE:
                                    Debug.WriteLine("awwdawawawd");
                                    break;
                            }
                            Hand.SetPositions();
                        }
                    }
                    else
                        tile.SetPos(tile.originalPos.X, tile.originalPos.Y);
                }
            }
        }

        public static void CheckDrag(dynamic index, Tile tile, MOVE moveType)
        {
            MouseState mouse = Mouse.GetState(Alfapet._window);
                if (tile == null || tile.Letter == '\0')
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
                        CheckDrag(new Vector2(x, y), Board.Tiles[y, x], Mouse.GetState(Alfapet._window).Y > (Board.YTiles + 2) * Board.TilesHeight ? MOVE.REMOVE : MOVE.CHANGE);
                }
            }

        }
    }
}