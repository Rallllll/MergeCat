using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ExtraItem
{
    public string itemID;
    public int price = 25;
    public int maxQuantity = 5;

    [Header("UI Bên Phải (Cửa Hàng)")]
    public Button buyButton;
    public TextMeshProUGUI rightAmount;
    public GameObject selectedBorder;

    [Header("UI Bên Trái (Slot Trang Bị)")]
    public GameObject leftSlotRoot;
    public TextMeshProUGUI leftAmount;

    [HideInInspector] public int currentQuantity;
    [HideInInspector] public bool isEquipped;
}

public class ExtraShopManager : MonoBehaviour
{
    public List<ExtraItem> extraItems;

    void Start()
    {
        LoadData();
        UpdateAllUI();
    }

    void LoadData()
    {
        foreach (var item in extraItems)
        {
            // Load số lượng
            item.currentQuantity = PlayerPrefs.GetInt("Extra_" + item.itemID, 0);

            // Load trạng thái xem có đang được chọn mang vào game không (1 là có, 0 là không)
            item.isEquipped = PlayerPrefs.GetInt("Equipped_" + item.itemID, 0) == 1;
        }
    }

    // --- HÀM MỚI: GẮN VÀO ONCLICK CỦA 4 CÁI NÚT BÊN TRÁI ---
    public void ToggleEquipItem(int index)
    {
        ExtraItem item = extraItems[index];

        // Nếu trong kho không có cái nào (chưa mua) thì cấm mang vào game
        if (item.currentQuantity <= 0)
        {
            Debug.Log("Bạn chưa mua " + item.itemID + ", không thể mang vào game!");
            return;
        }

        if (item.isEquipped)
        {
            // Đang chọn -> Bấm phát nữa để BỎ CHỌN
            item.isEquipped = false;
            PlayerPrefs.SetInt("Equipped_" + item.itemID, 0);
        }
        else
        {
            // Chưa chọn -> Kiểm tra xem đã mang đủ 3 món chưa
            int equipCount = 0;
            foreach (var i in extraItems)
            {
                if (i.isEquipped) equipCount++;
            }

            if (equipCount >= 3)
            {
                Debug.Log("Chỉ được mang tối đa 3 món phụ trợ vào trận!");
                return; // Chặn lại, không cho chọn thêm
            }

            // Hợp lệ -> ĐÁNH DẤU CHỌN
            item.isEquipped = true;
            PlayerPrefs.SetInt("Equipped_" + item.itemID, 1);
        }

        PlayerPrefs.Save();
        UpdateAllUI();
    }

    public void BuyItem(int index)
    {
        ExtraItem item = extraItems[index];

        if (item.currentQuantity >= item.maxQuantity) return;

        if (HubCurrencyManager.Instance.SpendGems(item.price))
        {
            item.currentQuantity++;
            PlayerPrefs.SetInt("Extra_" + item.itemID, item.currentQuantity);
            PlayerPrefs.Save();
            UpdateAllUI();
        }
    }

    public void UpdateAllUI()
    {
        int currentGems = HubCurrencyManager.Instance.GetGems();

        foreach (var item in extraItems)
        {
            string amountString = item.currentQuantity + "/" + item.maxQuantity;

            if (item.rightAmount != null) item.rightAmount.text = amountString;
            if (item.leftAmount != null) item.leftAmount.text = amountString;

            if (item.buyButton != null)
            {
                item.buyButton.interactable = !(item.currentQuantity >= item.maxQuantity || currentGems < item.price);
            }

            // Hiệu ứng viền trắng sẽ BẬT khi món đó ĐƯỢC CHỌN MANG VÀO GAME
            if (item.selectedBorder != null)
            {
                item.selectedBorder.SetActive(item.isEquipped);
            }
        }
    }
}