using UnityEngine;
using System.Collections;

public class Melee : MonoBehaviour
{
    [Header("Chỉ số chiến đấu")]
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;
    public int damage = 20;
    public float attackRange = 1.5f;
    public float sightRange = 15f;
    public float attackCooldown = 1.2f;
    public float moveSpeed = 2f;

    [Header("Hiệu ứng khi chết")]
    public GameObject deathPrefab;

    private Animator anim;
    private float cooldownTimer;
    private bool isAttacking = false;
    private bool isMoving = false;
    private bool isDead = false;

    private SpriteRenderer sr;
    private Color originalColor;

    private LayerMask enemyMask; // Tối ưu lag
    private Vector2 currentDirection = Vector2.right; // Mặc định nhìn sang phải

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        cooldownTimer = attackCooldown;
        enemyMask = LayerMask.GetMask("Enemy"); // Lưu LayerMask từ đầu

        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        anim.SetTrigger("Idle");
    }

    void Update()
    {
        if (isDead) return;

        cooldownTimer += Time.deltaTime;
        CheckLogic();

        if (isMoving && !isAttacking)
        {
            // Đi về hướng có địch
            transform.Translate(currentDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void CheckLogic()
    {
        // 1. Quét tầm chém CẢ 2 BÊN
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, attackRange, enemyMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, attackRange, enemyMask);

        // Ưu tiên phải, không có thì xem trái
        RaycastHit2D hitAttack = hitRight.collider != null ? hitRight : hitLeft;

        if (hitAttack.collider != null)
        {
            // Địch ở bên nào, quay mặt sang bên đó
            currentDirection = hitRight.collider != null ? Vector2.right : Vector2.left;
            FlipCharacter();

            isMoving = false;

            if (!isAttacking)
            {
                isAttacking = true;
                anim.SetTrigger("Idle");
            }

            if (cooldownTimer >= attackCooldown)
            {
                anim.SetTrigger("Attack");
                cooldownTimer = 0f;
            }
        }
        else
        {
            isAttacking = false;

            // 2. Quét tầm nhìn CẢ 2 BÊN
            RaycastHit2D sightRight = Physics2D.Raycast(transform.position, Vector2.right, sightRange, enemyMask);
            RaycastHit2D sightLeft = Physics2D.Raycast(transform.position, Vector2.left, sightRange, enemyMask);

            RaycastHit2D hitSight = sightRight.collider != null ? sightRight : sightLeft;

            if (hitSight.collider != null)
            {
                // Thấy địch từ xa -> Quay mặt và đi tới
                currentDirection = sightRight.collider != null ? Vector2.right : Vector2.left;
                FlipCharacter();

                if (!isMoving)
                {
                    isMoving = true;
                    anim.SetTrigger("Idle");
                }
            }
            else
            {
                // Không có ai -> Gác cổng
                if (isMoving)
                {
                    isMoving = false;
                    anim.SetTrigger("Idle");
                }
            }
        }
    }

    private void FlipCharacter()
    {
        // Lật nhân vật theo trục X nếu quay sang trái
        if (currentDirection == Vector2.left)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    public void DealDamage()
    {
        if (isDead) return;

        // Chém vào đúng hướng đang quay mặt
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, attackRange, enemyMask);
        if (hit.collider != null)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damageTaken)
    {
        if (isDead) return;

        currentHealth -= damageTaken;
        if (sr != null) StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    private void Die()
    {
        isDead = true;
        if (deathPrefab != null) Instantiate(deathPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Vẽ tia đỏ cả 2 bên
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * attackRange);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * attackRange);

        Gizmos.color = Color.yellow;
        // Vẽ tia vàng cả 2 bên
        Gizmos.DrawLine(transform.position + Vector3.right * attackRange, transform.position + Vector3.right * sightRange);
        Gizmos.DrawLine(transform.position + Vector3.left * attackRange, transform.position + Vector3.left * sightRange);
    }
}