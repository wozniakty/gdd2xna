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
        private readonly ViaGame game;

        /// <summary>
        /// The sound manager.
        /// </summary>
        private readonly SoundManager soundManager;

        /// <summary>
        /// The bar values for each tile type.
        /// </summary>
        private Dictionary<TileType, int> bars = new Dictionary<TileType, int>();

        /// <summary>
        /// The current X values for the tiles.
        /// </summary>
        private Dictionary<TileType, int> tileX = new Dictionary<TileType, int>();

        /// <summary>
        /// The goal X values for the tiles.
        /// </summary>
        private Dictionary<TileType, int> goalX = new Dictionary<TileType, int>();

        /// <summary>
        /// Creates a new Scores instance.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="soundManager">The sound manager.</param>
        public Scores(ViaGame game, SoundManager soundManager)
        {
            this.game = game;
            this.soundManager = soundManager;

            Reset();
        }

        /// <summary>
        /// Reset the scores.
        /// </summary>
        public void Reset()
        {
            // Populate the bars dictionary
            foreach (TileType next in Enum.GetValues(typeof(TileType)))
            {
                // No sammiches!
                if (next == TileType.Emp)
                    continue;

                bars[next] = 0;
                tileX[next] = -1;
                goalX[next] = -1;
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
        /// Get the index of the player that has locked the specified bar.
        /// </summary>
        /// <param name="type">The tile type of the bar.</param>
        /// <returns>The player index, or -1 if the bar is unlocked.</returns>
        private int getPlayerIndexForBar(TileType type)
        {
            if (bars[type] >= GOAL)
                return 1;
            else if (bars[type] <= -GOAL)
                return 0;
            return -1;
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

            int index = -1;

            // Check if the bar is already locked.
            if (isLocked(type))
            {
                // If the opponent has that tile locked, you lose points!
                if (getPlayerIndexForBar(type) != playerIndex)
                    amount *= -1;
                
                // Affect all unlocked bars instead.
                foreach (TileType next in Enum.GetValues(typeof(TileType)))
                {
                    // No sammiches!
                    if (next == TileType.Emp)
                        continue;

                    int localIndex = incrementBar(next, amount/5);
                    if (localIndex != -1)
                    {
                        index = localIndex;
                    }
                }
                return false;
            }
            else
            {
                index = incrementBar(type, amount);
            }
            
            return index == playerIndex;
        }

        /// <summary>
        /// Increment a bar value by the specified amount.
        /// </summary>
        /// <param name="type">The tile type to increment.</param>
        /// <param name="amount">The amount to increment by.</param>
        /// <returns></returns>
        private int incrementBar(TileType type, int amount)
        {
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
            return index;
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
        /// Update the scores.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            foreach (TileType next in Enum.GetValues(typeof(TileType)))
            {
                // No sammiches!
                if (next == TileType.Emp)
                    continue;

                if (tileX[next] < goalX[next])
                {
                    tileX[next]++;
                }
                else if (tileX[next] > goalX[next])
                {
                    tileX[next]--;
                }
            }
        }

        /// <summary>
        /// Draw the scores.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="font">The sprite font.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            const int BOARD_WIDTH = 400;
            const int GAP_WIDTH = 200;

            int baseX = BOARD_WIDTH + (GAP_WIDTH / 2);
            int x = baseX - (Grid.TILE_SIZE / 2);
            int y = 0;

            const int centerWidth = 1;
            const int height = 5;

            foreach (TileType next in Enum.GetValues(typeof(TileType)))
            {
                // No sammiches!
                if (next == TileType.Emp)
                    continue;

                // Do some math for the bars
                int middleBarX = baseX - (centerWidth / 2);
                int barY = y + (Grid.TILE_SIZE);

                // and for the score and bar width
                const int BUFFER = 20; // Buffer space between the boards and the tiles/line
                int score = bars[next];
                int barWidth = (int)(Math.Abs((double)score / GOAL) * (((GAP_WIDTH - BUFFER) - ((GAP_WIDTH - BUFFER) / 6)))) / 2;

                // Should the new style be enabled.
                const bool ENABLE_NEW_STYLE = true;

                if (ENABLE_NEW_STYLE)
                {
                    // Draw the line behind the tile
                    Rectangle line = new Rectangle(BOARD_WIDTH + BUFFER , y + (Grid.TILE_SIZE / 2), (GAP_WIDTH - (BUFFER*2)), 1);
                    spriteBatch.Draw(game.Pixel, line, Color.White);
                }

                // Draw the tile image
                int tileXOffset = 0;
                if (ENABLE_NEW_STYLE)
                {
                    if (score < 0)
                        tileXOffset = -barWidth;
                    else if (score > 1)
                        tileXOffset = barWidth;
                }
                Texture2D texture = Grid.getTileTexture(game, next);

                // Calculate the goal tile X
                int goalTileX = (x + tileXOffset);
                goalX[next] = goalTileX;

                // Draw at the current X
                int currentX = tileX[next];
                if (currentX == -1)
                {
                    currentX = goalTileX;
                    tileX[next] = goalTileX;
                }
                Rectangle rect = new Rectangle(currentX, y, Grid.TILE_SIZE, Grid.TILE_SIZE);

                // If that tile is locked, grey it out
                if (isLocked(next))
                {
                    //spriteBatch.Draw(game.Pixel, rect, Color.Black * 0.75f);
                    spriteBatch.Draw(texture, rect, Color.White * 0.5f);
                }
                else
                {
                    spriteBatch.Draw(texture, rect, Color.White);
                }

                if (!ENABLE_NEW_STYLE)
                {

                    // Get the color of the bar
                    Color barColor = getBarColor(next);

                    // Draw the center of the bar
                    Rectangle bar = new Rectangle(middleBarX, barY, centerWidth, height);
                    spriteBatch.Draw(game.Pixel, bar, barColor);

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
                }

                // Increment y
                y += Grid.TILE_SIZE + 5;
            }
        }
    }
}
