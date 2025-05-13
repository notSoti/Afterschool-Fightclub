using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour {
    [SerializeField] private int maxHealth = 100;
    
    private int currentHealth;
    private bool isDead;

    public UnityEvent<int> onHealthChanged;
    public UnityEvent onDeath;

    private void Start() {
        currentHealth = maxHealth;
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
            SceneManager.LoadScene("Victory Screen");
        } catch (System.Exception e) {
            Debug.LogError($"Failed to load scene: {e.Message}");
        }
    }
}
