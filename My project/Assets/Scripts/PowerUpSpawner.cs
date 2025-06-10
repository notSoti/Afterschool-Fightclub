using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnInterval = 13f;  // Fixed 13 second interval
    public float spawnChance = 0.5f;   // 50% chance to spawn
    public float spawnHeight = 8f;     // Height above ground
    public float spawnWidth = 6f;      // How far to the left and right of center to spawn

    [Header("Prefab Settings")]
    public GameObject powerUpPrefab;

    [Header("Power-up Icons")]
    public Sprite healSprite;
    public Sprite damageSprite;
    public Sprite ultChargeSprite;
    public Sprite ultDrainSprite;
    public Sprite speedBoostSprite;

    private float nextSpawnTime;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            if (Random.value < spawnChance)
            {
                SpawnPowerUp();
            }
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    void SpawnPowerUp()
    {
        if (powerUpPrefab == null)
        {
            Debug.LogError("Power-up prefab is not assigned in the PowerUpSpawner!");
            return;
        }

        float xPos = Random.Range(-spawnWidth, spawnWidth);
        Vector3 spawnPos = transform.position + new Vector3(xPos, spawnHeight, 0);

        // create the powerup
        GameObject powerUp = Instantiate(powerUpPrefab, spawnPos, Quaternion.identity);
        powerUp.SetActive(false);

        PowerUpEffect effect = powerUp.GetComponent<PowerUpEffect>();

        if (!powerUp.TryGetComponent<SpriteRenderer>(out var renderer))
        {
            Destroy(powerUp);
            return;
        }

        // Set the power-up type and corresponding sprite
        PowerUpEffect.PowerUpType type = (PowerUpEffect.PowerUpType)Random.Range(0, 5);
        effect.type = type;

        // Get the appropriate sprite
        Sprite selectedSprite = type switch
        {
            PowerUpEffect.PowerUpType.Heal => healSprite,
            PowerUpEffect.PowerUpType.Damage => damageSprite,
            PowerUpEffect.PowerUpType.UltCharge => ultChargeSprite,
            PowerUpEffect.PowerUpType.UltDrain => ultDrainSprite,
            PowerUpEffect.PowerUpType.SpeedBoost => speedBoostSprite,
            _ => healSprite // Default to heal sprite
        };

        if (powerUp.TryGetComponent<PowerUp>(out var powerUpComponent))
        {
            powerUpComponent.InitializeSprite(selectedSprite);
        }

        powerUp.transform.localScale = new Vector3(0.20f, 0.20f, 1f);

        powerUp.SetActive(true);

        if (renderer.sprite == null)
        {
            renderer.sprite = selectedSprite;
            renderer.enabled = true;
        }
    }
}
