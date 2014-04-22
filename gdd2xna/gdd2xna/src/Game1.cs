using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace gdd2xna
{
    public enum GameState
    {
        Menu,
        Animate, // Board should animate and disallow input
        Input // Board should allow input
    }

    public enum GameStep
    {
        Input,
        SwapBack,
        CheckMatch,
        CheckDeadlock,
        Waiting, 
        Complete
    }


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MusicManager musicManager;
        SoundManager soundManager;
        public Texture2D HamSandwich;
        public Texture2D Broccoli;
        public Texture2D Carrot;
        public Texture2D Corn;
        public Texture2D Eggplant;
        public Texture2D Tomato;
        public Texture2D Onion;
        public Texture2D Radish;
        public Texture2D Grid_Art;
        public Texture2D Pixel;
        private bool musicOn;

        /// <summary>
        /// The array of players.
        /// </summary>
        private readonly Player[] players;

        #endregion

        #region Properties
        public bool MusicOn
        {
            get { return musicOn; }
            set { musicOn = value; }
        }
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            musicManager = new MusicManager(this);
            soundManager = new SoundManager(this);
            Content.RootDirectory = "Content";

            players = new Player[2];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new Player(this, i, soundManager);
            }
            players[0].step = GameStep.Input;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            // load songs to musicManager and play
            musicManager.Initialize();
            musicOn = true;
            soundManager.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            HamSandwich = Content.Load<Texture2D>("Art/Ham_Sandwich");
            Broccoli = Content.Load<Texture2D>("Art/Brocoli_Tile");
            Carrot = Content.Load<Texture2D>("Art/Carrot_Tile");
            Corn = Content.Load<Texture2D>("Art/Corn_Tile");
            Eggplant = Content.Load<Texture2D>("Art/Eggplant_Tile");
            Tomato = Content.Load<Texture2D>("Art/Tomato_Tile");
            Onion = Content.Load<Texture2D>("Art/Onion_Tile");
            Radish = Content.Load<Texture2D>("Art/Radish_Tile");
            Grid_Art = Content.Load<Texture2D>("Art/VIA_Grid_V2");
            Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Pixel.SetData(new[] { Color.White });

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            musicManager.Update(gameTime);
            Input.Update();

            // Update each of the players
            foreach (Player next in players)
            {
                next.Update(gameTime);

                // Check if the player has finished their turn.
                if (next.step == GameStep.Complete)
                {
                    // If so, activate the next player.
                    next.step = GameStep.Waiting;
                    players[(next.Index + 1) % players.Length].step = GameStep.Input;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // Draw each of the players
            foreach (Player next in players)
            {
                next.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Utilities
        
        /// <summary>
        /// Will draw a border (hollow rectangle) of the given 'thicknessOfBorder' (in pixels)
        /// of the specified color.
        ///
        /// By Sean Colombo, from http://bluelinegamestudios.com/blog
        /// </summary>
        /// <param name="rectangleToDraw"></param>
        /// <param name="thicknessOfBorder"></param>
        public void DrawBorder(SpriteBatch sb, Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            sb.Draw(Pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            sb.Draw(Pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            sb.Draw(Pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            sb.Draw(Pixel, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }

        #endregion
    }
}
