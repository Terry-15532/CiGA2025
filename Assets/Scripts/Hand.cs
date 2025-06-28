using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Hand : MonoBehaviour
{
    float yVal, yValHand, targetY;
    bool clickable = false;
    bool holding = false;
    ObjectInteract currObj;

    public float acceleration = 9.81f; // Acceleration rate
    public float deceleration = 9.81f;
    public float maxSpeed = 20f;     // Optional cap on falling speed
    public float startRiseSpeed = 4f;
    private float currentSpeed = 0f;
    bool isFalling = false;
    bool isRising = false;

    private void Start()
    {
        yVal = transform.position.y;
        yValHand = transform.Find("Hand").position.y;
        targetY = yValHand + 0.3f;
    }

    void Update()
    {
        // Update sprite position
        transform.position = GetMousePositionInXZPlane(yVal);

        // Rotate only around X to face the camera
        //FaceCameraOnXAxisOnly();

        if(clickable && !holding && !currObj.isFalling && Input.GetMouseButtonDown(0))
        {
            currObj.transform.SetParent(transform.Find("Hand"));
            Vector3 handPos = transform.Find("Hand").position;
            currObj.transform.localPosition = /*new Vector3(handPos.x, handPos.y, handPos.z) +*/ new Vector3(0.42f, 9.56f, -0.62f);
            holding = true;
            transform.Find("Hand").GetComponent<SpriteRenderer>().sortingOrder = 1;
            currentSpeed = startRiseSpeed;
            isRising = true;
        }
        else if(holding && Input.GetMouseButtonDown(0))
        {
            currObj.transform.SetParent(null);
            currObj.Release();
            holding = false;
            isFalling = true;
        }

        if (isFalling && transform.Find("Hand").position.y > yValHand)
        {
            // Accelerate downward
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            // Move the object
            transform.Find("Hand").position -= new Vector3(0, currentSpeed * Time.deltaTime, 0);

            // Clamp if overshooting
            if (transform.Find("Hand").position.y <= yValHand)
            {
                Vector3 pos = transform.Find("Hand").position;
                pos.y = yValHand;
                transform.Find("Hand").position = pos;
                currentSpeed = 0f; // Stop
                transform.Find("Hand").GetComponent<SpriteRenderer>().sortingOrder = 3;
                isFalling = false;
            }
        }
        else if (isRising && transform.Find("Hand").position.y < targetY)
        {
            // Move upward
            transform.Find("Hand").position += new Vector3(0, currentSpeed * Time.deltaTime, 0);

            // Decelerate
            currentSpeed -= deceleration * Time.deltaTime;

            // Clamp if overshooting
            if (transform.Find("Hand").position.y >= targetY)
            {
                Vector3 pos = transform.Find("Hand").position;
                pos.y = targetY;
                transform.Find("Hand").position = pos;
                currentSpeed = 0f; // Stop
                isRising = false;
            }
        }


    }

    public Vector3 GetMousePositionInXZPlane(float yPlane)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Mathf.Approximately(ray.direction.y, 0f))
            return Vector3.zero;

        float distance = (yPlane - ray.origin.y) / ray.direction.y;
        Vector3 worldPoint = ray.origin + ray.direction * distance;

        return worldPoint;
    }

    private void FaceCameraOnXAxisOnly()
    {
        Vector3 toCamera = Camera.main.transform.position - transform.position;

        Vector3 flatDirection = new Vector3(toCamera.x, toCamera.y, 0);

        if (flatDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection, Vector3.forward);
            transform.rotation = targetRotation;
        }
    }

    public void HandEnterObjectInteract(ObjectInteract obj)
    {
        Debug.Log("Entered interact sphere " + obj.name);
        currObj = obj;
        clickable = true;
    }
    public void HandEnterOuterSphere()
    {
        Debug.Log("Entered outer sphere");
    }
    public void HandExitObjectInteract(ObjectInteract obj)
    {
        Debug.Log("Exited interact sphere " + obj.name);
        clickable = false;
    }
    public void HandExitOuterSphere()
    {
        Debug.Log("Exited outer sphere");
    }

}
