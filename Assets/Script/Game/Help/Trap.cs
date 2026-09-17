using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Thông số bẫy")]
    public int damage = 50;
    public int durability = 3;
    public GameObject explosionVFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tối ưu: Phải check xem thứ chạm vào CÓ ĐÚNG LÀ QUÁI KHÔNG trước khi xử lý
        // Đảm bảo quái của bạn ngoài Inspector đã được gắn Tag là "Enemy" nhé
        if (collision.CompareTag("Enemy"))
        {
            // BỎ DẤU // ĐỂ CODE CHẠY THẬT
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                // 1. Trừ máu quái
                enemy.TakeDamage(damage);

                // 2. Sinh hiệu ứng nổ/máu (nếu có)
                if (explosionVFX != null)
                {
                    Instantiate(explosionVFX, transform.position, Quaternion.identity);
                }

                // 3. Trừ độ bền của bẫy
                durability--;

                // 4. Bẫy hỏng thì tự tiêu hủy
                if (durability <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}