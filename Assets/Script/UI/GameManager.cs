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
    public GameObject storePanel;    // ✨ 新增：商店介面根物件

    [Header("提示 UI (尚未開發)")]
    public GameObject comingSoonPrefab; // 注意：這裡要拖入「Prefab」，不是場景物件
    public Transform uiRoot;            // 拖入 Canvas，讓生成的 UI 掛在 Canvas 下面

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
        // 監聽 ESC 鍵：優先關閉當前打開的 UI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 1. 優先檢查商店介面
            if (storePanel != null && storePanel.activeSelf)
            {
                CloseStoreUI();
            }
            // 2. 其次檢查設定介面
            else if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettingsUI();
            }
        }
    }

    // ================= 設定介面邏輯 =================

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

    // ================= 商店介面邏輯 =================

    public void ToggleStoreUI()
    {
        if (storePanel == null) return;
        bool isActive = !storePanel.activeSelf;
        storePanel.SetActive(isActive);
        if (isActive) ClearSelection();
    }

    public void CloseStoreUI()
    {
        if (storePanel != null && storePanel.activeSelf)
        {
            storePanel.SetActive(false);
            ClearSelection();
        }
    }

    // ================= 提示 UI 邏輯 =================

    public void ShowComingSoonUI()
    {
        if (comingSoonPrefab == null)
        {
            Debug.LogWarning("⚠️ GameManager 的 comingSoonPrefab 還沒設定！");
            return;
        }

        Vector3 spawnPos = Vector3.zero;
        if (uiRoot != null)
        {
            spawnPos = uiRoot.position;
        }

        GameObject instance = Instantiate(comingSoonPrefab, spawnPos, Quaternion.identity);

        if (uiRoot != null)
        {
            instance.transform.SetParent(uiRoot, false);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    // ================= 通用輔助方法 =================

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