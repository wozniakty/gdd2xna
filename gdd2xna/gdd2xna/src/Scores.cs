using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace gdd2xna
{
    public class Scores
    {
        
        /// <summary>
        /// The goal score.
        /// </summary>
        private static readonly int GOAL = 100;

        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly Game1 game;

        /// <summary>
        /// The sound manager.
        /// </summary>
        private readonly SoundManager soundManager;

        /// <summary>
        /// The bar values for each tile type.
        /// </summary>
        private Dictionary<TileType, int> bars = new Dictionary<TileType, int>();

        /// <summary>
        /// Creates a new Scores instance.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="soundManager">The sound manager.</param>
        public Scores(Game1 game, SoundManager soundManager)
        {
            this.game = game;
            this.soundManager = soundManager;

            // Populate the bars dictionary
            foreach (TileType next in Enum.GetValues(typeof(TileType)))
            {
                // No sammiches!
                if (next == TileType.Emp)
                    continue;

                this.bars.Add(next, 0);
            }
        }

        /// <summary>
        /// Is the bar for the specified type locked.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The value.</returns>
        private bool isLocked(TileType type)
        {
            return bars[type] >= GOAL || bars[type] <= -GOAL;
        }

        /// <summary>
        /// Get the color of a bar.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The color.</returns>
        private Color getBarColor(TileType type)
        {
            int value = Math.Abs(bars[type]);
            if (value > (GOAL / 20) && value <= (GOAL / 5))
            {
                return Color.Red;
            }
            else if (value > (GOAL / 5) && value <= (GOAL / 2))
            {
                return Color.Yellow;
            }
            else if (value > (GOAL / 2) && value < GOAL)
            {
                return Color.YellowGreen;
            }
            else if (value >= GOAL)
            {
                return Color.Green;
            }
            return Color.DarkRed;
        }

        /// <summary>
        /// Add a score from a match.
        /// </summary>
        /// <param name="type">The tile type.</param>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>If the player won the match.</returns>
        public bool add(TileType type, int playerIndex, int amount)
        {
            if (playerIndex == 0)
            {
                amount *= -1;
            }

            // Check if the bar is already locked.
            if (isLocked(type))
            {
                return false;
            }

            // Increment the bar
            bars[type] += amount;

            int index = -1;
            // Don't go over 100 in either direction!
            if (bars[type] >= GOAL)
            {
                bars[type] = GOAL;
                index = checkForWin();
            }
            else if (bars[type] <= -GOAL)
            {
                bars[type] = -GOAL;
                index = checkForWin();
            }
            return index == playerIndex;
        }

        /// <summary>
        /// Check for a win.
        /// </summary>
        /// <returns>The index of the winning player, or -1.</returns>
        private int checkForWin()
        {
            int index = getWinningPlayer();
            if (index != -1)
                game.SetWinner(index);
            return index;
        }

        /// <summary>
        /// Check if a player has won.
        /// </summary>
        /// <returns>The index of the winning player, or -1.</returns>
        private int getWinningPlayer()
        {
            int[] localScores = new int[2];
            foreach (TileType next in Enum.GetValues(typeof(TileType)))
            {
                // No sammiches!
                if (next == TileType.Emp)
                    continue;
                if (bars[next] >= GOAL)
                {
                    localScores[1]++;
                }
                else if (bars[next] <= -GOAL)
                {
                    localScores[0]++;
                }
            }
            for (int i = 0; i < localScores.Length; i++)
            {
                if (localScores[i] >= 4)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Draw the scores.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="font">The sprite font.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            const int boardWidth = 400;
            const int gapWidth = 200;

            int baseX = boardWidth + (gapWidth / 2);
            int x = baseX - (Grid.TILE_SIZE / 2);
            int y = 0;

            const int centerWidth = 1;
            const int height = 5;

            foreach (TileType next in Enum.GetValues(typeof(TileType)))
            {
                // No sammiches!
                if (next == TileType.Emp)
                    continue;

                // Draw the tile image
                Texture2D texture = Grid.getTileTexture(game, next);
                Rectangle rect = new Rectangle(x, y, Grid.TILE_SIZE, Grid.TILE_SIZE);
                spriteBatch.Draw(texture, rect, Color.White);

                // If that tile is locked, grey it out
                if (isLocked(next))
                {
                    spriteBatch.Draw(game.Pixel, rect, Color.Black * 0.5f);
                }

                // Get the color of the bar
                Color barColor = getBarColor(next);

                // Draw the center of the bar
                int middleBarX = baseX - (centerWidth / 2);
                int barY = y + (Grid.TILE_SIZE);
                Rectangle bar = new Rectangle(middleBarX, barY, centerWidth, height);
                spriteBatch.Draw(game.Pixel, bar, barColor);

                int score = bars[next];
                int barWidth = (int)(Math.Abs((double)score / GOAL) * ((gapWidth - (gapWidth / 6)))) / 2;
                // Draw the rest of the bar
                if (score < 0)
                {
                    // Draw towards the left player
                    bar = new Rectangle(middleBarX - barWidth, barY, barWidth, height);
                    
                }
                else if (score > 0)
                {
                    // Draw towards the right player
                    bar = new Rectangle(middleBarX + centerWidth, barY, barWidth, height);
                }
                spriteBatch.Draw(game.Pixel, bar, barColor);

                // Increment y
                y += Grid.TILE_SIZE + 5;
            }
        }
    }
}
