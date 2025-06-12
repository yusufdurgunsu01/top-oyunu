using UnityEngine;

public class FPSController : MonoBehaviour
{
    [SerializeField]
    private int targetFPS = 30;  // Buray� 30 veya 60 yapabilirsin.

    void Awake()
    {
        QualitySettings.vSyncCount = 0;          // VSync kapat�l�yor.
        Application.targetFrameRate = targetFPS; // Hedef FPS atan�yor.
        Debug.Log("Hedef FPS: " + targetFPS);
    }
}
