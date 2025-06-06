using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnOrExit : MonoBehaviour
{
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Character Selection");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
