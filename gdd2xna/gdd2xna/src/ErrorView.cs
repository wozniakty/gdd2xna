using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gdd2xna
{
    public class ErrorView
    {

        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// The okay button.
        /// </summary>
        private Button okButton;

        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a new ErrorView.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public ErrorView(ViaGame game)
        {
            this.game = game;
        }

        /// <summary>
        /// Called when loading is complete.
        /// </summary>
        /// <param name="defaultFont">The default font.</param>
        public void LoadingComplete(SpriteFont defaultFont)
        {
            // Do some math for button placement
            int currentY = (game.graphics.PreferredBackBufferHeight) - (2 * game.buttonTexture.Height);
            int halfWidth = (game.graphics.PreferredBackBufferWidth / 2);
            int buttonX = halfWidth - (game.buttonTexture.Width / 2);

            // Create the buttons
            okButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                "Ok",
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
        /// Update the view.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            okButton.Update(gameTime);
        }

        /// <summary>
        /// Draw the view.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="font">The default font.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            // Draw the current status
            Vector2 size = font.MeasureString(Message);
            float x = (game.graphics.PreferredBackBufferWidth / 2) - (size.X / 2);
            float y = (game.graphics.PreferredBackBufferHeight / 2) - (size.Y / 2);
            Vector2 location = new Vector2(x, y);
            spriteBatch.DrawString(font, Message, location, Color.Cyan);

            // Draw the cancel button
            okButton.Draw(gameTime, spriteBatch);
        }
    }
}
