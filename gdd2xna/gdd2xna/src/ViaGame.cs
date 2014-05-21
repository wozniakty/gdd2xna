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
        Instructions, // On the instructions screen
        Options, // On the game option screen
        NetworkSearch, // Searching for opponent online
        LocalPlay, // Playing locally against an opponent
        NetworkPlay, // Playing against an opponent on the network
        Error, // An error occurred; probably with the network
    }

    /// <summary>
    /// The possible game modes.
    /// </summary>
    public enum GameMode
    {
        TurnBased,
        RealTime
    }

    /// <summary>
    /// The possible game steps.
    /// </summary>
    public enum GameStep
    {
        Input,
        NetworkInput, 
        SwapBack,
        CheckMatch,
        CheckMatchShuffle,
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
        /// The build number of the game.
        /// </summary>
        public static readonly int GAME_BUILD = 10;

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
        public Texture2D rotten;
        

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
        /// The network game client.
        /// </summary>
        private readonly GameClient gameClient;

        /// <summary>
        /// The error view.
        /// </summary>
        private readonly ErrorView errorView;

        /// <summary>
        /// The instructions view.
        /// </summary>
        private readonly Instructions instructions;

        /// <summary>
        /// The options view.
        /// </summary>
        private readonly Options options;

        /// <summary>
        /// The random generator.
        /// </summary>
        private readonly ViaRandom random;

        /// <summary>
        /// The music manager.
        /// </summary>
        private readonly MusicManager musicManager;

        /// <summary>
        /// The sound manager.
        /// </summary>
        private readonly SoundManager soundManager;

        /// <summary>
        /// The current game state.
        /// </summary>
        private GameState state = GameState.Menu;

        /// <summary>
        /// The current game mode.
        /// </summary>
        private GameMode mode = GameMode.TurnBased;

        /// <summary>
        /// Is rotten mode enabled.
        /// </summary>
        private bool rottenMode = false;

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
                if (value == GameState.LocalPlay)
                {
                    Reset();
                }
                else if (value == GameState.NetworkSearch)
                {
                    gameClient.Initialize();
                }
                else if (value == GameState.Menu)
                {
                    gameClient.Shutdown(false, null);
                }
                else if (value == GameState.Options)
                {
                    options.Error = "";
                }
                state = value;
                if (value == GameState.NetworkPlay)
                {
                    Reset();
                }
            }
            get { return state; }

        }

        /// <summary>
        /// The current game mode property.
        /// </summary>
        public GameMode Mode
        {
            get
            {
                return mode;
            }

            set
            {
                mode = value;
            }
        }

        /// <summary>
        /// The game client property.
        /// </summary>
        public GameClient Client
        {
            get
            {
                return gameClient;
            }
        }

        /// <summary>
        /// The scores.
        /// </summary>
        public Scores Scores
        {
            get
            {
                return scores;
            }
        }

        /// <summary>
        /// The game options.
        /// </summary>
        public Options Options
        {
            get
            {
                return options;
            }
        }

        /// <summary>
        /// The error view property.
        /// </summary>
        public ErrorView Error
        {
            get
            {
                return errorView;
            }
        }

        /// <summary>
        /// The random generator property.
        /// </summary>
        public ViaRandom Random
        {
            get
            {
                return random;
            }
        }

        /// <summary>
        /// The rotten mode property.
        /// </summary>
        public bool RottenMode
        {
            get
            {
                return rottenMode;
            }
            set
            {
                rottenMode = value;
            }
        }

        #endregion

        /// <summary>
        /// Creates a new ViaGame instance.
        /// </summary>
        /// <param name="sizeMode">The mode for the window size.</param>
        public ViaGame(int sizeMode)
        {
            random = new ViaRandom(this);
            graphics = new GraphicsDeviceManager(this);
            //musicManager = new LegacyMusicManager(this);
            musicManager = new MusicManager();
            soundManager = new SoundManager(this);
            scores = new Scores(this, soundManager);
            mainMenu = new MainMenu(this, soundManager);
            gameMenu = new GameMenu(this);
            gameClient = new GameClient(this);
            errorView = new ErrorView(this);
            instructions = new Instructions(this);
            options = new Options(this);

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
        /// Get the player at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The player.</returns>
        public Player GetPlayer(int index)
        {
            return players[index];
        }

        /// <summary>
        /// Set the winner of the game.
        /// </summary>
        /// <param name="index">The index of the winning player.</param>
        public void SetWinner(int index)
        {
            players[index].step = GameStep.Win;
            // Mark all other players as losers;
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
            musicOn = true;

            //soundManager.Initialize();
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
            Broccoli = Content.Load<Texture2D>("Art/V2/Brocoli_Tile");
            Carrot = Content.Load<Texture2D>("Art/V2/Carrot_Tile");
            Corn = Content.Load<Texture2D>("Art/V2/Corn_Tile");
            Eggplant = Content.Load<Texture2D>("Art/V2/Eggplant_Tile");
            Tomato = Content.Load<Texture2D>("Art/V2/Tomato_Tile");
            Onion = Content.Load<Texture2D>("Art/V2/Onion_Tile");
            Radish = Content.Load<Texture2D>("Art/V2/Radish_Tile");
            Grid_Art = Content.Load<Texture2D>("Art/VIA_Grid_V2");
            logo = Content.Load<Texture2D>("Art/V2/VIA_Logo");
            rotten = Content.Load<Texture2D>("Art/V2/Rotten_Tile");
            buttonTexture = Content.Load<Texture2D>("Art/button");
            defaultFont = Content.Load<SpriteFont>("Fonts/Default");
            Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Pixel.SetData(new[] { Color.White });

            musicManager.Load(Content);

            foreach (Player next in players)
            {
                next.LoadingFinished(defaultFont);
            }
            mainMenu.LoadingComplete(defaultFont);
            gameMenu.LoadingComplete(defaultFont);
            gameClient.LoadingComplete(defaultFont);
            errorView.LoadingComplete(defaultFont);
            instructions.LoadingComplete(defaultFont);
            options.LoadingComplete(defaultFont);

            Console.WriteLine("Starting Via client build " + GAME_BUILD + ".");
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
                case GameState.Instructions:
                    instructions.Update(gameTime);
                    break;
                case GameState.Options:
                    options.Update(gameTime);
                    break;
                case GameState.NetworkSearch:
                    gameClient.Update(gameTime);
                    break;
                case GameState.LocalPlay:
                case GameState.NetworkPlay:
                    // Update each of the players
                    foreach (Player next in players)
                    {
                        next.Update(gameTime);

                        if (mode == GameMode.TurnBased && !IsGameOver())
                        {
                            // Check if the player has finished their turn.
                            if (next.step == GameStep.Complete)
                            {
                                // If so, mark them as waiting
                                next.step = GameStep.Waiting;

                                int index = (next.Index + 1) % players.Length;

                                // And activate the next player
                                if (state == GameState.LocalPlay)
                                {
                                    players[index].step = GameStep.Input;
                                }
                                else if (state == GameState.NetworkPlay)
                                {
                                    if (index == 0)
                                        players[index].step = GameStep.Input;
                                    else
                                        players[index].step = GameStep.NetworkInput;
                                }
                            }
                        }
                    }

                    // Update the scores
                    scores.Update(gameTime);

                    // Update the menu
                    gameMenu.Update(gameTime);
                    break;
                case GameState.Error:
                    errorView.Update(gameTime);
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
            GraphicsDevice.Clear(Color.DarkGray);

            // Start drawing
            spriteBatch.Begin();

            switch (state)
            {
                case GameState.Menu:
                    // Draw the main menu
                    mainMenu.Draw(gameTime, spriteBatch, defaultFont);
                    break;
                case GameState.Instructions:
                    instructions.Draw(gameTime, spriteBatch, defaultFont);
                    break;
                case GameState.Options:
                    options.Draw(gameTime, spriteBatch, defaultFont);
                    break;
                case GameState.NetworkSearch:
                    gameClient.Draw(gameTime, spriteBatch, defaultFont);
                    break;
                case GameState.LocalPlay:
                case GameState.NetworkPlay:
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
                case GameState.Error:
                    errorView.Draw(gameTime, spriteBatch, defaultFont);
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
