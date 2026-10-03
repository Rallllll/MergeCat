using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 25;
    public Vector3 moveDirection = Vector3.right;
    public float maxTravelDistance = 15f;
    private Vector3 startPosition;

    void OnEnable()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
        if (Vector3.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Chú ý: Script Enemy của m phải có hàm TakeDamage nhé
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        if (gameObject.activeInHierarchy && MultiBulletPool.Instance != null)
        {
            MultiBulletPool.Instance.ReturnBullet(gameObject);
        }
    }
}