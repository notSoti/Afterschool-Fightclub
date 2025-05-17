using UnityEngine;

public class GameManager : MonoBehaviour {
    public enum CharacterChoice {
        Tsuki,
        Mihu
    }

    [Header("Character Selection")]
    public CharacterChoice selectedCharacter = CharacterChoice.Tsuki;

    [Header("Character Prefabs")]
    public GameObject[] characterPrefabs; // Element 0 = Tsuki, Element 1 = Mihu

    [Header("Spawn Positions")]
    public Vector2 playerSpawnPosition = new(-5f, 0f); // Left side of the screen
    public Vector2 aiSpawnPosition = new(5f, 0f);      // Right side of the screen

    [Header("AI Settings")]
    public FighterAI.Difficulty aiDifficulty = FighterAI.Difficulty.Normal;
    private readonly FighterAI[] originalAIs;

    [Header("Camera")]
    public CameraTarget cameraTarget; // Assign this in inspector

    void Start() {
        // Disable original characters first
        DisableOriginalCharacters();

        // Wait a frame before spawning new characters
        StartCoroutine(SpawnCharactersNextFrame());
    }

    public void SetSelectedCharacter(CharacterChoice character) {
        selectedCharacter = character;
    }

    public void SetAIDifficulty(FighterAI.Difficulty difficulty) {
        aiDifficulty = difficulty;
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
        if (characterPrefabs == null || characterPrefabs.Length == 0) {
            Debug.LogError("Character prefabs not assigned in GameManager!");
            return;
        }

        // Get the selected character prefab
        int characterIndex = (int)selectedCharacter;
        if (characterIndex >= characterPrefabs.Length) {
            Debug.LogError($"Prefab not found for character: {selectedCharacter}");
            return;
        }

        // Spawn player character
        GameObject player = Instantiate(characterPrefabs[characterIndex], playerSpawnPosition, Quaternion.identity);
        player.name = $"{selectedCharacter}_Player";
        
        // Set up player-specific components
        if (player.TryGetComponent<FighterAI>(out var playerAI)) {
            playerAI.enabled = false;
        }
        if (player.TryGetComponent<ControllerMovements>(out var playerControls)) {
            playerControls.enabled = true;
        }
        // Ensure hitbox is disabled at spawn
        if (player.transform.Find("Player Hitbox").GetComponent<Collider2D>() is Collider2D hitbox) {
            hitbox.enabled = false;
        }
        player.SetActive(true);

        // Spawn AI character
        GameObject ai = Instantiate(characterPrefabs[characterIndex], aiSpawnPosition, Quaternion.identity);
        ai.name = $"{selectedCharacter}_AI";
        
        // Configure the AI
        if (ai.TryGetComponent<FighterAI>(out var fighterAI)) {
            fighterAI.enabled = true;
            fighterAI.aiDifficulty = aiDifficulty;
            fighterAI.player = player.transform;
            fighterAI.freeze = false;
        }
        else {
            Debug.LogWarning("FighterAI component not found on AI character prefab!");
        }
        ai.SetActive(true);

        // Set up camera target
        if (cameraTarget != null) {
            cameraTarget.player1 = player.transform;
            cameraTarget.player2 = ai.transform;
        }
    }
}
