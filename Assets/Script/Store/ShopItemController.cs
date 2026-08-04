using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemController : MonoBehaviour
{
    [Header("商品設定")]
    [Tooltip("在這裡直接輸入價格，遊戲開始時會自動顯示在 UI 上！")]
    public int price = 100;

    [Tooltip("用於判斷要給什麼功能 (例如: AutoClicker, BetterBarrel)")]
    public string itemName = "AutoClicker";

    [Header("UI 參考")]
    public Button buyButton;

    [Tooltip("拖入顯示價格的 TMP 文字元件")]
    public TMP_Text priceText;

    public TMP_Text woodCountText; // 商店頂部顯示總木材的文字

    private BarrelClicker barrelClicker;

    void Start()
    {
        if (buyButton == null) buyButton = GetComponent<Button>();
        barrelClicker = FindObjectOfType<BarrelClicker>();

        // ✨ 自動將 Inspector 設定的價格顯示在 UI 上
        if (priceText != null)
        {
            priceText.text = $"🪵 {price}";
        }

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    void OnBuyClicked()
    {
        if (barrelClicker == null) return;

        if (barrelClicker.totalClicks >= price)
        {
            // 1. 扣除木材
            barrelClicker.totalClicks -= price;

            // 2. 更新 UI
            barrelClicker.UpdateClickUI();
            if (woodCountText != null)
            {
                woodCountText.text = barrelClicker.totalClicks.ToString();
            }

            // 3. 執行對應功能
            ApplyItemEffect();

            // 4. 按鈕變暗並鎖定 (無法再次購買)
            if (buyButton != null)
            {
                buyButton.interactable = false;

                // 改變按鈕文字提示已購買
                TMP_Text btnText = buyButton.GetComponentInChildren<TMP_Text>();
                if (btnText != null) btnText.text = "已擁有";

                // 讓按鈕顏色變得更暗
                ColorBlock colors = buyButton.colors;
                colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                buyButton.colors = colors;
            }

            Debug.Log($"✅ 成功購買 {itemName}！剩餘木材: {barrelClicker.totalClicks}");
        }
        else
        {
            Debug.LogWarning($"❌ 木材不足！需要 {price}，目前只有 {barrelClicker.totalClicks}");
        }
    }

    void ApplyItemEffect()
    {
        if (itemName == "AutoClicker")
        {
            // 每次購買增加 1 次/秒 的自動點擊
            barrelClicker.UpgradeAutoClicker(1f);
        }
        else if (itemName == "BetterBarrel")
        {
            // 範例：木桶升級 (增加各階段門檻 20 次)
            barrelClicker.damageThresholds[0] += 20;
            barrelClicker.damageThresholds[1] += 20;
            barrelClicker.damageThresholds[2] += 20;
            Debug.Log("🛡️ 木桶升級！耐久度提升！");
        }
    }
}