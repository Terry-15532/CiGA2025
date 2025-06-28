using System;
using UnityEngine;
using System.Collections;

public class BookElement : MonoBehaviour
{
    private float currentAngle = 0f; // 当前 X 轴角度
    private Coroutine animationCoroutine;
    private float targetAngle = 50f;

    void Start(){
        PopUp(startAngle:-90f, overshootAngle:-10f,settleAngle: -30f);
    }
    
    private float EaseOutElastic(float t)
    {
        float c4 = (2 * Mathf.PI) / 3;

        if (t == 0f) return 0f;
        if (t == 1f) return 1f;

        return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    }


    /// <summary>
    /// 从 prefab 创建 BookElement 并设置其父物体
    /// </summary>
    public static BookElement CreateFromPrefab(GameObject prefab, Transform parent)
    {
        GameObject instance = Instantiate(prefab, parent);
        BookElement element = instance.GetComponent<BookElement>();
        if (element == null)
        {
            element = instance.AddComponent<BookElement>();
        }
        return element;
    }

    /// <summary>
    /// 弹起动画（绕X轴，带过冲，保留其他旋转）
    /// </summary>
    public void PopUp(float duration = 0.5f, float startAngle = 0f ,float overshootAngle = 70f, float settleAngle = 50f)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(PopUpRoutine(duration, startAngle, overshootAngle, settleAngle));
    }

    private IEnumerator PopUpRoutine(float duration, float startAngle, float overshootAngle, float settleAngle)
    {
        float elapsed = 0f;
        currentAngle = 0f;
        targetAngle = settleAngle;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float easedT = EaseOutElastic(t);
            float angle = Mathf.LerpUnclamped(startAngle, settleAngle, easedT); // 注意：使用Unclamped允许超调

            SetXRotation(angle);
            yield return null;
        }

        SetXRotation(settleAngle);
        currentAngle = settleAngle;
        animationCoroutine = null;
    }


    /// <summary>
    /// 倒下动画（模拟重力加速度）
    /// </summary>
    public void FallDown(float gravity = 500f)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(FallDownRoutine(gravity));
    }

    private IEnumerator FallDownRoutine(float gravity)
    {
        float angle = currentAngle;
        float velocity = 0f;

        while (angle > 0f)
        {
            velocity += gravity * Time.deltaTime;
            angle -= velocity * Time.deltaTime;

            if (angle <= 0f)
            {
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
    private void SetXRotation(float x)
    {
        Vector3 euler = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(x, euler.y, euler.z);
    }
}
