using System;

namespace gdd2xna
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ViaGame game = new ViaGame(ViaGame.SIZE_SMALL))
            {
                game.Run();
            }
        }
    }
#endif
}

