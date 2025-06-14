using UnityEngine;

public class ControllerMovements : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 7f;
    public float mainAttackCooldown = 1f;
    public float kickCooldown = 0.7f;

    private float mainAttackTimer = 0f;
    private float kickTimer = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private UltimateAbility ultimateAbility;
    private Collider2D hitboxCollider;

    private float speedBoostEndTime = -1f;
    private float originalSpeed;

    void Start()
    {
        if (!gameObject.name.Contains("Player"))
        {
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ultimateAbility = GetComponent<UltimateAbility>();

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
    }

    void Update()
    {
        mainAttackTimer -= Time.deltaTime;
        kickTimer -= Time.deltaTime;

        bool isFalling = rb.linearVelocity.y <= -0.01f || (!isGrounded && rb.linearVelocity.y < 0.2f);

        if (isFalling)
        {
            CancelMove();
        }

        // horizontal movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        rb.freezeRotation = true; // this line prevents tipping over
        rb.linearVelocity = new Vector2(moveHorizontal * moveSpeed, rb.linearVelocity.y); // Set walking animation
        animator.SetBool("isWalking", Mathf.Abs(moveHorizontal) > 0.1f);

        // flip the character
        if (moveHorizontal > 0.1f)
        {
            // facing left
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveHorizontal < -0.1f)
        {
            // facing right
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // jump (dpad up)
        if ((Input.GetAxis("Vertical") > 0.5f || Input.GetKey(KeyCode.Space)) && isGrounded)
        {
            isGrounded = false;
            animator.SetTrigger("Jump");
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        // crouch (dpad down)
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.JoystickButton2) && isGrounded)
        {
            animator.SetTrigger("Crouch Down");
            animator.SetBool("isCrouching", true);
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Crouch Down") && state.normalizedTime > 0.95f && state.normalizedTime < 1f)
            {
                animator.speed = 0f;
            }
        }
        else
        {
            animator.ResetTrigger("Crouch Up");
            animator.SetBool("isCrouching", false);
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Crouch Down") && state.normalizedTime > 0.95f && state.normalizedTime < 1f)
            {
                animator.speed = 1f;
            }
        }

        // main attack (X)
        if ((Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.E)) && !isFalling && mainAttackTimer <= 0f)
        {
            mainAttackTimer = mainAttackCooldown;  // Set cooldown first
            animator.SetBool("isAttacking", true);
            animator.SetTrigger("Main Attack");
            animator.SetBool("isAttacking", false);
        }

        // kick (square)
        if ((Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Q)) && !isFalling && kickTimer <= 0f)
        {
            kickTimer = kickCooldown;  // Set cooldown first
            animator.SetBool("isKicking", true);
            animator.SetTrigger("Kick");
            animator.SetBool("isKicking", false);
        }

        // ultimate move (R)
        if (ultimateAbility != null && !isFalling && ultimateAbility.IsUltimateReady() && (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton3)))
        {
            ultimateAbility.UseUltimate();
        }

        // Check if speed boost should expire
        if (speedBoostEndTime > 0 && Time.time >= speedBoostEndTime)
        {
            moveSpeed = originalSpeed;
            speedBoostEndTime = -1f;
        }
    }

    // This ensures the animations don't get weird after mid air moves
    public void OnMoveEnd()
    {
        CancelMove();
    }

    public void EnableHitbox()
    {
        // Debug.Log($"Enabling hitbox for {gameObject.name}");
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
        // Debug.Log($"Disabling hitbox for {gameObject.name}");
        if (hitboxCollider != null && hitboxCollider != GetComponent<Collider2D>())
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        originalSpeed = moveSpeed;
        moveSpeed *= multiplier;
        speedBoostEndTime = Time.time + duration;
    }
}
