using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    public enum CharacterChoice
    {
        Tsuki,
        Mihu
    }
    public enum MapChoice
    {
        Map1,
        Map2
    }

    [Header("Character Selection")]
    public CharacterChoice selectedPlayerCharacter = CharacterChoice.Tsuki;
    public CharacterChoice selectedAICharacter = CharacterChoice.Mihu;
    [Header("Map Selection")]
    public MapChoice selectedMap;

    [Header("Character Prefabs")]
    public GameObject[] characterPrefabs; // Element 0 = Tsuki, Element 1 = Mihu

    [Header("Spawn Positions")]
    public Vector2 playerSpawnPosition = new(-5f, 0f); // Left side of the screen
    public Vector2 aiSpawnPosition = new(5f, 0f);      // Right side of the screen

    [Header("AI Settings")]
    public FighterAI.Difficulty aiDifficulty = FighterAI.Difficulty.Normal;

    [Header("Camera")]
    public CameraTarget cameraTarget; // Assign this in inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else // this bare else might cause issues elsewhere, idk
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "FightScene")
        {
            StartCoroutine(InitializeFightScene());
        }
    }

    private IEnumerator InitializeFightScene()
    {
        yield return null; // Wait one frame

        // Find the camera target in the scene
        cameraTarget = FindFirstObjectByType<CameraTarget>();
        if (cameraTarget == null && SceneManager.GetActiveScene().name == "FightScene")
        {
            Debug.LogError("No CameraTarget found in the Fight scene!");
            yield break;
        }

        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogError("No character prefabs found!");
            yield break;
        }

        // Disable original characters first
        DisableOriginalCharacters();

        // Spawn new characters
        SpawnCharacters();
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Character Selection")
        {
            // Set default random player character
            selectedPlayerCharacter = (CharacterChoice)Random.Range(0, 2);
            selectedAICharacter = (CharacterChoice)Random.Range(0, 2);
            aiDifficulty = FighterAI.Difficulty.Normal;
            selectedMap = MapChoice.Map1;
        }
    }

    public void SetSelectedCharacter(CharacterChoice character, bool isAI = false)
    {
        if (isAI)
        {
            selectedAICharacter = character;
        }
        else
        {
            selectedPlayerCharacter = character;
        }
    }

    public void SetAIDifficulty(FighterAI.Difficulty difficulty)
    {
        aiDifficulty = difficulty;
    }
    public void SetSelectedMap(MapChoice map)
    {
        selectedMap = map;
    }

    private void DisableOriginalCharacters()
    {
        var sceneFighters = FindObjectsByType<FighterAI>(FindObjectsSortMode.None);
        foreach (var fighter in sceneFighters)
        {
            if (!fighter.gameObject.name.Contains("(Clone)"))
            {
                fighter.gameObject.SetActive(false);
            }
        }
    }

    void SpawnCharacters()
    {

        // Get the selected character prefabs
        int playerCharacterIndex = (int)selectedPlayerCharacter;
        int aiCharacterIndex = (int)selectedAICharacter;

        // Spawn player character
        GameObject player = Instantiate(characterPrefabs[playerCharacterIndex], playerSpawnPosition, Quaternion.identity);
        player.name = $"{selectedPlayerCharacter}_Player";

        // Set up player-specific components
        if (player.TryGetComponent<FighterAI>(out var playerAI))
        {
            playerAI.enabled = false;
        }
        if (player.TryGetComponent<ControllerMovements>(out var playerControls))
        {
            playerControls.enabled = true;
        }
        // Ensure hitbox is disabled at spawn
        if (player.transform.Find("Player Hitbox").GetComponent<Collider2D>() is Collider2D hitbox)
        {
            hitbox.enabled = false;
        }
        player.SetActive(true);

        // Spawn AI character
        GameObject ai = Instantiate(characterPrefabs[aiCharacterIndex], aiSpawnPosition, Quaternion.identity);
        ai.name = $"{selectedAICharacter}_AI";

        // Configure the AI
        if (ai.TryGetComponent<FighterAI>(out var fighterAI))
        {
            fighterAI.enabled = true;
            fighterAI.aiDifficulty = aiDifficulty;
            fighterAI.player = player.transform;
            fighterAI.freeze = false;
        }
        ai.SetActive(true);

        // Set up camera target
        if (cameraTarget != null)
        {
            cameraTarget.player1 = player.transform;
            cameraTarget.player2 = ai.transform;
        }
    }
}
