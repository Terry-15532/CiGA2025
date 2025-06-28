using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour{
    public static List<BookManager> instances = new List<BookManager>();
    public static BookManager currInstance;
    public static Transform bookPage, leftParent, rightParent; // 书本左右两页的Transform
    public BookElement[] bookElementsLeft, bookElementsRight; // 书本元素数组
    public int levelIndex;

    public static BookManager GetNextManager(){
        return instances.Count > currInstance.levelIndex ? instances[currInstance.levelIndex + 1] : null;
    }

    public void Awake(){
        Initialize();
    }

    public void Initialize(){
        instances.Add(this);
        currInstance = this;
        bookPage = transform.Find("MovingPage");

        // 隐藏所有元素
        foreach (var bookElement in bookElementsLeft){
            bookElement.gameObject.SetActive(false);
        }
        foreach (var bookElement in bookElementsRight){
            bookElement.gameObject.SetActive(false);
        }
    }

    public void ShowElements(){
        foreach (var bookElement in bookElementsLeft){
            Vector3 euler = transform.localEulerAngles;
            bookElement.transform.localEulerAngles = new Vector3(-90, euler.y, euler.z);
            bookElement.transform.SetParent(bookPage);
            bookElement.gameObject.SetActive(true);
        }
        Tools.CallDelayed(() => {
            leftParent.transform.SetParent(bookPage);
            foreach (var bookElement in bookElementsRight){
                bookElement.gameObject.SetActive(true);
            }
        }, 0.5f);
        Tools.CallDelayed(() => {
            leftParent.transform.SetParent(bookPage.parent);
            Tools.CallDelayed(() => {
                foreach (var bookElement in bookElementsLeft){
                    bookElement.PopUp();
                }
                foreach (var bookElement in bookElementsRight){
                    bookElement.PopUp();
                }
            }, 0.5f);
        }, 2.45f);
    }

    public void FallDown(){
        foreach (var bookElement in bookElementsLeft){
            bookElement.FallDown();
        }
        foreach (var bookElement in bookElementsRight){
            bookElement.FallDown();
        }
    }
    
    public void NextLevel(){
        var animator = GameObject.Find("BookAndTable").GetComponent<Animator>();
        animator.SetTrigger("NextLevel");
        if (GetNextManager() != null){
            GetNextManager().ShowElements();
        }
    }

}
