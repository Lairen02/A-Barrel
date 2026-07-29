using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI 參考")]
    public GameObject settingsPanel; // 設定介面根物件

    [Header("提示 UI (尚未開發)")]
    public GameObject comingSoonPrefab; // ⚠️ 注意：這裡要拖入「Prefab」，不是場景物件
    public Transform uiRoot; // 拖入 Canvas，讓生成的 UI 掛在 Canvas 下面

    [Header("存檔關鍵字")]
    public string saveKey_TotalClicks = "TotalClicks";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettingsUI();
            }
        }
    }

    public void ToggleSettingsUI()
    {
        if (settingsPanel == null) return;
        bool isActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isActive);
        if (isActive) ClearSelection();
    }

    public void CloseSettingsUI()
    {
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
            ClearSelection();
        }
    }

    // ✨ 修改為：生成飄浮提示
    public void ShowComingSoonUI()
    {
        if (comingSoonPrefab == null)
        {
            Debug.LogWarning("⚠️ GameManager 的 comingSoonPrefab 還沒設定！");
            return;
        }

        // 決定生成位置（例如螢幕正中央，或是按鈕上方）
        Vector3 spawnPos = Vector3.zero;
        if (uiRoot != null)
        {
            // 如果是在 Canvas 下，我們可以用螢幕中心
            // 這裡簡單設為 Canvas 的中心
            spawnPos = uiRoot.position;
        }

        // 生成 Prefab
        GameObject instance = Instantiate(comingSoonPrefab, spawnPos, Quaternion.identity);

        // 確保它掛在 Canvas 下面（保持 UI 層級正確）
        if (uiRoot != null)
        {
            instance.transform.SetParent(uiRoot, false);
            // 重置本地座標，讓它置中
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    private void ClearSelection()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void QuitGame()
    {
        SaveGame();
        Debug.Log("💾 遊戲已保存並準備離開...");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SaveGame()
    {
        BarrelClicker barrelClicker = FindObjectOfType<BarrelClicker>();
        if (barrelClicker != null)
        {
            PlayerPrefs.SetInt(saveKey_TotalClicks, barrelClicker.totalClicks);
        }
        PlayerPrefs.Save();
        Debug.Log("✅ 遊戲已成功存檔！");
    }

    public void LoadGame()
    {
        BarrelClicker barrelClicker = FindObjectOfType<BarrelClicker>();
        if (barrelClicker != null)
        {
            barrelClicker.totalClicks = PlayerPrefs.GetInt(saveKey_TotalClicks, 0);
            barrelClicker.UpdateClickUI();
        }
        Debug.Log("📂 遊戲讀檔完成。");
    }
}