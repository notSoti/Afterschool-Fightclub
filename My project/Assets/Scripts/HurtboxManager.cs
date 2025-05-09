using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public GameObject owner;

    public void TakeDamage(int amount)
    {
        Debug.Log($"{owner.name} took {amount} damage!");
        // add health and damage logic
    }
}
