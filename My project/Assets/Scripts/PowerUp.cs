using UnityEngine;

[RequireComponent(typeof(PowerUpEffect))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PowerUp : MonoBehaviour
{
    private PowerUpEffect effect;
    private readonly float lifetime = 10f;
    private readonly float flashingThreshold = 3f;
    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private bool isFlashing = false;
    private CircleCollider2D physicsCollider;
    private CircleCollider2D triggerCollider;
    private bool hasLanded = false;
    private Sprite initialSprite;
    private bool hasInitialized = false;

    public void InitializeSprite(Sprite sprite)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null && sprite != null)
        {
            initialSprite = sprite;
            spriteRenderer.sprite = sprite;
            spriteRenderer.enabled = true;
            spriteRenderer.sortingLayerName = "Default";
            spriteRenderer.sortingOrder = 1;
            spriteRenderer.color = Color.white;
            hasInitialized = true;
        }
    }

    void Awake()
    {
        // get the components early to avoid potential race conditions
        spriteRenderer = GetComponent<SpriteRenderer>();
        effect = GetComponent<PowerUpEffect>();
    }

    void Start()
    {
        if (!hasInitialized)
        {
            if (initialSprite != null)
            {
                InitializeSprite(initialSprite);
            }
        }

        SetupPhysics();
    }

    void SetupPhysics()
    {
        physicsCollider = gameObject.AddComponent<CircleCollider2D>();
        physicsCollider.isTrigger = false;
        physicsCollider.radius = 0.25f;

        var bouncyMaterial = new PhysicsMaterial2D("PowerUpBounce")
        {
            bounciness = 0.2f,
            friction = 0.8f
        };
        physicsCollider.sharedMaterial = bouncyMaterial;

        triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = 0.5f;
        triggerCollider.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1.2f;
        rb.mass = 0.5f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.1f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;  // smooth movement
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        gameObject.layer = LayerMask.NameToLayer("PowerUps");
        var powerUpsLayer = gameObject.layer;
        Physics2D.IgnoreLayerCollision(powerUpsLayer, powerUpsLayer);
        Physics2D.IgnoreLayerCollision(powerUpsLayer, LayerMask.NameToLayer("Player"));
    }

    void Update()
    {
        // skip update if not properly initialized
        if (!hasInitialized || spriteRenderer == null) return;

        timer += Time.deltaTime;

        if (timer >= lifetime - flashingThreshold && !isFlashing)
        {
            isFlashing = true;
            InvokeRepeating(nameof(ToggleVisibility), 0f, 0.2f);
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void ToggleVisibility()
    {
        if (spriteRenderer == null || !hasInitialized) return;

        if (spriteRenderer.sprite == null && initialSprite != null)
        {
            // try to recover the sprite
            spriteRenderer.sprite = initialSprite;
            spriteRenderer.enabled = true;
        }
        else if (spriteRenderer.sprite != null)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
        else
        {
            // sprite is missing
            CancelInvoke(nameof(ToggleVisibility));
            isFlashing = false;
            spriteRenderer.enabled = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            triggerCollider.enabled = true;

            // get proper positioning
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 groundNormal = contact.normal;
            Vector2 landingPoint = contact.point;

            // adjust position based on ground normal direction
            float offsetMultiplier = groundNormal.y > 0 ? 1f : 2f;
            float objectRadius = physicsCollider.radius;
            float offset = objectRadius * offsetMultiplier + 0.5f;

            // position the powerup slightly above the ground
            transform.position = new Vector3(transform.position.x, landingPoint.y + offset, transform.position.z);

            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;

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
