using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour{
    public static List<BookManager> instances = new List<BookManager>();
    public static BookManager currInstance;
    public static Transform bookPageLeft, bookPageRight; // 书本左右两页的Transform
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
            foreach (var bookElement in bookElementsRight){
                bookElement.transform.SetParent(bookPageRight);
                bookElement.gameObject.SetActive(true);
            }
        }, 0.5f);
        Tools.CallDelayed(() => {
            transform.SetParent(bookPageLeft.parent);
            transform.SetParent(bookPageLeft.parent);
            Tools.CallDelayed(() => {
                foreach (var bookElement in bookElementsLeft){
                    bookElement.PopUp();
                }
                foreach (var bookElement in bookElementsRight){
                    bookElement.PopUp();
                }
            }, 0.5f);
        }, 3f);
    }

}
