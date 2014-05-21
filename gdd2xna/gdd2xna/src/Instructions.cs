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
        /// The text.
        /// </summary>
        private static readonly string[] TEXT = { 
                                                    "Save the Veggies!", 
                                                    "Swap adjacent plots to create matches of 3 or more Veggies.", 
                                                    "Bigger matches moves Veggies closer to you.", 
                                                    "Capture the Veggies by moving them all the way to your side.", 
                                                    "Matching Captured Veggies moves all the Veggies closer to you.", 
                                                    "Matching Veggies captured by your foe moves all Veggies away from you.", 
                                                    "Capture 4 out of 7 Veggies to win the game!"
                                                
                                                };

        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// The main menu button.
        /// </summary>
        private Button mainMenuButton;

        /// <summary>
        /// The list of locations.
        /// </summary>
        private List<Vector2> locations;

        /// <summary>
        /// The font.
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// Creates a new Instructions.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public Instructions(ViaGame game)
        {
            this.game = game;
            locations = new List<Vector2>();
        }

        /// <summary>
        /// Recalculate the location of the text.
        /// </summary>
        private void recalculate()
        {
            locations.Clear();

            int halfWidth = (game.graphics.PreferredBackBufferWidth / 2);
            int currentY = 7;
            for (int i = 0; i < TEXT.Length; i++)
            {
                Vector2 size = font.MeasureString(TEXT[i]);
                Vector2 location = new Vector2(halfWidth - (size.X / 2), currentY);
                currentY += (int)(size.Y) + 7;

                locations.Add(location);
            }
        }

        /// <summary>
        /// Called when loading is complete.
        /// </summary>
        /// <param name="defaultFont">The default font.</param>
        public void LoadingComplete(SpriteFont defaultFont)
        {
            font = defaultFont;
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

            recalculate();
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
            for (int i = 0; i < TEXT.Length; i++)
            {
                spriteBatch.DrawString(font, TEXT[i], locations[i], i == 0 ? Color.DarkGreen : Color.Blue);
            }
            mainMenuButton.Draw(gameTime, spriteBatch);
        }
    }
}
