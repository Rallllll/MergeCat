using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Thông số bắn")]
    [HideInInspector] public int laneID = -1; // Mặc định sinh ra ở slot chờ là -1
    public float fireRate = 1f; // Bắn 1 phát / giây
    public float attackRange = 10f; // Tầm quét quái

    [Header("Tham chiếu")]
    public GameObject bulletPrefab;
    public GameObject shootVFXPrefab;

    [Header("Nhiều nòng súng (Fire Points)")]
    public Transform[] firePoints;

    private Animator anim;
    private float fireTimer;
    private bool isAttacking = false; // Biến kiểm tra trạng thái để tránh spam Trigger

    [Header("Merge Info")]
    public int turretLevel = 1;
    public Slot currentSlot;

    void Start()
    {
        anim = GetComponent<Animator>();
        SetIdleState(); // Mới sinh ra thì ép luôn vào trạng thái Idle đứng im
    }

    void Update()
    {
        // TRƯỜNG HỢP 1: Đang ở ô Merge chờ (laneID == -1)
        if (laneID == -1)
        {
            if (isAttacking)
            {
                SetIdleState(); // Ép về Idle nếu vừa bị nhấc từ trên làn đánh xuống
            }
            return; // Khóa luôn, không cho quét quái hay bắn súng nữa
        }

        // TRƯỜNG HỢP 2: Đang ở ô Chiến đấu trên đường
        bool hasEnemy = CheckEnemyInLane();

        if (hasEnemy)
        {
            // Nếu vừa thấy quái -> Chuẩn bị trạng thái bắn và nổ súng ngay lập tức phát đầu tiên
            if (!isAttacking)
            {
                isAttacking = true;
                fireTimer = 0f;
            }

            fireTimer -= Time.deltaTime;

            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = 1f / fireRate;
            }
        }
        else
        {
            // Nếu hết quái -> Trả về Idle
            if (isAttacking)
            {
                SetIdleState();
            }
        }
    }

    // --- BỘ HÀM XỬ LÝ ANIMATION (CHỐNG KẸT) ---
    private void SetIdleState()
    {
        isAttacking = false;
        if (anim != null)
        {
            anim.ResetTrigger("Attack"); // Cực quan trọng: Xóa lệnh bắn bị kẹt
            anim.SetTrigger("Idle");
        }
    }

    private void Shoot()
    {
        if (anim != null)
        {
            anim.ResetTrigger("Idle"); // Xóa lệnh Idle bị kẹt
            anim.SetTrigger("Attack");
        }
    }

    // --- BỘ HÀM QUÉT ĐỊCH VÀ XẢ ĐẠN ---
    bool CheckEnemyInLane()
    {
        if (firePoints == null || firePoints.Length == 0 || firePoints[0] == null) return false;

        // Đã sửa thành Vector2.right. 
        // Tia laser quét ngang, đâm xuyên mọi thứ và chỉ dừng lại khi chạm trúng Layer "Enemy"
        RaycastHit2D hit = Physics2D.Raycast(firePoints[0].position, Vector2.right, attackRange, LayerMask.GetMask("Enemy"));

        return hit.collider != null;
    }

    public void SpawnBulletAndVFX()
    {
        if (laneID == -1) return;
        if (firePoints == null || firePoints.Length == 0) return;

        foreach (Transform fp in firePoints)
        {
            if (fp == null) continue;

            if (VfxPool.Instance != null && shootVFXPrefab != null)
            {
                VfxPool.Instance.GetVfx(fp.position, Quaternion.identity);
            }

            if (bulletPrefab != null)
            {
                BulletPool.Instance.GetBullet(fp.position, Quaternion.identity);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoints == null) return;
        foreach (Transform fp in firePoints)
        {
            if (fp != null)
            {
                Gizmos.color = Color.red;
                // Đã đổi hướng vẽ tia đỏ sang phải để bạn căn chỉnh tầm bắn cho chuẩn
                Gizmos.DrawRay(fp.position, Vector2.right * attackRange);
            }
        }
    }
}