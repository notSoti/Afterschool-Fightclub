using UnityEngine;

[RequireComponent(typeof(PowerUpEffect))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PowerUp : MonoBehaviour
{
    private PowerUpEffect effect;
    private float lifetime = 10f; // How long the power-up exists before disappearing
    private float flashingThreshold = 3f; // When to start flashing before disappearing
    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private bool isFlashing = false;
    private CircleCollider2D physicsCollider;
    private CircleCollider2D triggerCollider;

    void Start()
    {
        effect = GetComponent<PowerUpEffect>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find the Floor object
        GameObject floor = GameObject.FindWithTag("Ground");
        if (floor != null)
        {
            // Get the floor's collider to find its bounds
            Collider2D floorCollider = floor.GetComponent<Collider2D>();
            if (floorCollider != null)
            {
                // Position the power-up just above the floor's top bound
                Vector3 position = transform.position;
                position.y = floorCollider.bounds.max.y + 0.5f; // Add a small offset to ensure we're above the floor
                transform.position = position;
            }
        }
        
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

        // Add a trigger collider for player interaction
        triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = 0.5f;

        // Set up the rigidbody with stable properties
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1.2f;
        rb.mass = 0.5f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.1f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // Make power-ups pass through each other
        gameObject.layer = LayerMask.NameToLayer("Default");
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer);

        // Apply a small random horizontal force when spawned
        float randomForce = Random.Range(-2f, 2f);
        rb.AddForce(new Vector2(randomForce, 0), ForceMode2D.Impulse);
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            effect.ApplyEffect(other.gameObject);
            Destroy(gameObject);
        }
    }
}
