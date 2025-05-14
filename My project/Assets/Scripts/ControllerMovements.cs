using UnityEngine;

public class ControllerMovements : MonoBehaviour {
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

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ultimateAbility = GetComponent<UltimateAbility>();
    }

    void Update() {
        mainAttackTimer -= Time.deltaTime;
        kickTimer -= Time.deltaTime;

        bool isFalling = rb.linearVelocity.y <= -0.01f || (!isGrounded && rb.linearVelocity.y < 0.2f);

        if (isFalling) {
            CancelMove();
        }

        // horizontal movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        rb.freezeRotation = true; // this line prevents tipping over
        rb.linearVelocity = new Vector2(moveHorizontal * moveSpeed, rb.linearVelocity.y); // Set walking animation
        animator.SetBool("isWalking", Mathf.Abs(moveHorizontal) > 0.1f);

        // flip the character
        if (moveHorizontal > 0.1f) {
            // facing left
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else if (moveHorizontal < -0.1f) {
            // facing right
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // jump (dpad up)
        if ((Input.GetAxis("Vertical") > 0.5f || Input.GetKey(KeyCode.Space)) && isGrounded) {
            isGrounded = false;
            animator.SetTrigger("Jump");
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        // crouch (dpad down)
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.JoystickButton2) && isGrounded) {
            animator.SetTrigger("Crouch Down");
            animator.SetBool("isCrouching", true);
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Crouch Down") && state.normalizedTime > 0.95f && state.normalizedTime < 1f)
            {
                animator.speed = 0f;
            }
        } else {
            animator.ResetTrigger("Crouch Up");
            animator.SetBool("isCrouching", false);
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Crouch Down") && state.normalizedTime > 0.95f && state.normalizedTime < 1f) {
                animator.speed = 1f;
            }
        }

        // main attack (X)
        if ((Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.E)) && !isFalling && mainAttackTimer <= 0f) {
            mainAttackTimer = mainAttackCooldown;  // Set cooldown first
            animator.SetBool("isAttacking", true);
            animator.SetTrigger("Main Attack");
            animator.SetBool("isAttacking", false);
        }

        // kick (square)
        if ((Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Q)) && !isFalling && kickTimer <= 0f) {
            kickTimer = kickCooldown;  // Set cooldown first
            animator.SetBool("isKicking", true);
            animator.SetTrigger("Kick");
            animator.SetBool("isKicking", false);
        }

        // ultimate move (R)
        if (ultimateAbility != null && !isFalling && ultimateAbility.IsUltimateReady() && (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton3))) {
            ultimateAbility.UseUltimate();
        }
    }

    // This ensures the animations don't get weird after mid air moves
    public void OnMoveEnd() {
        CancelMove();
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
        // Debug.Log($"Hitbox disabled: {hitboxCollider.gameObject.name}");
        if (hitboxCollider != GetComponent<Collider2D>()) {
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

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
    }
}
