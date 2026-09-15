using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 25;

    [Header("Hướng bay của đạn")]
    public Vector3 moveDirection = Vector3.right; // Mặc định bay sang phải

    [Header("Giới hạn tầm bắn")]
    public float maxTravelDistance = 15f;
    private Vector3 startPosition;

    void OnEnable()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Đạn di chuyển theo hướng moveDirection
        transform.Translate(moveDirection * speed * Time.deltaTime);

        if (Vector3.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        if (gameObject.activeInHierarchy)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }
}