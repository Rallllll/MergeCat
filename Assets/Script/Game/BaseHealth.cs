using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    [Header("Thông số Thành")]
    public int maxHealth = 1000;
    private int currentHealth;

    [Header("Giao diện UI")]
    public Slider healthSlider;

    [Header("Cài đặt mua máu (Cho Nút Bấm)")]
    public int healAmount = 100; // Lượng máu hồi mỗi lần bấm
    public int healCost = 50;    // Giá tiền (tùy chỉnh ở Inspector)

    private Collider2D baseCollider;

    void Start()
    {
        currentHealth = maxHealth;
        baseCollider = GetComponent<Collider2D>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            if (baseCollider != null) baseCollider.enabled = false;
            GameOver();
        }
    }

    // --- HÀM NÀY DÀNH RIÊNG CHO NÚT BẤM (BUTTON) TRÊN CANVAS ---
    public void Button_BuyHeal()
    {
        // 1. Chặn ngay nếu máu đã đầy (Tránh người chơi bấm nhầm mất tiền oan)
        if (currentHealth >= maxHealth)
        {
            Debug.Log("Máu đã đầy, không cần mua thêm!");
            return;
        }

        // 2. Giao dịch với CurrencyManager (Kiểm tra và trừ tiền)
        // Nếu có đủ tiền, SpendGold sẽ tự động trừ đi healCost và báo về true
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.SpendGold(healCost))
        {
            // --- GIAO DỊCH THÀNH CÔNG -> BẮT ĐẦU BƠM MÁU ---
            currentHealth += healAmount;

            // Ép máu không được vượt qua mức giới hạn Max
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            // Nếu Thành đang trong trạng thái vỡ (máu = 0), bơm máu xong phải bật lại khiên
            if (baseCollider != null && !baseCollider.enabled)
            {
                baseCollider.enabled = true;
            }

            // Cập nhật lại thanh máu trên màn hình
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }

            Debug.Log("Bơm máu thành công! Trừ " + healCost + " vàng.");
        }
        else
        {
            // --- GIAO DỊCH THẤT BẠI ---
            Debug.Log("Không đủ " + healCost + " vàng để mua máu!");
        }
    }

    void GameOver()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLosePanel();
       }
    }
}