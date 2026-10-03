using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Dữ liệu")]
    public LevelConfig[] allLevels; // Kéo các file Level_1, Level_2 vào đây
    public Transform[] spawnPoints;

    [Header("UI Liên kết")]
    public TextMeshProUGUI waveText;
    public GameObject winPanel;

    private LevelConfig currentLevel;

    // Biến quan trọng: Đếm số quái còn sống trên bản đồ
    public static int enemiesAlive = 0;

    void Start()
    {
        // --- DỌN DẸP SẠCH SẼ MỌI TÀN DƯ CỦA TRẬN CŨ ---
        Time.timeScale = 1f; // Đảm bảo thời gian trôi bình thường (đề phòng m pause game lúc Win)
        enemiesAlive = 0;    // Reset bộ đếm quái về 0
        StopAllCoroutines(); // Dừng mọi nhịp đẻ quái của trận trước (nếu có)
        // ----------------------------------------------

        if (winPanel != null) winPanel.SetActive(false);

        foreach (var lvl in allLevels)
        {
            if (lvl.levelID == LevelSelectManage.selectedLevelID)
            {
                currentLevel = lvl;
                break;
            }
        }

        if (currentLevel != null) StartCoroutine(RunLevel());
    }

    IEnumerator RunLevel()
    {
        for (int i = 0; i < currentLevel.waves.Length; i++)
        {
            WaveInfo currentWave = currentLevel.waves[i];

            if (waveText != null)
                waveText.text = $"WAVE {i + 1} / {currentLevel.waves.Length}";

            // Cho người chơi 2 giây thở giữa các Wave
            yield return new WaitForSeconds(2f);

            // Bắt đầu đẻ quái của Wave này
            for (int j = 0; j < currentWave.enemyCount; j++)
            {
                SpawnSingleEnemy(currentWave);
                yield return new WaitForSeconds(currentWave.spawnDelay);
            }

            // Wave đẻ xong -> Khoá mõm chờ giết hết quái mới cho sang Wave tiếp
            while (enemiesAlive > 0)
            {
                yield return null;
            }
        }

        // Qua hết Wave mà không chết -> Hiện bảng Win
        if (winPanel != null) winPanel.SetActive(true);
    }

    void SpawnSingleEnemy(WaveInfo waveData)
    {
        if (spawnPoints.Length == 0 || waveData.allowedEnemies.Length == 0) return;

        // Chọn 1 loại quái ngẫu nhiên trong danh sách cho phép của Wave này
        GameObject randomPrefab = waveData.allowedEnemies[Random.Range(0, waveData.allowedEnemies.Length)];

        // Chọn điểm rơi
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Kêu Kho lấy quái ra
        GameObject enemyObj = MultiEnemyPool.Instance.GetEnemy(randomPrefab, randomPoint.position);

        // Bơm máu thêm theo độ khó
        Enemy enemyScript = enemyObj.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.maxHp += waveData.enemyExtraHP;
        }

        enemiesAlive++;
    }
}