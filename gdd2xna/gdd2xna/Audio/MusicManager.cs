using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace gdd2xna
{
    /// <summary>
    /// The only instance of this should be in an instance of Game.cs, this currently loads and plays instances of SoundEffect randomly
    /// from all the instances of SoundEffect that were loaded as directed by MusicInfo.txt
    /// </summary>
    public class MusicManager
    {
        #region instance variables
        private List<Song> songList = new List<Song>();
        private String ContentMusicPath = "Music//";
        private String MusicInfoFileName = "../../../../gdd2xnaContent/Music/MusicInfo.txt";
        private Game game;
        private bool canPlay;
        private Song currentSong;
        private Random gen;
        private double secondsSinceSongStart;
        #endregion

        //relevent if you just want to start or stop the music from code outside this
        #region public methods
        /// <summary>
        /// Stops music from being played, if a song is playing, it isn't saved for when Play() is called
        /// </summary>
        public void Stop()
        {
            if (currentSong != null) currentSong.Stop();
            currentSong = null;
            canPlay = false;
        }

        /// <summary>
        /// Allows the class to play music
        /// Does nothing if music is already playing
        /// </summary>
        public void Play()
        {
            canPlay = true;
        }

        //constructor
        public MusicManager(Game g)
        {
            this.game = g;
            gen = new Random();
        }
        public void Initialize()
        {
            canPlay = true;
            GetMusicInfo();
        }
        #endregion

        #region internal
        // Creates instances of Song for songList from MusicInfoFileName
        private void GetMusicInfo()
        {
            String commentIndicator = "*";
            String seperator = ".";

            String line;
            Song toAdd;
            int seperatorIndex;

            StreamReader reader = new StreamReader(MusicInfoFileName);

            while ((line = reader.ReadLine()) != null)
            {
                //make sure line is not commented out and has the seperator
                if (!line.Contains(commentIndicator) && line.Contains(seperator))
                {
                    toAdd = new Song();
                    line.Trim();
                    seperatorIndex = line.IndexOf(seperator);
                    //get name
                    toAdd.Name = line.Substring(0, seperatorIndex);
                    //get length in seconds
                    try { toAdd.SecondsLength = Convert.ToInt32(line.Substring(seperatorIndex + 1)); }
                    catch { throw new FormatException("MusicInfo.txt song length formatted incorrectly"); }
                    //load SoundEffect with same name from Content
                    try { toAdd.Audio = game.Content.Load<SoundEffect>(ContentMusicPath + toAdd.Name); }
                    catch { throw new FileNotFoundException("MusicInfo.txt song name not found in Content"); }

                    //add song to songList
                    songList.Add(toAdd);
                }
            }
        }

        internal void Update(GameTime gameTime)
        {
            if (canPlay)
            {
                // add to time song has been playing (reset to zero when PlaySong() is called)
                secondsSinceSongStart += gameTime.ElapsedGameTime.TotalSeconds;
                // play first song or new song when sufficient time has passed
                if ((currentSong == null || secondsSinceSongStart >= currentSong.SecondsLength) && songList.Count > 0)
                {
                    PlayRandomSong();
                }
            }
        }

        // Calls PlaySong() using a random song in songList 
        private void PlayRandomSong()
        {
            int songIndex = gen.Next(songList.Count);
            // if new song would be same as old, change to next song in list
            if (currentSong != null && songList.IndexOf(currentSong) == songIndex)
            {
                songIndex = (songIndex + 1) % songList.Count;
            }
            PlaySong(songList[songIndex]);
        }

        private void PlaySong(Song s)
        {
            if (currentSong != null) currentSong.Stop();
            secondsSinceSongStart = 0;
            s.Play();
            currentSong = s;
        }
    }
    // like a struct, but with methods
    internal class Song
    {
        internal String Name { get; set; }
        internal int SecondsLength { get; set; }
        internal SoundEffect Audio
        {
            set
            {
                instance = value.CreateInstance();
            }
        }
        private SoundEffectInstance instance;
        internal void Play()
        {
            if (instance != null) instance.Play();
        }

        internal void Stop()
        {
            if (instance != null) instance.Stop();
        }
    }
        #endregion
}