using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClickable : MonoBehaviour
{
    private bool clickable = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (clickable && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Clickable Object clicked");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            clickable = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            clickable = false;
        }
    }
}
