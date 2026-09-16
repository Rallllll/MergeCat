using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    // Quản lý danh sách Enemy theo chỉ số Lane (VD: Lane 0, Lane 1, Lane 2...)
    public int laneIndex;
    public List<GameObject> enemiesInLane = new List<GameObject>();

    // Gọi hàm này khi Spawner tạo Enemy trên Lane này
    public void RegisterEnemy(GameObject enemy)
    {
        if (!enemiesInLane.Contains(enemy))
        {
            enemiesInLane.Add(enemy);
        }
    }

    // Gọi hàm này khi Enemy bị tiêu diệt hoặc đi hết đường
    public void UnregisterEnemy(GameObject enemy)
    {
        if (enemiesInLane.Contains(enemy))
        {
            enemiesInLane.Remove(enemy);
        }
    }

    // Kiểm tra xem Lane có Enemy nào không
    public bool HasEnemy()
    {
        // Loại bỏ các tham chiếu null (Enemy đã bị Destroy nhưng chưa Unregister)
        enemiesInLane.RemoveAll(enemy => enemy == null);
        return enemiesInLane.Count > 0;
    }

    // Lấy Enemy đầu tiên trên Lane (gần Mèo nhất)
    public GameObject GetFirstEnemy()
    {
        if (HasEnemy())
        {
            return enemiesInLane[0];
        }
        return null;
    }
}