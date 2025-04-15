using UnityEngine;

public class ControllerMovements : MonoBehaviour {
    public float moveSpeed = 7f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        bool isFalling = rb.linearVelocity.y <= -0.01f || (!isGrounded && rb.linearVelocity.y < 0.2f);

        if (isFalling) {
            CancelMove();
        }

        // horizontal movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        rb.freezeRotation = true; // this line prevents tipping over
        rb.linearVelocity = new Vector2(moveHorizontal * moveSpeed, rb.linearVelocity.y);

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
            animator.SetBool("Crouch", true);
        } else {
            animator.SetBool("Crouch", false);
        }

        // main attack (X)
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.E) && !isFalling) {
            animator.SetBool("isAttacking", true);
            animator.SetTrigger("Main Attack");
            animator.SetBool("isAttacking", false);
        }

        // kick (square)
        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Q) && !isFalling) {
            animator.SetBool("isKicking", true);
            animator.SetTrigger("Kick");
            animator.SetBool("isKicking", false);
        }
    }

    void CancelMove() {
        animator.ResetTrigger("Kick");
        animator.ResetTrigger("Main Attack");
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
    }
}
