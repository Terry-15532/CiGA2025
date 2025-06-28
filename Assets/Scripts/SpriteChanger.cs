using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SpriteChanger : MonoBehaviour{
    public Sprite targetSprite;

    [Header("播放动画结束后调用的事件")]
    public UnityEvent onAnimationEnd;

    [Header("触碰到该物体后将图片替换为targetSprite")]
    public GameObject targetObject;

    [Header("是否播放动画")]
    public bool playAnimation;

    [Header("是否仅播放一次动画")]
    public bool singleAnimation = true;

    public bool aniPlayed = false;
    public float animationDuration = 0.5f;

    [Header("首次执行的动画")]
    public Sprite[] sprites;

    [Header("是否循环播放第二组动画")]
    public bool looped = false;

    [Header("循环播放的动画")]
    public Sprite[] spritesLooped;

    public float loopFPS;

    public Coroutine animationCoroutine;

    public void OnTriggerEnter(Collider other){
        if (!aniPlayed && other.gameObject == targetObject){
            //GetComponent<ShakeOnEnter>().StopShaking();
            aniPlayed = true;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (!playAnimation){
                spriteRenderer.sprite = targetSprite;
            }
            else{
                if (animationCoroutine != null)
                    StopCoroutine(animationCoroutine);
                animationCoroutine = StartCoroutine(PlayAnimation(spriteRenderer));
            }
        }
    }

    public IEnumerator PlayAnimation(SpriteRenderer spriteRenderer){
        float elapsed = 0f;
        float interval = animationDuration / (sprites.Length - 1);
        int index = 0;

        while (index < sprites.Length){
            elapsed += Time.deltaTime;
            spriteRenderer.sprite = sprites[index];
            index++;
            yield return new WaitForSeconds(interval);
        }
        if (onAnimationEnd != null){
            onAnimationEnd.Invoke();
        }
        index = 0;
        while (looped){
            index++;
            index = index % spritesLooped.Length;
            spriteRenderer.sprite = spritesLooped[index];
            yield return new WaitForSeconds(1f / loopFPS);
        }
        spriteRenderer.sprite = targetSprite;
    }
}
