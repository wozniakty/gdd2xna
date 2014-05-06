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
    /// The menu while a game is in progess.
    /// </summary>
    class GameMenu
    {

        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// The button to create a new game.
        /// </summary>
        private Button newGameButton;

        /// <summary>
        /// The button to exit the current game.
        /// </summary>
        private Button exitGameButton;

        /// <summary>
        /// Creates a new GameMenu.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public GameMenu(ViaGame game)
        {
            this.game = game;
        }

        /// <summary>
        /// Called when loading is complete.
        /// </summary>
        /// <param name="defaultFont">The default font.</param>
        public void LoadingComplete(SpriteFont defaultFont)
        {
            int currentY = (game.graphics.PreferredBackBufferHeight) - (2 * game.buttonTexture.Height);
            int halfWidth = (game.graphics.PreferredBackBufferWidth / 2);
            int buttonX = halfWidth - (game.buttonTexture.Width / 2);

            // Create the buttons
            newGameButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                "New Game",
                defaultFont,
                delegate(Button button)
                {
                    if (game.State == GameState.LocalPlay)
                    {
                        game.State = GameState.LocalPlay;
                    }
                    else if (game.State == GameState.NetworkPlay)
                    {
                        game.State = GameState.NetworkSearch;
                    }
                },
                null,
                delegate(Button button)
                {
                    return game.IsGameOver() && game.State != GameState.NetworkPlay;
                }
                );

            currentY += game.buttonTexture.Height;

            exitGameButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                "Exit Game",
                defaultFont,
                delegate(Button button)
                {
                    game.State = GameState.Menu;
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
            newGameButton.Update(gameTime);
            exitGameButton.Update(gameTime);
        }

        /// <summary>
        /// Draw the menu.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="defaultFont">The default font.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont defaultFont)
        {
            newGameButton.Draw(gameTime, spriteBatch);
            exitGameButton.Draw(gameTime, spriteBatch);
        }
        

    }
}
