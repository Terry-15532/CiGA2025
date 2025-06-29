using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BookManager : MonoBehaviour{
    public static Dictionary<int, BookManager> instances = new Dictionary<int, BookManager>();
    public static BookManager currInstance;
    static readonly int dissolvePercentID = Shader.PropertyToID("_DissolvePercent");
    public BookElement[] bookElements;
    public BookElement levelPreview;
    public int levelIndex;

    private static Sound bgm;


    private BookManager GetNextManager(){
        return instances.Count > currInstance.levelIndex ? instances[currInstance.levelIndex + 1] : null;
    }

    public void Awake(){
        instances.TryAdd(levelIndex, this);
    }

    public void Start(){
        foreach (var bookElement in bookElements){
            bookElement.gameObject.SetActive(false);
        }
        if (levelIndex == 1){
            if (bgm == null)
            {
                bgm = SoundSys.PlaySound("bgm 1", true);
            }
            currInstance = this;
            levelPreview?.DecalFadeIn();
            ShowElements(3f);
            Debug.Log("Showed Elements");
        }
    }

    // public void Update(){
    //     if (this == currInstance && Input.GetKeyDown(KeyCode.Space)){
    //         NextLevel();
    //     }
    // }

    public void ShowElements(float delay = 2.5f){
        Tools.CallDelayed(() => {
            levelPreview?.DecalFadeOut();
            Debug.Log("Faded");
            Shader.SetGlobalFloat(dissolvePercentID, 1);
            StartCoroutine(Tools.Repeat((f) => { Shader.SetGlobalFloat(dissolvePercentID, 1 - f); }, 2f, new WaitForEndOfFrame(), () => { Shader.SetGlobalFloat(dissolvePercentID, 0f); }));
            foreach (var bookElement in bookElements){
                try{
                    bookElement.SetActive(true);
                }
                finally{ }
            }
        }, delay);
        Tools.CallDelayed(() => {
            foreach (var bookElement in bookElements){
                bookElement.PopUp();
            }
            Tools.CallDelayed(() => {
            }, 1f);
        }, delay + 2.5f);
    }

    public void FallDown(){
        foreach (var bookElement in bookElements){
            bookElement.FallDown();
        }
        Tools.CallDelayed(() => {
            StartCoroutine(Tools.Repeat((f) => { Shader.SetGlobalFloat(dissolvePercentID, f); }, 2f, new WaitForEndOfFrame(), () => { Shader.SetGlobalFloat(dissolvePercentID, 1f); }));
        }, 1f);
        Tools.CallDelayed(() => {
            foreach (var bookElement in bookElements){
                bookElement.SetActive(false);
            }
        }, 3.5f);
    }

    public void NextLevel(){
        FallDown();
        Tools.CallDelayed(() => {
            var animator = GameObject.Find("BookAndTable").GetComponent<Animator>();
            animator.SetTrigger("NextPage");
            Tools.CallDelayed(() => {
                var nextManager = GetNextManager();
                if (nextManager != null){
                    nextManager.ShowElements();
                    currInstance = nextManager;
                }
            }, 3.5f);
        }, 2f);
    }

}
