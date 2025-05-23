using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnInterval = 13f;  // Fixed 13 second interval
    public float spawnChance = 0.5f;   // 50% chance to spawn
    public float spawnHeight = 5f;
    public float spawnWidth = 6f; // How far to the left and right of center to spawn

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

        // Create the power-up
        GameObject powerUp = Instantiate(powerUpPrefab, spawnPos, Quaternion.identity);

        PowerUpEffect effect = powerUp.GetComponent<PowerUpEffect>();
        SpriteRenderer renderer = powerUp.GetComponent<SpriteRenderer>();

        PowerUpEffect.PowerUpType type = (PowerUpEffect.PowerUpType)Random.Range(0, 6);
        effect.type = type;

        renderer.sprite = type switch
        {
            PowerUpEffect.PowerUpType.Heal => healSprite,
            PowerUpEffect.PowerUpType.Damage => damageSprite,
            PowerUpEffect.PowerUpType.UltCharge => ultChargeSprite,
            PowerUpEffect.PowerUpType.UltDrain => ultDrainSprite,
            PowerUpEffect.PowerUpType.SpeedBoost => speedBoostSprite,
            _ => renderer.sprite
        };
        renderer.color = Color.white;
        powerUp.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
    }
}
