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
        if (musicSlider != null)
        {
            AudioManager.musicValue = musicSlider.value;
            // gotta jail the sliders in an IF cuz they dont exist in all scenes
        }
        float volume = AudioManager.musicValue;
        myMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume); // save music volume settings between game restarts

    }

    public void SetSFXVolume()
    {
        if (SFXSlider != null)
        {
            AudioManager.SFXValue = SFXSlider.value;
            // JAILLLLLL
        }
        float volume = AudioManager.SFXValue;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume); // save sfx volume settings between game restarts

    }

    private void LoadVolume()
    {
        if (SFXSlider != null && musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            // II II II
        }
        SetMusicVolume();
        SetSFXVolume();
    }

}
