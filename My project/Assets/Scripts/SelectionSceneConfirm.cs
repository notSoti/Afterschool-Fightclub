using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SelectionSceneButtons : MonoBehaviour
{
    public GameObject confirmcharacterbutton, difficultybutton;
    public GameObject countdownscreen;

    public void ConfirmCharacter()
    {
        confirmcharacterbutton.SetActive(false);
        countdownscreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); //unselect previous object
        EventSystem.current.SetSelectedGameObject(difficultybutton);
    }
    public void StartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void UnConfirmCharacter()
    {
        confirmcharacterbutton.SetActive(true);
        countdownscreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null); //unselect previous object
        EventSystem.current.SetSelectedGameObject(confirmcharacterbutton);
    }

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }


}
