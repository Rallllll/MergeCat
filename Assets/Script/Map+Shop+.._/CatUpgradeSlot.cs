using UnityEngine;
using UnityEngine.UI;

public class CatUpgradeSlot : MonoBehaviour
{
    public string catID = "Cat_01";

    [Header("Giao diện UI")]
    public Button upgradeButton; // Nút dấu +
    public Image progressBarImage; // Cái object hiển thị ảnh thanh tiến trình

    [Tooltip("Kéo 6 cái ảnh thanh tiến trình vào đây (Từ lúc 0 vạch đến 5 vạch)")]
    public Sprite[] progressSprites;

    void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }
        RefreshUI();
    }

    void OnUpgradeClicked()
    {
        int currentClicks = PlayerPrefs.GetInt("UpgradeCount_" + catID, 0);

        if (currentClicks < 5)
        {
            currentClicks++;
            PlayerPrefs.SetInt("UpgradeCount_" + catID, currentClicks);
            PlayerPrefs.Save();

            RefreshUI();
        }
    }

    void RefreshUI()
    {
        int currentClicks = PlayerPrefs.GetInt("UpgradeCount_" + catID, 0);

        // ĐỔI ẢNH THANH TIẾN TRÌNH THEO SỐ LẦN BẤM
        if (progressBarImage != null && progressSprites != null && progressSprites.Length > 0)
        {
            int index = Mathf.Clamp(currentClicks, 0, progressSprites.Length - 1);
            progressBarImage.sprite = progressSprites[index];
        }

        // Đầy 5 vạch thì khoá nút
        if (currentClicks >= 5 && upgradeButton != null)
        {
            upgradeButton.interactable = false;
        }
    }
}