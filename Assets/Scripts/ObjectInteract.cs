using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    public bool isHeld = false;
    public Sprite heldSprite;
    private Sprite oldSprite;

    public Vector3 offset;
    public static int emissionID = Shader.PropertyToID("_Emission");
    public static int colorID = Shader.PropertyToID("_EdgeColor");
    public static int thresholdID = Shader.PropertyToID("_Threshold");

    public bool playanim_disable_click = false;

    public bool railroaded = false;
    public Vector3 hand_final_pos;
    public GameObject target_object;


    // Start is called before the first frame update
    void Start()
    {
        yVal = transform.position.y;
        oldSprite = GetComponent<SpriteRenderer>().sprite;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sharedMaterial.SetFloat(emissionID, 1);
        sr.sharedMaterial.SetFloat(thresholdID, 2f);
        sr.sharedMaterial.SetColor(colorID, UnityEngine.Color.black);
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

    private void OnMouseEnter()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sharedMaterial.SetFloat(emissionID, 2.5f);
        sr.sharedMaterial.SetFloat(thresholdID, 0.0001f);
        sr.sharedMaterial.SetColor(colorID, UnityEngine.Color.yellow);
    }

    private void OnMouseExit()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sharedMaterial.SetFloat(emissionID, 1);
        sr.sharedMaterial.SetFloat(thresholdID, 2f);
        sr.sharedMaterial.SetColor(colorID, UnityEngine.Color.black);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!(playanim_disable_click && GetComponent<SpriteChanger>().aniPlayed) && other.CompareTag("Hand"))
        {
            other.GetComponent<Hand>().HandEnterObjectInteract(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!(playanim_disable_click && GetComponent<SpriteChanger>().aniPlayed) && other.CompareTag("Hand"))
        {
            other.GetComponent<Hand>().HandExitObjectInteract(this);
        }
    }

    public void Release()
    {
        //transform.position = new Vector3(transform.position.x, yVal, transform.position.z);
        isFalling = true;
    }

    public void StartHolding()
    {
        isHeld = true;
        if (heldSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = heldSprite;
        }
    }

    public void StopHolding()
    {
        isHeld = false;
        if (heldSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = oldSprite;
        }
    }

}
