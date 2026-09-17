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
        }

        // GIAI ĐOẠN 3: THẢ CHUỘT -> XỬ LÝ GỘP / ĐỔI CHỖ
        if (Input.GetMouseButtonUp(0) && selectedTurret != null)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Slot"));

            if (hit.collider != null)
            {
                Slot targetSlot = hit.collider.GetComponent<Slot>();
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
        // Lấy đúng vị trí của ô đó cộng với bù trừ tọa độ riêng của chính nó
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