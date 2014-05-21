using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gdd2xna
{
    /// <summary>
    /// Contains outgoing packet constants.
    /// </summary>
    public class OutgoingPackets
    {
        /// <summary>
        /// The ID of the PING packet.
        /// </summary>
        public static readonly int PING = 0;

        /// <summary>
        /// The ID of the REQUEST_GAME packet.
        /// </summary>
        public static readonly int REQUEST_GAME = 1;

        /// <summary>
        /// The ID of the SWAP_TILES packet.
        /// </summary>
        public static readonly int SWAP_TILES = 2;

        /// <summary>
        /// The ID of the SHUFFLE packet.
        /// </summary>
        public static readonly int SHUFFLE = 3;

        /// <summary>
        /// The ID of the LOGOUT packet.
        /// </summary>
        public static readonly int LOGOUT = 4;
        
        /// <summary>
        /// The ID of the REQUEST_RANDOM packet.
        /// </summary>
        public static readonly int REQUEST_RANDOM = 5;

        /// <summary>
        /// The ID of the GAME_OVER packet.
        /// </summary>
        public static readonly int GAME_OVER = 6;

        /// <summary>
        /// The ID of the INCREASE_SCORE packet.
        /// </summary>
        public static readonly int INCREASE_SCORE = 7;
    }
}
