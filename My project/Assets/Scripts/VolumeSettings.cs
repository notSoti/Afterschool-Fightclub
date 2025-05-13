using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MyVolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume")) // if player has put their own volume input
        {
            LoadVolume();
        }
        else // if player dont got their own volume input
        { 
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    public void SetMusicVolume()
    { 
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume); // save music volume settings between game restarts

    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume); // save sfx volume settings between game restarts

    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        SetMusicVolume();
        SetSFXVolume();
    }

}
