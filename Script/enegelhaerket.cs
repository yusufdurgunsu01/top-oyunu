using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager için gerekli

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;          // Hareket hýzý
    public float distance = 5f;       // Z ekseninde ileri geri ne kadar gideceði

    private Vector3 startPos;         // Baþlangýç pozisyonu

    void Start()
    {
        startPos = transform.position;
        Debug.Log("Enemy baþlatýldý. Baþlangýç pozisyonu: " + startPos);
    }

    void Update()
    {
        // Sinüs fonksiyonu ile ileri geri z hareketi oluþtur
        float zMovement = Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(startPos.x, startPos.y, startPos.z + zMovement);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Çarpýþma tespit edildi! Çarpan nesne: " + collision.gameObject.name);

        // Eðer çarpýþan nesne Player ise
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player ile çarpýþma tespit edildi! Oyun yeniden baþlatýlýyor...");
            // Oyunu yeniden baþlat
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Alternatif çarpýþma kontrolü
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger tespit edildi! Tetikleyen nesne: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player trigger tespit edildi! Oyun yeniden baþlatýlýyor...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}