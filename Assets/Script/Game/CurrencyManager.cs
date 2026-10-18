using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("Giao diện UI")]
    public TextMeshProUGUI goldText;

    [Header("Test Hack Tiền")]
    public int testGoldAmount = 10000; // Nhập số tiền m muốn hack vào đây ngoài Inspector

    private int currentGold = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentGold = PlayerPrefs.GetInt("PlayerGold", 0);
        UpdateGoldUI();
    }

    void Update()
    {
        // Nhấn phím G để hack thêm tiền ngay trong lúc chơi
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddTestGold();
        }
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        PlayerPrefs.SetInt("PlayerGold", currentGold);
        PlayerPrefs.Save();
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            PlayerPrefs.SetInt("PlayerGold", currentGold);
            PlayerPrefs.Save();
            UpdateGoldUI();
            return true;
        }
        return false;
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = currentGold.ToString();
        }
    }

    // Cái này tạo ra 1 nút bấm ẩn. M nhấp chuột phải vào tên script "Currency Manager" ngoài Inspector sẽ thấy chữ "Bơm Tiền Test"
    [ContextMenu("Bơm Tiền Test")]
    public void AddTestGold()
    {
        AddGold(testGoldAmount);
        Debug.Log($"[Test] Đã hack thêm {testGoldAmount} vàng! Tổng tiền hiện tại: {currentGold}");
    }
}