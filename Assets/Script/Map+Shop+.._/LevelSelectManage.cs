using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManage : MonoBehaviour
{
    // Biến toàn cục để WaveManager ở Scene Game đọc xem người chơi vừa chọn Level mấy
    public static int selectedLevelID = 1;

    [Header("Cài đặt Chuyển Cảnh")]
    [Tooltip("Điền ID của Scene Game trong Build Settings (Ví dụ: 2)")]
    public int gameSceneIndex = 2;

    // Hàm này sẽ được gọi khi m bấm vào các nút chọn Level
    // Tham số 'levelID' m điền ngoài Inspector tương ứng với từng nút (1, 2, 3...)
    public void LoadLevel(int levelID)
    {
        // 1. Lưu lại ID Level người chơi vừa chọn
        selectedLevelID = levelID;

        // 2. Kiểm tra và chạy Loading sang Scene Game
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadScene(gameSceneIndex);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy LoadingManager (Chắc do test thẳng từ Hub). Sẽ load thẳng vào Game!");
            SceneManager.LoadScene(gameSceneIndex);
        }
    }
}