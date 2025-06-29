using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Hand : MonoBehaviour
{
    private float yVal, yValHand;
    public bool followMouse = true;
    public bool clickable = false;
    public bool clickable_railroad = false;
    public bool holding = false;
    public ObjectInteract currObj;

    public float duration = 0.75f;
    public float elapsedTime = 0f;

    private bool seekMouse = false;
    public float seekStrength = 10f;
    public float snapDistance = 0.1f;

    public static int emissionID = Shader.PropertyToID("_Emission");
    public static int colorID = Shader.PropertyToID("_EdgeColor");
    public static int thresholdID = Shader.PropertyToID("_Threshold");

    public bool isFalling = false;
    public bool isRising = false;
    public bool isRailroadingDown = false;
    public bool isRailroadingUp = false;

    public bool railroaded = false;
    public bool railroaddown = false;
    public Vector3 hand_final_pos;
    public GameObject target_object;
    public Vector3 startPos;

    private Vector3 localHandPos;

    private void Start()
    {
        yVal = transform.position.y;
        yValHand = transform.Find("Hand").position.y;
        localHandPos = transform.Find("Hand").localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            followMouse = true;
            seekMouse = false;
            isFalling = false;
            isRising = false;
            isRailroadingDown = false;
            isRailroadingUp = false;
            railroaded = false;
            ReleaseObjectNoRise();
            holding = false;
            transform.Find("Hand").localPosition = localHandPos;
        }
        
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

        if (clickable_railroad && railroaded && holding && !isFalling && !isRising && !isRailroadingDown && !isRailroadingUp && Input.GetMouseButtonDown(0))
        {
            followMouse = false;
            elapsedTime = 0f;
            startPos = transform.Find("Hand").position;
            isRailroadingDown = true;
        }
        else if (clickable && !holding && !isFalling && !isRising && !isRailroadingDown && !isRailroadingUp && Input.GetMouseButtonDown(0))
        {
            followMouse = false;
            elapsedTime = 0f;
            isFalling = true;

        }
        else if(holding && !isFalling && !isRising && !isRailroadingDown && !isRailroadingUp && Input.GetMouseButtonDown(0))
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
        else if (isRailroadingDown)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Smooth easing (ease-in, ease-out)
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            transform.Find("Hand").position = Vector3.Lerp(startPos, hand_final_pos, easedT);

            if (t >= 1f)
            {
                isRailroadingDown = false;
                railroaddown = true;
                ReleaseObject();
            }
        }
        else if (isRailroadingUp)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Smooth easing (ease-in, ease-out)
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            transform.Find("Hand").position = Vector3.Lerp(hand_final_pos, startPos, easedT);

            if (t >= 1f)
            {
                isRailroadingUp = false;
                railroaded = false;
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
        if (!holding)
        {
            currObj = obj;
            clickable = true;
        }
        else if (currObj.GetComponent<SpriteChanger>() != null && currObj.GetComponent<SpriteChanger>().targetObject == obj.gameObject)
        {
            Debug.Log("Starting outline");
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            sr.sharedMaterial.SetFloat(emissionID, 20f);
            sr.sharedMaterial.SetFloat(thresholdID, 0.0001f);
            sr.sharedMaterial.SetColor(colorID, UnityEngine.Color.yellow);
        }
    }

    public void HandExitObjectInteract(ObjectInteract obj)
    {
        Debug.Log("Exited interact sphere " + obj.name);
        if (!holding)
        {
            currObj = null;
            clickable = false;
        }
        else if (currObj.GetComponent<SpriteChanger>() != null && currObj.GetComponent<SpriteChanger>().targetObject == obj.gameObject)
        {
            Debug.Log("Stopping outline");
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            sr.sharedMaterial.SetFloat(emissionID, 1);
            sr.sharedMaterial.SetFloat(thresholdID, 2f);
            sr.sharedMaterial.SetColor(colorID, UnityEngine.Color.black);
        }
    }

    public void HandEnterOuterSphere()
    {
        Debug.Log("Entered outer sphere");
    }
    
    public void HandExitOuterSphere()
    {
        Debug.Log("Exited outer sphere");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (railroaded && holding && other.gameObject == target_object)
        {
            clickable_railroad = true;
            SpriteRenderer sr = currObj.GetComponent<SpriteRenderer>();
            sr.sharedMaterial.SetFloat(emissionID, 2.5f);
            sr.sharedMaterial.SetFloat(thresholdID, 0.0001f);
            sr.sharedMaterial.SetColor(colorID, UnityEngine.Color.yellow);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (railroaded && holding && other.gameObject == target_object)
        {
            clickable_railroad = false;
            SpriteRenderer sr = currObj.GetComponent<SpriteRenderer>();
            sr.sharedMaterial.SetFloat(emissionID, 1);
            sr.sharedMaterial.SetFloat(thresholdID, 2f);
            sr.sharedMaterial.SetColor(colorID, UnityEngine.Color.black);
        }
    }

    void TakeObject()
    {
        if (currObj != null)
        {
            currObj.transform.SetParent(transform.Find("Hand"));
            currObj.StartHolding();
            Vector3 handPos = transform.Find("Hand").position;
            currObj.transform.localPosition = currObj.offset;
            holding = true;
            transform.Find("Hand").GetComponent<SpriteRenderer>().sortingOrder = 5;
            if (currObj.railroaded)
            {
                railroaded = true;
                hand_final_pos = currObj.hand_final_pos;
                target_object = currObj.target_object;
            }
        }
        elapsedTime = 0f;
        isRising = true;
    }

    void ReleaseObject()
    {
        if (currObj != null)
        {
            currObj.transform.SetParent(null);
            currObj.StopHolding();
            currObj.Release();
            holding = false;
            transform.Find("Hand").GetComponent<SpriteRenderer>().sortingOrder = 7;
        }

        if (!railroaddown)
        {
            elapsedTime = 0f;
            isRising = true;
        }
        else
        {
            railroaddown = false;
            elapsedTime = 0f;
            isRailroadingUp = true;
        }

    }

    void ReleaseObjectNoRise()
    {
        if (currObj != null)
        {
            currObj.transform.SetParent(null);
            currObj.StopHolding();
            currObj.Release();
            holding = false;
            transform.Find("Hand").GetComponent<SpriteRenderer>().sortingOrder = 7;
        }
    }

}

