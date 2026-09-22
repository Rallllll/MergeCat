using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("UI Liên kết")]
    public GameObject loadingGroup;
    public Slider progressBar;

    [Header("Cài đặt Mượt mà")]
    [Tooltip("Tốc độ chạy thanh loading. Số càng nhỏ chạy càng chậm (VD: 1 = mất 1s, 0.5 = mất 2s)")]
    public float sliderSpeed = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (loadingGroup != null) loadingGroup.SetActive(false);
    }

    public void LoadScene(int sceneIndex)
    {
        if (loadingGroup != null) loadingGroup.SetActive(true);
        if (progressBar != null) progressBar.value = 0f;

        StartCoroutine(LoadSceneSmooth(sceneIndex));
    }

    IEnumerator LoadSceneSmooth(int sceneIndex)
    {
        // 1. NGƯNG 1 FRAME: Để Unity kịp vẽ cái bảng Loading màu đen lên màn hình mượt mà rồi mới load nặng
        yield return null;

        // 2. Tải ngầm, nhưng KHOÁ CHỐT không cho nhảy Scene tự động
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float visualProgress = 0f; // Biến chạy thanh ảo

        // 3. VÒNG LẶP: Đợi đến khi load thật xong (đạt 0.9) VÀ thanh Slider chạy đầy 100%
        while (!operation.isDone)
        {
            // Unity tải xong nó dừng ở 0.9, lúc đó mình đặt mục tiêu cho Slider là 1.0 (100%)
            float targetProgress = operation.progress < 0.9f ? operation.progress : 1f;

            // Cho thanh Slider chạy từ từ lên mục tiêu (mượt mà)
            visualProgress = Mathf.MoveTowards(visualProgress, targetProgress, sliderSpeed * Time.deltaTime);
            if (progressBar != null) progressBar.value = visualProgress;

            // 4. Nếu thanh Slider chạy đầy 100% rồi -> Mở chốt cho bay sang Scene mới
            if (visualProgress >= 1f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null; // Lặp lại qua từng frame
        }

        // Ẩn bảng Loading khi mọi thứ xong xuôi
        if (loadingGroup != null) loadingGroup.SetActive(false);
    }
}