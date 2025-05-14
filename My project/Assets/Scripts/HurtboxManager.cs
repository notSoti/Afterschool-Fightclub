using UnityEngine;

public class Hurtbox : MonoBehaviour {
    public GameObject owner;
    private Health ownerHealth;

    private void Start() {
        if (owner != null) {
            ownerHealth = owner.GetComponent<Health>();
            if (ownerHealth == null) {
                Debug.LogWarning($"No Health component found on {owner.name}. Damage will not affect health.");
            }
        }
    }

    public void TakeDamage(int amount) {
        ownerHealth?.TakeDamage(amount);
    }
}
