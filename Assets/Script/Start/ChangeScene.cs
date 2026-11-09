using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Hàm này phải là public để nút bấm (Button) có thể gọi được
    public void LoadGameScene(string sceneName)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        SceneManager.LoadScene(sceneName);
    }
}
