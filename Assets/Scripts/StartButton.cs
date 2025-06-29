using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPressed()
    {
        SoundSys.PlaySound("start game");
        SceneSwitching.SwitchTo("SampleScene", 0.5f, 0.5f);
    }

    private void OnMouseEnter()
    {
        SoundSys.PlaySound("button hover");
    }
}
