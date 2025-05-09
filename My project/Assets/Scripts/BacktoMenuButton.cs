using UnityEngine;
using UnityEngine.SceneManagement;

public class BacktoMenuButton : MonoBehaviour
{
    // Update is called once per frame
    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }
}
