using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Alfapet
{
    public class DragDrop : Game
    {
        private static bool Dragging;

        private enum Move
        {
            Place, // Placerar en bricka på brädan (från handen)
            Change, // Byter brickor i brädan 
            Remove // Lägger tillbaka brickan i handen från bordet
        }

        /*
         * Kallas när användaren släppt en bricka.
         * Gör olika saker beroende på vilken Move typ
        */
        private static void DoDrop(dynamic index, Tile originTile, Move moveType)
        {
            // Behöver inte en destinations bricka
            if (moveType == Move.Remove)
            {
                originTile.ResetPos(); // Sätter tillbaka brickans positionens till bordet
                
                Hand.InsertLetter(originTile.Letter);
                
                // Brickan på bordet borde nu vara tom
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
                        // Om muspekaren är över brickan, är det den användaren vill släppa på
                        if (Util.IsHovering(destinationTile.GetPos(), originTile.GetSize()))
                        {
                            // Låt inte användaren släppa på en bricka med en boktav
                            if (destinationTile.Letter != '\0')
                                break;
                            
                            switch (moveType)
                            {
                                case Move.Place:
                                    // Sätter brickan på bordet till bokstaven i handen
                                    destinationTile.Letter = originTile.Letter;
                                    destinationTile.TempPlaced = true; // Nya brickor måste markeras som temporärt placerade

                                    Hand.Tiles[index].Letter = '\0';
                                    
                                    Board.TilesPlaced++;
                                    Board.CheckTilesPlaced();
                                    
                                    break;
                                case Move.Change:
                                    // Byter brickorna på bordet
                                    destinationTile.Letter = originTile.Letter;
                                    destinationTile.TempPlaced = true;

                                    // Återställ brickan man bytt från
                                    originTile.ResetPos();
                                    originTile.TempPlaced = false;
                                    originTile.Letter = '\0';

                                    break;
                            }

                            break;
                        }
                        // Om man inte hittar, sätt tillbaka brickans position
                        originTile.ResetPos();
                    }
                }
            }
            Hand.SetPositions();
        }

        /*
         * Kollar om man försöker dra på en bricka
        */
        private static void CheckDrag(dynamic index, Tile tile, Move moveType)
        {
            if (tile.Letter == '\0')
                return;

            var mouse = Mouse.GetState(Alfapet.Window);
            if (tile.Dragging)
            {
                // Man håller på att dra och leftclick är fortfarande nere, brickan fortsätter att följa musen
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    tile.SetPos(mouse.X - tile.W / 2, mouse.Y - tile.H / 2); // Sätter brickan till i mitten av muspekaren
                }
                // Man håller på att dra och inte längre håller leftclick, användaren vill släppa brickan
                else
                {
                    // Inget dras längre
                    Dragging = false;
                    tile.Dragging = false;
                    
                    DoDrop(index, tile, moveType);
                }
            }
            // Man drar en bricka men inte denna
            else if (Dragging)
            {
                return;
            }

            // Om man håller leftclick och är över en bricka, börja dra på brickan
            if (mouse.LeftButton == ButtonState.Pressed && Util.IsHovering(tile.GetPos(), new Vector2(Hand.TilesWidth, Hand.TilesHeight)))
            {
                tile.Dragging = true;
                Dragging = true;
            }
        }

        public static void Check()
        {
            // Kolla inget om det inte är användarens tur
            if (Ai.Playing)
                return;
            
            // Kolla om brickor i handen blir dragna
            for (var i = 0; i < Hand.Tiles.Length; i++)
            {
                CheckDrag(i, Hand.Tiles[i], Move.Place);
            }
            // Kolla brickorna på bordet
            for (var y = 0; y < Board.YTiles; y++)
            {
                for (var x = 0; x < Board.XTiles; x++)
                {
                    // Låt inte användaren röra fasta eller bottens brickor
                    if (!Board.Tiles[y, x].TempPlaced)
                        continue;

                    // Om muspekaren är vid handens position vill man ta bort, annars flytta till en annan bricka på bordet
                    var mouseInHandRange = Mouse.GetState(Alfapet.Window).Y > (Board.YTiles + 2) * Board.TilesHeight;
                    CheckDrag(new Vector2(x, y), Board.Tiles[y, x], mouseInHandRange ? Move.Remove : Move.Change);
                }
            }
        }
    }
}