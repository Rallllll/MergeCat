using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Cài đặt Bom")]
    public int damage = 500;           // Sát thương nổ
    public float explosionRadius = 1.5f; // Tầm nổ lan

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi quái dẫm vào bom
        if (collision.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    void Explode()
    {
        // 1. GỌI HIỆU ỨNG TỪ POOL
        if (ExplosionPool.Instance != null)
        {
            ExplosionPool.Instance.GetExplosion(transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Chưa có ExplosionPool trên Scene!");
        }

        // 2. SÁT THƯƠNG LAN (AOE)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in hitEnemies)
        {
            if (col.CompareTag("Enemy"))
            {
                Enemy enemyScript = col.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(damage);
                }
            }
        }

        // 3. BIẾN MẤT
        // (Nếu quả bom của bạn cũng được quản lý bằng một Pool khác từ DragManager, 
        // thì thay Destroy bằng hàm ReturnBomb tương tự như hiệu ứng nổ)
        Destroy(gameObject);
    }

    // Vẽ vòng tròn đỏ để dễ căn tầm nổ ngoài Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}