//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Glid_n_Shadow
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector2 baseScreenSize = new Vector2(Tile.Width*40, Tile.Height*30);
        private Matrix globalTransform;
        int backbufferWidth;
        int backbufferHeight;

        //Game State
        private GameStateManager manager;
        private ContentManager mapContent;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            mapContent = new ContentManager(this.Services, Content.RootDirectory);

            //graphics.IsFullScreen = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

            //graphics.PreferredBackBufferWidth = Tile.Width * 60;
            //graphics.PreferredBackBufferHeight = Tile.Height * 45;

            //bkg = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Content.Load<Song>("MenuMusic"));

            //Load fonts, overlay & background textures in gamestatemanager
            manager = new GameStateManager(Content, spriteBatch, baseScreenSize, mapContent);

            //baseScreenSize = new Vector2((map.Width-1) * Tile.Width, map.Height * Tile.Height);
            //ScaleScreen();
        }

        public void ScaleScreen()
        {
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horizontalScaling = backbufferWidth / baseScreenSize.X;
            float verticalScaling = backbufferHeight / baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horizontalScaling, verticalScaling, 1);
            globalTransform = Matrix.CreateScale(screenScalingFactor);
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Check if screen was resized
            if (backbufferHeight != GraphicsDevice.PresentationParameters.BackBufferHeight || backbufferWidth != GraphicsDevice.PresentationParameters.BackBufferWidth)
            {
                ScaleScreen();
            }

            manager.UpdateGameState(gameTime);
            if (manager.ExitGame) Exit();

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransform);

            manager.DrawGameState(gameTime);

            spriteBatch.End();
            base.Draw(gameTime);
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

    }
}
