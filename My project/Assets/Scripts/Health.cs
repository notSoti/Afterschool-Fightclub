using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] public AudioManager audioManager;

    private bool isDead;
    private bool isPlayer;
    private Image healthBar;
    public GameObject endGamePanel;
    private static string winner;

    // Cache the animation state hashes
    private static readonly int IdleState = Animator.StringToHash("idle");
    private static readonly int DeathState = Animator.StringToHash("Death");

    // Cache the animation parameter hashes
    private static readonly int JumpTrigger = Animator.StringToHash("Jump");
    private static readonly int MainAttackTrigger = Animator.StringToHash("Main Attack");
    private static readonly int KickTrigger = Animator.StringToHash("Kick");
    private static readonly int IsCrouchingParam = Animator.StringToHash("isCrouching");
    private static readonly int IsKickingParam = Animator.StringToHash("isKicking");
    private static readonly int IsAttackingParam = Animator.StringToHash("isAttacking");

    public UnityEvent<int> onHealthChanged;
    public UnityEvent onDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        isPlayer = name.Contains("Player");


        // Find the appropriate health bar based on whether this is a player or AI
        string barName = isPlayer ? "Player1HealthBar" : "Player2HealthBar";
        GameObject healthBarObj = GameObject.Find(barName);
        if (healthBarObj != null)
        {
            healthBar = healthBarObj.GetComponent<Image>();
            if (healthBar == null)
            {
                // Try to find it as a child component
                healthBar = healthBarObj.GetComponentInChildren<Image>();
            }
        }

        if (healthBar == null)
        {
            Debug.LogWarning($"Could not find health bar '{barName}' for {gameObject.name}");
        }
    }

    private void Update()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = Mathf.Clamp(GetHealthPercentage(), 0, 1);
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        audioManager.PlaySFX(audioManager.hurt);
        // Debug.Log($"{gameObject.name} took {amount} damage. Health remaining: {currentHealth}/{maxHealth}");
        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            audioManager.PlaySFX(audioManager.death);
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        onDeath?.Invoke();

        // Find all characters in the scene by looking for character components
        var allPlayers = FindObjectsByType<ControllerMovements>(FindObjectsSortMode.None)
                            .Select(p => p.gameObject);
        var allAIs = FindObjectsByType<FighterAI>(FindObjectsSortMode.None)
                            .Select(ai => ai.gameObject);
        var allCharacters = allPlayers.Concat(allAIs).Distinct();

        // Find who died and who survived
        GameObject deadCharacter = this.gameObject;
        GameObject survivor = null;

        foreach (var character in allCharacters)
        {
            if (character != null && character.activeSelf && character != deadCharacter)
            {
                survivor = character;
                break;
            }
        }

        if (survivor != null)
        {
            winner = survivor.name.Contains("Player") ? "Player 1" : "Player 2";

            // First stop all physics movement
            if (survivor.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic; // Ignore physics
            }

            // Then disable movement components
            if (survivor.TryGetComponent<ControllerMovements>(out var playerMovement))
            {
                playerMovement.enabled = false;
            }
            if (survivor.TryGetComponent<FighterAI>(out var aiComponent))
            {
                aiComponent.enabled = false;
                aiComponent.freeze = true;
            }

            // Then handle animation
            if (survivor.TryGetComponent<Animator>(out var survivorAnimator))
            {
                // First reset all parameters
                survivorAnimator.ResetTrigger(JumpTrigger);
                survivorAnimator.ResetTrigger(MainAttackTrigger);
                survivorAnimator.ResetTrigger(KickTrigger);
                survivorAnimator.SetBool(IsCrouchingParam, false);
                survivorAnimator.SetBool(IsKickingParam, false);
                survivorAnimator.SetBool(IsAttackingParam, false);

                // Force into idle state
                survivorAnimator.Rebind();
                survivorAnimator.Update(0f);
                survivorAnimator.Play(IdleState, 0, 0f);
                survivorAnimator.Update(0f);
            }
        }

        // Handle the dead character last
        if (TryGetComponent<Rigidbody2D>(out var deadRb))
        {
            deadRb.linearVelocity = Vector2.zero;
            deadRb.angularVelocity = 0f;
            deadRb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (TryGetComponent<ControllerMovements>(out var deadMovement))
        {
            deadMovement.enabled = false;
        }
        if (TryGetComponent<FighterAI>(out var deadAI))
        {
            deadAI.enabled = false;
            deadAI.freeze = true;
        }
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.ResetTrigger(JumpTrigger);
            animator.ResetTrigger(MainAttackTrigger);
            animator.ResetTrigger(KickTrigger);
            animator.SetBool(IsCrouchingParam, false);
            animator.SetBool(IsKickingParam, false);
            animator.SetBool(IsAttackingParam, false);

            animator.Rebind(); // This fully resets the animator
            animator.Update(0f);
            animator.Play(DeathState, 0, 0f);
            animator.Update(0f); // Update again after playing
        }

        // Start the delayed scene transition
        StartCoroutine(LoadVictoryScreen());
    }

    private IEnumerator LoadVictoryScreen()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        audioManager.PlaySFX(audioManager.victorysound);
        endGamePanel.SetActive(true);
    }

    public bool IsDead() => isDead;
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    public static string GetWinner() => winner;
}
