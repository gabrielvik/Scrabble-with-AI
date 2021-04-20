using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alfapet
{
    class Rounds : Game
    {
        public static void DoMove()
        {
            if (!Board.PlacedValidWord)
            {
                // notifier
                return;
            }
            
            foreach(Tile tile in Board.Tiles)
            {
                if (tile.TempPlaced)
                    tile.TempPlaced = false;
            }


        }
    }
}
