using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour {
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    private bool isDead;
    private bool isPlayer;
    private Image healthBar;

    public UnityEvent<int> onHealthChanged;
    public UnityEvent onDeath;

    private void Start() {
        currentHealth = maxHealth;
        isPlayer = name.Contains("Player");
        
        // Find the appropriate health bar based on whether this is a player or AI
        string barName = isPlayer ? "Player1HealthBar" : "Player2HealthBar";
        GameObject healthBarObj = GameObject.Find(barName);
        if (healthBarObj != null) {
            healthBar = healthBarObj.GetComponent<Image>();
            if (healthBar == null) {
                // Try to find it as a child component
                healthBar = healthBarObj.GetComponentInChildren<Image>();
            }
        }
        
        if (healthBar == null) {
            Debug.LogWarning($"Could not find health bar '{barName}' for {gameObject.name}");
        }
    }

    private void Update() 
    {
        if (healthBar != null) {
            healthBar.fillAmount = Mathf.Clamp(GetHealthPercentage(), 0, 1);
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        // Debug.Log($"{gameObject.name} took {amount} damage. Health remaining: {currentHealth}/{maxHealth}");
        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Heal(int amount) {
        if (amount <= 0 || isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth);
    }

    private void Die() {
        if (isDead) return;
        
        isDead = true;
        onDeath?.Invoke();

        // Debug.Log($"{gameObject.name} has died!");

        try {
            SceneManager.LoadScene("VictoryScreen");
        } catch (System.Exception e) {
            Debug.LogError($"Failed to load scene: {e.Message}");
        }
    }

    public bool IsDead() => isDead;
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}
