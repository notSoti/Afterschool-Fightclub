using UnityEngine;

public class FighterAI : MonoBehaviour
{
    public enum Difficulty { Easy, Normal, Hard, Extreme }
    public Difficulty aiDifficulty = Difficulty.Normal;
    public Transform player;
    public bool freeze = true;

    private const float BASE_SPEED = 7f;
    private const float BASE_JUMP_FORCE = 7f;
    private const float BASE_ATTACK_RANGE = 1.5f;
    private const float MAIN_ATTACK_COOLDOWN = 1f;
    private const float KICK_COOLDOWN = 0.7f;

    private enum AIState { Idle, Chase, Attack }

    private float moveSpeed, jumpForce, attackRange, reactionDelay;
    private float mainAttackTimer, kickTimer, lastDecisionTime;
    private AIState currentState;
    private UltimateAbility ultimate;
    private Animator animator;

    private Rigidbody2D rb;

    private bool isGrounded = false;

    private float speedBoostEndTime = -1f;
    private float speedMultiplier = 1f;

    void Start()
    {
        ultimate = GetComponent<UltimateAbility>();
        animator = GetComponent<Animator>();
        currentState = AIState.Idle;
        rb = GetComponent<Rigidbody2D>();

        // Find and set up hitbox for attacks
        Transform hitboxTransform = transform.Find("Player Hitbox");
        if (hitboxTransform != null)
        {
            hitboxCollider = hitboxTransform.GetComponent<Collider2D>();
            if (hitboxCollider == null)
            {
                Debug.LogError($"No Collider2D found on Player Hitbox for {gameObject.name}");
            }
            else
            {
                hitboxCollider.enabled = false; // Make sure hitbox starts disabled
            }
            // Set up the hitbox owner
            if (hitboxTransform.TryGetComponent<Hitbox>(out var hitbox))
            {
                hitbox.owner = gameObject;
            }
        }
        else
        {
            Debug.LogError($"No Player Hitbox found for {gameObject.name}");
        }

        // Set up the hurtbox owner
        Transform hurtboxTransform = transform.Find("Player Hurtbox");
        if (hurtboxTransform != null && hurtboxTransform.TryGetComponent<Hurtbox>(out var hurtbox))
        {
            hurtbox.owner = gameObject;
        }

        // Set up difficulty-based parameters
        switch (aiDifficulty)
        {
            case Difficulty.Easy:
                moveSpeed = BASE_SPEED * 0.8f;
                jumpForce = BASE_JUMP_FORCE * 0.9f;
                attackRange = BASE_ATTACK_RANGE * 0.8f;
                reactionDelay = 0.5f;
                break;
            case Difficulty.Normal:
                moveSpeed = BASE_SPEED;
                jumpForce = BASE_JUMP_FORCE;
                attackRange = BASE_ATTACK_RANGE;
                reactionDelay = 0.2f;
                break;
            case Difficulty.Hard:
                moveSpeed = BASE_SPEED * 1.2f;
                jumpForce = BASE_JUMP_FORCE * 1.1f;
                attackRange = BASE_ATTACK_RANGE * 1.2f;
                reactionDelay = 0f;
                break;
            case Difficulty.Extreme:
                moveSpeed = BASE_SPEED * 1.2f;
                jumpForce = BASE_JUMP_FORCE * 1.1f;
                attackRange = BASE_ATTACK_RANGE * 1.3f;
                reactionDelay = 0f;
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void Update()
    {
        if (freeze) return;
        mainAttackTimer -= Time.deltaTime;
        kickTimer -= Time.deltaTime;

        // Check if speed boost should expire
        if (speedBoostEndTime > 0 && Time.time >= speedBoostEndTime)
        {
            speedMultiplier = 1f;
            speedBoostEndTime = -1f;
        }

        // Add reaction delay based on difficulty
        if (Time.time - lastDecisionTime < reactionDelay) return;
        lastDecisionTime = Time.time;

        float distance = Vector2.Distance(transform.position, player.position);

        // Try to use ultimate more aggressively
        if (ultimate != null && ultimate.IsUltimateReady())
        {
            float ultimateRange = aiDifficulty == Difficulty.Extreme ? attackRange * 1.5f :
                                aiDifficulty == Difficulty.Hard ? attackRange * 1.3f :
                                aiDifficulty == Difficulty.Normal ? attackRange * 1.1f :
                                attackRange * 0.9f;

            if (distance <= ultimateRange)
            {
                UseUltimate();
            }
        }

        // Check if we should try to escape before anything else
        if (ShouldEscape())
        {
            Escape();
            return;
        }

        // More aggressive state transitions
        switch (currentState)
        {
            case AIState.Idle:
                animator.SetBool("isWalking", false);
                // More eager to chase
                if (distance < attackRange * 3f)
                {
                    currentState = AIState.Chase;
                }
                break;

            case AIState.Chase:
                // More eager to attack
                if (distance <= attackRange * 1.4f)
                {  // Increased attack range
                    currentState = AIState.Attack;
                }
                MoveTowardsPlayer();
                // Try to attack even while chasing if very close
                if (distance <= attackRange && (mainAttackTimer <= 0f || kickTimer <= 0f))
                {
                    Attack();
                }
                break;

            case AIState.Attack:
                // Stay in attack mode longer
                if (distance > attackRange * 1.8f)
                {  // More lenient distance for staying in attack
                    currentState = AIState.Chase;
                }
                else
                {
                    Attack();
                    // Move while attacking to maintain position
                    if (distance > attackRange * 0.5f || distance < attackRange * 0.3f)
                    {
                        MoveTowardsPlayer();
                    }
                }
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        // Calculate ideal distance and speed multiplier based on state and difficulty
        float idealDistance = currentState == AIState.Attack ? attackRange * 0.6f :
            attackRange * (1.1f - (int)aiDifficulty * 0.1f);

        float currentSpeed = moveSpeed * speedMultiplier;

        // Boost speed when far from ideal position
        float distanceFromIdeal = Mathf.Abs(distance - idealDistance);
        if (distanceFromIdeal > attackRange * 0.3f)
        {
            currentSpeed *= 1.5f;
        }

        // Add quick bursts of speed for higher difficulties
        if (aiDifficulty >= Difficulty.Hard && Random.value < 0.2f)
        {
            currentSpeed *= 1.3f;
        }

        // More direct movement with less variation
        Vector2 moveDirection = direction;
        if (currentState == AIState.Attack && distanceFromIdeal < attackRange * 0.2f)
        {
            // Only add sideways movement when very close during attack
            float sidewaysAmount = Mathf.Sin(Time.time * 3f) * 0.2f;
            moveDirection = new Vector2(direction.x + sidewaysAmount, direction.y).normalized;
        }

        // Apply velocity with less smoothing for more responsive movement
        Vector2 targetVelocity = new(moveDirection.x * currentSpeed, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, Time.deltaTime * 20f);

        animator.SetBool("isWalking", true);
        rb.freezeRotation = true;

        // More aggressive jumping
        if ((player.position.y > transform.position.y + 0.2f && distance < attackRange * 2.5f) || // Jump when player is slightly above
            (distance < attackRange * 1.5f && Random.value < 0.1f) || // More frequent combat jumps
            (Mathf.Abs(player.position.x - transform.position.x) < 0.8f && Random.value < 0.15f))
        { // More aggressive dodge jumps
            if (isGrounded)
            {
                Jump(rb);
            }
        }

        // Face the player
        float relativeX = player.position.x - transform.position.x;
        transform.localScale = new Vector3(
            relativeX > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private Vector3 lastPosition;
    private float lastMoveTime;

    bool ShouldEscape()
    {
        if (Vector3.Distance(transform.position, lastPosition) > 0.1f)
        {
            lastPosition = transform.position;
            lastMoveTime = Time.time;
            return false;
        }

        // Calculate escape time based on difficulty (4f - difficulty * 0.5f)
        float escapeTime = 4f - ((int)aiDifficulty * 0.5f);
        return Time.time - lastMoveTime > escapeTime;
    }

    void Escape()
    {
        // Smarter escape direction for higher difficulties
        Vector2 direction = aiDifficulty >= Difficulty.Hard ?
            (transform.position - player.position).normalized :
            new Vector2(Random.value > 0.5f ? 1 : -1, 0);

        // Escape speed increases with difficulty
        float escapeSpeed = moveSpeed * (1.2f + ((int)aiDifficulty * 0.2f));
        rb.linearVelocity = new Vector2(direction.x * escapeSpeed, rb.linearVelocity.y);

        if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            Jump(rb);
        }

        currentState = AIState.Idle;
    }

    void Attack()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        float randomValue = Random.value;

        // Calculate attack probabilities based on difficulty and distance
        float difficultyMultiplier = 0.6f + ((int)aiDifficulty * 0.1f);
        float proximityBonus = Mathf.Clamp(1f - (distance / attackRange), 0f, 0.5f);

        float mainAttackProb = difficultyMultiplier + proximityBonus;
        float kickProb = difficultyMultiplier - 0.05f + proximityBonus;

        // Combo chance based on difficulty
        bool shouldTryCombo = Random.value < (0.15f + ((int)aiDifficulty * 0.15f));

        // Attack decision making
        if (distance <= attackRange * 1.4f)
        {  // Increased attack range
            bool canMainAttack = mainAttackTimer <= 0f;
            bool canKick = kickTimer <= 0f;

            if (canMainAttack && canKick)
            {
                // Try both attacks more frequently
                if (randomValue < mainAttackProb)
                {
                    MainAttack();
                    if (shouldTryCombo)
                    {
                        Invoke(nameof(Kick), 0.2f);
                    }
                }
                if (randomValue < kickProb)
                {  // Independent chance for kick
                    Kick();
                    if (shouldTryCombo)
                    {
                        Invoke(nameof(MainAttack), 0.2f);
                    }
                }
            }
            else if (canMainAttack)
            {
                if (randomValue < mainAttackProb)
                {
                    MainAttack();
                }
            }
            else if (canKick)
            {
                if (randomValue < kickProb)
                {
                    Kick();
                }
            }
        }
        else
        {
            currentState = AIState.Chase;
        }
    }

    void Jump(Rigidbody2D rb)
    {
        GetComponent<Animator>().SetTrigger("Jump");
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Kick()
    {
        kickTimer = KICK_COOLDOWN;
        animator.SetBool("isKicking", true);
        animator.SetTrigger("Kick");
        animator.SetBool("isKicking", false);
    }

    void MainAttack()
    {
        mainAttackTimer = MAIN_ATTACK_COOLDOWN;
        animator.SetBool("isAttacking", true);
        animator.SetTrigger("Main Attack");
        animator.SetBool("isAttacking", false);
    }
    void UseUltimate()
    {
        if (ultimate == null || !ultimate.IsUltimateReady()) return;
        ultimate.UseUltimate();
    }

    public Collider2D hitboxCollider;

    public void EnableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;

            if (hitboxCollider.TryGetComponent<Hitbox>(out var hitbox))
            {
                hitbox.ResetHits();
            }
        }
    }


    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
    }

    void CancelMove()
    {
        animator.ResetTrigger("Kick");
        animator.ResetTrigger("Main Attack");
        animator.ResetTrigger("Jump");
        animator.SetBool("isKicking", false);
        animator.SetBool("isAttacking", false);
    }

    public void OnMoveEnd()
    {
        CancelMove();
    }

    public void SetSpeedMultiplier(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        speedBoostEndTime = Time.time + duration;
    }
}
