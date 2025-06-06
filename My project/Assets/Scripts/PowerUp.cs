using UnityEngine;

[RequireComponent(typeof(PowerUpEffect))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PowerUp : MonoBehaviour
{
    private PowerUpEffect effect;
    private readonly float lifetime = 10f; // How long the power-up exists before disappearing
    private readonly float flashingThreshold = 3f; // When to start flashing before disappearing
    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private bool isFlashing = false;
    private CircleCollider2D physicsCollider;
    private CircleCollider2D triggerCollider;
    private bool hasLanded = false;

    void Start()
    {
        effect = GetComponent<PowerUpEffect>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set up the physical collider for ground collision
        physicsCollider = gameObject.AddComponent<CircleCollider2D>();
        physicsCollider.isTrigger = false;
        physicsCollider.radius = 0.25f;

        // Create a bouncy material for the collider
        var bouncyMaterial = new PhysicsMaterial2D("PowerUpBounce")
        {
            bounciness = 0.2f,
            friction = 0.8f
        };
        physicsCollider.sharedMaterial = bouncyMaterial;

        // Add a trigger collider for player interaction (initially disabled)
        triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = 0.5f;
        triggerCollider.enabled = false; // Start disabled until landing

        // Set up the rigidbody with stable properties
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1.2f;
        rb.mass = 0.5f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.1f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Make power-ups pass through each other and players (until landed)
        gameObject.layer = LayerMask.NameToLayer("PowerUps");
        var powerUpsLayer = gameObject.layer;
        Physics2D.IgnoreLayerCollision(powerUpsLayer, powerUpsLayer); // Only ignore collisions between power-ups
        Physics2D.IgnoreLayerCollision(powerUpsLayer, LayerMask.NameToLayer("Player")); // Ignore players while falling
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Start flashing when near the end of lifetime
        if (timer >= lifetime - flashingThreshold && !isFlashing)
        {
            isFlashing = true;
            InvokeRepeating(nameof(ToggleVisibility), 0f, 0.2f);
        }

        // Destroy when lifetime is up
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void ToggleVisibility()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            triggerCollider.enabled = true;

            // Get the collision point and place the power-up on top of the ground
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 groundNormal = contact.normal;
            Vector2 landingPoint = contact.point;

            // Calculate offset based on whether we hit the top or bottom of the ground
            float offsetMultiplier = groundNormal.y > 0 ? 1f : 2f; // Use larger offset if hitting from below
            float objectRadius = physicsCollider.radius;
            float offset = objectRadius * offsetMultiplier + 0.5f; // Add extra fixed offset

            // Move the power-up above the ground
            transform.position = new Vector3(transform.position.x, landingPoint.y + offset, transform.position.z);

            // Switch to kinematic mode to prevent further physics interactions
            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;

            // Re-enable collisions with players after landing
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasLanded && other.CompareTag("Player"))
        {
            effect.ApplyEffect(other.gameObject);
            Destroy(gameObject);
        }
    }
}
