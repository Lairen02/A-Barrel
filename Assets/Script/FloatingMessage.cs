using UnityEngine;
using System.Collections;

public class FloatingMessage : MonoBehaviour
{
    [Header("動畫設定")]
    public float floatSpeed = 50f;      // 往上飄的速度
    public float fadeDuration = 1.5f;   // 淡出所需的時間
    public float startDelay = 0.5f;     // 飄出前停留的時間

    private CanvasGroup canvasGroup;
    private Vector3 startPosition;

    void Awake()
    {
        // 獲取 CanvasGroup 元件（用來控制整體透明度）
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // 如果沒有 CanvasGroup，嘗試加一個
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        startPosition = transform.position;
    }

    void Start()
    {
        StartCoroutine(FloatAndFade());
    }

    IEnumerator FloatAndFade()
    {
        // 1. 先停留一下 (startDelay)
        yield return new WaitForSeconds(startDelay);

        float elapsed = 0f;

        // 2. 開始往上飄並淡出
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // 往上移動
            transform.position = startPosition + Vector3.up * floatSpeed * elapsed;

            // 慢慢淡出 (Alpha 從 1 變 0)
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            }

            yield return null;
        }

        // 3. 動畫結束，銷毀自己
        Destroy(gameObject);
    }
}