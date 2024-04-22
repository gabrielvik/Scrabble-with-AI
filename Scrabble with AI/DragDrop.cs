using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Alfapet
{
    public class DragDrop : Game
    {
        private static bool Dragging;

        private enum Move
        {
            Place,
            Change,
            Remove
        }

        /*
         * Called when the user releases a tile.
         * Performs different actions depending on the Move type.
        */
        private static void DoDrop(dynamic index, Tile originTile, Move moveType)
        {
            // No destination tile needed
            if (moveType == Move.Remove)
            {
                originTile.ResetPos(); // Resets the tile's position to the board

                Hand.InsertLetter(originTile.Letter);

                // The tile on the board should now be empty
                originTile.Letter = '\0';
                originTile.TempPlaced = false;

                Board.TilesPlaced--;
                Board.CheckTilesPlaced();
            }
            else
            {
                for (var y = 0; y < Board.YTiles; y++)
                {
                    for (var x = 0; x < Board.XTiles; x++)
                    {
                        var destinationTile = Board.Tiles[y, x];
                        // If the cursor is over the tile, it is where the user wants to drop
                        if (Util.IsHovering(destinationTile.GetPos(), originTile.GetSize()))
                        {
                            // Prevent the user from dropping onto a tile with a letter
                            if (destinationTile.Letter != '\0')
                                break;

                            switch (moveType)
                            {
                                case Move.Place:
                                    // Sets the tile on the board to the letter in hand
                                    destinationTile.Letter = originTile.Letter;
                                    destinationTile.TempPlaced = true; // New tiles must be marked as temporarily placed

                                    Hand.Tiles[index].Letter = '\0';

                                    Board.TilesPlaced++;
                                    Board.CheckTilesPlaced();

                                    break;
                                case Move.Change:
                                    // Swaps the tiles on the board
                                    destinationTile.Letter = originTile.Letter;
                                    destinationTile.TempPlaced = true;

                                    // Reset the tile being swapped from
                                    originTile.ResetPos();
                                    originTile.TempPlaced = false;
                                    originTile.Letter = '\0';

                                    break;
                            }

                            break;
                        }
                        // If not found, reset the tile's position
                        originTile.ResetPos();
                    }
                }
            }
            Hand.SetPositions();
        }

        /*
         * Checks if a tile is being dragged.
        */
        private static void CheckDrag(dynamic index, Tile tile, Move moveType)
        {
            if (tile.Letter == '\0')
                return;

            var mouse = Mouse.GetState(Alfapet.Window);
            if (tile.Dragging)
            {
                // Currently dragging and left-click is still held, the tile continues to follow the mouse
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    tile.SetPos(mouse.X - tile.W / 2, mouse.Y - tile.H / 2); // Sets the tile to the center of the cursor
                }
                // Currently dragging but no longer holding left-click, user wants to drop the tile
                else
                {
                    // No longer dragging
                    Dragging = false;
                    tile.Dragging = false;

                    DoDrop(index, tile, moveType);
                }
            }
            // Dragging a tile but not this one
            else if (Dragging)
            {
                return;
            }

            // If holding left-click and over a tile, start dragging the tile
            if (mouse.LeftButton == ButtonState.Pressed && Util.IsHovering(tile.GetPos(), new Vector2(Hand.TilesWidth, Hand.TilesHeight)))
            {
                tile.Dragging = true;
                Dragging = true;
            }
        }

        public static void Check()
        {
            // Do nothing if it's not the user's turn
            if (Ai.Playing)
                return;

            // Check if tiles in the hand are being dragged
            for (var i = 0; i < Hand.Tiles.Length; i++)
            {
                CheckDrag(i, Hand.Tiles[i], Move.Place);
            }
            // Check the tiles on the board
            for (var y = 0; y < Board.YTiles; y++)
            {
                for (var x = 0; x < Board.XTiles; x++)
                {
                    // Prevent the user from moving fixed or bottom tiles
                    if (!Board.Tiles[y, x].TempPlaced)
                        continue;

                    // If the cursor is at the hand's position, want to remove; otherwise, move to another tile on the board
                    var mouseInHandRange = Mouse.GetState(Alfapet.Window).Y > (Board.YTiles + 2) * Board.TilesHeight;
                    CheckDrag(new Vector2(x, y), Board.Tiles[y, x], mouseInHandRange ? Move.Remove : Move.Change);
                }
            }
        }
    }
}
