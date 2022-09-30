using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static string musicVolumeKey = "MusicVolume";
    private static string sfxVolumeKey = "SFXVolume";

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private List<AudioSource> musicSources;
    private List<AudioSource> sfxSources;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        #region Organize Audio Sources
        musicSources = new List<AudioSource>();
        sfxSources = new List<AudioSource>();

        var audioSources = FindObjectsOfType<AudioSource>().ToList();

        foreach(var source in audioSources)
        {
            if (source.CompareTag("MusicSource"))
            {
                musicSources.Add(source);
            }
            else if (source.CompareTag("SFXSource"))
            {
                sfxSources.Add(source);
            }
        }
        #endregion

        musicVolume = PlayerPrefs.GetFloat(musicVolumeKey, 1f);
        sfxVolume = PlayerPrefs.GetFloat(sfxVolumeKey, 1f);

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        PlayerPrefs.SetFloat(musicVolumeKey, volume);

        foreach (var source in musicSources)
        {
            source.volume = musicVolume;
        }

        musicVolumeSlider.value = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat(sfxVolumeKey, volume);

        foreach (var source in sfxSources)
        {
            source.volume = sfxVolume;
        }

        sfxVolumeSlider.value = sfxVolume;
    }
}
