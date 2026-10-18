using UnityEngine;
using UnityEngine.UI; // Thêm thư viện này để dùng Image
using TMPro;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;

    [Header("Cài đặt Hệ thống")]
    public GameObject[] turretPrefabs;
    public Slot[] allSlots;

    [Header("Tự động đẻ Mèo Level 1 (Theo thời gian)")]
    public bool useAutoSpawn = true;
    public float autoSpawnInterval = 10f;
    private float spawnTimer;

    [Header("Cài đặt Mua Mèo (Bằng Vàng)")]
    public int currentCatPrice = 50;
    public int priceIncreasePerBuy = 10;

    [Header("Nâng cấp Cửa Hàng (Đổi Level Mèo)")]
    public int buysToUpgrade = 5;      // Mua bao nhiêu con thì Shop lên đời bán mèo xịn hơn?
    private int totalCatsBought = 0;   // Đếm số mèo đã mua ở level hiện tại
    private int currentShopLevel = 0;  // Shop đang bán mèo Level mấy? (0 = Mèo Lv1)

    [Header("Giao diện Nút Mua (UI)")]
    public Image catIconImage;         // Cục Image chứa ảnh mèo trên nút
    public TextMeshProUGUI nameText;   // Cục Text chứa tên mèo
    public TextMeshProUGUI priceText;  // Cục Text chứa giá tiền

    [Header("Dữ liệu kho Mèo (Khớp với Turret Prefabs)")]
    public Sprite[] shopCatIcons;      // Cầm ảnh mèo Level 1, 2, 3... thả vào đây
    public string[] shopCatNames;      // Viết tên mèo vào đây ("Mèo Béo", "Mèo Bắn Tỉa"...)

    private Camera cam;
    private Turret selectedTurret;
    private Slot originalSlot;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        cam = Camera.main;
        spawnTimer = autoSpawnInterval;
        UpdateShopUI(); // Bật lên cái là cập nhật UI luôn
    }

    void Update()
    {
        // --- NÚT TEST SPACE (Đẻ mèo Level 1) ---
        if (Input.GetKeyDown(KeyCode.Space)) SpawnTestTurret();

        // --- ĐỒNG HỒ TỰ ĐỘNG ĐẺ (Đẻ mèo Level 1) ---
        if (useAutoSpawn)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnTestTurret();
                spawnTimer = autoSpawnInterval;
            }
        }

        // --- HỆ THỐNG KÉO THẢ (Giữ nguyên) ---
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                Turret clickedTurret = hit.collider.GetComponent<Turret>();
                if (clickedTurret != null)
                {
                    selectedTurret = clickedTurret;
                    originalSlot = selectedTurret.currentSlot;
                }
            }
        }

        if (Input.GetMouseButton(0) && selectedTurret != null)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            selectedTurret.transform.position = mousePos;

            RaycastHit2D hitTrash = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Trash"));
            SpriteRenderer sr = selectedTurret.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (hitTrash.collider != null) sr.color = new Color(1f, 0f, 0f, 0.5f);
                else sr.color = Color.white;
            }
        }

        if (Input.GetMouseButtonUp(0) && selectedTurret != null)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitTrash = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Trash"));
            if (hitTrash.collider != null)
            {
                originalSlot.currentTurret = null;
                Destroy(selectedTurret.gameObject);
                selectedTurret = null;
                return;
            }

            SpriteRenderer sr = selectedTurret.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.white;

            RaycastHit2D hitSlot = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Slot"));
            if (hitSlot.collider != null)
            {
                Slot targetSlot = hitSlot.collider.GetComponent<Slot>();
                if (targetSlot != null) HandleDrop(targetSlot);
                else ReturnToOriginalSlot();
            }
            else ReturnToOriginalSlot();

            selectedTurret = null;
        }
    }

    public void Button_BuyCat()
    {
        bool hasSpace = false;
        foreach (Slot slot in allSlots)
        {
            if (slot.slotLaneID == -1 && slot.IsEmpty())
            {
                hasSpace = true;
                break;
            }
        }

        if (!hasSpace)
        {
            Debug.Log("Hết ô trống rồi, không thể mua thêm!");
            return;
        }

        if (CurrencyManager.Instance != null && CurrencyManager.Instance.SpendGold(currentCatPrice))
        {
            SpawnBoughtTurret(); // Gọi hàm đẻ con mèo theo Level của Shop

            totalCatsBought++; // Cộng 1 lần mua

            // Kiểm tra xem đã đủ số lần mua để nâng cấp Shop chưa?
            // Điều kiện: Mua đủ số lượng VÀ kho mèo vẫn còn level cao hơn để lên
            if (totalCatsBought >= buysToUpgrade && currentShopLevel < turretPrefabs.Length - 1)
            {
                currentShopLevel++;       // Lên đời!
                totalCatsBought = 0;      // Đếm lại từ đầu cho đợt nâng cấp sau
                currentCatPrice += 50;    // Khi lên đời shop, lạm phát giá tăng vọt thêm 50 vàng
                Debug.Log("CỬA HÀNG ĐÃ NÂNG CẤP! BÁN MÈO LEVEL " + (currentShopLevel + 1));
            }
            else
            {
                currentCatPrice += priceIncreasePerBuy; // Nếu chưa lên đời thì giá tăng bình thường
            }

            UpdateShopUI(); // Vẽ lại nút (Đổi ảnh, tên, giá)
        }
        else
        {
            Debug.Log("Nghèo! Cần " + currentCatPrice + " vàng!");
        }
    }

    private void UpdateShopUI()
    {
        // Vẽ lại chữ Tiền
        if (priceText != null) priceText.text = currentCatPrice.ToString();

        // Vẽ lại chữ Tên
        if (nameText != null && shopCatNames.Length > currentShopLevel)
            nameText.text = shopCatNames[currentShopLevel];

        // Vẽ lại cục Ảnh
        if (catIconImage != null && shopCatIcons.Length > currentShopLevel)
            catIconImage.sprite = shopCatIcons[currentShopLevel];
    }

    private Vector3 GetTargetPosition(Slot slot)
    {
        return slot.transform.position + slot.customOffset;
    }

    void HandleDrop(Slot targetSlot)
    {
        if (targetSlot == originalSlot)
        {
            ReturnToOriginalSlot();
            return;
        }

        if (targetSlot.IsEmpty())
        {
            originalSlot.currentTurret = null;
            targetSlot.currentTurret = selectedTurret;
            selectedTurret.currentSlot = targetSlot;

            selectedTurret.transform.position = GetTargetPosition(targetSlot);
            selectedTurret.laneID = targetSlot.slotLaneID;
        }
        else
        {
            Turret targetTurret = targetSlot.currentTurret;

            if (targetTurret.turretLevel == selectedTurret.turretLevel)
            {
                originalSlot.currentTurret = null;
                int nextLevel = targetTurret.turretLevel; // (tương đương với Level Index hiện tại + 1 vì bên trong m code sẵn turretLevel là 1, 2, 3 rồi)

                Destroy(selectedTurret.gameObject);
                Destroy(targetTurret.gameObject);

                if (nextLevel < turretPrefabs.Length)
                {
                    Vector3 spawnPos = GetTargetPosition(targetSlot);
                    GameObject newTurretObj = Instantiate(turretPrefabs[nextLevel], spawnPos, Quaternion.identity);
                    Turret newTurret = newTurretObj.GetComponent<Turret>();

                    newTurret.currentSlot = targetSlot;
                    targetSlot.currentTurret = newTurret;
                    newTurret.laneID = targetSlot.slotLaneID;
                }
            }
            else
            {
                originalSlot.currentTurret = targetTurret;
                targetTurret.currentSlot = originalSlot;
                targetTurret.transform.position = GetTargetPosition(originalSlot);
                targetTurret.laneID = originalSlot.slotLaneID;

                targetSlot.currentTurret = selectedTurret;
                selectedTurret.currentSlot = targetSlot;
                selectedTurret.transform.position = GetTargetPosition(targetSlot);
                selectedTurret.laneID = targetSlot.slotLaneID;
            }
        }
    }

    void ReturnToOriginalSlot()
    {
        selectedTurret.transform.position = GetTargetPosition(originalSlot);
    }

    // ĐẺ MÈO KHI MUA BẰNG TIỀN (Sẽ lấy theo Current Shop Level)
    public bool SpawnBoughtTurret()
    {
        foreach (Slot slot in allSlots)
        {
            if (slot.slotLaneID == -1 && slot.IsEmpty())
            {
                Vector3 spawnPos = GetTargetPosition(slot);
                GameObject newTurretObj = Instantiate(turretPrefabs[currentShopLevel], spawnPos, Quaternion.identity);
                Turret newTurret = newTurretObj.GetComponent<Turret>();

                newTurret.currentSlot = slot;
                slot.currentTurret = newTurret;
                newTurret.laneID = slot.slotLaneID;

                return true; // Báo cho GoldManager biết là đã đẻ thành công!
            }
        }
        return false; // Báo là hết chỗ trống rồi
    }

    // ĐẺ MÈO FREE DO HỆ THỐNG / SPACE (Luôn đẻ con level 1 - Index 0)
    void SpawnTestTurret()
    {
        foreach (Slot slot in allSlots)
        {
            if (slot.slotLaneID == -1 && slot.IsEmpty())
            {
                Vector3 spawnPos = GetTargetPosition(slot);
                GameObject newTurretObj = Instantiate(turretPrefabs[0], spawnPos, Quaternion.identity);
                Turret newTurret = newTurretObj.GetComponent<Turret>();

                newTurret.currentSlot = slot;
                slot.currentTurret = newTurret;
                newTurret.laneID = slot.slotLaneID;
                return;
            }
        }
    }
}