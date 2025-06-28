using UnityEngine;

public class CameraController : MonoBehaviour{
    public static CameraController instance;
    
    [Header("Idle Sway Settings")]
    public float idleFrequency = 0.5f;
    public float idleAmplitude = 0.5f;

    [Header("Shake Settings")]
    public float shakeFrequency = 25f;
    public float shakeAmplitude = 5f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float mouseSmoothing = 5f;
    public float maxMouseOffset = 5f; // Maximum angle offset caused by mouse

    private float shakeTimer = 0f;
    private Vector2 currentMouseOffset;
    private Vector2 smoothedMouseOffset;

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
        // Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        instance = this;
    }

    void Update()
    {
        float time = Time.time;
        float idleX = Mathf.PerlinNoise(time * idleFrequency * 0.4f, time * idleFrequency * 0.3f) * idleAmplitude;
        float idleY = Mathf.Cos(time * idleFrequency) * idleAmplitude;
        
        float shakeX = 0f;
        float shakeY = 0f;
        if (shakeTimer > 0f)
        {
            shakeX = Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) * 2f - 1f;
            shakeY = Mathf.PerlinNoise(0f, Time.time * shakeFrequency) * 2f - 1f;
            shakeX *= shakeAmplitude;
            shakeY *= shakeAmplitude;
            shakeTimer -= Time.deltaTime;
        }
        
        Vector2 mousePos = Input.mousePosition;
        float mouseX = (mousePos.x / Screen.width - 0.5f) * 2f;
        float mouseY = (mousePos.y / Screen.height - 0.5f) * 2f;
        currentMouseOffset = new Vector2(mouseX, mouseY) * maxMouseOffset;
        smoothedMouseOffset = Vector2.Lerp(smoothedMouseOffset, currentMouseOffset, Time.deltaTime * mouseSmoothing);
        
        Quaternion idleRotation = Quaternion.Euler(idleY + shakeY, idleX + shakeX, 0f);
        Quaternion mouseRotation = Quaternion.Euler(-smoothedMouseOffset.y, smoothedMouseOffset.x, 0f);

        transform.localRotation = initialRotation * mouseRotation * idleRotation;
    }

    public void Shake(float duration)
    {
        shakeTimer = duration;
    }
}
