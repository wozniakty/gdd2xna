using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace gdd2xna
{
    class SoundManager
    {
        #region instance variables
        private List<NamedSoundEffect> soundList = new List<NamedSoundEffect>();
        private String ContentSoundPath = "Sound//";
        private String SoundInfoFileName = "../../../../gdd2xnaContent/Sound/SoundInfo.txt";
        private Game game;
        #endregion

        #region public methods
        //constructor
        public SoundManager(Game g)
        {
            this.game = g;
        }

        public void Play(SoundEffectName name)
        {
            int i = 0;
            bool played = false;
            while(i < soundList.Count && played == false)
            {
                if (name.Equals(soundList[i].Name))
                {
                    soundList[i].Play();
                    played = true;
                }
                i++;
            }
            if (played == false) throw new Exception("SoundManager asked to play SoundEffect that does not exist");
        }

        public void Initialize()
        {
            GetSoundInfo();
        }
        #endregion

        #region internal
        //Adds to soundList and gives each NamedSoundEffect its Name and Audio
        private void GetSoundInfo()
        {
            String commentIndicator = "*";
            String seperator = ".";

            String line;
            NamedSoundEffect toAdd;
            String fileName;
            int seperatorIndex;

            StreamReader reader = new StreamReader(SoundInfoFileName);

            while ((line = reader.ReadLine()) != null)
            {
                //make sure line is not commented out and has the seperator
                if (!line.Contains(commentIndicator) && line.Contains(seperator))
                {
                    toAdd = new NamedSoundEffect();
                    line.Trim();
                    seperatorIndex = line.IndexOf(seperator);
                    //get file name
                    fileName = line.Substring(0, seperatorIndex);
                    //load SoundEffect with said file name from Content
                    try { toAdd.Audio = game.Content.Load<SoundEffect>(ContentSoundPath + fileName); }
                    catch { throw new FileNotFoundException("SoundInfo.txt file name not found in Content"); }
                    //get SoundEffectName
                    foreach(int value in Enum.GetValues(typeof(SoundEffectName)))
                    {
                        String name = Enum.GetName(typeof(SoundEffectName), value);
                        //compare the given SoundEffectName string with each SoundEffectName string
                        if (name.Equals(line.Substring(seperatorIndex + 1)))
                        {
                            toAdd.Name = (SoundEffectName)Enum.Parse(typeof(SoundEffectName), name);
                        }
                    }

                    //add song to songList
                    soundList.Add(toAdd);
                }
            }
        }

        //includes a SoundEffectName, SoundEffect, and it's own SoundEffectInstance
        internal class NamedSoundEffect
        {
            internal SoundEffectName Name { get; set; }
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
}