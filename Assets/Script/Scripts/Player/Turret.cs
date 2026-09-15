using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Thông số bắn")]
    [HideInInspector] public int laneID;
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
    }

    void Update()
    {
        // TRƯỜNG HỢP 1: Đang ở ô Merge chờ (laneID == -1)
        if (laneID == -1)
        {
            if (isAttacking)
            {
                anim.SetTrigger("Idle"); // Ép về Idle nếu vừa bị nhấc từ trên xuống
                isAttacking = false;
            }
            return;
        }

        // TRƯỜNG HỢP 2: Đang ở ô Chiến đấu
        bool hasEnemy = CheckEnemyInLane();

        if (hasEnemy)
        {
            // Nếu vừa thấy quái -> Chuẩn bị trạng thái bắn và nổ súng ngay lập tức
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
                anim.SetTrigger("Idle");
                isAttacking = false;
            }
        }
    }

    bool CheckEnemyInLane()
    {
        if (firePoints == null || firePoints.Length == 0 || firePoints[0] == null) return false;

        RaycastHit2D hit = Physics2D.Raycast(firePoints[0].position, Vector2.up, attackRange, LayerMask.GetMask("Enemy"));
        return hit.collider != null;
    }

    void Shoot()
    {
        anim.SetTrigger("Attack");
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
                Gizmos.DrawRay(fp.position, Vector2.up * attackRange);
            }
        }
    }
}