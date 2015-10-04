using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace RPGProject
{
    public class SoundManager
    {
        string currentBackgroundMusic;
        public string CurrentBackgroundMusic
        {
            get
            {
                return currentBackgroundMusic;
            }
        }

        SoundEffectInstance backgroundMusicInstance;
        List<SoundEffect> loadedAudio;

        public SoundManager()
        {
            loadedAudio = new List<SoundEffect>();
        }

        public void LoadSound(string soundFileName)
        {
            bool soundExists = false;
            foreach (SoundEffect sound in loadedAudio)
            {
                if (sound.Name == soundFileName)
                {
                    soundExists = true;
                }
            }

            if (soundExists == false)
            {
               SoundEffect newSound = GameClass.ContentManager.Load<SoundEffect>(soundFileName);
               newSound.Name = soundFileName;
               loadedAudio.Add(newSound);
            }
        }
        public void PlaySoundEffect(string soundFileName)
        {
            bool soundExists = false;
            foreach (SoundEffect sound in loadedAudio)
            {
                if (sound.Name == soundFileName)
                {
                    soundExists = true;
                    sound.Play();
                }
            }

            if (soundExists == false)
            {
                throw new Exception("No sound found to play.");
            }
        }
        public void SetBackgroundMusic(string soundFileName, bool loop)
        {
            currentBackgroundMusic = soundFileName;
            bool soundExists = false;
            foreach (SoundEffect sound in loadedAudio)
            {
                if (sound.Name == soundFileName)
                {
                    soundExists = true;
                    backgroundMusicInstance = sound.CreateInstance();
                    backgroundMusicInstance.IsLooped = loop;
                }
            }

            if (soundExists == false)
            {
                throw new Exception("No such sound exists.");
            }
        }
        public void PlayBackgroundMusic()
        {
            switch (backgroundMusicInstance.State)
            {
                case SoundState.Paused:
                    backgroundMusicInstance.Resume();
                    break;
                case SoundState.Stopped:
                    backgroundMusicInstance.Play();
                    break;
            }
        }
        public void StopBackgroundMusic()
        {
            if(backgroundMusicInstance != null)
            {
                backgroundMusicInstance.Stop();
            }
        }
    }
}
