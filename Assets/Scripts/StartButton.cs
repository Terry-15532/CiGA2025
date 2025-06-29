using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour, IPointerEnterHandler
{
    Sound menu_bgm;
    // Start is called before the first frame update
    void Start()
    {
        menu_bgm = SoundSys.PlaySound("menu bgm");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartPressed()
    {
        SoundSys.PlaySound("start game");
        Destroy(menu_bgm.gameObject);
        SceneSwitching.SwitchTo("SampleScene", 0.5f, 0.5f);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundSys.PlaySound("button hover");
    }
}

