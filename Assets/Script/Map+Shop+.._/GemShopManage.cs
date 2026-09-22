using UnityEngine;
using UnityEngine.UI;
using System;

public class GemShopManage : MonoBehaviour
{
    [Header("Cài đặt phần thưởng")]
    public int gemRewardAmount = 20;
    public float cooldownSeconds = 60f; // Thời gian chờ (giây)

    [Header("UI Liên kết")]
    public Button freeButton;

    [Tooltip("Kéo cái màng đen mờ đè lên nút vào đây")]
    public Image cooldownOverlay;

    private string prefsKey = "NextFreeGemTime";
    private DateTime nextClaimTime;
    private bool isCooldownActive = false;

    void Start()
    {
        LoadCooldown();
    }

    void Update()
    {
        // BẤM PHÍM 'P' ĐỂ HỒI CHIÊU NGAY LẬP TỨC (TEST MỐDE)
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cheat_ResetCooldown();
        }

        if (isCooldownActive)
        {
            // Tính toán thời gian còn lại
            float timeLeft = (float)(nextClaimTime - DateTime.Now).TotalSeconds;

            if (timeLeft <= 0)
            {
                // Đã hồi xong
                isCooldownActive = false;
                freeButton.interactable = true;
                if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f; // Xóa sạch màng đen
            }
            else
            {
                // Đang hồi -> Tính % để giảm dần màng đen
                if (cooldownOverlay != null)
                {
                    cooldownOverlay.fillAmount = timeLeft / cooldownSeconds;
                }
            }
        }
    }

    // GẮN VÀO NÚT BẤM
    public void ClaimFreeGems()
    {
        if (isCooldownActive) return;

        // Cộng tiền
        if (HubCurrencyManager.Instance != null)
        {
            HubCurrencyManager.Instance.AddGems(gemRewardAmount);
        }

        // Kích hoạt Cooldown
        nextClaimTime = DateTime.Now.AddSeconds(cooldownSeconds);
        PlayerPrefs.SetString(prefsKey, nextClaimTime.ToBinary().ToString());
        PlayerPrefs.Save();

        isCooldownActive = true;
        freeButton.interactable = false;

        // Phủ màng đen lên 100%
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 1f;
    }

    // HÀM CHEAT 
    public void Cheat_ResetCooldown()
    {
        // Ép thời gian về quá khứ để bắt nó hoàn thành đếm ngược
        nextClaimTime = DateTime.Now.AddSeconds(-1);
        PlayerPrefs.SetString(prefsKey, nextClaimTime.ToBinary().ToString());
        PlayerPrefs.Save();

        isCooldownActive = false;
        freeButton.interactable = true;
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;

        Debug.Log("Đã dùng Hack: Hồi nút Free Gem ngay lập tức!");
    }

    private void LoadCooldown()
    {
        string savedTimeStr = PlayerPrefs.GetString(prefsKey, "");

        if (long.TryParse(savedTimeStr, out long temp))
        {
            nextClaimTime = DateTime.FromBinary(temp);

            float timeLeft = (float)(nextClaimTime - DateTime.Now).TotalSeconds;
            if (timeLeft > 0)
            {
                // Vẫn đang trong thời gian hồi lúc mở game
                isCooldownActive = true;
                freeButton.interactable = false;
                if (cooldownOverlay != null) cooldownOverlay.fillAmount = timeLeft / cooldownSeconds;
            }
            else
            {
                isCooldownActive = false;
                freeButton.interactable = true;
                if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            }
        }
        else
        {
            isCooldownActive = false;
            freeButton.interactable = true;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        }
    }
}