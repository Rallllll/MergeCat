using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Thông số bắn")]
    [HideInInspector] public int laneID = -1;
    public float fireRate = 1f;
    public float attackRange = 10f;

    [Header("Tham chiếu cơ bản")]
    public GameObject bulletPrefab;
    public GameObject shootVFXPrefab;
    public Transform[] firePoints;

    private Animator anim;
    private float fireTimer;
    private bool isAttacking = false;

    [Header("Merge Info")]
    public int turretLevel = 1;
    public Slot currentSlot;

    [Header("Nâng cấp thông số (Khớp với Hub)")]
    public string catID = "Cat_01";
    public float[] upgradeFireRates = new float[4];
    public GameObject[] upgradeBullets = new GameObject[4];

    void Start()
    {
        anim = GetComponent<Animator>();
        SetIdleState();
        ApplyUpgradeStats();
    }

    void ApplyUpgradeStats()
    {
        int clickCount = PlayerPrefs.GetInt("UpgradeCount_" + catID, 0);
        int milestone = 0;

        // Chưa nâng cấp (0) hoặc nâng 1 lần (1) -> Mốc 0 (Lấy Element 0: Basic Bullet)
        if (clickCount == 0 || clickCount == 1)
        {
            milestone = 0;
        }
        // Nâng cấp 2 hoặc 3 lần -> Mốc 1 (Lấy Element 1: Medium Bullet)
        else if (clickCount == 2 || clickCount == 3)
        {
            milestone = 1;
        }
        // Nâng cấp 4 hoặc 5 lần -> Mốc 2 (Lấy Element 2: Strongest Bullet)
        else if (clickCount >= 4)
        {
            milestone = 2;
        }

        // Ép an toàn từ 0 đến 2, vì mục đạn của m đang có đúng 3 mốc (0, 1, 2)
        milestone = Mathf.Clamp(milestone, 0, 2);

        // Nạp Tốc độ bắn
        if (upgradeFireRates != null && upgradeFireRates.Length > milestone)
            fireRate = upgradeFireRates[milestone];

        // Nạp Đạn
        if (upgradeBullets != null && upgradeBullets.Length > milestone)
            bulletPrefab = upgradeBullets[milestone];
    }

    void Update()
    {
        if (laneID == -1)
        {
            if (isAttacking) SetIdleState();
            return;
        }

        if (CheckEnemyInLane())
        {
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
            if (isAttacking) SetIdleState();
        }
    }

    private void SetIdleState()
    {
        isAttacking = false;
        if (anim != null)
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Idle");
        }
    }

    private void Shoot()
    {
        if (anim != null)
        {
            anim.ResetTrigger("Idle");
            anim.SetTrigger("Attack");
        }
    }

    bool CheckEnemyInLane()
    {
        if (firePoints == null || firePoints.Length == 0 || firePoints[0] == null) return false;
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
                VfxPool.Instance.GetVfx(fp.position, Quaternion.identity);

            if (bulletPrefab != null && MultiBulletPool.Instance != null)
                MultiBulletPool.Instance.GetBullet(bulletPrefab, fp.position, Quaternion.identity);
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
                Gizmos.DrawRay(fp.position, Vector2.right * attackRange);
            }
        }
    }
}