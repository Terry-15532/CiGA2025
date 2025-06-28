using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectInteract : MonoBehaviour
{
    float yVal;
    public float acceleration = 9.81f; // Acceleration rate
    public float maxSpeed = 20f;     // Optional cap on falling speed
    private float currentSpeed = 0f;
    public bool isFalling = false;

    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        yVal = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling && transform.position.y > yVal)
        {
            // Accelerate downward
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            // Move the object
            transform.position -= new Vector3(0, currentSpeed * Time.deltaTime, 0);

            // Clamp if overshooting
            if (transform.position.y <= yVal)
            {
                Vector3 pos = transform.position;
                pos.y = yVal;
                transform.position = pos;
                currentSpeed = 0f; // Stop
                isFalling = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            other.GetComponent<Hand>().HandEnterObjectInteract(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            other.GetComponent<Hand>().HandExitObjectInteract(this);
        }
    }

    public void Release()
    {
        //transform.position = new Vector3(transform.position.x, yVal, transform.position.z);
        isFalling = true;
    }

}
