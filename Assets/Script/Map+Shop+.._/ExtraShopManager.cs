using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ExtraItem
{
    public string itemID;      // Quan trọng: Phải khớp với tên gọi bên Scene Game (VD: "MeleeCat", "Bomb")
    public int price = 25;     // Giá mua (VD: 25 Gems)
    public int maxQuantity = 5;// Số lượng tối đa (VD: 5)

    [Header("UI Bên Phải (Cửa Hàng)")]
    public Button buyButton;            // Nút bấm mua (có hình kim cương)
    public TextMeshProUGUI rightAmount; // Text "3/5" bên phải
    public GameObject selectedBorder;   // Cái viền trắng bật lên khi được gán vào slot trái

    [Header("UI Bên Trái (Slot Trang Bị)")]
    public GameObject leftSlotRoot;     // Toàn bộ Object của slot bên trái (để ẩn nếu chưa có)
    public TextMeshProUGUI leftAmount;  // Text "3/5" bên trái

    [HideInInspector] public int currentQuantity;
    [HideInInspector] public bool isEquipped; // Biến kiểm tra xem có đang nằm trong 4 slot trái không
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
            // Load số lượng người chơi đang có, key lưu trữ là "Extra_" + itemID
            item.currentQuantity = PlayerPrefs.GetInt("Extra_" + item.itemID, 0);

            // Tạm thời logic: Nếu có gắn UI bên trái (leftSlotRoot != null) thì mặc định là đang được Equip
            item.isEquipped = (item.leftSlotRoot != null);
        }
    }

    // Hàm gắn vào sự kiện OnClick của các nút Buy bên phải
    public void BuyItem(int index)
    {
        ExtraItem item = extraItems[index];

        // 1. Kiểm tra giới hạn số lượng
        if (item.currentQuantity >= item.maxQuantity) return;

        // 2. Kiểm tra tiền và thanh toán
        if (HubCurrencyManager.Instance.SpendGems(item.price))
        {
            item.currentQuantity++;

            // 3. Lưu dữ liệu để Scene Game có thể gọi ra dùng
            PlayerPrefs.SetInt("Extra_" + item.itemID, item.currentQuantity);
            PlayerPrefs.Save();

            UpdateAllUI();
        }
    }

    // Cập nhật toàn bộ giao diện, chặn thao tác nếu hết tiền/full đồ
    public void UpdateAllUI()
    {
        int currentGems = HubCurrencyManager.Instance.GetGems();

        foreach (var item in extraItems)
        {
            string amountString = item.currentQuantity + "/" + item.maxQuantity;

            // Cập nhật Text
            if (item.rightAmount != null) item.rightAmount.text = amountString;
            if (item.leftAmount != null) item.leftAmount.text = amountString;

            // Xử lý nút mua (Interactable)
            if (item.buyButton != null)
            {
                if (item.currentQuantity >= item.maxQuantity || currentGems < item.price)
                {
                    item.buyButton.interactable = false; // Mờ đi không cho bấm
                }
                else
                {
                    item.buyButton.interactable = true;
                }
            }

            // Xử lý Border bên phải và trạng thái hiển thị bên trái
            if (item.isEquipped)
            {
                if (item.selectedBorder != null) item.selectedBorder.SetActive(true);
                if (item.leftSlotRoot != null) item.leftSlotRoot.SetActive(true);
            }
            else
            {
                if (item.selectedBorder != null) item.selectedBorder.SetActive(false);
                // Nếu muốn slot trái hiện ổ khóa thay vì ẩn đi, bạn có thể custom đoạn này
                if (item.leftSlotRoot != null) item.leftSlotRoot.SetActive(false);
            }
        }
    }

    // Gọi hàm này mỗi khi HubCurrencyManager trừ tiền thành công để nó check lại nút Buy
    void Update()
    {
        // (Tùy chọn) Có thể tối ưu hơn bằng C# Events, nhưng để đơn giản 
        // bạn chỉ cần gọi UpdateAllUI() từ HubCurrencyManager sau khi update tiền là được.
    }
}