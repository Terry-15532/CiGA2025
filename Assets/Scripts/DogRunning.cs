using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogRunning : MonoBehaviour{

    public bool running = false;
    public GameObject target;
    public float runningSpeed = 1;

    public void startRunning(){
        running = true;
    }

    public void Update(){
        if (running){
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * runningSpeed);
        }
        if (Vector3.Magnitude(transform.position - target.transform.position) < 0.1f){
            running = false;
            Tools.CallDelayed(() => {
                BookManager.currInstance.NextLevel();
            }, 2f);
        }
    }


}
