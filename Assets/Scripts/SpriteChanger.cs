using UnityEngine;

public class SpriteChanger : MonoBehaviour{
    public Sprite targetSprite;

    [Header("触碰到该物体后将图片替换为targetSprite")]
    public GameObject targetObject;

    public void OnTriggerEnter(Collider other){
        if (other.gameObject == targetObject){
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = targetSprite;
        }
    }
}
