using UnityEngine;
using UnityEngine.UI; // ThÃªm thÆ° viá»‡n nÃ y Ä‘á»ƒ dÃ¹ng Image
using TMPro;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;

    [Header("CÃ i Ä‘áº·t Há»‡ thá»‘ng")]
    public GameObject[] turretPrefabs;
    public Slot[] allSlots;

    [Header("Tá»± Ä‘á»™ng Ä‘áº» MÃ¨o Level 1 (Theo thá»i gian)")]
    public bool useAutoSpawn = true;
    public float autoSpawnInterval = 10f;
    private float spawnTimer;

    [Header("CÃ i Ä‘áº·t Mua MÃ¨o (Báº±ng VÃ ng)")]
    public int currentCatPrice = 50;
    public int priceIncreasePerBuy = 10;

    [Header("NÃ¢ng cáº¥p Cá»­a HÃ ng (Äá»•i Level MÃ¨o)")]
    public int buysToUpgrade = 5;      // Mua bao nhiÃªu con thÃ¬ Shop lÃªn Ä‘á»i bÃ¡n mÃ¨o xá»‹n hÆ¡n?
    private int totalCatsBought = 0;   // Äáº¿m sá»‘ mÃ¨o Ä‘Ã£ mua á»Ÿ level hiá»‡n táº¡i
    private int currentShopLevel = 0;  // Shop Ä‘ang bÃ¡n mÃ¨o Level máº¥y? (0 = MÃ¨o Lv1)

    [Header("Giao diá»‡n NÃºt Mua (UI)")]
    public Image catIconImage;         // Cá»¥c Image chá»©a áº£nh mÃ¨o trÃªn nÃºt
    public TextMeshProUGUI nameText;   // Cá»¥c Text chá»©a tÃªn mÃ¨o
    public TextMeshProUGUI priceText;  // Cá»¥c Text chá»©a giÃ¡ tiá»n

    [Header("Dá»¯ liá»‡u kho MÃ¨o (Khá»›p vá»›i Turret Prefabs)")]
    public Sprite[] shopCatIcons;      // Cáº§m áº£nh mÃ¨o Level 1, 2, 3... tháº£ vÃ o Ä‘Ã¢y
    public string[] shopCatNames;      // Viáº¿t tÃªn mÃ¨o vÃ o Ä‘Ã¢y ("MÃ¨o BÃ©o", "MÃ¨o Báº¯n Tá»‰a"...)

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
        UpdateShopUI(); // Báº­t lÃªn cÃ¡i lÃ  cáº­p nháº­t UI luÃ´n
    }

    void Update()
    {
        // --- NÃšT TEST SPACE (Äáº» mÃ¨o Level 1) ---
        if (Input.GetKeyDown(KeyCode.Space)) SpawnTestTurret();

        // --- Äá»’NG Há»’ Tá»° Äá»˜NG Äáºº (Äáº» mÃ¨o Level 1) ---
        if (useAutoSpawn)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnTestTurret();
                spawnTimer = autoSpawnInterval;
            }
        }

        // --- Há»† THá»NG KÃ‰O THáº¢ (Giá»¯ nguyÃªn) ---
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
                // 1. TÃ­nh tiá»n bÃ¡n (25 vÃ ng nhÃ¢n vá»›i Level cá»§a con mÃ¨o)
                int sellPrice = 25 * selectedTurret.turretLevel;

                // 2. Cá»™ng tiá»n vÃ o vÃ­
                if (CurrencyManager.Instance != null)
                {
                    CurrencyManager.Instance.AddGold(sellPrice);
                    Debug.Log($"ÄÃ£ bÃ¡n mÃ¨o Level {selectedTurret.turretLevel} láº¥y {sellPrice} vÃ ng!");
                }

                // 3. XoÃ¡ mÃ¨o vÃ  dá»n dáº¹p Slot
                if (AudioManager.Instance != null) AudioManager.Instance.PlayCoin();
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
            Debug.Log("Háº¿t Ã´ trá»‘ng rá»“i, khÃ´ng thá»ƒ mua thÃªm!");
            if (AudioManager.Instance != null) AudioManager.Instance.PlayError();
            return;
        }

        if (CurrencyManager.Instance != null && CurrencyManager.Instance.SpendGold(currentCatPrice))
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCoin();
            SpawnBoughtTurret(); // Gá»i hÃ m Ä‘áº» con mÃ¨o theo Level cá»§a Shop

            totalCatsBought++; // Cá»™ng 1 láº§n mua

            // Kiá»ƒm tra xem Ä‘Ã£ Ä‘á»§ sá»‘ láº§n mua Ä‘á»ƒ nÃ¢ng cáº¥p Shop chÆ°a?
            // Äiá»u kiá»‡n: Mua Ä‘á»§ sá»‘ lÆ°á»£ng VÃ€ kho mÃ¨o váº«n cÃ²n level cao hÆ¡n Ä‘á»ƒ lÃªn
            if (totalCatsBought >= buysToUpgrade && currentShopLevel < turretPrefabs.Length - 1)
            {
                currentShopLevel++;       // LÃªn Ä‘á»i!
                totalCatsBought = 0;      // Äáº¿m láº¡i tá»« Ä‘áº§u cho Ä‘á»£t nÃ¢ng cáº¥p sau
                currentCatPrice += 50;    // Khi lÃªn Ä‘á»i shop, láº¡m phÃ¡t giÃ¡ tÄƒng vá»t thÃªm 50 vÃ ng
                Debug.Log("Cá»¬A HÃ€NG ÄÃƒ NÃ‚NG Cáº¤P! BÃN MÃˆO LEVEL " + (currentShopLevel + 1));
            }
            else
            {
                currentCatPrice += priceIncreasePerBuy; // Náº¿u chÆ°a lÃªn Ä‘á»i thÃ¬ giÃ¡ tÄƒng bÃ¬nh thÆ°á»ng
            }

            UpdateShopUI(); // Váº½ láº¡i nÃºt (Äá»•i áº£nh, tÃªn, giÃ¡)
        }
        else
        {
            Debug.Log("NghÃ¨o! Cáº§n " + currentCatPrice + " vÃ ng!");
            if (AudioManager.Instance != null) AudioManager.Instance.PlayError();
        }
    }

    private void UpdateShopUI()
    {
        // Váº½ láº¡i chá»¯ Tiá»n
        if (priceText != null) priceText.text = currentCatPrice.ToString();

        // Váº½ láº¡i chá»¯ TÃªn
        if (nameText != null && shopCatNames.Length > currentShopLevel)
            nameText.text = shopCatNames[currentShopLevel];

        // Váº½ láº¡i cá»¥c áº¢nh
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
                if (AudioManager.Instance != null) AudioManager.Instance.PlayMerge();
                originalSlot.currentTurret = null;
                int nextLevel = targetTurret.turretLevel; // (tÆ°Æ¡ng Ä‘Æ°Æ¡ng vá»›i Level Index hiá»‡n táº¡i + 1 vÃ¬ bÃªn trong m code sáºµn turretLevel lÃ  1, 2, 3 rá»“i)

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

    // Äáºº MÃˆO KHI MUA Báº°NG TIá»€N (Sáº½ láº¥y theo Current Shop Level)
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

                return true; // BÃ¡o cho GoldManager biáº¿t lÃ  Ä‘Ã£ Ä‘áº» thÃ nh cÃ´ng!
            }
        }
        return false; // BÃ¡o lÃ  háº¿t chá»— trá»‘ng rá»“i
    }

    // Äáºº MÃˆO FREE DO Há»† THá»NG / SPACE (LuÃ´n Ä‘áº» con level 1 - Index 0)
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
