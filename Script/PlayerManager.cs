using System.Reflection;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private Transform ball;
    private Vector3 startMousePos, startBallPos;
    private bool moveTheBall;

    [Range(0f, 1f)] public float maxSpeed = 0.3f;
    [Range(0f, 1f)] public float camSpeed = 0.5f;
    [Range(0f, 50f)] public float pathSpeed = 10f;

    private float velocity, camVelocityX, camVelocityY;

    private Camera mainCam;
    public Transform path;

    private Rigidbody rb;
    private Collider _collider;
    private Renderer ballrenderer;
    public ParticleSystem CollideParticle;
    public ParticleSystem airEffect;

    // Ses ile ilgili değişkenler
    public AudioClip coloredBallHitSound; // Renkli toplara çarpınca çalacak ses
    public AudioClip pathCollisionSound;  // "path" objesine çarpınca çalacak ses (isteğe bağlı)
    private AudioSource audioSource;      // Bu objenin AudioSource bileşeni

    // Art arda çarpma sayaç değişkenleri
    private int consecutiveHits = 0;
    private float lastHitTime;
    public float hitTimeout = 0.5f; // Maksimum süre aralığı (saniye)

    void Start()
    {
        ball = transform;
        mainCam = Camera.main;

        rb = GetComponent<Rigidbody>();

        _collider = GetComponent<Collider>();
        ballrenderer = GetComponent<Renderer>();

        // AudioSource bileşenini al veya ekle
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // Başlangıçta ses çalmasın
        audioSource.spatialBlend = 1f; // Sesin 3D olmasını isterseniz (objenin konumuna göre ses)
                                       // 0f yaparsanız 2D olur (ekranın her yerinde aynı şiddette)
    }

    void Update()
    {
        if (MenuManager.MenuManagerInstance.GameState && !rb.isKinematic)
        {
            // Top ileri doğru giderken dönsün
            rb.AddForce(Vector3.forward * pathSpeed, ForceMode.Force);
            rb.AddTorque(Vector3.right * 10f, ForceMode.Force); // X ekseninde döndür (teker gibi)
        }
        if (Input.GetMouseButtonDown(0) && MenuManager.MenuManagerInstance.GameState)
        {
            moveTheBall = true;

            Plane newPlane = new Plane(Vector3.up, 0f);
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (newPlane.Raycast(ray, out float distance))
            {
                startMousePos = ray.GetPoint(distance);
                startBallPos = ball.position;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            moveTheBall = false;
        }


        if (moveTheBall)
        {
            Plane newPlane = new Plane(Vector3.up, 0f);
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (newPlane.Raycast(ray, out float distance))
            {
                Vector3 mouseNewPos = ray.GetPoint(distance);
                Vector3 delta = mouseNewPos - startMousePos;
                Vector3 desiredBallPos = startBallPos + delta;

                desiredBallPos.x = Mathf.Clamp(desiredBallPos.x, -1.5f, 1.5f);

                float smoothX = Mathf.SmoothDamp(ball.position.x, desiredBallPos.x, ref velocity, maxSpeed);
                ball.position = new Vector3(smoothX, ball.position.y, ball.position.z);
            }
        }

        if (MenuManager.MenuManagerInstance.GameState)
        {
            Vector3 pathPosition = path.position;
            path.position = Vector3.MoveTowards(
                pathPosition,
                new Vector3(pathPosition.x, pathPosition.y, -1000f),
                Time.deltaTime * pathSpeed
            );
        }
    }

    void LateUpdate()
    {
        Vector3 cameraNewPos = mainCam.transform.position;

        if (rb.isKinematic)
        {
            mainCam.transform.position = new Vector3(
                Mathf.SmoothDamp(cameraNewPos.x, ball.transform.position.x, ref camVelocityX, camSpeed),
                Mathf.SmoothDamp(cameraNewPos.y, ball.transform.position.y + 4f, ref camVelocityY, camSpeed),
                cameraNewPos.z
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("obstacle"))
        {
            gameObject.SetActive(false);
            MenuManager.MenuManagerInstance.GameState = false;
        }

        // Renkli toplara çarpma kontrolü ve ses çalma
        if (other.CompareTag("red") || other.CompareTag("blue") || other.CompareTag("green") || other.CompareTag("yellow"))
        {
            // Ses çalma kısmı
            if (audioSource != null && coloredBallHitSound != null)
            {
                audioSource.PlayOneShot(coloredBallHitSound);
            }

            // Art arda çarpma sayaç kontrolü
            if (Time.time - lastHitTime <= hitTimeout)
            {
                consecutiveHits++;
            }
            else
            {
                consecutiveHits = 1;
            }

            lastHitTime = Time.time;

            if (consecutiveHits >= 3)
            {
                // Aniden hızlanma ve zıplama
                pathSpeed = 15f; // Hız aniden artar

                // Air effect hızını artır
                var airEffectMain = airEffect.main;
                airEffectMain.simulationSpeed = 15f;

                // Kamera sarsıntısı başlat
                StartCoroutine(CameraShake());

                consecutiveHits = 0;
            }

            // Renk değiştirme, parçacık efekti ve objeyi devre dışı bırakma
            other.gameObject.SetActive(false);
            ballrenderer.material = other.GetComponent<Renderer>().material;
            var newParticle = Instantiate(CollideParticle, transform.position, Quaternion.identity);
            newParticle.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("path"))
        {
            rb.isKinematic = false;

            rb.linearVelocity = new Vector3(0f, 8f, 0f); // Burada linearVelocity değil velocity kullanıldı
            pathSpeed = pathSpeed * 2;

            var airEffectMain = airEffect.main;
            airEffectMain.simulationSpeed = 10f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("path"))
        {
            rb.isKinematic = true;
            _collider.isTrigger = true;
            pathSpeed = 30f;

            var airEffectMain = airEffect.main;
            airEffectMain.simulationSpeed = 4f;

            // "path" objesiyle çarpışma sesi çal (isteğe bağlı)
            if (audioSource != null && pathCollisionSound != null)
            {
                if (!audioSource.isPlaying) // Sadece ses çalmıyorsa çal
                {
                    audioSource.PlayOneShot(pathCollisionSound);
                }
            }
        }
    }

    // Kamera sarsıntısı efekti coroutine
    private IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCam.transform.position;
        float elapsed = 0f;
        float duration = 0.3f;
        float strength = 0.3f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            mainCam.transform.position = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.position = originalPos;
    }
}