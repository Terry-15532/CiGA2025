using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    float yval;
    bool clickable = false;
    bool holding = false;
    ObjectInteract currObj;

    private void Start()
    {
        yval = transform.position.y;
    }

    void Update()
    {
        // Update sprite position
        transform.position = GetMousePositionInXZPlane(yval);

        // Rotate only around X to face the camera
        //FaceCameraOnXAxisOnly();

        if(clickable && !holding && !currObj.isFalling && Input.GetMouseButtonDown(0))
        {
            currObj.transform.SetParent(transform.Find("Hand"));
            Vector3 handPos = transform.Find("Hand").position;
            currObj.transform.localPosition = /*new Vector3(handPos.x, handPos.y, handPos.z) +*/ new Vector3(0.42f, 9.56f, -0.62f);
            holding = true;
        }
        else if(holding && Input.GetMouseButtonDown(0))
        {
            currObj.transform.SetParent(null);
            currObj.Release();
            holding = false;
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
