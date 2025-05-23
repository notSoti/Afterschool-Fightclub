using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class SelectionScript : MonoBehaviour
{
    private GameManager gameManager;
    // select character script

    public enum Character { Tsuki, Mihu };
    [Header("--------Chosen Character--------")]
    public Character player;
    // doesnt do anything if on difficulty object

    public enum Difficulty { Easy, Normal, Hard, Extreme };
    [Header("---------Bot Difficulty---------")]
    public Difficulty botdifficulty;
    // doesnt do anything if on player object
    [SerializeField]
    private TMP_Text difftext;

    void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager instance not found!");
            return;
        }

        // Update UI to reflect current settings
        UpdateDifficultyText();
    }

    private void UpdateDifficultyText()
    {
        if (difftext != null)
        {
            string difficultyName = gameManager.aiDifficulty.ToString();
            difftext.text = $"Current Difficulty: {difficultyName}";
        }
    }

    public void SelectCharacter()
    {
        if (gameManager != null)
        {
            GameManager.CharacterChoice choice = (GameManager.CharacterChoice)player;
            gameManager.SetSelectedCharacter(choice, false);
        }
    }
    public void SelectDifficulty()
    {
        if (gameManager != null)
        {
            FighterAI.Difficulty aiFifficulty = (FighterAI.Difficulty)botdifficulty;
            gameManager.SetAIDifficulty(aiFifficulty);
        }
    }

    // --- Difficulty options ---
    public void SelectEasy()
    {
        botdifficulty = Difficulty.Easy;
        SelectDifficulty();
        difftext.text = "Current Difficulty: Easy";
    }
    public void SelectNormal()
    {
        botdifficulty = Difficulty.Normal;
        SelectDifficulty();
        difftext.text = "Current Difficulty: Normal";
    }
    public void SelectHard()
    {
        botdifficulty = Difficulty.Hard;
        SelectDifficulty();
        difftext.text = "Current Difficulty: Hard";
    }
    public void SelectExtreme()
    {
        botdifficulty = Difficulty.Extreme;
        SelectDifficulty();
        difftext.text = "Current Difficulty: Extreme";
    }


    // --- Character options ---
    public void SelectTsuki()
    {
        player = Character.Tsuki;
        SelectCharacter();
    }

    public void SelectMihu()
    {
        player = Character.Mihu;
        SelectCharacter();
    }

    public void SelectRandom()
    {
        player = (Character)Random.Range(0, 2);
        SelectCharacter();
    }

}
