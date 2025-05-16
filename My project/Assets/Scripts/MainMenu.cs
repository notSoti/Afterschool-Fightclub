
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {
  
    public GameObject mainMenu, optionsMenu, controlsPanel;

    public GameObject optionsFirstButton, optionsClosedButton, controlsFirstButton, controlsClosedButton;


    public void PlayGame() {
        SceneManager.LoadScene("Character Selection");
    }

    public void OpenOptions()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null); //unselect previous object
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); //unselect previous object
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
        mainMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null); //unselect previous object
        EventSystem.current.SetSelectedGameObject(controlsFirstButton);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        mainMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); //unselect previous object
        EventSystem.current.SetSelectedGameObject(controlsClosedButton);
    }

    public void QuitGame() {
        Application.Quit();
        Debug.Log("Quitting");
    }
}
