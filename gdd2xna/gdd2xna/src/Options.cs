using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gdd2xna
{
    /// <summary>
    /// The game options view.
    /// </summary>
    public class Options
    {

        /// <summary>
        /// The title text.
        /// </summary>
        public static readonly string TITLE_TEXT = "Game Options";

        /// <summary>
        /// The game mode text.
        /// </summary>
        public static readonly string GAME_MODE_TEXT = "Game Mode";

        /// <summary>
        /// The opponent location text.
        /// </summary>
        public static readonly string OPPONENT_LOCATION_TEXT = "Opponent Location";

        /// <summary>
        /// The rotten mode text.
        /// </summary>
        public static readonly string ROTTON_TEXT = "Rotten Mode";

        /// <summary>
        /// The names of the game modes.
        /// </summary>
        public static readonly string[] GAME_MODE_NAMES = { "Turns", "Realtime" };

        /// <summary>
        /// The next state of the game.
        /// </summary>
        public GameState NextState { get; set; }

        /// <summary>
        /// The error property.
        /// </summary>
        public String Error
        {
            get
            {
                return error;
            }
            set
            {
                setError(value);
            }
        }

        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// The location of the title text.
        /// </summary>
        private Vector2 titleTextLocation;

        /// <summary>
        /// The location of the game mode text.
        /// </summary>
        private Vector2 gameModeTextLocation;

        /// <summary>
        /// The opponent location text location.
        /// </summary>
        private Vector2 opponontLocationTextLocation;

        /// <summary>
        /// The rotten mode text location.
        /// </summary>
        private Vector2 rottenModeTextLocation;

        /// <summary>
        /// The error text location
        /// </summary>
        private Vector2? errorTextLocation = null;

        /// <summary>
        /// The current error.
        /// </summary>
        private string error;

        /// <summary>
        /// The default font.
        /// </summary>
        private SpriteFont defaultFont;

        /// <summary>
        /// The main menu button.
        /// </summary>
        private Button mainMenuButton;

        /// <summary>
        /// The start button.
        /// </summary>
        private Button startButton;

        /// <summary>
        /// The game mode button.
        /// </summary>
        private Button gameModeButton;

        /// <summary>
        /// The opponent location button.
        /// </summary>
        private Button opponentLocationButton;

        /// <summary>
        /// The rotton button.
        /// </summary>
        private Button rottenModeButton;

        /// <summary>
        /// Creates a new Options.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public Options(ViaGame game)
        {
            this.game = game;
            NextState = GameState.LocalPlay;
        }

        /// <summary>
        /// Set the selected game mode.
        /// </summary>
        /// <param name="gameModeIndex">The game mode index.</param>
        private void setGameMode(int gameModeIndex)
        {
            GameMode[] values = (GameMode[])Enum.GetValues(typeof(GameMode));
            game.Mode = values[gameModeIndex];
            gameModeButton.Text = GAME_MODE_NAMES[gameModeIndex];
        }

        /// <summary>
        /// Set the current error message.
        /// </summary>
        /// <param name="error">The error message.</param>
        private void setError(string error)
        {
            this.error = error;

            // Determine the height of the title text
            int halfWidth = (game.graphics.PreferredBackBufferWidth / 2);
            Vector2 size = defaultFont.MeasureString(error);
            int currentY = 10;

            // Go beyond that
            currentY += (int)size.Y + 5;

            // Save the location of the error text.
            errorTextLocation = new Vector2(halfWidth - (size.X / 2), currentY);
        }

        /// <summary>
        /// Called when loading is complete.
        /// </summary>
        /// <param name="defaultFont">The default font.</param>
        public void LoadingComplete(SpriteFont defaultFont)
        {
            // Store the font for error message measuring
            this.defaultFont = defaultFont;

            // Do some math!
            int halfWidth = (game.graphics.PreferredBackBufferWidth / 2);
            int buttonX = halfWidth - (game.buttonTexture.Width / 2);
            int currentButtonY = game.graphics.PreferredBackBufferHeight - game.buttonTexture.Height - 5;
            int middleY = (game.graphics.PreferredBackBufferHeight / 2);

            int currentY = 10;
            // Determine the location of the title text
            Vector2 size = defaultFont.MeasureString(TITLE_TEXT);
            titleTextLocation = new Vector2(halfWidth - (size.X / 2), currentY);

            size = defaultFont.MeasureString(GAME_MODE_TEXT);
            const int optionCount = 3;
            currentY = (game.graphics.PreferredBackBufferHeight / 2) - ((int)((size.Y * optionCount) + game.buttonTexture.Height * optionCount) / 2);
            gameModeTextLocation = new Vector2(halfWidth - (size.X / 2), currentY);

            currentY += (int)size.Y + 5;

            gameModeButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                GAME_MODE_NAMES[0],
                defaultFont,
                delegate(Button button)
                {
                    int index = (int)game.Mode;
                    index++;
                    GameMode[] values = (GameMode[])Enum.GetValues(typeof(GameMode));
                    index = index % values.Length; // Wrap around
                    setGameMode(index);
                },
                null,
                null
                );

            currentY += game.buttonTexture.Height + 5;

            size = defaultFont.MeasureString(OPPONENT_LOCATION_TEXT);
            opponontLocationTextLocation = new Vector2(halfWidth - (size.X / 2), currentY);

            currentY += (int)size.Y + 5;

            opponentLocationButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                "Local",
                defaultFont,
                delegate(Button button)
                {
                    if (NextState == GameState.LocalPlay)
                    {
                        NextState = GameState.NetworkSearch;
                        button.Text = "Online";
                    }
                    else if (NextState == GameState.NetworkSearch)
                    {
                        NextState = GameState.LocalPlay;
                        button.Text = "Local";
                    }
                },
                null,
                null
                );

            currentY += game.buttonTexture.Height + 5;

            size = defaultFont.MeasureString(ROTTON_TEXT);
            rottenModeTextLocation = new Vector2(halfWidth - (size.X / 2), currentY);

            currentY += (int)size.Y + 5;

            rottenModeButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                "Off",
                defaultFont,
                delegate(Button button)
                {
                    game.RottenMode = !game.RottenMode;
                    if (game.RottenMode)
                    {
                        button.Text = "On";
                    }
                    else
                    {
                        button.Text = "Off";
                    }
                },
                null,
                null
                );

            // Create the bottom buttons
            mainMenuButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentButtonY),
                "Main Menu",
                defaultFont,
                delegate(Button button)
                {
                    game.State = GameState.Menu;
                },
                null,
                null
                );

            currentButtonY -= (game.buttonTexture.Height + 5);

            startButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentButtonY),
                "Start Game",
                defaultFont,
                delegate(Button button)
                {
                    if (NextState == GameState.LocalPlay && game.Mode == GameMode.RealTime)
                    {
                        setError("Realtime games can only be played in online mode.");
                        return;
                    }
                    game.State = NextState;
                },
                null,
                null
                );
        }

        /// <summary>
        /// Update the options.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            mainMenuButton.Update(gameTime);
            startButton.Update(gameTime);
            gameModeButton.Update(gameTime);
            opponentLocationButton.Update(gameTime);
            rottenModeButton.Update(gameTime);
        }

        /// <summary>
        /// Draw the options.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="defaultFont">The default font.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont defaultFont)
        {
            // Draw the title text
            spriteBatch.DrawString(defaultFont, TITLE_TEXT, titleTextLocation, Color.Blue);
            
            // Draw the game mode text & button
            spriteBatch.DrawString(defaultFont, GAME_MODE_TEXT, gameModeTextLocation, Color.DarkGreen);
            gameModeButton.Draw(gameTime, spriteBatch);

            // Draw the opponent location text & button
            spriteBatch.DrawString(defaultFont, OPPONENT_LOCATION_TEXT, opponontLocationTextLocation, Color.DarkGreen);
            opponentLocationButton.Draw(gameTime, spriteBatch);

            // Draw the rotten option text & button
            spriteBatch.DrawString(defaultFont, ROTTON_TEXT, rottenModeTextLocation, Color.DarkGreen);
            rottenModeButton.Draw(gameTime, spriteBatch);

            // Draw error text, if applicable
            if (errorTextLocation != null)
            {
                spriteBatch.DrawString(defaultFont, error, errorTextLocation.Value, Color.DarkRed);
            }

            mainMenuButton.Draw(gameTime, spriteBatch);
            startButton.Draw(gameTime, spriteBatch);

        }
    }
}
