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
    /// The main menu view.
    /// </summary>
    class MainMenu
    {
        /// <summary>
        /// The text for the authors of the game.
        /// </summary>
        private readonly string AUTHOR_TEXT = "by Nate Ford, Will Hagen, Jason McEvoy, Ryan Rule-Hoffman, & Tyler Wozniak";

        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// The sound manager.
        /// </summary>
        private readonly SoundManager soundManager;

        /// <summary>
        /// The bounds of the logo.
        /// </summary>
        private Rectangle logoBounds;

        /// <summary>
        /// The location of the author text.
        /// </summary>
        private Vector2 authorTextLocation;

        /// <summary>
        /// The new game button.
        /// </summary>
        private Button newLocalGameButton;

        /// <summary>
        /// The new network game button.
        /// </summary>
        private Button newNetworkGameButton;

        /// <summary>
        /// The quit button.
        /// </summary>
        private Button quitButton;

        /// <summary>
        /// The instructions button.
        /// </summary>
        private Button instructionsButton;

        /// <summary>
        /// Creates a new MainMenu.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="soundManager">The sound manager.</param>
        public MainMenu(ViaGame game, SoundManager soundManager)
        {
            this.game = game;
            this.soundManager = soundManager;
        }

        /// <summary>
        /// Called when loading is complete.
        /// </summary>
        /// <param name="defaultFont">The default font.</param>
        public void LoadingComplete(SpriteFont defaultFont)
        {
            // Determine the position of the logo
            const double SCALE_FACTOR = 3.5;
            int halfWidth = (game.graphics.PreferredBackBufferWidth / 2);
            int x = halfWidth - ((int)(game.logo.Width / SCALE_FACTOR) / 2);
            int logoHeight = (int)(game.logo.Height / SCALE_FACTOR);
            logoBounds = new Rectangle(x, 0, (int)(game.logo.Width / SCALE_FACTOR), logoHeight);
            
            // Determine the position of the author text.
            Vector2 size = defaultFont.MeasureString(AUTHOR_TEXT);
            int currentY = logoHeight - 15;
            authorTextLocation = new Vector2(halfWidth - (size.X / 2), currentY);
            currentY += (int)(size.Y) + 7;

            int buttonX = halfWidth - (game.buttonTexture.Width / 2);

            // Create the buttons
            newLocalGameButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                //"Local Game",
                "New Game", 
                defaultFont,
                delegate(Button button)
                {
                    //game.Options.NextState = GameState.LocalPlay;
                    game.State = GameState.Options;
                },
                null, 
                null
                );

            /*currentY += game.buttonTexture.Height;*/

            newNetworkGameButton = new Button(
                game.buttonTexture, 
                new Vector2(buttonX, currentY), 
                "Online Game", 
                defaultFont,
                delegate(Button button)
                {
                    game.Options.NextState = GameState.NetworkSearch;
                    game.State = GameState.Options;
                },
                null, 
                null
                );

            currentY += game.buttonTexture.Height + 3;

            instructionsButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                "Help",
                defaultFont,
                delegate(Button button)
                {
                    game.State = GameState.Instructions;
                },
                null,
                null
                );

            currentY += game.buttonTexture.Height + 3;

            quitButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                "Exit",
                defaultFont,
                delegate(Button button)
                {
                    game.Exit();
                },
                null, 
                null
                );
        }

        /// <summary>
        /// Update the menu.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            newLocalGameButton.Update(gameTime);
            //newNetworkGameButton.Update(gameTime);
            quitButton.Update(gameTime);
            instructionsButton.Update(gameTime);
        }

        /// <summary>
        /// Draw the menu.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="defaultFont">The default font.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont defaultFont)
        {
            // Draw the logo
            spriteBatch.Draw(game.logo, logoBounds, Color.White);

            // Draw the author text
            spriteBatch.DrawString(defaultFont, AUTHOR_TEXT, authorTextLocation, Color.Blue);

            // Draw the buttons
            newLocalGameButton.Draw(gameTime, spriteBatch);
            //newNetworkGameButton.Draw(gameTime, spriteBatch);
            quitButton.Draw(gameTime, spriteBatch);
            instructionsButton.Draw(gameTime, spriteBatch);
        }
    }
}
