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
    public bool isAI;
    // doesnt do anything if on difficulty object

    public enum Difficulty { Easy, Normal, Hard, Extreme };
    [Header("---------Bot Difficulty---------")]
    public Difficulty botdifficulty;
    // doesnt do anything if on player object
    public enum Map { Map1, Map2 };
    [Header("--------Chosen Map--------")]
    public Map map;

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
    }

    private void UpdateDifficultyText(string difficultyName)
    {
        difftext.text = $"Current Difficulty: {difficultyName}";
    }

    public void SelectCharacter()
    {
        if (gameManager != null)
        {
            GameManager.CharacterChoice choice = (GameManager.CharacterChoice)player;
            gameManager.SetSelectedCharacter(choice, isAI);
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
    public void SelectMap()
    {
        if (gameManager != null)
        {
            GameManager.MapChoice mapChoice = (GameManager.MapChoice)map;
            gameManager.SetSelectedMap(mapChoice);
        }
    }

    // --- Difficulty options ---
    public void SelectEasy()
    {
        botdifficulty = Difficulty.Easy;
        SelectDifficulty();
        UpdateDifficultyText("Easy");
    }
    public void SelectNormal()
    {
        botdifficulty = Difficulty.Normal;
        SelectDifficulty();
        UpdateDifficultyText("Normal");
    }
    public void SelectHard()
    {
        botdifficulty = Difficulty.Hard;
        SelectDifficulty();
        UpdateDifficultyText("Hard");
    }
    public void SelectExtreme()
    {
        botdifficulty = Difficulty.Extreme;
        SelectDifficulty();
        UpdateDifficultyText("Extreme");
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

    // --- Map options ---
    public void SelectMap1()
    {
        map = Map.Map1;
        SelectMap();
    }
    public void SelectMap2()
    {
        map = Map.Map2;
        SelectMap();
    }
}
