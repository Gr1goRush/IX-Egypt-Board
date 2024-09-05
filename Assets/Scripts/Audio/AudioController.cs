using System.Collections;
using UnityEngine;

public class AudioController : Singleton<AudioController>
{
    public SoundsController Sounds => soundsController;
    public bool SoundsMuted { get; private set; }
    public bool MusicMuted { get; private set; }

    [SerializeField] private AudioSource soundsSource, musicSource;
    [SerializeField] private SoundsController soundsController;

    protected override void Awake()
    {
        base.Awake();

        SoundsMuted = PlayerPrefs.GetInt("MuteSounds", 0) == 1;
        soundsSource.mute = SoundsMuted;

        MusicMuted = PlayerPrefs.GetInt("MuteMusic", 0) == 1;
        musicSource.mute = MusicMuted;
    }

    public void SetSoundsMuted(bool v)
    {
        SoundsMuted = v;
        soundsSource.mute = v;
        PlayerPrefs.SetInt("MuteSounds", v ? 1 : 0);
    }

    public void SetMusicMuted(bool v)
    {
        MusicMuted = v;
        musicSource.mute = v;
        PlayerPrefs.SetInt("MuteMusic", v ? 1 : 0);
    }

    public void PlayOneShot(AudioClip clip, float volume)
    {
        soundsSource.PlayOneShot(clip, volume);
    }
}