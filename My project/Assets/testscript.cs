using UnityEngine;

public class testscript : MonoBehaviour
{
    AudioManager audioManager;

    public void CallSound()
    { 
        audioManager.PlaySFX(audioManager.hurt);
    }

}
