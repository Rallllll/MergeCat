using UnityEngine;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;

    [Header("Cài đặt Hệ thống")]
    public GameObject[] turretPrefabs;
    public Slot[] allSlots;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnTestTurret();
        }

        // GIAI ĐOẠN 1: BẤM CHUỘT -> NHẶT SÚNG
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

        // GIAI ĐOẠN 2: GIỮ CHUỘT -> KÉO SÚNG THEO
        if (Input.GetMouseButton(0) && selectedTurret != null)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            selectedTurret.transform.position = mousePos;

            // KIỂM TRA ĐÈ LÊN THÙNG RÁC ĐỂ ĐỔI MÀU
            RaycastHit2D hitTrash = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Trash"));
            SpriteRenderer sr = selectedTurret.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                if (hitTrash.collider != null)
                {
                    sr.color = new Color(1f, 0f, 0f, 0.5f); // Đổi thành màu Đỏ Mờ
                }
                else
                {
                    sr.color = Color.white; // Trả về màu gốc
                }
            }
        }

        // GIAI ĐOẠN 3: THẢ CHUỘT -> XỬ LÝ GỘP / ĐỔI CHỖ / XOÁ
        if (Input.GetMouseButtonUp(0) && selectedTurret != null)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            // 1. KIỂM TRA NẾU THẢ VÀO THÙNG RÁC THÌ XOÁ LUÔN
            RaycastHit2D hitTrash = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Trash"));
            if (hitTrash.collider != null)
            {
                originalSlot.currentTurret = null;  // Giải phóng ô slot cũ
                Destroy(selectedTurret.gameObject); // Xoá vĩnh viễn con mèo
                selectedTurret = null;              // Reset trạng thái tay cầm
                return;                             // Cắt ngang, không chạy code thả xuống ô bên dưới nữa
            }

            // Đảm bảo trả lại màu trắng bình thường nếu thả trượt ra ngoài
            SpriteRenderer sr = selectedTurret.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.white;

            // 2. LOGIC THẢ XUỐNG SLOT BÌNH THƯỜNG CỦA M
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

    // --- HÀM TÍNH TOÁN VỊ TRÍ ĐÃ ĐƯỢC TỐI GIẢN ---
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
                int nextLevel = targetTurret.turretLevel;

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

    public bool SpawnBoughtTurret()
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

                return true;
            }
        }
        return false;
    }

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

                Debug.Log($"[Test] Đã đẻ 1 súng Level 1 tại ô: {slot.gameObject.name}");
                return;
            }
        }
        Debug.Log("[Test] Hết ô trống rồi, không đẻ được nữa!");
    }
}