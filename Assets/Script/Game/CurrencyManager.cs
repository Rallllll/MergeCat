using UnityEngine;
using TMPro; // Bắt buộc dùng TextMeshPro cho chữ sắc nét

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("Giao diện UI")]
    public TextMeshProUGUI goldText;

    private int currentGold = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Tự động load tiền từ máy người chơi khi vừa vào Scene.
        // Chữ "PlayerGold" là chìa khóa (key) để tìm file save, 0 là số tiền mặc định.
        currentGold = PlayerPrefs.GetInt("PlayerGold", 0);
        UpdateGoldUI();
    }

    // Hàm gọi khi giết quái
    public void AddGold(int amount)
    {
        currentGold += amount;

        // Lưu thẳng xuống máy tính/điện thoại ngay lập tức
        PlayerPrefs.SetInt("PlayerGold", currentGold);
        PlayerPrefs.Save();

        UpdateGoldUI();
    }

    // Hàm gọi khi mua đồ trong Shop
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            PlayerPrefs.SetInt("PlayerGold", currentGold);
            PlayerPrefs.Save();
            UpdateGoldUI();
            return true; // Báo hiệu trừ tiền thành công để nhả item
        }
        return false; // Trả về false nếu nghèo không đủ tiền mua
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = currentGold.ToString();
        }
    }
}