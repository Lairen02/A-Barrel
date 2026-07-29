using UnityEngine;
using System.Collections;

public class FloatingWood : MonoBehaviour
{
    [Header("漂浮設定")]
    public float floatDuration = 1.0f;  // 漂浮持續時間 (秒)
    public float floatSpeed = 3.0f;     // 往上飄的速度
    public float spreadAmount = 1.5f;   // 左右隨機散開的範圍

    private SpriteRenderer sr;
    private Vector3 moveDirection;
    private Color startColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;

        // 隨機決定往上並帶有左右偏移的方向
        float randomX = Random.Range(-spreadAmount, spreadAmount);
        moveDirection = new Vector3(randomX, 1f, 0f).normalized;

        StartCoroutine(FadeAndMove());
    }

    IEnumerator FadeAndMove()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / floatDuration;

            // 1. 移動：沿著隨機方向往上飄
            transform.position = startPos + moveDirection * floatSpeed * elapsed;

            // 2. 淡出：使用平方曲線讓結尾的消失更自然
            float alpha = 1f - (t * t);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // 時間到，自動銷毀自身以節省記憶體與效能
        Destroy(gameObject);
    }
}