using UnityEngine;
using UnityEngine.EventSystems;

public class CloseOnClickOutside : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("️ [除錯] 偵測到點擊事件！");

        // 確保點擊的確實是這個背景物件本身，而不是背景裡面的按鈕或子物件
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            Debug.Log("✅ [除錯] 確認點擊的是背景本身，準備關閉選單！");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CloseSettingsUI();
            }
            else
            {
                Debug.LogError("❌ [除錯] GameManager.Instance 是 null！請確認場景中有 GameManager 物件！");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ [除錯] 點擊的不是背景本身，而是其他物件：" + eventData.pointerCurrentRaycast.gameObject.name);
        }
    }
}