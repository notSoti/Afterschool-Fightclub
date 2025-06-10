using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class EndPanelScript : MonoBehaviour
{
    public GameObject exitButton;
    public GameObject endPanel;
    private TMP_Text winnerText;
    private AudioManager audioManager;
    void Start()
    {
        exitButton.SetActive(false);
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void BringPanel(string winner)
    {
        endPanel.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        winnerText = GameObject.FindGameObjectWithTag("WinText").GetComponent<TextMeshProUGUI>();
        if (winner == "Player 1")
        {
            audioManager.PlaySFX(audioManager.victorysound);
        }
        else
        {
            audioManager.PlaySFX(audioManager.defeatsound);
        }
        winnerText.text = $"{winner} Wins!";
        exitButton.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);//unselect previous object
        EventSystem.current.SetSelectedGameObject(exitButton);
    }
   
}
