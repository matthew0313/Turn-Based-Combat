using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Settings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        m_volume = 1.0f;
        m_musicVolume = 1.0f;
    }

    static float m_volume;
    public static float volume
    {
        get => m_volume;
        set
        {
            Debug.Log(value);
            if(m_volume != value)
            {
                m_volume = value;
                onVolumeChange?.Invoke();
            }
        }
    }
    public static Action onVolumeChange;

    static float m_musicVolume;
    public static float musicVolume
    {
        get => m_musicVolume;
        set
        {
            if (m_musicVolume != value)
            {
                m_musicVolume = value;
                onMusicVolumeChange?.Invoke();
            }
        }
    }
    public static Action onMusicVolumeChange;
}