using System.Collections.Generic;
using UnityEngine;

public class MultiBulletPool : MonoBehaviour
{
    public static MultiBulletPool Instance;
    private Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
        poolDict.Clear();
    }

    public GameObject GetBullet(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string bulletName = prefab.name;

        if (!poolDict.ContainsKey(bulletName))
            poolDict[bulletName] = new Queue<GameObject>();

        if (poolDict[bulletName].Count == 0)
        {
            GameObject newBullet = Instantiate(prefab, position, rotation);
            newBullet.name = bulletName;
            newBullet.transform.SetParent(transform);
            return newBullet;
        }

        GameObject bullet = poolDict[bulletName].Dequeue();
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        if (!poolDict.ContainsKey(bullet.name))
            poolDict[bullet.name] = new Queue<GameObject>();

        poolDict[bullet.name].Enqueue(bullet);
    }
}