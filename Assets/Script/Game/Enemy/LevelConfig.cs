using UnityEngine;

[System.Serializable]
public class WaveInfo
{
    public int enemyCount = 5;
    public float spawnDelay = 1.5f;
    public int enemyExtraHP = 0;

    [Header("Danh sách quái được phép ra ở Wave này")]
    public GameObject[] allowedEnemies;
}

[CreateAssetMenu(fileName = "Level_1", menuName = "Game Data/Level Config")]
public class LevelConfig : ScriptableObject
{
    public int levelID;
    public WaveInfo[] waves;
}