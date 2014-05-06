using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gdd2xna
{
    /// <summary>
    /// Generates random numbers, or grabs them from the server.
    /// </summary>
    public class ViaRandom
    {
        /// <summary>
        /// The maximum random value the server will generate.
        /// </summary>
        public static readonly int MAX_SERVER_RAND = 60000;

        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// The local random generator.
        /// </summary>
        private readonly Random random;

        /// <summary>
        /// Creates a new ViaRandom.
        /// </summary>
        /// <param name="game">The game.</param>
        public ViaRandom(ViaGame game)
        {
            this.game = game;
            this.random = new Random();
        }

        /// <summary>
        /// Get a random number between two values.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>The next number.</returns>
        public int Next(int playerIndex, int minValue, int maxValue)
        {
            int result;
            // Use the server for network games
            if (game.State == GameState.NetworkPlay)
            {
                int value = game.Client.GetNextRandom(playerIndex);
                double ratio = ((double)value / (double)MAX_SERVER_RAND);

                result = (int)((ratio * (maxValue - 1)) + minValue);
            }
            else
            {
                // Otherwise, make numbers locally
                result = random.Next(minValue, maxValue);
            }
            //Console.WriteLine("Random: " + result);

            return result;
        }

    }
}
