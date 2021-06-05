using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using SharpDX;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Alfapet
{
    class StartScreen : Game
    {
        public static string LoadString = "";
        private static Button PlayBtn;

        private static async void Start(int boardTiles, int handTiles)
        {
            Board.XTiles = Board.YTiles = boardTiles;
            AlfapetConfig.HandAmount = handTiles;
            
            Alfapet.DrawFunction = delegate ()
            {
                UI.DrawCenterText(UI.MontserratBoldSmaller, LoadString, new Vector2(0, 0), Color.White, Alfapet.Graphics.GraphicsDevice.Viewport.Width, Alfapet.Graphics.GraphicsDevice.Viewport.Height);
            };
            
            await Alfapet.Start();
            
            Alfapet.UpdateFunction = delegate ()
            {
                DragDrop.Think();
                Button.ListenForPresses();
            };

            Alfapet.DrawFunction = delegate ()
            {
                Board.Draw();
                Hand.Draw();
                Button.Draw();
            };
        }

        private static void BuildGameSettingsMenu()
        {
            if (PlayBtn != null)
                Button.List.Remove(PlayBtn);

            var boardNumAmount = 9;
            var handNumAmount = 8;
            
            var height = 40;
            var width = 125;
            var x = Alfapet.Graphics.GraphicsDevice.Viewport.Width / 2 - width / 2;
            var y = Alfapet.Graphics.GraphicsDevice.Viewport.Height / 2 - height / 2 - height * 2;
            Alfapet.DrawFunction = delegate
            {
                Button.Draw();

                UI.DrawCenterText(UI.MontserratBoldSmaller, "Board Tiles Amount", new Vector2(0, y - 24 - 10), Color.White, Alfapet.Graphics.GraphicsDevice.Viewport.Width, 0);
                UI.DrawCenterText(UI.MontserratBoldSmaller, "Hand Tiles Amount", new Vector2(0, y + 64 + height - 10), Color.White, Alfapet.Graphics.GraphicsDevice.Viewport.Width, 0);
                
                var tilesNumRec = new Rectangle(x, y, width, height);
                UI.StylishRectangle(tilesNumRec);
                UI.DrawCenterText(UI.MontserratBoldSmaller, boardNumAmount+ "x" + boardNumAmount, new Vector2(tilesNumRec.X, tilesNumRec.Y), Color.White, tilesNumRec.Width, tilesNumRec.Height);
                
                var handTilesNumRec = new Rectangle(x, y + 64 + height + 24, width, height);
                UI.StylishRectangle(handTilesNumRec);
                UI.DrawCenterText(UI.MontserratBoldSmaller, handNumAmount.ToString(), new Vector2(handTilesNumRec.X, handTilesNumRec.Y), Color.White, handTilesNumRec.Width, handTilesNumRec.Height);
            };
            
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
                UI.DrawCenterText(UI.MontserratBold, "GameTitle", new Vector2(0, 0), Color.White, Alfapet.Graphics.GraphicsDevice.Viewport.Width, Alfapet.Graphics.GraphicsDevice.Viewport.Height / 2);
            };
            Alfapet.UpdateFunction = Button.ListenForPresses;

            BuildPlayBtn();
        }
    }
}
