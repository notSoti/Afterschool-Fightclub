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
    public float duration = 3f;  // Duration for temporary effects (speed boost/debuff)
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
                    ApplySpeedEffect(movement, speedMultiplier);
                else if (ai != null)
                    ApplyAISpeedEffect(ai, speedMultiplier);
                break;
        }
    }

    private void ApplySpeedEffect(ControllerMovements movement, float multiplier)
    {
        if (movement == null) return;
        movement.ApplySpeedBoost(multiplier, duration);
    }

    private void ApplyAISpeedEffect(FighterAI ai, float multiplier)
    {
        if (ai == null) return;
        ai.SetSpeedMultiplier(multiplier, duration);
    }
}
