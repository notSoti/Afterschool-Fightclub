using UnityEngine;

public class ControllerMovements : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // horizontal movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        rb.freezeRotation = true; // This line prevents tipping over
        rb.linearVelocity = new Vector2(moveHorizontal * moveSpeed, rb.linearVelocity.y);

        // flip the character
        if (moveHorizontal > 0.1f)
        {
            // facing left
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else if (moveHorizontal < -0.1f)
        {
            // facing right
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // jump (dpad up)
        if ((Input.GetAxis("Vertical") > 0.5f || Input.GetKey(KeyCode.Space)) && isGrounded)
        {
            GetComponent<Animator>().SetTrigger("Jump");
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }

        // crouch
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.JoystickButton2))
        {
            GetComponent<Animator>().SetBool("Crouch", true);
        } else {
            GetComponent<Animator>().SetBool("Crouch", false);
        }

        // main attack (X)
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.E))
        {
            GetComponent<Animator>().SetTrigger("Main Attack");
        }

        // kick (square)
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
