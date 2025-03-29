using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    void Update()
    {
        if (player1 != null && player2 != null)
        {
            // Calculate the midpoint between the two players
            Vector3 middlePoint = (player1.position + player2.position) / 2f;
            transform.position = middlePoint;
        }
    }
}
