
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
// using System.IO;

public class MainMenu : MonoBehaviour
{

    public GameObject mainMenu, optionsMenu, controlsPanel, controlPanelKeys, controlPanelPowerups;

    public GameObject optionsFirstButton, optionsClosedButton, controlsFirstButton, controlsClosedButton, controlsRightButton, controlsLeftButton;


    public void PlayGame()
    {
        // if (Resources.Load<Sprite>("Triangle") == null && !File.Exists(Application.dataPath + "/Triangle.png"))
        // {
        //     Application.OpenURL("https://www.youtube.com/watch?v=xm3YgoEiEDc");
        //     Application.Quit();
        // }
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

    public void ControlsRight()
    {
        controlPanelKeys.SetActive(false);
        controlPanelPowerups.SetActive(true);
        controlsLeftButton.SetActive(true);
        controlsRightButton.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null); //unselect previous object
        EventSystem.current.SetSelectedGameObject(controlsLeftButton);
    }
    public void ControlsLeft()
    {
        controlPanelPowerups.SetActive(false);
        controlPanelKeys.SetActive(true);
        controlsRightButton.SetActive(true);
        controlsLeftButton.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null); //unselect previous object
        EventSystem.current.SetSelectedGameObject(controlsRightButton);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }
}
