using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Alfapet
{
    class StartScreen : Game
    {
        public static string LoadString = "";
        private static Button PlayBtn;
        
        /*
         * När användaren vill börja spelet, ändrar på draw och update funktionen
         * Sätter hur många brickor på bordet och på handen
        */
        private static async void Start(int boardTiles, int handTiles)
        {
            Board.XTiles = Board.YTiles = boardTiles;
            Hand.Amount = handTiles;
            
            // Sätter draw funktionen till att visa LoadString
            Alfapet.DrawFunction = delegate
            {
                Ui.DrawCenterText(Ui.MontserratBoldSmaller, LoadString, new Vector2(0, 0),
                    new Vector2(Alfapet.Graphics.GraphicsDevice.Viewport.Width,
                        Alfapet.Graphics.GraphicsDevice.Viewport.Height), Color.White);
            };
            
            await Alfapet.Start();
            
            Alfapet.UpdateFunction = delegate
            {
                DragDrop.Check();
                Button.ListenForPresses();
            };

            Alfapet.DrawFunction = delegate
            {
                Board.Draw();
                Hand.Draw();
                Button.Draw();
                Notifications.Draw();
            };
        }

        /*
         * Skapar en meny där man väljer hur många brickor på brädan och handen
        */
        private static void BuildGameSettingsMenu()
        {
            // Ta bort play knappen om den finns
            if (PlayBtn != null)
                Button.List.Remove(PlayBtn);

            // Default värden på hur många brickor på brädan och i handen
            var boardNumAmount = 9;
            var handNumAmount = 8;
            
            // Storleken på rektanglerna
            var height = 40;
            var width = 125;

            var x = Alfapet.Graphics.GraphicsDevice.Viewport.Width / 2 - width / 2;
            var y = Alfapet.Graphics.GraphicsDevice.Viewport.Height / 2 - height / 2 - height * 2;
            Alfapet.DrawFunction = delegate
            {
                Button.Draw();

                // Text över rektangeln
                Ui.DrawCenterText(Ui.MontserratBoldSmaller, "Board Tiles Amount", new Vector2(0, y - 24 - 10),
                    new Vector2(Alfapet.Graphics.GraphicsDevice.Viewport.Width, 0), Color.White);
                Ui.DrawCenterText(Ui.MontserratBoldSmaller, "Hand Tiles Amount", new Vector2(0, y + 64 + height - 10),
                    new Vector2(Alfapet.Graphics.GraphicsDevice.Viewport.Width, 0), Color.White);
                
                // Rektangeln där det står hur många brickor på bordet
                var tilesNumRec = new Rectangle(x, y, width, height);
                Ui.OutlinedRectangle(tilesNumRec);
                Ui.DrawCenterText(Ui.MontserratBoldSmaller, boardNumAmount + "x" + boardNumAmount,
                    new Vector2(tilesNumRec.X, tilesNumRec.Y), new Vector2(tilesNumRec.Width, tilesNumRec.Height),
                    Color.White);
                
                // Rektangeln där det står hur många brickor i handen
                var handTilesNumRec = new Rectangle(x, y + 64 + height + 24, width, height);
                Ui.OutlinedRectangle(handTilesNumRec);
                Ui.DrawCenterText(Ui.MontserratBoldSmaller, handNumAmount.ToString(),
                    new Vector2(handTilesNumRec.X, handTilesNumRec.Y),
                    new Vector2(handTilesNumRec.Width, handTilesNumRec.Height), Color.White);
            };
            
            // Knappar för att ändra hur många brickor på brädan
            var boardNumPrev = new Button();
            boardNumPrev.SetSize(40, height);
            boardNumPrev.SetPos(x - boardNumPrev.W - 5, y);
            boardNumPrev.SetText("<");

            boardNumPrev.ClickEvent = delegate
            {
                boardNumAmount = Math.Max(boardNumAmount - 1, 7);
            };
            
            var boardNumNext = new Button();
            boardNumNext.SetSize(40, height);
            boardNumNext.SetPos(x + width + 5, y);
            boardNumNext.SetText(">");
            
            boardNumNext.ClickEvent = delegate
            {
                boardNumAmount = Math.Min(boardNumAmount + 1, 14);
            };
            
            // Knappar för att ändra hur många brickor i handen
            var handNumPrev = new Button();
            handNumPrev.SetSize(40, height);
            handNumPrev.SetPos(x - handNumPrev.W - 5, y + 64 + height + 24);
            handNumPrev.SetText("<");

            handNumPrev.ClickEvent = delegate
            {
                handNumAmount = Math.Max(handNumAmount - 1, 6);
            };
            
            var handNumNext = new Button();
            handNumNext.SetSize(40, height);
            handNumNext.SetPos(x + width + 5, y + 64 + height + 24);
            handNumNext.SetText(">");
            
            handNumNext.ClickEvent = delegate
            {
                handNumAmount = Math.Min(handNumAmount + 1, 13);
            };

            // Knappen för att starta spelet
            var start = new Button();
            start.SetSize(80 + 10 + width, 75);
            start.SetPos(boardNumPrev.X, handNumNext.Y + 150);
            start.SetText("Start");
            
            start.ClickEvent = delegate
            {
                Button.List = new List<Button>();
                Start(boardNumAmount, handNumAmount);
            };
        }
        
        private static void BuildPlayBtn()
        {
            PlayBtn = new Button();
            PlayBtn.SetSize(200, 75);
            PlayBtn.SetPos(Alfapet.Graphics.GraphicsDevice.Viewport.Width / 2 - 100, Alfapet.Graphics.GraphicsDevice.Viewport.Height / 2 + 75);
            PlayBtn.SetText("Play");

            PlayBtn.ClickEvent = BuildGameSettingsMenu;
        }

        public new static void Initialize()
        {
            Alfapet.DrawFunction = delegate ()
            {
                Button.Draw();

                // Ritar spelets titel i mitten av skärmen
                Ui.DrawCenterText(Ui.MontserratBold, Config.GameTitle, new Vector2(0, 0),
                    new Vector2(Alfapet.Graphics.GraphicsDevice.Viewport.Width,
                        Alfapet.Graphics.GraphicsDevice.Viewport.Height / 2), Color.White);
            };
            Alfapet.UpdateFunction = Button.ListenForPresses;

            BuildPlayBtn();
        }
    }
}
