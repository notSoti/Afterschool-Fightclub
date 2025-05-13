using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage = 1;
    public GameObject owner;
    private readonly HashSet<GameObject> alreadyHit = new();
    private UltimateAbility ultimateAbility;

    private void Start() {
        if (owner != null) {
            ultimateAbility = owner.GetComponent<UltimateAbility>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null && hurtbox.owner != owner && !alreadyHit.Contains(hurtbox.owner)) {
            hurtbox.TakeDamage(damage);
            alreadyHit.Add(hurtbox.owner);
            
            // Charge ultimate when dealing damage
            ultimateAbility?.AddCharge(damage);
        }
    }

    public void ResetHits() {
        alreadyHit.Clear();
    }
}
