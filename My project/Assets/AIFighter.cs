using UnityEngine;

public class FighterAI : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float moveSpeed = 7f;
    public float jumpForce = 7f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    private float attackTimer = 0f;
    private enum AIState { Idle, Chase, Attack }
    private AIState currentState = AIState.Idle;

    void Update()
    {
        attackTimer -= Time.deltaTime;
        float distance = Vector2.Distance(transform.position, player.position);

        // State logic
        switch (currentState)
        {
            case AIState.Idle:
                if (distance < attackRange * 2) currentState = AIState.Chase;
                break;

            case AIState.Chase:
                MoveTowardsPlayer();
                if (distance <= attackRange) currentState = AIState.Attack;
                break;

            case AIState.Attack:
                if (attackTimer <= 0)
                {
                    Attack();
                    attackTimer = attackCooldown;
                }
                if (distance > attackRange) currentState = AIState.Chase;
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += moveSpeed * Time.deltaTime * (Vector3)direction;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        
        // Jump when player is above the AI
        if (player.position.y > transform.position.y + 1f)
        {
            if (rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.1f) // Only jump if grounded
            {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        // Flip the character based on movement direction
        if (direction.x > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Facing right
        }
        else if (direction.x < -0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Facing left
        }
    }

    void Attack()
    {
        Debug.Log("AI Attacks!");
        // Implement attack logic here (e.g., damage player)
    }
}
