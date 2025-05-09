using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage = 1;
    public GameObject owner;
    private readonly HashSet<GameObject> alreadyHit = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null && hurtbox.owner != owner && !alreadyHit.Contains(hurtbox.owner))
        {
            hurtbox.TakeDamage(damage);
            alreadyHit.Add(hurtbox.owner);
        }
    }

    public void ResetHits()
    {
        alreadyHit.Clear();
    }
}
