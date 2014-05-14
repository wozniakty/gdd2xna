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
using System.IO;

namespace gdd2xna
{
    /// <summary>
    /// Manages the music of the game.
    /// </summary>
    class MusicManager
    {

        /// <summary>
        /// The list of songs.
        /// </summary>
        private readonly List<Song> songs = new List<Song>();

        /// <summary>
        /// The random number generator.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// Is music enabled.
        /// </summary>
        private bool enabled = true;

        /// <summary>
        /// Creates a new MusicManager.
        /// </summary>
        public MusicManager()
        {
        }

        /// <summary>
        /// Load the music.
        /// </summary>
        /// <param name="Content">The content manager.</param>
        public void Load(ContentManager Content)
        {
            Stream stream = TitleContainer.OpenStream("Content/Music/Songs.txt");
            StreamReader reader = new StreamReader(stream);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("//"))
                    continue;
                Song song = Content.Load<Song>("Music/" + line);
                
                songs.Add(song);
            }
            reader.Close();
            stream.Close();
        }

        /// <summary>
        /// Toggle if music is enabled.
        /// </summary>
        public void Toggle()
        {
            enabled = !enabled;
            if (!enabled)
                MediaPlayer.Stop();
        }

        /// <summary>
        /// Update the music manager.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            if (!enabled)
                return;
            if (MediaPlayer.State == MediaState.Stopped)
            {
                PlayRandomSong();
            }
        }

        /// <summary>
        /// Play a random song.
        /// </summary>
        private void PlayRandomSong()
        {
            // Try and stop the current song.
            try
            {
                MediaPlayer.Stop();
            }
            catch (Exception ex)
            {
            }
            int index = random.Next(0, songs.Count());
            Song song = songs[index];
            MediaPlayer.Play(song);
        }

    }
}
