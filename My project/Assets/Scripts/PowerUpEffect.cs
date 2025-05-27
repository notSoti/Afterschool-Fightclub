using UnityEngine;

public class PowerUpEffect : MonoBehaviour
{
    public enum PowerUpType
    {
        Heal,           // +20 HP
        Damage,         // -20 HP
        UltCharge,      // +20% Ult charge
        UltDrain,       // -20% Ult charge
        SpeedBoost,     // Temporary speed increase
    }

    public PowerUpType type;
    public float duration = 5f;  // Duration for temporary effects (speed boost/debuff)
    public float speedMultiplier = 1.5f;  // For speed effects
    [SerializeField] public AudioManager audioManager;

    public void ApplyEffect(GameObject target)
    {
        // audioManager.PlaySFX(audioManager.powerupring);
        Health health = target.GetComponent<Health>();
        UltimateAbility ultimate = target.GetComponent<UltimateAbility>();
        ControllerMovements movement = target.GetComponent<ControllerMovements>();
        FighterAI ai = target.GetComponent<FighterAI>();

        switch (type)
        {
            case PowerUpType.Heal:
                if (health != null)
                    health.Heal(20);
                break;

            case PowerUpType.Damage:
                if (health != null)
                    health.TakeDamage(20);
                break;

            case PowerUpType.UltCharge:
                if (ultimate != null)
                    ultimate.AddCharge(20);
                break;

            case PowerUpType.UltDrain:
                if (ultimate != null)
                    ultimate.AddCharge(-20);
                break;

            case PowerUpType.SpeedBoost:
                if (movement != null)
                    StartCoroutine(ApplySpeedEffect(movement, speedMultiplier));
                else if (ai != null)
                    StartCoroutine(ApplyAISpeedEffect(ai, speedMultiplier));
                break;
        }
    }

    private System.Collections.IEnumerator ApplySpeedEffect(ControllerMovements movement, float multiplier)
    {
        float originalSpeed = movement.moveSpeed;
        movement.moveSpeed *= multiplier;
        yield return new WaitForSeconds(duration);
        movement.moveSpeed = originalSpeed;
    }

    private System.Collections.IEnumerator ApplyAISpeedEffect(FighterAI ai, float multiplier)
    {
        // The AI uses BASE_SPEED internally, so we'll modify its current speed setting
        ai.SetSpeedMultiplier(multiplier);
        yield return new WaitForSeconds(duration);
        ai.SetSpeedMultiplier(1f);
    }
}
