using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameExtraButton : MonoBehaviour
{
    [Header("Cài đặt Nút")]
    public string itemID; // QUAN TRỌNG: Phải gõ chữ giống HỆT bên Main Hub (VD: "MeleeCat", "Bomb")
    public int maxQuantity = 5;

    [Header("Giao diện UI")]
    public TextMeshProUGUI amountText; // Kéo Text hiển thị số lượng của nút này vào đây
    public Button myButton; // Kéo component Button (hoặc component Drag của m) vào để khóa lại nếu hết

    private int currentQuantity;

    void Start()
    {
        LoadData();
    }

    public void LoadData()
    {
        // Lấy số lượng vừa mua từ Main Hub sang
        currentQuantity = PlayerPrefs.GetInt("Extra_" + itemID, 0);
        UpdateUI();
    }

    // HÀM NÀY SẼ ĐƯỢC GỌI KHI M THẢ QUẢ BOM HOẶC LÍNH XUỐNG MAP THÀNH CÔNG
    public bool TryUseItem()
    {
        if (currentQuantity > 0)
        {
            currentQuantity--; // Dùng 1 cái thì trừ 1

            // Lưu lại luôn để nếu quay về Main Hub thì số lượng được đồng bộ giảm xuống
            PlayerPrefs.SetInt("Extra_" + itemID, currentQuantity);
            PlayerPrefs.Save();

            UpdateUI();
            return true; // Báo về là: CÒN HÀNG, CHO PHÉP THẢ!
        }

        return false; // Hết sạch rồi, cấm thả!
    }

    void UpdateUI()
    {
        if (amountText != null)
        {
            amountText.text = currentQuantity + "/" + maxQuantity;
        }

        // Tắt khả năng bấm/kéo nếu hết hàng
        if (myButton != null)
        {
            myButton.interactable = (currentQuantity > 0);
        }
    }
}