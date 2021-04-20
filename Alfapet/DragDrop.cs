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
        static private void DoDrop(dynamic index, Tile tile, Action<dynamic, Vector2, Tile, Tile> callback = null, Action<dynamic, Tile> failCallback = null) // Kallas när användaren släppt
        {
            bool foundReceiver = false;
            for(int y = 0; y < Board.YTiles; y++)
            {
                for (int x = 0; x < Board.XTiles; x++)
                {
                    Tile _tile = Board.Tiles[y, x];
                    if (Alfapet_Util.IsHovering(_tile.GetPos(), new Vector2(tile.W, tile.H)))
                    {
                        if (_tile.Letter != '\0') // Om platsen inte är tom returnar man
                            continue;
                        else
                            foundReceiver = true;
                            callback?.Invoke(index, new Vector2(x, y), tile, _tile);
                    }
                }
            }
            if (!foundReceiver)
            {
                System.Diagnostics.Debug.WriteLine("no receiver");
                tile.SetPos(tile.originalPos.X, tile.originalPos.Y);
                if(failCallback != null)
                    failCallback?.Invoke(index, tile);
            }

        }

        public static void CheckDrag(dynamic index, Tile tile, Action<dynamic, Vector2, Tile, Tile> callback = null, Action<dynamic, Tile> failCallback = null)
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
                        DoDrop(index, tile, callback, failCallback);
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

        static public void Think(GameWindow window)
        {
            for(int i = 0; i < Hand.Tiles.Length; i++)
            {
                CheckDrag(i, Hand.Tiles[i], Hand.DragCallback);
            }
            for(int y = 0; y < Board.YTiles; y++)
            {
                for(int x = 0; x < Board.XTiles; x++)
                {
                    if (!Board.Tiles[y, x].TempPlaced)
                        continue;
                    else
                        CheckDrag(new Vector2(x, y), Board.Tiles[y, x], Board.TempDragCallback, Board.FailDragCallback);
                }
            }
        }
    }
}