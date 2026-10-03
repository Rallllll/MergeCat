using System.Collections.Generic;
using UnityEngine;

public class MultiEnemyPool : MonoBehaviour
{
    public static MultiEnemyPool Instance;

    // Kho thông minh tự phân chia tên Quái thành các ngăn riêng
    private Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        // 1. Ép nhận chủ mới, bỏ qua cái xác cũ
        Instance = this;

        // 2. Xoá sạch toàn bộ rác trong từ điển nếu còn sót
        poolDict.Clear();
    }

    public GameObject GetEnemy(GameObject prefab, Vector3 position)
    {
        string key = prefab.name;

        if (!poolDict.ContainsKey(key))
            poolDict[key] = new Queue<GameObject>();

        if (poolDict[key].Count > 0)
        {
            GameObject obj = poolDict[key].Dequeue();
            obj.transform.position = position;
            obj.SetActive(true);
            return obj;
        }

        GameObject newObj = Instantiate(prefab, position, Quaternion.identity);
        newObj.name = key; // Ép tên để lúc cất biết nhét vào đúng ngăn
        return newObj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        poolDict[obj.name].Enqueue(obj);
    }
}