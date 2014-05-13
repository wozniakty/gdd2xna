using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gdd2xna
{
    class Button
    {
        /// <summary>
        /// The texture of the button.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// The bounds (x, y, width, height) of the button.
        /// </summary>
        private Rectangle bounds;

        /// <summary>
        /// The text of the button.
        /// </summary>
        private string text;

        /// <summary>
        /// The location to draw the text at.
        /// </summary>
        private Vector2? textLocation = null;

        /// <summary>
        /// The font that should be used to draw the button's text.
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// Is the button currently being hovered over.
        /// </summary>
        private bool hover;

        /// <summary>
        /// The callback for when the button is clicked.
        /// </summary>
        private ButtonClickHandler callback;

        /// <summary>
        /// The enabler for the button.
        /// </summary>
        private ButtonEnabler enabler;

        /// <summary>
        /// The enabler for the button's visibility.
        /// </summary>
        private ButtonEnabler visible;

        /// <summary>
        /// The text property.
        /// </summary>
        public string Text
        {
            set
            {
                // Flag a recalculate of the location
                textLocation = null;
                text = value;
            }
            get
            {
                return text;
            }
        }
        
        /// <summary>
        /// The delegate to handle button clicks.
        /// </summary>
        /// <param name="button">The button that was clicked.</param>
        public delegate void ButtonClickHandler(Button button);

        /// <summary>
        /// A delegate to tell when the button is enabled.
        /// </summary>
        /// <param name="button">The button that is being checked.</param>
        /// <returns>If the button should be enabled.</returns>
        public delegate bool ButtonEnabler(Button button);

        /// <summary>
        /// Creates a new Button.
        /// </summary>
        /// <param name="texture">The texture for the button.</param>
        /// <param name="location">The location of the button.</param>
        /// <param name="text">The text of the button.</param>
        /// <param name="font">The font that the text should be drawn in.</param>
        /// <param name="callback">The callback for when the button is clicked.</param>
        /// <param name="enabler">The enabler that determines if the button is clickable.</param>
        /// <param name="visible">The enabler that determines if the button is visible.</param>
        public Button(
            Texture2D texture, 
            Vector2 location, 
            string text, 
            SpriteFont font,
            ButtonClickHandler callback, 
            ButtonEnabler enabler, 
            ButtonEnabler visible
            )
        {
            this.texture = texture;
            this.bounds = new Rectangle(
                (int)location.X, 
                (int)location.Y, 
                (int)(texture.Width), 
                (int)(texture.Height)
                );
            this.text = text;
            this.font = font;
            this.callback = callback;
            this.enabler = enabler;
            this.visible = visible;
        }

        /// <summary>
        /// Update the button.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            // Make sure the button is visible.
            if (visible != null && !visible(this))
            {
                return;
            }

            // Check if the mouse is over the button.
            Point position = Input.MousePos();
            if (bounds.Contains(position))
            {
                hover = true;

                // Check if the button is being clicked.
                if (Input.LeftClick())
                {
                    if (enabler == null || enabler(this))
                    {
                        callback(this);
                    }
                }
            }
            else if (hover)
            {
                hover = false;
            }
        }

        /// <summary>
        /// Draw the button.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Make sure the button is visible.
            if (visible != null && !visible(this))
            {
                return;
            }

            // The text location being set to null
            // is a flag to recalculate it.
            if (textLocation == null)
            {
                Vector2 size = font.MeasureString(text);
                float x = bounds.X + ((texture.Width / 2) - (size.X / 2));
                float y = bounds.Y + ((texture.Height / 2) - (size.Y / 2));
                textLocation = new Vector2(x, y);
            }

            // By default the button has no tint.
            Color backgroundColor = Color.White;
            Color textColor = Color.Black;

            if (enabler != null && !enabler(this))
            {
                backgroundColor = Color.DarkGray;
            }
            else
            {
                // Tint the button when it is hovered over.
                if (hover)
                {
                    backgroundColor = Color.LightGray;

                    // This makes it kind of hard to read...
                    //textColor = Color.DarkGray;
                }
            }
            
            // Draw the background of the button
            spriteBatch.Draw(texture, bounds, backgroundColor);

            // Draw the text on the button
            spriteBatch.DrawString(font, text, textLocation.Value, textColor);
        }

    }
}
