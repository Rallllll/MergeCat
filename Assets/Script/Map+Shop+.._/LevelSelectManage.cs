using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelButtonData
{
    public int levelID;
    public Button buttonComponent;

    [Header("Nơi hiển thị ảnh trên UI")]
    public Image targetImage; // Khung Image của nút

    [Header("Object Text con (Hiển thị số Level)")]
    public GameObject levelTextObj; // Kéo object Text con vào đây

    [Header("Kéo file ảnh (Sprite) vào đây")]
    public Sprite lockedVisual;
    public Sprite unlockedVisual;
    public Sprite finishedVisual;
}

public class LevelSelectManage : MonoBehaviour
{
    public static int selectedLevelID = 1;

    [Header("Cài đặt Chuyển Cảnh")]
    public int gameSceneIndex = 2;

    [Header("Quản lý Giao diện Nút Level")]
    public LevelButtonData[] levelButtons;

    void Start()
    {
        UpdateAllLevelButtons();
    }

    public void UpdateAllLevelButtons()
    {
        foreach (LevelButtonData btn in levelButtons)
        {
            int defaultState = (btn.levelID == 1) ? 1 : 0;
            int state = PlayerPrefs.GetInt("LevelState_" + btn.levelID, defaultState);

            if (btn.buttonComponent != null)
            {
                btn.buttonComponent.interactable = (state != 0);
            }

            if (btn.targetImage == null) continue;

            // XỬ LÝ ĐỔI ẢNH VÀ BẬT/TẮT TEXT
            if (state == 0) // KHOÁ
            {
                if (btn.lockedVisual != null) btn.targetImage.sprite = btn.lockedVisual;
                if (btn.levelTextObj != null) btn.levelTextObj.SetActive(false); // Ẩn Text đi
            }
            else if (state == 1) // MỞ KHOÁ (CHƯA QUA)
            {
                if (btn.unlockedVisual != null) btn.targetImage.sprite = btn.unlockedVisual;
                if (btn.levelTextObj != null) btn.levelTextObj.SetActive(true);  // Hiện Text lên
            }
            else if (state == 2) // ĐÃ FINISH
            {
                if (btn.finishedVisual != null) btn.targetImage.sprite = btn.finishedVisual;
                if (btn.levelTextObj != null) btn.levelTextObj.SetActive(true);  // Hiện Text lên
            }
        }
    }

    public void LoadLevel(int levelID)
    {
        selectedLevelID = levelID;

        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadScene(gameSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(gameSceneIndex);
        }
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        UpdateAllLevelButtons();
    }
}