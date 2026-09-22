using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Thông số chiến đấu")]
    public int maxHp = 100;
    [SerializeField] private int currentHp;
    public float speed = 1.5f;
    public int damage = 20;

    [Header("Dò mục tiêu (Hitbox)")]
    public float attackRange = 0.8f;
    public float attackRate = 2f;

    private bool isDead = false;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private GameObject currentTarget;
    private Animator anim;

    private SpriteRenderer sr;
    private Color originalColor;
    private LayerMask playerMask;

    public int goldDrop = 15;

    // --- BIẾN ĐIỀU HƯỚNG ---
    private Vector2 currentDirection = Vector2.left; // Mặc định quái sinh ra là đi sang trái

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        playerMask = LayerMask.GetMask("Player", "Wall");
    }

    void OnEnable()
    {
        currentHp = maxHp;
        isDead = false;
        isAttacking = false;
        currentTarget = null;
        attackTimer = 0f;
        GetComponent<Collider2D>().enabled = true;

        if (sr != null) sr.color = originalColor;

        // Reset hướng về bên trái mỗi khi được lấy ra từ Pool
        currentDirection = Vector2.left;
        FlipCharacter();

        if (anim != null) anim.SetTrigger("Walk");
    }

    void Update()
    {
        // 1. CHỐNG NHẤP NHÁY VÀ ĐẢM BẢO QUÁI NỔI LÊN TRÊN HÌNH NỀN
        if (sr != null)
        {
            sr.sortingOrder = 30000 + Mathf.RoundToInt(transform.position.y * -100f) + Mathf.RoundToInt(transform.position.x * -10f);
        }

        if (isDead) return;

        // 2. DI CHUYỂN HOẶC TẤN CÔNG
        if (!isAttacking)
        {
            // Đi lùi (moonwalk) fix bằng Space.World
            transform.Translate(currentDirection * speed * Time.deltaTime, Space.World);
            CheckForTarget();
        }
        else
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                // 3. KIỂM TRA MỤC TIÊU CÒN SỐNG HAY KHÔNG (Fix lỗi chém không khí)
                bool targetIsDead = false;

                // Nếu mục tiêu đã bị xóa hẳn khỏi màn hình
                if (currentTarget == null || !currentTarget.activeInHierarchy)
                {
                    targetIsDead = true;
                }
                else
                {
                    // Nếu mục tiêu vẫn còn hình ảnh, NHƯNG lớp va chạm đã bị tắt (Tức là hết máu/vỡ thành)
                    Collider2D targetCol = currentTarget.GetComponent<Collider2D>();
                    if (targetCol == null || !targetCol.enabled)
                    {
                        targetIsDead = true;
                    }
                }

                // QUYẾT ĐỊNH HÀNH ĐỘNG
                if (targetIsDead)
                {
                    ResumeWalking(); // Mục tiêu đã chết/vỡ -> Đi tiếp!
                }
                else
                {
                    anim.SetTrigger("Attack"); // Mục tiêu còn sống -> Chém tiếp!
                    attackTimer = attackRate;
                }
            }
        }
    }

    void CheckForTarget()
    {
        // Quét tia cả 2 bên
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, attackRange, playerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, attackRange, playerMask);

        // Ưu tiên bên trái (vì nó đang hành quân sang trái), nếu không có thì check bên phải
        RaycastHit2D hitAttack = hitLeft.collider != null ? hitLeft : hitRight;

        if (hitAttack.collider != null)
        {
            // Xác định xem mục tiêu đang ở bên nào để quay mặt sang đó
            currentDirection = hitLeft.collider != null ? Vector2.left : Vector2.right;
            FlipCharacter();

            currentTarget = hitAttack.collider.gameObject;
            isAttacking = true;
            anim.SetTrigger("Idle");
            attackTimer = 0.2f;
        }
    }

    private void FlipCharacter()
    {
        // Giả sử sprite gốc của Quái đang quay mặt sang TRÁI (scale = 1).
        // Nếu nó cần đánh mục tiêu bên PHẢI -> Lật scale thành -1.
        if (currentDirection == Vector2.right)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    public void DealDamage()
    {
        if (currentTarget != null && !isDead)
        {
            Melee defender = currentTarget.GetComponent<Melee>();
            if (defender != null) defender.TakeDamage(damage);

            BaseHealth baseTarget = currentTarget.GetComponent<BaseHealth>();
            if (baseTarget != null) baseTarget.TakeDamage(damage);
        }
    }

    void ResumeWalking()
    {
        isAttacking = false;
        currentTarget = null;

        // Làm gì xong đều quay lại về phía trụ 
        currentDirection = Vector2.left;
        FlipCharacter();

        anim.SetTrigger("Walk");
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHp -= damageAmount;
        if (sr != null) StartCoroutine(FlashRed());

        // --- GỌI SỐ SÁT THƯƠNG TỪ TRONG KHO (POOL) CỦA M VÀO ĐÂY ---
        if (DamageTextPool.Instance != null)
        {
            // Thêm offset tí xíu để nếu bị bắn liên tục, các số bung ra mượt mà không bị đè cứng lên nhau
            Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.3f, 0.8f), 0);
            DamageTextPool.Instance.SpawnDamageText(transform.position + offset, damageAmount);
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        anim.SetTrigger("Dead");
        GetComponent<Collider2D>().enabled = false;

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGold(goldDrop);
        }

        StartCoroutine(DeactivateAfterDelay(1.5f));
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // Vẽ tia vàng ra cả 2 hướng để dễ căn chỉnh hitbox trên Scene
        Gizmos.DrawRay(transform.position, Vector2.left * attackRange);
        Gizmos.DrawRay(transform.position, Vector2.right * attackRange);
    }
}