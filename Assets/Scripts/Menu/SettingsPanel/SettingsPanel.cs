using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private CustomToggle musicToggle, soundsToggle, vibrationToggle;

    private void Start()
    {
        musicToggle.IsOn = !AudioController.Instance.MusicMuted;
        musicToggle.onSwitched.AddListener(OnMusicSwitched);

        soundsToggle.IsOn = !AudioController.Instance.SoundsMuted;
        soundsToggle.onSwitched.AddListener(OnSoundsSwitched);

        vibrationToggle.IsOn = VibrationManager.Instance.VibrationEnabled;
        vibrationToggle.onSwitched.AddListener(OnVibrationSwitched);
    }

    private void OnMusicSwitched(bool v)
    {
        AudioController.Instance.SetMusicMuted(!v);
    }

    private void OnSoundsSwitched(bool v)
    {
        AudioController.Instance.SetSoundsMuted(!v);
    }

    private void OnVibrationSwitched(bool v)
    {
        VibrationManager.Instance.SetVibrationEnabled(v);
    }
}
