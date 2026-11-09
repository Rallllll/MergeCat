using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingUI : MonoBehaviour
{
    [Header("Giao diện UI")]
    [Tooltip("Kéo Panel Setting từ Hierarchy thả vào đây")]
    public GameObject settingPanel;

    [Header("Cài đặt Nút Âm Thanh (Tùy chọn)")]
    public Image musicButtonImage;
    public Image soundButtonImage;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    void Start()
    {
        UpdateAudioIcons();
    }

    // Hàm gọi khi bấm nút Mở (VD: Nút bánh răng ngoài màn hình)
    public void OpenSetting()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
            UpdateAudioIcons();
        }
    }

    // Hàm gọi khi bấm nút Đóng (VD: Nút X trong bảng setting)
    public void CloseSetting()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    // Hàm bật/tắt luân phiên (Chỉ cần 1 nút dùng chung cho cả việc mở và đóng)
    public void ToggleSetting()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        if (settingPanel != null)
        {
            settingPanel.SetActive(!settingPanel.activeSelf);
            if (settingPanel.activeSelf) UpdateAudioIcons();
        }
    }

    // Gắn vào nút Bật/Tắt Nhạc Nền (Music)
    public void ToggleMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMusic();
            AudioManager.Instance.PlayClick();
            UpdateAudioIcons();
        }
    }

    // Gắn vào nút Bật/Tắt Hiệu Ứng Âm Thanh (Sound)
    public void ToggleSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleSound();
            AudioManager.Instance.PlayClick();
            UpdateAudioIcons();
        }
    }

    public void UpdateAudioIcons()
    {
        if (AudioManager.Instance == null) return;

        bool isMusic = AudioManager.Instance.IsMusicEnabled();
        if (musicButtonImage != null)
        {
            if (isMusic && musicOnSprite != null) musicButtonImage.sprite = musicOnSprite;
            else if (!isMusic && musicOffSprite != null) musicButtonImage.sprite = musicOffSprite;
        }

        bool isSound = AudioManager.Instance.IsSoundEnabled();
        if (soundButtonImage != null)
        {
            if (isSound && soundOnSprite != null) soundButtonImage.sprite = soundOnSprite;
            else if (!isSound && soundOffSprite != null) soundButtonImage.sprite = soundOffSprite;
        }
    }

    // --- HÀM MỚI CHO NÚT RESTART ---
    public void RestartGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
