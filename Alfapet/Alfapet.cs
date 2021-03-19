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

        private Hand hand = new Hand();

        private List<GameObject> objects;

        public static Texture2D TransparentBack;

        public Alfapet()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            objects = GameObject.GetAllObjects();

            _graphics.PreferredBackBufferWidth = 900;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 800;   // set this value to the desired height of your window
            _graphics.ApplyChanges();

            hand.Init();

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
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(47, 54, 64));

            _spriteBatch.Begin();

            hand.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
