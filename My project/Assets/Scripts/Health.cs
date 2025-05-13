using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour {
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    private bool isDead;

    public UnityEvent<int> onHealthChanged;
    public UnityEvent onDeath;
    public Image healthBar;

    private void Start() {
        currentHealth = maxHealth;
    }

    private void Update() 
    {
        healthBar.fillAmount = Mathf.Clamp((float)GetCurrentHealth() / GetMaxHealth() , 0, 1);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        Debug.Log($"{gameObject.name} took {amount} damage. Health remaining: {currentHealth}/{maxHealth}");
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

        Debug.Log($"{gameObject.name} has died!");

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
