using UnityEngine;

public class SettingUI : MonoBehaviour
{
    [Header("Giao diện UI")]
    [Tooltip("Kéo Panel Setting từ Hierarchy thả vào đây")]
    public GameObject settingPanel;

    // Hàm gọi khi bấm nút Mở (VD: Nút bánh răng ngoài màn hình)
    public void OpenSetting()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
            // Time.timeScale = 0; // (Tùy chọn) Bỏ comment dòng này nếu muốn game tạm dừng khi mở setting
        }
    }

    // Hàm gọi khi bấm nút Đóng (VD: Nút X trong bảng setting)
    public void CloseSetting()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
            // Time.timeScale = 1; // (Tùy chọn) Bỏ comment dòng này để game chạy tiếp
        }
    }

    // Hàm bật/tắt luân phiên (Chỉ cần 1 nút dùng chung cho cả việc mở và đóng)
    public void ToggleSetting()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(!settingPanel.activeSelf);
        }
    }
}