using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator ConfirmCharacterSelection()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
