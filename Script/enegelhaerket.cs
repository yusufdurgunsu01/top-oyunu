using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager i�in gerekli

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;          // Hareket h�z�
    public float distance = 5f;       // Z ekseninde ileri geri ne kadar gidece�i

    private Vector3 startPos;         // Ba�lang�� pozisyonu

    void Start()
    {
        startPos = transform.position;
        Debug.Log("Enemy ba�lat�ld�. Ba�lang�� pozisyonu: " + startPos);
    }

    void Update()
    {
        // Sin�s fonksiyonu ile ileri geri z hareketi olu�tur
        float zMovement = Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(startPos.x, startPos.y, startPos.z + zMovement);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("�arp��ma tespit edildi! �arpan nesne: " + collision.gameObject.name);

        // E�er �arp��an nesne Player ise
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player ile �arp��ma tespit edildi! Oyun yeniden ba�lat�l�yor...");
            // Oyunu yeniden ba�lat
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Alternatif �arp��ma kontrol�
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger tespit edildi! Tetikleyen nesne: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player trigger tespit edildi! Oyun yeniden ba�lat�l�yor...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}