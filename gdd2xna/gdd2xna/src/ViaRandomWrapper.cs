using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gdd2xna
{
    public class ViaRandomWrapper
    {
        /// <summary>
        /// The player index.
        /// </summary>
        private readonly int playerIndex;

        /// <summary>
        /// The underlying random generator.
        /// </summary>
        private readonly ViaRandom random;

        /// <summary>
        /// Creates a new ViaRandomWrapper.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="random">The underlying random generator.</param>
        public ViaRandomWrapper(int playerIndex, ViaRandom random)
        {
            this.playerIndex = playerIndex;
            this.random = random;
        }

        /// <summary>
        /// Get a random number between two values.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>The next number.</returns>
        public int Next(int minValue, int maxValue)
        {
            return random.Next(playerIndex, minValue, maxValue);
        }
    }
}
