using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour{
    public static List<BookManager> instances = new List<BookManager>();
    public static BookManager currInstance;
    public BookElement[] bookElements;
    public int levelIndex;


    public static BookManager GetNextManager(){
        return instances.Count > currInstance.levelIndex ? instances[currInstance.levelIndex + 1] : null;
    }

    public void Awake(){
        Initialize();
    }

    public void Start(){
        foreach (var bookElement in bookElements){
            bookElement.gameObject.SetActive(false);
        }
        if (levelIndex == 1){
            ShowElements(0);
        }
    }

    public void Initialize(){
        instances.Add(this);
        currInstance = this;
    }

    public void ShowElements(float delay = 2.5f){
        Tools.CallDelayed(() => {
            foreach (var bookElement in bookElements){
                try{
                    bookElement.SetActive(true);
                    bookElement.SetAttr(0, ColorAttr.a);
                    bookElement.SetAttrAni(0, 1, time: 1f, ColorAttr.a);
                }
                finally{ }
            }
        }, delay);
        Tools.CallDelayed(() => {
            foreach (var bookElement in bookElements){
                bookElement.PopUp();
            }
        }, delay + 1f);
    }

    public void FallDown(){
        foreach (var bookElement in bookElements){
            bookElement.FallDown();
        }
        Tools.CallDelayed(() => {
            foreach (var bookElement in bookElements){
                bookElement.SetAttrAni(1, 0, time: 1f, ColorAttr.a);
            }
        }, 2f);
    }

    public void NextLevel(){
        var animator = GameObject.Find("BookAndTable").GetComponent<Animator>();
        animator.SetTrigger("NextLevel");
        FallDown();
        if (GetNextManager() != null){
            GetNextManager().ShowElements();
        }
    }

}
