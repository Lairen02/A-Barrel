using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    // 單例模式，方便全域呼叫
    public static GameManager Instance { get; private set; }

    [Header("UI 參考")]
    public GameObject settingsPanel; // 拖入設定介面的根物件

    [Header("存檔關鍵字")]
    public string saveKey_TotalClicks = "TotalClicks";
    // public string saveKey_WoodCount = "WoodCount"; // 未來擴充用

    private void Awake()
    {
        // 確保全域只有一個 GameManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject); // 如果未來有多場景，可取消註解
    }

    private void Update()
    {
        // 監聽 ESC 鍵：優先關閉當前打開的 UI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettingsUI();
            }
            // 未來如果有 PausePanel，可以在這裡加 else if
        }
    }

    // 1. 開關設定介面
    public void ToggleSettingsUI()
    {
        if (settingsPanel == null) return;

        bool isActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isActive);

        if (isActive)
        {
            // 開啟 UI 時，自動清除其他 UI 的焦點，避免之前的按鈕還被選中
            ClearSelection();
            // Time.timeScale = 0f; // 如果需要暫停遊戲，可取消註解
        }
        else
        {
            // Time.timeScale = 1f; 
        }
    }

    // 專門用於「點擊空白處」或「ESC鍵」關閉
    public void CloseSettingsUI()
    {
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
            ClearSelection();
        }
    }

    // 清除 UI 焦點 (優化體驗的關鍵)
    private void ClearSelection()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // 2. 離開遊戲
    public void QuitGame()
    {
        SaveGame(); // 離開前自動存檔，確保數據不遺失
        Debug.Log("💾 遊戲已保存並準備離開...");

#if UNITY_EDITOR
        // 在 Unity 編輯器中停止播放
        EditorApplication.isPlaying = false;
#else
        // 在打包後的應用程式中真正退出
        Application.Quit();
#endif
    }

    // 3. 存檔功能 (集中管理)
    public void SaveGame()
    {
        // 從場景中的 BarrelClicker 獲取最新數據
        BarrelClicker barrelClicker = FindObjectOfType<BarrelClicker>();
        if (barrelClicker != null)
        {
            PlayerPrefs.SetInt(saveKey_TotalClicks, barrelClicker.totalClicks);
            // 未來如果有木材數量，可以在這裡一起存：
            // PlayerPrefs.SetInt(saveKey_WoodCount, barrelClicker.woodCount);
        }

        PlayerPrefs.Save();
        Debug.Log("✅ 遊戲已成功存檔！");
    }

    // 讀檔功能 (供遊戲開始時呼叫)
    public void LoadGame()
    {
        BarrelClicker barrelClicker = FindObjectOfType<BarrelClicker>();
        if (barrelClicker != null)
        {
            barrelClicker.totalClicks = PlayerPrefs.GetInt(saveKey_TotalClicks, 0);
            barrelClicker.UpdateClickUI(); // 呼叫 BarrelClicker 裡的公開方法更新畫面
        }
        Debug.Log("📂 遊戲讀檔完成。");
    }
}