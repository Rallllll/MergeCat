using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Thông số bẫy")]
    public int damage = 50;       // Sát thương mỗi lần dẫm
    public int durability = 3;    // Độ bền (Ví dụ: dẫm 3 lần là hỏng bẫy)
    public GameObject explosionVFX; // (Tùy chọn) Kéo prefab hiệu ứng nổ vào đây

    // Hàm này tự chạy khi có quái (hoặc vật thể khác) đi vào vùng Is Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem thứ dẫm lên bẫy có phải là quái không
        //Enemy enemy = collision.GetComponent<Enemy>();
        //if (enemy != null)
       // {
            // 1. Trừ máu quái
           // enemy.TakeDamage(damage);

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
        //}
    }
}