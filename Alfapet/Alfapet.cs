using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;

namespace Alfapet
{
    public class Alfapet : Game
    {
        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;

        //private Button btn = new Button();

        //private List<Tile> objects;

        public static Texture2D TransparentBack;

        public Alfapet()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            IsFixedTimeStep = false; // tar bort FPS cap
            //Window.Position = new Point(-1500, 0);
            _graphics.ApplyChanges();

            Board.Build();
            Hand.Build();
            Dictionaries.Initialize("english");
            ButtonRig.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TransparentBack = new Texture2D(GraphicsDevice, 1, 1);
            TransparentBack.SetData(new Color[] { Color.White * 0.5f });

            Fonts.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            /*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); 
            */
            
            DragDrop.Think(Window);
            Button.Think(Window);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(47, 54, 64));

            _spriteBatch.Begin();
                Board.Draw();
                Hand.Draw();
                Button.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
