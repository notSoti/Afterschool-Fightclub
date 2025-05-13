using UnityEngine;

public class Example : MonoBehaviour
{
    public static bool fullScreen;
    void Update()
    {
        // Toggle fullscreen
        Screen.fullScreen = !Screen.fullScreen;
    }
}
