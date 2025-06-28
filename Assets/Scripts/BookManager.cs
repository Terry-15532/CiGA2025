using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour{
    public static List<BookManager> instances = new List<BookManager>();
    public static BookManager currInstance;
    public static Transform bookPageLeft, bookPageRight, leftParent, rightParent; // 书本左右两页的Transform
    public BookElement[] bookElementsLeft, bookElementsRight; // 书本元素数组
    public int levelIndex;

    public static BookManager GetNextManager(){
        return instances.Count > currInstance.levelIndex ? instances[currInstance.levelIndex + 1] : null;
    }

    public void ShowElements(){
        foreach (var bookElement in bookElementsLeft){
            bookElement.transform.SetParent(bookPageLeft);
            bookElement.gameObject.SetActive(true);
        }
        Tools.CallDelayed(() => {
            leftParent.transform.SetParent(bookPageLeft);
            rightParent.transform.SetParent(bookPageRight);
            foreach (var bookElement in bookElementsRight){
                bookElement.gameObject.SetActive(true);
            }
        }, 0.5f);
        Tools.CallDelayed(() => {
            leftParent.transform.SetParent(bookPageLeft.parent);
            rightParent.transform.SetParent(bookPageRight.parent);
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

}
