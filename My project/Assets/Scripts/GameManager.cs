using UnityEngine;

public class GameManager : MonoBehaviour {
    public enum CharacterChoice {
        Tsuki,
        Mihu
    }

    [Header("Character Selection")]
    public CharacterChoice selectedCharacter = CharacterChoice.Tsuki;

    [Header("Character Prefabs")]
    public GameObject[] playerPrefabs; // Element 0 = Tsuki, Element 1 = Mihu
    public GameObject[] aiPrefabs;     // Element 0 = Tsuki AI, Element 1 = Mihu AI

    [Header("Spawn Positions")]
    public Vector2 playerSpawnPosition = new(-5f, 0f); // Left side of the screen
    public Vector2 aiSpawnPosition = new(5f, 0f);      // Right side of the screen

    [Header("AI Settings")]
    public FighterAI.Difficulty aiDifficulty = FighterAI.Difficulty.Normal;    // Keep track of original scene characters
    private readonly FighterAI[] originalAIs;

    [Header("Camera")]
    public CameraTarget cameraTarget; // Assign this in inspector

    void Start() {
        // Disable original characters first
        DisableOriginalCharacters();

        // Wait a frame before spawning new characters
        StartCoroutine(SpawnCharactersNextFrame());
    }

    private void DisableOriginalCharacters() {
        var sceneFighters = FindObjectsByType<FighterAI>(FindObjectsSortMode.None);
        foreach (var fighter in sceneFighters) {
            if (!fighter.gameObject.name.Contains("(Clone)")) {
                fighter.gameObject.SetActive(false);
            }
        }
    }

    private System.Collections.IEnumerator SpawnCharactersNextFrame() {
        yield return null; // Wait one frame otherwise they spawn disabled
        SpawnCharacters();
    }

    void SpawnCharacters() {
        // Check if we have valid prefabs
        if (playerPrefabs == null || playerPrefabs.Length == 0 || 
            aiPrefabs == null || aiPrefabs.Length == 0) {
            Debug.LogError("Character prefabs not assigned in GameManager!");
            return;
        }

        // Get the selected character prefabs
        int characterIndex = (int)selectedCharacter;
        if (characterIndex >= playerPrefabs.Length || characterIndex >= aiPrefabs.Length) {
            Debug.LogError($"Prefab not found for character: {selectedCharacter}");
            return;
        }

        // Spawn player character
        GameObject player = Instantiate(playerPrefabs[characterIndex], playerSpawnPosition, Quaternion.identity);
        player.SetActive(true);

        // Spawn AI character
        GameObject ai = Instantiate(aiPrefabs[characterIndex], aiSpawnPosition, Quaternion.identity);
        ai.SetActive(true);

        // Configure the AI
        if (ai.TryGetComponent<FighterAI>(out var fighterAI)) {
            fighterAI.aiDifficulty = aiDifficulty;
            fighterAI.player = player.transform;
            fighterAI.freeze = false;
        }

        // Set up camera target
        if (cameraTarget != null) {
            cameraTarget.player1 = player.transform;
            cameraTarget.player2 = ai.transform;
        }
    }
}
