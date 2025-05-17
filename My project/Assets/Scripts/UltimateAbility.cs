using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

        // Find the enemy's health component
        string myName = gameObject.name;
        string enemyName;
        
        // Determine the enemy's name based on our name
        if (myName.Contains("_Player")) {
            // If we're the player, look for the AI version
            enemyName = myName.Replace("_Player", "_AI");
        } else {
            // If we're the AI, look for the player version
            enemyName = myName.Replace("_AI", "_Player");
        }

        GameObject enemy = GameObject.Find(enemyName);
        if (enemy != null) {
            enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth == null) {
                Debug.LogError($"No Health component found on {enemyName}");
            }
        } else {
            Debug.LogError($"Could not find {enemyName} object");
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
        Debug.Log($"{gameObject.name}'s Ultimate charge: {Mathf.Floor(currentCharge)}/{maxUltCharge}");
        onChargeChanged?.Invoke(chargePercentage);

        if (currentCharge >= maxUltCharge && !isUltimateReady) {
            isUltimateReady = true;
            onUltimateReady?.Invoke();
            Debug.Log($"{gameObject.name}'s Ultimate is ready!");
        }
    }

    public void UseUltimate() {
        if (!isUltimateReady) return;

        Debug.Log($"{gameObject.name} used their Ultimate!");
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
