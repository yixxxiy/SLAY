using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XGame
{

    public class AudioManager : Singleton<AudioManager>
    {
        private GameObject Root;
        private AudioSource mBgmAs;
        private bool mMusicOn = false;
        private bool mSoundOn = false;

        public bool MusicOn
        {
            get
            {
                return mMusicOn;
            }
            set
            {
                if (value != mMusicOn)
                {
                    mMusicOn = value;
                    LocalDataUtil.SetBool(AudioModel.MusicOnKey, value);
                }
            }
        }
        public bool SoundOn
        {
            get
            {
                return mSoundOn;
            }
            set
            {
                if (value != mSoundOn)
                {
                    mSoundOn = value;
                    LocalDataUtil.SetBool(AudioModel.SoundOnKey, value);
                }
            }
        }

        public void Init()
        {
            mTemp.Clear();
            if (Root != null) return;
            Root = new GameObject("AudioRoot");
            GameObject.DontDestroyOnLoad(Root);
            mMusicOn = LocalDataUtil.GetBool(AudioModel.MusicOnKey, true);
            mSoundOn = LocalDataUtil.GetBool(AudioModel.SoundOnKey, true);
            //PlayBgm();
        }
        private readonly Dictionary<string, AudioClip> loadedAudioclipDic = new Dictionary<string, AudioClip>();
        private readonly Dictionary<int, AudioClip> loadedBallNumClipDic = new Dictionary<int, AudioClip>();
        private readonly List<AudioSource> allPlayer = new List<AudioSource>();
        private readonly List<string> allPlayerType = new List<string>();

        public AudioSource PlayOneShot(string playArea, bool loop = false)
        {
            
            AudioClip tempClip = null;
            if (!loadedAudioclipDic.ContainsKey(playArea))
            {
                tempClip = ResourceUtil.Load<AudioClip>(AudioModel.LoadPath + playArea);
                if (tempClip == null)
                {
                    LogUtil.Err("配置的音频文件路径错误:" + playArea);
                    return null;
                }
                loadedAudioclipDic.Add(playArea, tempClip);
            }

            loadedAudioclipDic.TryGetValue(playArea, out tempClip);
            if (tempClip == null)
            {
                loadedAudioclipDic.Remove(playArea);
                LogUtil.Err("音频路径配置错误，类型:" + playArea);
                return null;
            }
            int asCount = allPlayer.Count;
            AudioSource tempAs;
            int areaIndex = -1;
            int stopIndex = -1;
            for (int i = 0; i < asCount; i++)
            {
                if (allPlayerType[i] == playArea && !allPlayer[i].isPlaying)
                    if (areaIndex == -1)
                        areaIndex = i;
                if (!allPlayer[i].isPlaying)
                    if (stopIndex == -1)
                        stopIndex = i;
            }
            if (areaIndex != -1)
            {
                tempAs = allPlayer[areaIndex];
                allPlayerType[areaIndex] = playArea;
            }
            else if (stopIndex != -1)
            {
                tempAs = allPlayer[stopIndex];
                allPlayerType[stopIndex] = playArea;
            }
            else
            {
                tempAs = Root.AddComponent<AudioSource>();
                allPlayer.Add(tempAs);
                allPlayerType.Add(playArea);
            }
            tempAs.clip = tempClip;
            tempAs.loop = loop;
            tempAs.mute = !SoundOn;
            tempAs.pitch = 1;
            tempAs.Play();
            //Debug.Log(tempAs.name + "     ???????????????????");
            return tempAs;

        }


        public List<AudioSource> mTemp = new List<AudioSource>();

        public AudioSource PlayOneShot_1(string playArea, bool loop = false, float pitch = 1f)
        {
            AudioClip tempClip = null;
            if (!loadedAudioclipDic.ContainsKey(playArea))
            {
                tempClip = ResourceUtil.Load<AudioClip>(AudioModel.LoadPath + playArea);
                if (tempClip == null)
                {
                    LogUtil.Err("配置的音频文件路径错误:" + playArea);
                    return null;
                }
                loadedAudioclipDic.Add(playArea, tempClip);
            }

            loadedAudioclipDic.TryGetValue(playArea, out tempClip);
            if (tempClip == null)
            {
                loadedAudioclipDic.Remove(playArea);
                LogUtil.Err("音频路径配置错误，类型:" + playArea);
                return null;
            }
            int asCount = allPlayer.Count;
            AudioSource tempAs;
            int areaIndex = -1;
            int stopIndex = -1;
            for (int i = 0; i < asCount; i++)
            {
                if (allPlayerType[i] == playArea && !allPlayer[i].isPlaying)
                    if (areaIndex == -1)
                        areaIndex = i;
                if (!allPlayer[i].isPlaying)
                    if (stopIndex == -1)
                        stopIndex = i;
            }
            if (areaIndex != -1)
            {
                tempAs = allPlayer[areaIndex];
                allPlayerType[areaIndex] = playArea;
            }
            else if (stopIndex != -1)
            {
                tempAs = allPlayer[stopIndex];
                allPlayerType[stopIndex] = playArea;
            }
            else
            {
                tempAs = Root.AddComponent<AudioSource>();
                allPlayer.Add(tempAs);
                allPlayerType.Add(playArea);
            }
            tempAs.clip = tempClip;
            tempAs.loop = loop;
            tempAs.mute = !SoundOn;
            tempAs.pitch = pitch;
            tempAs.Play();
            mTemp.Add(tempAs);
           // Debug.Log(tempAs.clip + "  tempAs");
            return tempAs;

        }
        public void StopAudio(string playArea)
        {
            foreach(var audio in mTemp)
            {
                if(audio.clip.ToString().Contains(playArea))
                {
                    audio.Stop();
                }
            }
        }


        public AudioSource PlayLoop(string playArea)
        {
            return PlayOneShot(playArea, true);
        }
        private void PlayBgm()
        {
            AudioClip tempClip = ResourceUtil.Load<AudioClip>(AudioModel.LoadPath + AudioModel.BGM);
            if (tempClip == null)
            {
                LogUtil.Err("背景音乐文件路径配置错误");
                return;
            }
            mBgmAs = Root.AddComponent<AudioSource>();
            mBgmAs.clip = tempClip;
            mBgmAs.loop = true;
            mBgmAs.mute = !MusicOn;
            mBgmAs.Play();
        }

        public void ChangeBgm(string playArea)
        {
            AudioClip tempClip = ResourceUtil.Load<AudioClip>(AudioModel.LoadPath + playArea);
            if (tempClip == null)
            {
                LogUtil.Err("配置的音频文件路径错误:" + playArea);             
            }
            else
            {
                if (Root.GetComponent<AudioSource>() == null)
                {
                    mBgmAs = Root.AddComponent<AudioSource>();
                }
                mBgmAs.clip = tempClip;
                mBgmAs.loop = true;
                mBgmAs.mute = !MusicOn;
                mBgmAs.Play();
            }
        }

        public void SetMusicState(bool isOn)
        {
            mBgmAs.mute = !isOn;
            MusicOn = isOn;
        }
        public void SetSoundState(bool isOn)
        {
            SoundOn = isOn;
            int count = allPlayer.Count;
            for (int i = 0; i < count; i++)
            {
                allPlayer[i].mute = !isOn;
            }
        }
        public void PauseBgm(bool pause)
        {
            if (pause)
                mBgmAs.Pause();
            else
                mBgmAs.UnPause();
        }

    }

}
