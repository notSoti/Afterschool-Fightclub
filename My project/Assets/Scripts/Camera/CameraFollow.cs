using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public SpriteRenderer background;

    public float smoothSpeed = 5f;
    public float orthographicSize = 3f; // Smaller value = smaller view

    void Start()
    {
        Camera.main.orthographicSize = orthographicSize;
    }

    void LateUpdate()
    {
        if (target != null && background != null)
        {
            // Calculate camera half sizes
            float camHeight = Camera.main.orthographicSize;
            float camWidth = camHeight * Camera.main.aspect;

            // Get background bounds
            Bounds bgBounds = background.bounds;
            float minX = bgBounds.min.x + camWidth;
            float maxX = bgBounds.max.x - camWidth;
            float minY = bgBounds.min.y + camHeight;
            float maxY = bgBounds.max.y - camHeight;

            // Clamp the target position so camera stays within background bounds
            float posX = Mathf.Clamp(target.position.x, minX, maxX);
            float posY = Mathf.Clamp(target.position.y, minY, maxY);

            Vector3 newPosition = new Vector3(posX, posY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
