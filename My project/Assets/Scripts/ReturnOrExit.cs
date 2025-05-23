using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnOrExit : MonoBehaviour
{
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
