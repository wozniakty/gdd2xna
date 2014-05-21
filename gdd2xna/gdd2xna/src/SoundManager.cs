using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gdd2xna
{
    /// <summary>
    /// The sound manager.
    /// </summary>
    public class SoundManager
    {
        /// <summary>
        /// The instance of the game.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// Creates a new SoundManager.
        /// </summary>
        /// <param name="game">The instance of the game.</param>
        public SoundManager(ViaGame game)
        {
            this.game = game;
        }
    }
}
