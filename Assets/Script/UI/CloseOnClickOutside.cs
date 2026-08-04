using UnityEngine;
using UnityEngine.EventSystems;

public class CloseUIPanelOnClick : MonoBehaviour, IPointerClickHandler
{
    [Header("設定")]
    public string panelToClose = "Settings"; // 輸入 "Settings" 或 "Store"

    public void OnPointerClick(PointerEventData eventData)
    {
        // 確保點擊的是背景本身，不是子物件
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance 是 null！");
            return;
        }

        // 根據設定關閉不同的 UI
        if (panelToClose == "Settings")
        {
            GameManager.Instance.CloseSettingsUI();
        }
        else if (panelToClose == "Store")
        {
            GameManager.Instance.CloseStoreUI();
        }
        else
        {
            Debug.LogWarning($"⚠️ 未知的 panelToClose: {panelToClose}");
        }
    }
}