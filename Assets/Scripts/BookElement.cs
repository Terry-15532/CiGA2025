using System;
using UnityEngine;
using System.Collections;

public class BookElement : CustomElement{
    private Coroutine animationCoroutine;

    [Header("是否自动检测e直立角度")]
    public bool autoDetectTargetDegree = true;

    [Header("直立角度")]
    public float targetDegree;

    [Header("倒下的角度")]
    public float initialDegree = -90f;

    public void Awake(){
        if (autoDetectTargetDegree){
            targetDegree = transform.localEulerAngles.x;
            SetXRotation(initialDegree + 0.01f);
        }
        base.Awake();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            FallDown();
        }

    }

    private float EaseOutElastic(float t){
        float c4 = (2 * Mathf.PI) / 3;
        if (t == 0f) return 0f;
        if (Mathf.Approximately(t, 1f)) return 1f;

        return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    }

    public void PopUp(){
        PopUp(startAngle: initialDegree, settleAngle: targetDegree);
    }

    /// <summary>
    /// 弹起动画（绕X轴，带弹性插值，保留其他旋转）
    /// </summary>
    private void PopUp(float duration = 2f, float startAngle = 0f, float settleAngle = 50f){
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(PopUpRoutine(duration, startAngle, settleAngle));
    }

    private IEnumerator PopUpRoutine(float duration, float startAngle, float settleAngle){
        // 1. 先把物体瞬间置到 startAngle
        SetXRotation(startAngle);
        // float currentAngle = startAngle;
        // float targetAngle = settleAngle;

        float elapsed = 0f;
        while (elapsed < duration){
            elapsed += Time.deltaTime;
            // 规范化进度
            float t = Mathf.Clamp01(elapsed / duration);

            // SmoothStep 对进度做缓入缓出，让初始加速度更小
            float tSmooth = t * t * (3f - 2f * t);

            // 再用弹性曲线
            float easedT = EaseOutElastic(tSmooth);

            // LerpUnclamped 产生超调
            float angle = LerpAngleUnclamped(startAngle, settleAngle, easedT);

            SetXRotation(angle);
            yield return null;
        }

        // 最后确保准确落到 settleAngle
        SetXRotation(settleAngle);
        // currentAngle = settleAngle;
        animationCoroutine = null;
    }

    public static float LerpAngleUnclamped(float a, float b, float t){
        float num = Mathf.Repeat(b - a, 360f);
        if ((double)num > 180.0)
            num -= 360f;
        return a + num * t;
    }

    /// <summary>
    /// 倒下动画（模拟重力加速度）
    /// </summary>
    public void FallDown(float acc = 5f){
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(FallDownRoutine(acc));
    }

    private IEnumerator FallDownRoutine(float acc){
        float startAngle = transform.localEulerAngles.x;
        float velocity = 0f;

        float t = 0;

        while (t < 1){
            velocity += acc * Time.deltaTime;
            t += velocity * Time.deltaTime;

            SetXRotation(Mathf.LerpAngle(startAngle, initialDegree, t));
            yield return null;
        }

        SetXRotation(initialDegree);
        animationCoroutine = null;
    }

    /// <summary>
    /// 仅设置局部 X 轴角度，保留 Y/Z
    /// </summary>
    private void SetXRotation(float x){
        Vector3 euler = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(x, euler.y, euler.z);
    }
}
