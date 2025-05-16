using UnityEngine;

public class FighterAI : MonoBehaviour
{
    public enum Difficulty { Easy, Normal, Hard, Extreme }
    public Difficulty aiDifficulty = Difficulty.Normal;
    
    public Transform player;
    public bool freeze = true;
    public float baseSpeed = 7f;
    public float baseJumpForce = 7f;
    public float baseAttackRange = 1.5f;
    
    private float moveSpeed;
    private float jumpForce;
    private float attackRange;
    private UltimateAbility ultimate;
    private Animator animator;
    public float mainAttackCooldown = 1f;
    public float kickCooldown = 0.7f;
    private float reactionDelay = 0f;
    private float lastDecisionTime = 0f;

    private float mainAttackTimer = 0f;
    private float kickTimer = 0f;
    private enum AIState { Idle, Chase, Attack }
    private AIState currentState = AIState.Idle;

    private Rigidbody2D rb;

    void Start() {
        ultimate = GetComponent<UltimateAbility>();
        animator = GetComponent<Animator>();
        currentState = AIState.Idle;
        rb = GetComponent<Rigidbody2D>();

        // Find and set up hitbox for attacks
        Transform hitboxTransform = transform.Find("Player Hitbox");
        if (hitboxTransform != null) {
            hitboxCollider = hitboxTransform.GetComponent<Collider2D>();
            if (hitboxCollider == null) {
                Debug.LogError($"No Collider2D found on Player Hitbox for {gameObject.name}");
            } else {
                hitboxCollider.enabled = false; // Make sure hitbox starts disabled
            }
            // Set up the hitbox owner
            if (hitboxTransform.TryGetComponent<Hitbox>(out var hitbox)) {
                hitbox.owner = gameObject;
            }
        } else {
            Debug.LogError($"No Player Hitbox found for {gameObject.name}");
        }

        // Set up the hurtbox owner
        Transform hurtboxTransform = transform.Find("Player Hurtbox");
        if (hurtboxTransform != null && hurtboxTransform.TryGetComponent<Hurtbox>(out var hurtbox)) {
            hurtbox.owner = gameObject;
        }
        
        // Set up difficulty-based parameters
        switch (aiDifficulty) {
            case Difficulty.Easy:
                moveSpeed = baseSpeed * 0.8f;
                jumpForce = baseJumpForce * 0.9f;
                attackRange = baseAttackRange * 0.8f;
                reactionDelay = 0.5f;
                break;
            case Difficulty.Normal:
                moveSpeed = baseSpeed;
                jumpForce = baseJumpForce;
                attackRange = baseAttackRange;
                reactionDelay = 0.2f;
                break;
            case Difficulty.Hard:
                moveSpeed = baseSpeed * 1.2f;
                jumpForce = baseJumpForce * 1.1f;
                attackRange = baseAttackRange * 1.2f;
                reactionDelay = 0f;
                break;
            case Difficulty.Extreme:
                moveSpeed = baseSpeed * 1.2f;
                jumpForce = baseJumpForce * 1.1f;
                attackRange = baseAttackRange * 1.3f;
                reactionDelay = 0f;
                break;
        }
    }

    void Update() {
        if (freeze) return;
        mainAttackTimer -= Time.deltaTime;
        kickTimer -= Time.deltaTime;

        // Add reaction delay based on difficulty
        if (Time.time - lastDecisionTime < reactionDelay) return;
        lastDecisionTime = Time.time;

        float distance = Vector2.Distance(transform.position, player.position);

        // Try to use ultimate when close to the player - harder difficulties are more strategic
        if (ultimate != null && ultimate.IsUltimateReady()) {
            float ultimateRange = aiDifficulty == Difficulty.Extreme ? attackRange * 1.0f :
                                aiDifficulty == Difficulty.Hard ? attackRange * 0.9f : 
                                aiDifficulty == Difficulty.Normal ? attackRange * 0.7f : 
                                attackRange * 0.5f;
            if (distance <= ultimateRange) {
                UseUltimate();
            }
        }
        
        animator.SetBool("isWalking", false);

        switch (currentState) {
            case AIState.Idle:
                if (ShouldEscape()) {
                    Escape();
                }
                if (distance < attackRange * 2) currentState = AIState.Chase;
                break;

            case AIState.Chase:
                if (ShouldEscape()) {
                    Escape();
                }
                MoveTowardsPlayer();
                if (distance <= attackRange) currentState = AIState.Attack;
                break;

            case AIState.Attack:
                if (ShouldEscape()) {
                    Escape();
                }
                Attack();
                break;
        }
    }

    void MoveTowardsPlayer() {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);
        
        // Harder difficulties have better positioning
        float idealDistance = aiDifficulty == Difficulty.Extreme ? attackRange * 0.9f :
                            aiDifficulty == Difficulty.Hard ? attackRange * 0.8f :
                            aiDifficulty == Difficulty.Normal ? attackRange * 0.6f :
                            attackRange * 0.4f;
        
        // Adjust speed based on positioning strategy
        float speedMultiplier = 1f;
        if (aiDifficulty == Difficulty.Extreme || aiDifficulty == Difficulty.Hard) {
            // Extreme/Hard: More aggressive approach and better distance management
            speedMultiplier = distance > idealDistance ? 1.3f : (distance < idealDistance * 0.5f ? 0.7f : 1f);
            
            // Extreme gets small random speed bursts
            if (aiDifficulty == Difficulty.Extreme && Random.value < 0.1f) {
                speedMultiplier *= 1.5f;
            }
        }
        
