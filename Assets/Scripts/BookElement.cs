using System;
using UnityEngine;
using System.Collections;

public class BookElement : MonoBehaviour {
    private float currentAngle = 0f; // 当前 X 轴角度
    private Coroutine animationCoroutine;
    private float targetAngle = 50f;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            PopUp(startAngle: -90f, settleAngle: -30f);
    }

    private float EaseOutElastic(float t) {
        float c4 = (2 * Mathf.PI) / 3;
        if (t == 0f) return 0f;
        if (t == 1f) return 1f;
        return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    }

    /// <summary>
    /// 弹起动画（绕X轴，带弹性插值，保留其他旋转）
    /// </summary>
    public void PopUp(float duration = 1f, float startAngle = 0f, float settleAngle = 50f) {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(PopUpRoutine(duration, startAngle, settleAngle));
    }

    private IEnumerator PopUpRoutine(float duration, float startAngle, float settleAngle)
    {
        // 1. 先把物体瞬间置到 startAngle
        SetXRotation(startAngle);
        currentAngle = startAngle;
        targetAngle = settleAngle;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 规范化进度
            float t = Mathf.Clamp01(elapsed / duration);

            // SmoothStep 对进度做缓入缓出，让初始加速度更小
            float tSmooth = t * t * (3f - 2f * t);

            // 再用弹性曲线
            float easedT = EaseOutElastic(tSmooth);

            // LerpUnclamped 产生超调
            float angle = Mathf.LerpUnclamped(startAngle, settleAngle, easedT);

            SetXRotation(angle);
            yield return null;
        }

        // 最后确保准确落到 settleAngle
        SetXRotation(settleAngle);
        currentAngle = settleAngle;
        animationCoroutine = null;
    }


    /// <summary>
    /// 倒下动画（模拟重力加速度）
    /// </summary>
    public void FallDown(float gravity = 500f) {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(FallDownRoutine(gravity));
    }

    private IEnumerator FallDownRoutine(float gravity) {
        float angle = currentAngle;
        float velocity = 0f;

        while (angle > 0f) {
            velocity += gravity * Time.deltaTime;
            angle -= velocity * Time.deltaTime;

            if (angle <= 0f) {
                angle = 0f;
                break;
            }

            SetXRotation(angle);
            yield return null;
        }

        SetXRotation(0f);
        currentAngle = 0f;
        animationCoroutine = null;
    }

    /// <summary>
    /// 仅设置局部 X 轴角度，保留 Y/Z
    /// </summary>
    private void SetXRotation(float x) {
        Vector3 euler = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(x, euler.y, euler.z);
    }
}
