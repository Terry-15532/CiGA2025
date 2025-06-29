// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class ObjectOuter : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
//         
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         
//     }
//
//     void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Hand"))
//         {
//             // Call your custom function here
//             other.GetComponent<Hand>().HandEnterOuterSphere();
//         }
//     }
//
//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Hand"))
//         {
//             // Call your custom function here
//             other.GetComponent<Hand>().HandExitOuterSphere();
//         }
//     }
// }
