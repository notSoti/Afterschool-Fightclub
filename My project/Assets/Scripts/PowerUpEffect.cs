using UnityEngine;

public class PowerUpEffect : MonoBehaviour
{
    public enum PowerUpType
    {
        Heal,           // +30 HP
        UltCharge,      // +50% Ult charge
        SpeedBoost,     // Temporary speed increase
        Damage,         // -30 HP
        UltDrain,       // -50% Ult charge
        SpeedDebuff     // Temporary speed decrease
    }

    public PowerUpType type;
    public float duration = 5f;  // Duration for temporary effects (speed boost/debuff)
    public float speedMultiplier = 1.5f;  // For speed effects

    public void ApplyEffect(GameObject target)
    {
        Health health = target.GetComponent<Health>();
        UltimateAbility ultimate = target.GetComponent<UltimateAbility>();
        ControllerMovements movement = target.GetComponent<ControllerMovements>();
        FighterAI ai = target.GetComponent<FighterAI>();

        switch (type)
        {
            case PowerUpType.Heal:
                if (health != null)
                    health.Heal(30);
                break;

            case PowerUpType.UltCharge:
                if (ultimate != null)
                    ultimate.AddCharge(50);
                break;

            case PowerUpType.SpeedBoost:
                if (movement != null)
                    StartCoroutine(ApplySpeedEffect(movement, speedMultiplier));
                else if (ai != null)
                    StartCoroutine(ApplyAISpeedEffect(ai, speedMultiplier));
                break;

            case PowerUpType.Damage:
                if (health != null)
                    health.TakeDamage(30);
                break;

            case PowerUpType.UltDrain:
                if (ultimate != null)
                    ultimate.AddCharge(-50);
                break;

            case PowerUpType.SpeedDebuff:
                break;
                if (movement != null)
                    StartCoroutine(ApplySpeedEffect(movement, 0.5f));
                else if (ai != null)
                    StartCoroutine(ApplyAISpeedEffect(ai, 0.5f));
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
