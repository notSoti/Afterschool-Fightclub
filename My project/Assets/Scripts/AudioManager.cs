using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource; // music and soundeffects
    [SerializeField] AudioSource SFXSource;
    public static float musicValue;
    public static float SFXValue;

    [Header("----------- Audio Clip -----------")]
    public AudioClip backround1; //main menu music
    public AudioClip backround2; //character selection music (temp victory music)
    public AudioClip backround3; // main game backround music
    public AudioClip hurt;
    public AudioClip death;
    public AudioClip specialbell; // special ability is available


    private void Start()
    {
        switch (SceneManager.GetActiveScene().buildIndex) // select sound pou 8a pextei, analoga scene
        {
            case 0:
                musicSource.clip = backround1; 
                break;

            case 1:
                musicSource.clip = backround2;
                break;

            case 2:
                musicSource.clip = backround3;
                break;

            case 3:
                musicSource.clip = backround2;
                break;
        }
        musicSource.Play(); // play selected sound
    }

    public void PlaySFX(AudioClip clip) // audioclip is type object
    {
        SFXSource.PlayOneShot(clip);
    }

}
