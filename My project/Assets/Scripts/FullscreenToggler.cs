using UnityEngine;

public class Example : MonoBehaviour
{
    public static bool fullScreen;
    void Start()
    {
        // Toggle fullscreen
        Screen.fullScreen = !Screen.fullScreen;
    }
}
