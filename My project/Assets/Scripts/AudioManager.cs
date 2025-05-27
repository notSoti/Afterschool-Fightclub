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
    public AudioClip backround; //backround music, changes on each scene analoga ti ekxorisa sto AudioManager ekeinhs ths skhnhs
    public AudioClip hurt;
    public AudioClip death;
    public AudioClip specialbell; // notifies when special ability is available
    public AudioClip powerupring; // your phone linging, ling-ling-ling (notifies when powerup is picked-up)
    public AudioClip tsukipower;
    public AudioClip mihupower;
    public AudioClip victorysound;
    public AudioClip defeatsound;

    private void Start()
    {
        musicSource.clip = backround; // select sound na paijei
        musicSource.Play(); // play selected sound
    }
    public void TestSFX() // test hxos gia to menu
    {
        PlaySFX(specialbell);
    }
    public void PlaySFX(AudioClip clip) // audioclip is type object
    {
        SFXSource.PlayOneShot(clip);
    }

}
