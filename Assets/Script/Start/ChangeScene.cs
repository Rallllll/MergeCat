using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải có thư viện này

public class ChangeScene : MonoBehaviour
{
    // Hàm này phải là public để nút bấm (Button) có thể gọi được
    public void LoadGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
