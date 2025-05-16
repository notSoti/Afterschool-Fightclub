using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UltimateAbility : MonoBehaviour
{    
    [SerializeField] private float maxUltCharge = 100f;

    private float currentCharge;
    private bool isUltimateReady;
    private bool isPlayer;
    private Image ultBar;

    public UnityEvent<float> onChargeChanged; // Sends charge percentage (0-1)
    public UnityEvent onUltimateReady;
    public UnityEvent onUltimateUsed;

    private void Start() {
        currentCharge = 0f;
        isUltimateReady = false;
        isPlayer = name.Contains("Player");
        
        // Find the appropriate ult bar based on whether this is a player or AI
        string barName = isPlayer ? "Player1UltBar" : "Player2UltBar";
        GameObject ultBarObj = GameObject.Find(barName);
        if (ultBarObj != null) {
            ultBar = ultBarObj.GetComponent<Image>();
            if (ultBar == null) {
                // Try to find it as a child component
                ultBar = ultBarObj.GetComponentInChildren<Image>();
            }
        }
        
        if (ultBar == null) {
            Debug.LogWarning($"Could not find ultimate bar '{barName}' for {gameObject.name}");
        }
    }

    public void Update() {
        if (ultBar != null) {
            ultBar.fillAmount = Mathf.Clamp(GetChargePercentage(), 0, 1);
        }
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
