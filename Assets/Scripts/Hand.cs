using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Hand : MonoBehaviour
{
    private float yVal, yValHand;
    private bool followMouse = true;
    private bool clickable = false;
    private bool holding = false;
    private ObjectInteract currObj;

    public float duration = 0.75f;
    private float elapsedTime = 0f;

    private bool isFalling = false;
    private bool isRising = false;

    private bool seekMouse = false;
    public float seekStrength = 10f;
    public float snapDistance = 0.1f;

    private void Start()
    {
        yVal = transform.position.y;
        yValHand = transform.Find("Hand").position.y;
    }

    void Update()
    {
        // Update sprite position
        if (seekMouse)
        {
            Vector3 target = GetMousePositionInXZPlane(yVal);
            Vector3 toTarget = target - transform.position;
            float distance = toTarget.magnitude;

            if (distance < snapDistance)
            {
                transform.position = target;
                seekMouse = false;
                followMouse = true;
            }
            else
            {
                // Move faster the farther away you are
                float speed = seekStrength * distance;
                transform.position += toTarget.normalized * speed * Time.deltaTime;
            }
        }
        else if (followMouse)
        {
            transform.position = GetMousePositionInXZPlane(yVal);
        }

        // Rotate only around X to face the camera
        //FaceCameraOnXAxisOnly();

        if (clickable && !holding && !currObj.isFalling && Input.GetMouseButtonDown(0))
        {
            followMouse = false;
            elapsedTime = 0f;
            isFalling = true;

        }
        else if(holding && Input.GetMouseButtonDown(0))
        {
            followMouse = false;
            elapsedTime = 0f;
            isFalling = true;
        }

        if (isFalling && transform.Find("Hand").position.y > yVal)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Eased fall: smooth at start and end
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            float newY = Mathf.Lerp(yValHand, yVal, easedT);

            Vector3 pos = transform.Find("Hand").position;
            pos.y = newY;
            transform.Find("Hand").position = pos;

            if (t >= 1f)
            {
                isFalling = false;
                if (!holding)
                {
                    TakeObject();
                }
                else
                {
                    ReleaseObject();
                }
                
            }
        }
        else if (isRising && transform.Find("Hand").position.y < yValHand)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Smoothstep easing (ease-in and ease-out)
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            float newY = Mathf.Lerp(yVal, yValHand, easedT);

            Vector3 pos = transform.Find("Hand").position;
            pos.y = newY;
            transform.Find("Hand").position = pos;

            if (t >= 1f)
            {
                isRising = false;
                seekMouse = true;
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

    void TakeObject()
    {
        currObj.transform.SetParent(transform.Find("Hand"));
        Vector3 handPos = transform.Find("Hand").position;
        currObj.transform.localPosition = /*new Vector3(handPos.x, handPos.y, handPos.z) +*/ new Vector3(0.42f, 9.56f, -0.62f);
        holding = true;
        transform.Find("Hand").GetComponent<SpriteRenderer>().sortingOrder = 1;
        elapsedTime = 0f;
        isRising = true;
    }

    void ReleaseObject()
    {
        currObj.transform.SetParent(null);
        currObj.Release();
        holding = false;
        transform.Find("Hand").GetComponent<SpriteRenderer>().sortingOrder = 3;
        elapsedTime = 0f;
        isRising = true;
    }

}
