using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShakeOnEnter : MonoBehaviour
{
    GameObject targetObject;
    public bool isShaking = false;
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 20f;

    private Vector3 originalPosition;
    private float shakeTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        targetObject = GetComponent<SpriteChanger>().targetObject.transform.Find("Outer Sphere").gameObject;
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            shakeTimer += Time.deltaTime * shakeSpeed;

            float offsetX = Mathf.PerlinNoise(shakeTimer, 0f) * 2 - 1;
            float offsetY = Mathf.PerlinNoise(0f, shakeTimer) * 2 - 1;

            Vector3 offset = new Vector3(offsetX, offsetY, 0f) * shakeAmount;
            transform.localPosition = originalPosition + offset;
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            isShaking = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            isShaking = false;
        }
    }

    public void StopShaking()
    {
        isShaking = false;
    }
}
