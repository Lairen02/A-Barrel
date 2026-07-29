using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class BarrelClicker : MonoBehaviour, IPointerClickHandler
{
    [Header("木桶圖片 (依序: 完整, 裂開, 散架)")]
    public Sprite[] sprites;
    private SpriteRenderer sr;

    [Header("後台數值控制 (未來商店升級用)")]
    [Tooltip("三個階段的點擊閾值 (例如: 30次裂開, 65次散架, 80次觸發修復)")]
    public int[] damageThresholds = new int[] { 30, 65, 80 };

    [Tooltip("木桶修復所需的時間 (秒)")]
    public float repairTime = 5f;

    [Header("修復條 UI")]
    public Slider repairSlider;       // 拖入修復條的 Slider 元件
    public GameObject repairTextObj;  // 拖入「木桶修復中」的文字物件
    private bool isRepairing = false; // 是否正在修復中

    [Header("動畫控制")]
    [Tooltip("控制隨機搖晃與縮放幅度的 Float 值 (建議 0.05 ~ 0.2)")]
    public float animIntensity = 0.15f;

    [Header("UI 元件")]
    public TMP_Text clickCountText;   // 顯示總點擊次數

    [Header("音效")]
    public AudioSource clickSound;

    [Header("特效設定")]
    [Tooltip("點擊時噴出的木材圖示 Prefab")]
    public GameObject woodParticlePrefab;

    // 內部變數
    private int currentClicks = 0;
    public int totalClicks = 0;       // 改為 public，方便 GameManager 讀取
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Coroutine animCoroutine;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sprites != null && sprites.Length > 0)
        {
            sr.sprite = sprites[0];
        }

        originalScale = transform.localScale;
        originalRotation = transform.rotation;

        // 初始化時隱藏修復條與文字
        if (repairSlider != null)
        {
            repairSlider.gameObject.SetActive(false);
            repairSlider.value = 0f;
        }
        if (repairTextObj != null)
            repairTextObj.SetActive(false);

        // 遊戲開始時，嘗試從 GameManager 讀取存檔 (如果有的話)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame();
        }
        else
        {
            UpdateClickUI(); // 如果 GameManager 還沒初始化，至少更新一次 UI
        }
    }

    // 方式 A: IPointerClickHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        HandleClick();
    }

    // 方式 B: OnMouseDown (雙重保險)
    void OnMouseDown()
    {
        //  新增：如果滑鼠點擊的位置有 UI 物件（例如設定選單），就不處理木桶點擊
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; // 直接結束，不執行下面的 HandleClick()
        }

        HandleClick();
    }

    // 統一的點擊邏輯
    void HandleClick()
    {
        if (isRepairing) return;

        if (clickSound != null)
            clickSound.Play();

        currentClicks++;
        totalClicks++;

        // ✨ 新增：生成噴出的木材特效
        SpawnFloatingWood();

        // 檢查是否達到第三階段（觸發修復的門檻）
        if (damageThresholds.Length >= 3 && currentClicks >= damageThresholds[2])
        {
            if (sprites != null && sprites.Length > 2)
                sr.sprite = sprites[2]; // 確保顯示散架圖

            StartCoroutine(StartRepairProcess());
            return;
        }

        UpdateBarrelSprite();
        UpdateClickUI();
        PlayClickAnimation();
    }

    // ✨ 新增：生成漂浮木材的方法
    void SpawnFloatingWood()
    {
        if (woodParticlePrefab != null)
        {
            // 在木桶中心位置生成，並帶點微小的隨機偏移，讓連續點擊時木材不會完全重疊
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0f);
            Instantiate(woodParticlePrefab, spawnPos, Quaternion.identity);
        }
    }

    // 修復流程
    IEnumerator StartRepairProcess()
    {
        isRepairing = true;
        UpdateClickUI();

        if (repairSlider != null)
        {
            repairSlider.gameObject.SetActive(true);
            repairSlider.value = 1f;
        }
        if (repairTextObj != null)
            repairTextObj.SetActive(true);

        float timeLeft = repairTime;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            if (repairSlider != null)
            {
                repairSlider.value = Mathf.Clamp01(timeLeft / repairTime);
            }
            yield return null;
        }

        if (repairSlider != null)
            repairSlider.gameObject.SetActive(false);
        if (repairTextObj != null)
            repairTextObj.SetActive(false);

        // 重置狀態
        currentClicks = 0;
        isRepairing = false;

        if (sprites != null && sprites.Length > 0)
            sr.sprite = sprites[0];

        transform.localScale = originalScale;
        transform.rotation = originalRotation;
    }

    // 根據三個閾值切換圖片
    void UpdateBarrelSprite()
    {
        if (sprites == null || sprites.Length < 3 || damageThresholds.Length < 2) return;

        if (currentClicks < damageThresholds[0])
        {
            sr.sprite = sprites[0]; // 完整
        }
        else if (currentClicks < damageThresholds[1])
        {
            sr.sprite = sprites[1]; // 裂開
        }
        else
        {
            sr.sprite = sprites[2]; // 散架
        }
    }

    // ⚠️ 修正處：加上 public，讓 GameManager 可以存取
    public void UpdateClickUI()
    {
        if (clickCountText != null)
        {
            clickCountText.text = totalClicks.ToString();
        }
    }

    void PlayClickAnimation()
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        animCoroutine = StartCoroutine(DoClickAnimation());
    }

    // 隨機化的點擊動畫
    IEnumerator DoClickAnimation()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        float randomScaleFactor = 1f + Random.Range(-animIntensity, animIntensity);
        Vector3 targetScale = originalScale * randomScaleFactor;

        float randomDirection = Random.Range(0f, 1f) < 0.5f ? -1f : 1f;
        float maxShakeAngle = animIntensity * 100f;
        float targetRotationAngle = Random.Range(5f, maxShakeAngle) * randomDirection;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = t * t * (3f - 2f * t);

            transform.localScale = Vector3.Lerp(targetScale, originalScale, smoothT);
            float currentAngle = Mathf.Lerp(targetRotationAngle, 0f, smoothT);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            yield return null;
        }

        transform.localScale = originalScale;
        transform.rotation = originalRotation;
    }
}