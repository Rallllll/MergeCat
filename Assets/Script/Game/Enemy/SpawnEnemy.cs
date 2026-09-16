using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [Header("Cài đặt Spawner")]
    public GameObject[] enemyPrefabs; // Kéo các Prefab quái thường vào đây
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;

    [Header("Cài đặt Pool")]
    public int poolSizePerType = 10;

    private List<GameObject>[] enemyPools;
    private float timer;

    void Start()
    {
        // Khởi tạo từng ngăn kho cho từng loại quái
        enemyPools = new List<GameObject>[enemyPrefabs.Length];

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            enemyPools[i] = new List<GameObject>();
            for (int j = 0; j < poolSizePerType; j++)
            {
                GameObject obj = Instantiate(enemyPrefabs[i]);
                obj.SetActive(false);
                enemyPools[i].Add(obj);
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            EnemySpawn();
            timer = 0f;
        }
    }

    private void EnemySpawn()
    {
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0) return;

        // Chọn ngẫu nhiên 1 loại quái (tỷ lệ ra các con là như nhau)
        int randomTypeIndex = Random.Range(0, enemyPrefabs.Length);

        // Tìm quái đang rảnh trong đúng ngăn kho của loại đó
        GameObject enemyToSpawn = null;
        foreach (GameObject enemy in enemyPools[randomTypeIndex])
        {
            if (!enemy.activeInHierarchy)
            {
                enemyToSpawn = enemy;
                break;
            }
        }

        // Nếu kho hết thì đẻ thêm
        if (enemyToSpawn == null)
        {
            enemyToSpawn = Instantiate(enemyPrefabs[randomTypeIndex]);
            enemyPools[randomTypeIndex].Add(enemyToSpawn);
        }

        // Chọn làn đường và thả quái ra
        int randomLane = Random.Range(0, spawnPoints.Length);
        enemyToSpawn.transform.position = spawnPoints[randomLane].position;
        enemyToSpawn.transform.rotation = Quaternion.identity;

        // Đánh thức quái (Kích hoạt OnEnable trong Enemy.cs)
        enemyToSpawn.SetActive(true);
    }
}