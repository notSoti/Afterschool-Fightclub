using UnityEngine;

public class FighterAI : MonoBehaviour
{
    public Transform player;
    public bool freeze = true;
    public float moveSpeed = 7f;
    public float jumpForce = 7f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    private float attackTimer = 0f;
    private enum AIState { Idle, Chase, Attack }
    private AIState currentState = AIState.Idle;

    void Update()
    {
        if (freeze) return;
        attackTimer -= Time.deltaTime;
        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case AIState.Idle:
                if (ShouldEscape())
                {
                    Escape();
                }
                if (distance < attackRange * 2) currentState = AIState.Chase;
                break;

            case AIState.Chase:
                if (ShouldEscape())
                {
                    Escape();
                }
                MoveTowardsPlayer();
                if (distance <= attackRange) currentState = AIState.Attack;
                break;

            case AIState.Attack:
                if (ShouldEscape())
                {
                    Escape();
                }
                if (attackTimer <= 0f)
                {
                    float randomValue = Random.value;
                    if (distance <= attackRange * 0.5f)
                    {
                        // Close range; more likely to use main attack
                        if (randomValue < 0.7f)
                        {
                            MainAttack();
                        }
                        else
                        {
                            Kick();
                        }
                    }
                    else if (distance <= attackRange)
                    {
                        // Medium range; more likely to kick
                        if (randomValue < 0.6f)
                        {
                            Kick();
                        }
                        else
                        {
                            MainAttack();
                        }
                    }
                    else
                    {
                        currentState = AIState.Chase;
                        return;
                    }
                    attackTimer = attackCooldown;
                }
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        // Increase speed when close to the player
        float currentSpeed = moveSpeed * (1 + (2f / (distance + 0.5f)));
        transform.position += currentSpeed * Time.deltaTime * (Vector3)direction;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (player.position.y > transform.position.y + 0.5f || distance < 2f)
        {
            if (rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
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

    bool ShouldEscape()
    {
        if (Vector3.Distance(transform.position, lastPosition) > 0.1f)
        {
            lastPosition = transform.position;
            lastMoveTime = Time.time;
            return false;
        }

        return Time.time - lastMoveTime > 3f;
    }

    void Escape()
    {
        Vector2 direction = new(Random.value > 0.5f ? 1 : -1, 0);
        float escapeSpeed = moveSpeed * 1.5f;
        transform.position += escapeSpeed * Time.deltaTime * (Vector3)direction;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            Jump(rb);
        }

        currentState = AIState.Idle;
    }

    void Jump(Rigidbody2D rb)
    {
        GetComponent<Animator>().SetTrigger("Jump");
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Kick()
    {
        GetComponent<Animator>().SetTrigger("Kick");
    }

    void MainAttack()
    {
        GetComponent<Animator>().SetTrigger("Main Attack");
    }
}
