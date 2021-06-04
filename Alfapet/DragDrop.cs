using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Alfapet
{
    public class DragDrop : Game
    {
        public static bool Dragging = false;
        public enum MOVE
        {
            PLACE, // Placerar en bricka på brädan (från handen)
            CHANGE, // Byter brickor i brädan 
            REMOVE // Lägger tillbaka brickan i handen från bordet
        }

        /*
         * Kallas när användaren släppt en bricka.
         * 
         */
        private static void DoDrop(dynamic index, Tile tile, MOVE moveType)
        {
            if (moveType == MOVE.REMOVE) // Om man stoppar tillbaka brickan i handen
            {
                tile.SetPos(tile.originalPos.X, tile.originalPos.Y); // Sätt brickan som var på bordet tillbaka
                foreach (Tile _tile in Hand.Tiles) // Loopa igenom handen tills man hittar en tomm bricka
                {
                    if (_tile.Letter == '\0')
                    {
                        _tile.Letter = tile.Letter; // Sätt hand brickans bokstav till vad bokstaven på bordet var
                        break;
                    }
                }
                tile.Letter = '\0'; // Brickan på bordet borde vara tomm
                tile.TempPlaced = false;

                Board.TilesPlaced--;
                if (Board.TilesPlaced <= 0) // Om man har placerat mindre än 1 bricka på bordet, sätt text till "skip"
                    ButtonRig.Buttons[0].SetText("Skip");
            }
            else
            {
                for (int y = 0; y < Board.YTiles; y++) // Loop igenom bordets brickor med y, x
                {
                    for (int x = 0; x < Board.XTiles; x++)
                    {
                        Tile destinationTile = Board.Tiles[y, x];
                        if (Alfapet_Util.IsHovering(destinationTile.GetPos(), new Vector2(destinationTile.W, destinationTile.H))) // Om muspekaren är över brickan
                        {
                            if (destinationTile.Letter != '\0') // Om platsen inte är tom returnar man
                            {
                                break;
                            }
                            else
                            {
                                switch (moveType)
                                {
                                    case MOVE.PLACE:
                                        destinationTile.Letter = tile.Letter; // Byt bordets brickas bokstav till handens bokstav
                                        destinationTile.TempPlaced = true; // Nya brickor måste markeras som temporärt placerade

                                        Hand.Tiles[index].Letter = '\0'; // Sätt handens brickas bokstav till tomm
                                        Board.TilesPlaced++;
                                        ButtonRig.Buttons[0].SetText("Move"); // Man kommer ha placerat mer än 1 bokstav
                                        break;
                                    case MOVE.CHANGE:
                                        destinationTile.Letter = tile.Letter;
                                        destinationTile.TempPlaced = true;

                                        tile.SetPos(tile.originalPos.X, tile.originalPos.Y);
                                        tile.TempPlaced = false;

                                        tile.Letter = '\0';

                                        break;
                                }
                            }
                        }
                        else
                        {
                            tile.SetPos(tile.originalPos.X, tile.originalPos.Y);
                        }
                    }
                }
            }
            Hand.SetPositions();
        }

        public static void CheckDrag(dynamic index, Tile tile, MOVE moveType)
        {
            if (tile == null || tile.Letter == '\0')
                return;

            MouseState mouse = Mouse.GetState(Alfapet._window);

            if (tile.Dragging)
            {
                if (mouse.LeftButton == ButtonState.Pressed) // Om man håller leftclick
                {
                    tile.SetPos(mouse.X - tile.W / 2, mouse.Y - tile.H / 2); // Sätter brickan till i mitten av muspekaren
                }
                else // Man drar en bricka men har slutat hålla leftclick
                {
                    Dragging = false;
                    tile.Dragging = false;
                    DoDrop(index, tile, moveType); // Försök hitta en receiver
                }
            }
            else if (Dragging) // Om detta objekt inte blir draggen fast någon annan blir det, fortsätt till nästa
                return;

            // Om man håller leftclick och är över en bricka, börja dra på brickan
            if (mouse.LeftButton == ButtonState.Pressed && Alfapet_Util.IsHovering(tile.GetPos(), new Vector2(Hand.TilesWidth, Hand.TilesHeight)))
            {
                tile.Dragging = true;
                Dragging = true;
            }
        }

        public static void Think()
        {
            for (int i = 0; i < Hand.Tiles.Length; i++)
            {
                CheckDrag(i, Hand.Tiles[i], MOVE.PLACE);
            }
            for (int y = 0; y < Board.YTiles; y++)
            {
                for (int x = 0; x < Board.XTiles; x++)
                {
                    if (!Board.Tiles[y, x].TempPlaced || !Board.Tiles[y, x].PlayerPlaced) // Man ska bara kunna ta bort och byta plats på brickor som är temporerade placerade
                        continue;
                    else
                        CheckDrag(new Vector2(x, y), Board.Tiles[y, x], Mouse.GetState(Alfapet._window).Y > (Board.YTiles + 2) * Board.TilesHeight ? MOVE.REMOVE : MOVE.CHANGE);
                }
            }

        }
    }
}