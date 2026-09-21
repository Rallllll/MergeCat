using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Thông số bẫy")]
    public int damage = 50;
    public int durability = 3;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                durability--;

                if (durability <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}