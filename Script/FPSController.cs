using UnityEngine;

public class FPSController : MonoBehaviour
{
    [SerializeField]
    private int targetFPS = 30;  // Burayý 30 veya 60 yapabilirsin.

    void Awake()
    {
        QualitySettings.vSyncCount = 0;          // VSync kapatýlýyor.
        Application.targetFrameRate = targetFPS; // Hedef FPS atanýyor.
        Debug.Log("Hedef FPS: " + targetFPS);
    }
}
