using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundClipData
{
    public string id;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
}

public class SoundsController : MonoBehaviour
{
    [SerializeField] private SoundClipData[] soundClipsData;

    public void PlayOneShot(string clipId)
    {
        int clipIndex = -1;

        for (int i = 0; i < soundClipsData.Length; i++)
        {
            if (soundClipsData[i].id.Equals(clipId))
            {
                clipIndex = i;
                break;
            }
        }

        if (clipIndex < 0)
        {
            print("Clip " + clipId + " not found!");
            return;
        }

        SoundClipData clipData = soundClipsData[clipIndex];
        AudioController.Instance.PlayOneShot(clipData.clip, clipData.volume);
    }
}