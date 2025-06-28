using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bounce : MonoBehaviour
{
    public bool debug_mode = false;
    public int direction = -1;               // -1 is left, 1 is right
    public float gravity = 9.81f;              // Acceleration downward
    public float bounceForce = 3f;          // Magnitude of diagonal launch
    public float horizontalMultiplier = 1f;  // X/Z push on bounce
    private float groundY;
    public bool useZ = true;                // If true, bounce along Z instead of X

    private Vector3 velocity;
    private bool bouncing = false;

    private void Start()
    {
        groundY = transform.position.y;
    }

    void Update()
    {
        if (debug_mode && Input.GetMouseButtonDown(0))
        {
            StartBounce();
        }
        if (bouncing)
        {
            // Accelerate downward manually
            velocity.y -= gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;

            // Hit the ground?
            if (transform.position.y <= groundY)
            {
                bouncing = false;
            }
        }
    }

    public void StartBounce()
    {
        if (!bouncing)
        {
            bouncing = true;

            // Assign diagonal velocity
            velocity = new Vector3(
                useZ ? 0f : direction * horizontalMultiplier,
                bounceForce,
                useZ ? direction * horizontalMultiplier : 0f
            );
        }
    }
}