        // Simplified speed calculation
        float currentSpeed = moveSpeed * speedMultiplier;
        
        // Apply velocity directly to rigidbody
        rb.linearVelocity = new Vector2(direction.x * currentSpeed, rb.linearVelocity.y);

        animator.SetBool("isWalking", true);
        rb.freezeRotation = true;

        if (player.position.y > transform.position.y + 0.5f || distance < 2f) {
            if (Mathf.Abs(rb.linearVelocity.y) < 0.1f) {
                Jump(rb);
            }
        }

        // Always face the player
        float relativeX = player.position.x - transform.position.x;
        transform.localScale = new Vector3(
            relativeX > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );

        currentState = AIState.Chase;
    }

    private Vector3 lastPosition;
    private float lastMoveTime;

    bool ShouldEscape() {
        if (Vector3.Distance(transform.position, lastPosition) > 0.1f) {
            lastPosition = transform.position;
            lastMoveTime = Time.time;
            return false;
        }

        // Harder difficulties escape faster when stuck
        float escapeTime = aiDifficulty == Difficulty.Extreme ? 1.5f :
                          aiDifficulty == Difficulty.Hard ? 2f :
                          aiDifficulty == Difficulty.Normal ? 3f : 4f;
        return Time.time - lastMoveTime > escapeTime;
    }

    void Escape() {
        Vector2 direction = new(Random.value > 0.5f ? 1 : -1, 0);
        // Hard difficulty makes smarter escape decisions
        if (aiDifficulty == Difficulty.Hard) {
            direction = (transform.position - player.position).normalized;
        }
        
        float escapeSpeed = moveSpeed * (
            aiDifficulty == Difficulty.Hard ? 1.8f :
            aiDifficulty == Difficulty.Normal ? 1.5f : 1.2f
        );
                                       
        // Apply escape velocity to rigidbody
        rb.linearVelocity = new Vector2(direction.x * escapeSpeed, rb.linearVelocity.y);

        if (Mathf.Abs(rb.linearVelocity.y) < 0.1f) {
            Jump(rb);
        }

        currentState = AIState.Idle;
    }

    void Attack() {
        if (mainAttackTimer <= 0f || kickTimer <= 0f) {
            float randomValue = Random.value;
            float distance = Vector2.Distance(transform.position, player.position);

            // Adjust attack probabilities based on difficulty
            float mainAttackCloseProb = aiDifficulty == Difficulty.Extreme ? 0.85f :
                                       aiDifficulty == Difficulty.Hard ? 0.8f :
                                       aiDifficulty == Difficulty.Normal ? 0.7f : 0.6f;
            float kickMediumProb = aiDifficulty == Difficulty.Extreme ? 0.8f :
                                  aiDifficulty == Difficulty.Hard ? 0.7f :
                                  aiDifficulty == Difficulty.Normal ? 0.6f : 0.5f;

            // Extreme difficulty: Chance to do a quick combo
            bool canCombo = aiDifficulty == Difficulty.Extreme && Random.value < 0.3f;

            if (distance <= attackRange * 0.5f) {
                // Close range
                if (randomValue < mainAttackCloseProb && mainAttackTimer <= 0f) {
                    MainAttack();
                    if (canCombo && kickTimer <= 0f) {
                        Invoke(nameof(Kick), 0.2f); // Quick follow-up kick
                    }
                } else if (kickTimer <= 0f) {
                    Kick();
                    if (canCombo && mainAttackTimer <= 0f) {
                        Invoke(nameof(MainAttack), 0.2f); // Quick follow-up attack
                    }
                }
            } else if (distance <= attackRange) {
                // Medium range
                if (randomValue < kickMediumProb && kickTimer <= 0f) {
                    Kick();
                } else if (mainAttackTimer <= 0f) {
                    MainAttack();
                }
            } else {
                currentState = AIState.Chase;
                return;
            }
        }
    }

    void Jump(Rigidbody2D rb) {
        GetComponent<Animator>().SetTrigger("Jump");
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Kick() {
        kickTimer = kickCooldown;
        animator.SetBool("isKicking", true);
        animator.SetTrigger("Kick");
        animator.SetBool("isKicking", false);
    }

    void MainAttack() {
        mainAttackTimer = mainAttackCooldown;
        animator.SetBool("isAttacking", true);
        animator.SetTrigger("Main Attack");
        animator.SetBool("isAttacking", false);
    }
    void UseUltimate() {
        if (ultimate == null || !ultimate.IsUltimateReady()) return;
        ultimate.UseUltimate();
    }

    public Collider2D hitboxCollider;

    public void EnableHitbox() {
        if (hitboxCollider != null) {
            hitboxCollider.enabled = true;

            if (hitboxCollider.TryGetComponent<Hitbox>(out var hitbox)) {
                hitbox.ResetHits();
            }
        }
    }


    public void DisableHitbox() {
        if (hitboxCollider != null)
        {
            // Debug.Log($"Disabling: {hitboxCollider.gameObject.name}");
            hitboxCollider.enabled = false;
        }
    }

    void CancelMove() {
        animator.ResetTrigger("Kick");
        animator.ResetTrigger("Main Attack");
        animator.ResetTrigger("Jump");
        animator.SetBool("isKicking", false);
        animator.SetBool("isAttacking", false);
    }

    public void OnMoveEnd() {
        CancelMove();
    }
}
