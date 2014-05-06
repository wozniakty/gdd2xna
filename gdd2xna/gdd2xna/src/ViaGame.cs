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
    /// <summary>
    /// The possible game states.
    /// </summary>
    public enum GameState
    {
        Menu, // On the main menu
        LocalPlay
    }

    /// <summary>
    /// The possible game steps.
    /// </summary>
    public enum GameStep
    {
        Input,
        SwapBack,
        CheckMatch,
        CheckDeadlock,
        Waiting, 
        Complete,
        Win, 
        Lose
    }


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ViaGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// The constant for the large game size.
        /// </summary>
        public static readonly int SIZE_LARGE = 0;

        /// <summary>
        /// The constant for the small game size.
        /// </summary>
        public static readonly int SIZE_SMALL = 1;

        #region Fields
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MusicManager musicManager;
        SoundManager soundManager;
        SpriteFont defaultFont;
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
        public Texture2D buttonTexture;
        public Texture2D logo;
        public Random random;
        

        private bool musicOn;

        /// <summary>
        /// The array of players.
        /// </summary>
        private readonly Player[] players;

        /// <summary>
        /// The scores instance.
        /// </summary>
        private readonly Scores scores;

        /// <summary>
        /// The main menu.
        /// </summary>
        private readonly MainMenu mainMenu;

        /// <summary>
        /// The in game menu.
        /// </summary>
        private readonly GameMenu gameMenu;

        /// <summary>
        /// The current game state.
        /// </summary>
        private GameState state = GameState.Menu;

        #endregion

        #region Properties
        public bool MusicOn
        {
            get { return musicOn; }
            set { musicOn = value; }
        }
        public int SizeMode { get; set; }

        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameState State
        {
            set
            {
                state = value;
                if (state == GameState.LocalPlay)
                {
                    Reset();
                }
            }
            get { return state; }

        }

        #endregion

        /// <summary>
        /// Creates a new ViaGame instance.
        /// </summary>
        /// <param name="sizeMode">The mode for the window size.</param>
        public ViaGame(int sizeMode)
        {
            random = new Random();
            graphics = new GraphicsDeviceManager(this);
            musicManager = new MusicManager(this);
            soundManager = new SoundManager(this);
            scores = new Scores(this, soundManager);
            mainMenu = new MainMenu(this, soundManager);
            gameMenu = new GameMenu(this);

            SizeMode = sizeMode;
            if (sizeMode == SIZE_SMALL)
            {
                Grid.TILE_SIZE = 50;
                graphics.PreferredBackBufferWidth = 1000;
                graphics.PreferredBackBufferHeight = 550; // 480 or 700
            }
            else
            {
                graphics.PreferredBackBufferHeight = 900;
                graphics.PreferredBackBufferWidth = 1600;
            }
            Content.RootDirectory = "Content";

            players = new Player[2];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new Player(this, i, soundManager, scores);
            }
            players[0].step = GameStep.Input;
        }

        /// <summary>
        /// Set the winner of the game.
        /// </summary>
        /// <param name="index">The index of the winning player.</param>
        public void SetWinner(int index)
        {
            // Mark all other players as losers;
            // the winner knows they won from the Scores.add() function
            for (int i = 0; i < players.Length; i++)
            {
                if (i != index)
                    players[i].step = GameStep.Lose;
            }
        }

        /// <summary>
        /// Reset the game data.
        /// </summary>
        public void Reset()
        {
            // Reset each player.
            foreach (Player next in players)
            {
                next.Reset();
            }
            
            // Reset the scores.
            scores.Reset();

            // Initialize player 1
            players[0].step = GameStep.Input;
        }

        /// <summary>
        /// Is the current game over.
        /// </summary>
        /// <returns>The value.</returns>
        public bool IsGameOver()
        {
            foreach (Player next in players)
            {
                if (next.step == GameStep.Win)
                    return true;
            }
            return false;
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
            logo = Content.Load<Texture2D>("Art/VIA_Logo");
            buttonTexture = Content.Load<Texture2D>("Art/button");
            defaultFont = Content.Load<SpriteFont>("Fonts/Default");
            Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Pixel.SetData(new[] { Color.White });

            foreach (Player next in players)
            {
                next.LoadingFinished(defaultFont);
            }
            mainMenu.LoadingComplete(defaultFont);
            gameMenu.LoadingComplete(defaultFont);
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

            // Update the music manager
            musicManager.Update(gameTime);

            // Check for the toggle music button
            if (Input.KeyPress(Keys.M))
            {
                musicManager.Toggle();
            }

            // Update input
            Input.Update();

            switch (state)
            {
                case GameState.Menu:
                    mainMenu.Update(gameTime);
                    break;
                case GameState.LocalPlay:
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

                    // Update the scores
                    scores.Update(gameTime);

                    // Update the menu
                    gameMenu.Update(gameTime);
                    break;
            }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // No more cornflower blue!
            GraphicsDevice.Clear(Color.Black);

            // Start drawing
            spriteBatch.Begin();

            switch (state)
            {
                case GameState.Menu:
                    // Draw the main menu
                    mainMenu.Draw(gameTime, spriteBatch, defaultFont);
                    break;
                case GameState.LocalPlay:
                    // Draw each of the players
                    foreach (Player next in players)
                    {
                        next.Draw(gameTime, spriteBatch, defaultFont);
                    }

                    // Draw the scores
                    scores.Draw(gameTime, spriteBatch, defaultFont);

                    // Draw the menu
                    gameMenu.Draw(gameTime, spriteBatch, defaultFont);
                    break;
            }
            

            // Trying to get the logo in there
            /*Vector2 pos = new Vector2(450, 500);
            Color c = new Color();
            spriteBatch.Draw(logo, pos, c);*/

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
