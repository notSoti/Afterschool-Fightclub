using UnityEngine;

public class ControllerMovements : MonoBehaviour
{
    public float moveSpeed = 7f; // Speed of the character
    public float jumpForce = 7f; // Jump force of the character

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Horizontal movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        rb.freezeRotation = true; // This line prevents tipping over
        rb.linearVelocity = new Vector2(moveHorizontal * moveSpeed, rb.linearVelocity.y);

        // Flip the character based on movement direction
        if (moveHorizontal > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Facing right
        }
        else if (moveHorizontal < -0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Facing left
        }

        // Jump with dpad up
        if ((Input.GetAxis("Vertical") > 0.5f || Input.GetKey(KeyCode.Space)) && isGrounded)
        {
            GetComponent<Animator>().SetTrigger("Jump");
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }

        // Main attack (X)
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.E))
        {
            GetComponent<Animator>().SetTrigger("Main Attack");
        }

        // Kick with square
        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Q))
        {
            GetComponent<Animator>().SetTrigger("Kick");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
