using UnityEngine;
using UnityEngine.Events;

public class UltimateAbility : MonoBehaviour
{    [SerializeField] private float maxUltCharge = 100f;

    private float currentCharge;
    private bool isUltimateReady;

    public UnityEvent<float> onChargeChanged; // Sends charge percentage (0-1)
    public UnityEvent onUltimateReady;
    public UnityEvent onUltimateUsed;

    private void Start() {
        currentCharge = 0f;
        isUltimateReady = false;
    }
    public void AddCharge(float damageDealt) {
        if (isUltimateReady) return;

        currentCharge = Mathf.Min(maxUltCharge, currentCharge + (damageDealt * 2.5f));

        float chargePercentage = currentCharge / maxUltCharge;
        Debug.Log($"{gameObject.name}'s Ultimate charge: {Mathf.Floor(currentCharge)}/{maxUltCharge}");
        onChargeChanged?.Invoke(chargePercentage);

        if (currentCharge >= maxUltCharge && !isUltimateReady) {
            isUltimateReady = true;
            onUltimateReady?.Invoke();
            Debug.Log($"{gameObject.name}'s Ultimate is ready!");
        }
    }

    public void UseUltimate() {
        if (!isUltimateReady) return;

        Debug.Log($"{gameObject.name} used their Ultimate!");
        onUltimateUsed?.Invoke();
        
        // Reset charge
        currentCharge = 0f;
        isUltimateReady = false;
        onChargeChanged?.Invoke(0f);

        // TODO: Iadd ult logic here
    }

    public float GetChargePercentage() => currentCharge / maxUltCharge;
    public bool IsUltimateReady() => isUltimateReady;
}
