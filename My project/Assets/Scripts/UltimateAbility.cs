using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class UltimateAbility : MonoBehaviour
{    
    [SerializeField] private float maxUltCharge = 100f;
    [SerializeField] private int ultimateDamage = 20;
    [SerializeField] private int ultimateHealAmount = 20;

    private float currentCharge;
    private bool isUltimateReady;
    private bool isPlayer;
    private Image ultBar;
    private Health ownerHealth;
    private Health enemyHealth;
    private Animator animator;
    private GameObject enemyObject;

    public UnityEvent<float> onChargeChanged; // Sends charge percentage (0-1)
    public UnityEvent onUltimateReady;
    public UnityEvent onUltimateUsed;

    private void Start() {
        currentCharge = 0f;
        isUltimateReady = false;
        isPlayer = name.Contains("Player");
        
        // Find the appropriate ult bar based on whether this is a player or AI
        string barName = isPlayer ? "Player1UltBar" : "Player2UltBar";
        GameObject ultBarObj = GameObject.Find(barName);
        if (ultBarObj != null) {
            ultBar = ultBarObj.GetComponent<Image>();
            if (ultBar == null) {
                // Try to find it as a child component
                ultBar = ultBarObj.GetComponentInChildren<Image>();
            }
        }
        
        if (ultBar == null) {
            Debug.LogWarning($"Could not find ultimate bar '{barName}' for {gameObject.name}");
        }

        // Get components
        ownerHealth = GetComponent<Health>();
        animator = GetComponent<Animator>();

        // The enemy might not be spawned yet when this script starts
        // Wait until next frame to look for the enemy
        StartCoroutine(FindEnemyNextFrame());
    }

    private IEnumerator FindEnemyNextFrame() {
        yield return null; // Wait one frame to let other objects spawn

        string myName = gameObject.name;
        
        // Make sure we have the base character name
        string baseCharacterName;
        if (myName.Contains("Tsuki")) {
            baseCharacterName = "Tsuki";
        } else if (myName.Contains("Mihu")) {
            baseCharacterName = "Mihu";
        } else {
            Debug.LogError($"Unknown character name: {myName}");
            yield break;
        }

        // Determine if we're the player or AI
        isPlayer = myName.Contains("Player");
        
        // Find all possible opponents
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        // First try to find character of same type (for mirror matches)
        foreach (GameObject obj in allObjects) {
            if (obj != gameObject && // Not ourselves
                obj.name.Contains(baseCharacterName) && // Same character type
                (isPlayer ? obj.name.Contains("AI") : obj.name.Contains("Player"))) // Opposite player/AI status
            {
                enemyObject = obj;
                break;
            }
        }

        // If no same-character opponent found, look for any character
        if (enemyObject == null) {
            foreach (GameObject obj in allObjects) {
                if (obj != gameObject && // Not ourselves
                    (obj.name.Contains("Tsuki") || obj.name.Contains("Mihu")) && // Is a character
                    (isPlayer ? obj.name.Contains("AI") : obj.name.Contains("Player"))) // Opposite player/AI status
                {
                    enemyObject = obj;
                    break;
                }
            }
        }

        if (enemyObject != null) {
            enemyHealth = enemyObject.GetComponent<Health>();
        }
    }

    public void Update() {
        if (ultBar != null) {
            ultBar.fillAmount = Mathf.Clamp(GetChargePercentage(), 0, 1);
        }
    }

    public void AddCharge(float damageDealt) {
        if (isUltimateReady) return;

        currentCharge = Mathf.Min(maxUltCharge, currentCharge + (damageDealt * 2.5f));

        float chargePercentage = currentCharge / maxUltCharge;
        onChargeChanged?.Invoke(chargePercentage);

        if (currentCharge >= maxUltCharge && !isUltimateReady) {
            isUltimateReady = true;
            onUltimateReady?.Invoke();
        }
    }

    public void UseUltimate() {
        if (!isUltimateReady) return;

        onUltimateUsed?.Invoke();
        
        // Set animator parameter
        animator.SetBool("isUlting", true);

        // Execute character-specific ultimate ability
        if (name.Contains("Tsuki")) {
            // Tsuki's ultimate: Deal damage to enemy
            if (enemyHealth != null) {
                enemyHealth.TakeDamage(ultimateDamage);
            }
        }
        else if (name.Contains("Mihu")) {
            // Mihu's ultimate: Heal self
            if (ownerHealth != null) {
                ownerHealth.Heal(ultimateHealAmount);
            }
        }
        
        // Reset charge
        currentCharge = 0f;
        isUltimateReady = false;
        animator.SetBool("isUlting", false);
        onChargeChanged?.Invoke(0f);
    }

    public float GetChargePercentage() => currentCharge / maxUltCharge;
    public bool IsUltimateReady() => isUltimateReady;
}
