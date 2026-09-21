using UnityEngine;
using TMPro;

public class HubCurrencyManager : MonoBehaviour
{
    public static HubCurrencyManager Instance;

    [Header("UI Tiền Tệ")]
    public TextMeshProUGUI gemsText;

    [Header("Cài đặt Game")]
    public int startingGems = 1000;
    public bool testMode_ForceReset = false;

    private int currentGems;

    // ĐƯA LOGIC LOAD TIỀN LÊN AWAKE ĐỂ NÓ CHẠY TRƯỚC TIÊN
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Load tiền ngay từ lúc game vừa thức dậy
        if (testMode_ForceReset)
        {
            currentGems = startingGems;
            PlayerPrefs.SetInt("HubGems", currentGems);
            PlayerPrefs.Save();
        }
        else
        {
            currentGems = PlayerPrefs.GetInt("HubGems", startingGems);
        }
    }

    void Start()
    {
        // Start chỉ làm nhiệm vụ hiển thị UI thôi
        UpdateUI();
    }

    public bool SpendGems(int amount)
    {
        if (currentGems >= amount)
        {
            currentGems -= amount;
            PlayerPrefs.SetInt("HubGems", currentGems);
            PlayerPrefs.Save();
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddGems(int amount)
    {
        currentGems += amount;
        PlayerPrefs.SetInt("HubGems", currentGems);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public int GetGems()
    {
        return currentGems;
    }

    public void UpdateUI()
    {
        if (gemsText != null)
        {
            gemsText.text = currentGems.ToString();
        }
    }
}