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
    /// The instructions view.
    /// </summary>
    class Instructions
    {
        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// The main menu button.
        /// </summary>
        private Button mainMenuButton;

        /// <summary>
        /// Creates a new Instructions.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public Instructions(ViaGame game)
        {
            this.game = game;
        }

        /// <summary>
        /// Called when loading is complete.
        /// </summary>
        /// <param name="defaultFont">The default font.</param>
        public void LoadingComplete(SpriteFont defaultFont)
        {
            int halfWidth = (game.graphics.PreferredBackBufferWidth / 2);
            int buttonX = halfWidth - (game.buttonTexture.Width / 2);
            int buttonY = game.graphics.PreferredBackBufferHeight - game.buttonTexture.Height - 5;

            // Create the buttons
            mainMenuButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, buttonY),
                "Main Menu",
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
        /// Update the instructions.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            mainMenuButton.Update(gameTime);
        }

        /// <summary>
        /// Draw the instructions.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="defaultFont">The default font.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont defaultFont)
        {
            mainMenuButton.Draw(gameTime, spriteBatch);
        }
    }
}
