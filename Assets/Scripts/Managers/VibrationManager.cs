using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager : Singleton<VibrationManager>
{
    public bool VibrationEnabled { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        VibrationEnabled = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
    }

    public void SetVibrationEnabled(bool v)
    {
        VibrationEnabled = v;
        PlayerPrefs.SetInt("VibrationEnabled", v ? 1 : 0);
    }

    public void Vibrate()
    {
        if (VibrationEnabled)
        {
            Handheld.Vibrate();
        }
    }
}
