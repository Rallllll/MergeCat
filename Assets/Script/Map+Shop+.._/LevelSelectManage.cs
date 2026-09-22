using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManage : MonoBehaviour
{
    // Hàm này sẽ được gọi khi m bấm vào các nút chọn Level
    // Tham số 'sceneIndex' m có thể điền thẳng ở ngoài Inspector của từng nút
    public void LoadLevel(int sceneIndex)
    {
        // Kiểm tra xem hệ thống Loading từ màn hình Start có bay sang đây không
        if (LoadingManager.Instance != null)
        {
            // Nếu có thì gọi bảng Loading lên che màn hình và chạy thanh Slider
            LoadingManager.Instance.LoadScene(sceneIndex);
        }
        else
        {
            // Dành cho lúc m test game thẳng từ Scene Hub trong Unity Editor
            Debug.LogWarning("Không tìm thấy LoadingManager (Chắc do test thẳng từ Hub). Sẽ load thẳng vào Game!");
            SceneManager.LoadScene(sceneIndex);
        }
    }
}