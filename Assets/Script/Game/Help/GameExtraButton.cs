using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameExtraButton : MonoBehaviour
{
    [Header("Cài đặt Nút")]
    public string itemID;
    public int maxQuantity = 5;

    [Header("Giao diện UI")]
    public TextMeshProUGUI amountText;
    public Button myButton;

    private int currentQuantity;

    void Start()
    {
        LoadData();
    }

    public void LoadData()
    {
        // 1. KIỂM TRA QUYỀN VÀO GAME
        int isEquipped = PlayerPrefs.GetInt("Equipped_" + itemID, 0);

        if (isEquipped == 0)
        {
            // NẾU KHÔNG ĐƯỢC CHỌN BÊN HUB -> ẨN LUÔN NÚT NÀY CHO GỌN UI
            gameObject.SetActive(false);
            return;
        }

        // 2. NẾU ĐƯỢC CHỌN -> HIỂN THỊ VÀ LẤY SỐ LƯỢNG
        gameObject.SetActive(true);
        currentQuantity = PlayerPrefs.GetInt("Extra_" + itemID, 0);
        UpdateUI();
    }

    public bool TryUseItem()
    {
        if (currentQuantity > 0)
        {
            currentQuantity--;
            PlayerPrefs.SetInt("Extra_" + itemID, currentQuantity);
            PlayerPrefs.Save();
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (amountText != null)
        {
            amountText.text = currentQuantity + "/" + maxQuantity;
        }

        if (myButton != null)
        {
            myButton.interactable = (currentQuantity > 0);
        }
    }
}