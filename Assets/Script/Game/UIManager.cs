using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải có để chuyển Scene / Load lại game

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Các Bảng Giao Diện (Panels)")]
    public GameObject winPanel;
    public GameObject losePanel;

    void Awake()
    {
        // Khởi tạo Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Đảm bảo lúc mới vào game thì 2 bảng này phải tắt đi
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        Time.timeScale = 1f; // Đảm bảo thời gian chạy bình thường
    }

    // --- CÁC HÀM NÀY ĐỂ GỌI TỪ SCRIPT KHÁC (như BaseHealth, Enemy...) ---

    public void ShowLosePanel()
    {
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f; // Dừng mọi hoạt động của game (quái ngừng đi, lính ngừng đánh)
    }

    public void ShowWinPanel()
    {
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- CÁC HÀM NÀY ĐỂ GẮN VÀO NÚT BẤM (ON CLICK) ---

    // Gắn vào nút "Chơi lại" (Retry)
    public void Button_Retry()
    {
        Time.timeScale = 1f; // Trả lại thời gian trước khi load scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Load lại màn hiện tại
    }

    // Gắn vào nút "Về Shop" hoặc "Home"
    public void Button_ChangeScene(string sceneName)
    {
        Time.timeScale = 1f; // Nhớ phải có dòng này để nhả thời gian ra nếu đang ở màn hình Thua/Thắng
        SceneManager.LoadScene(sceneName);
    }
}