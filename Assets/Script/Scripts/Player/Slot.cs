using UnityEngine;

public class Slot : MonoBehaviour
{
    public Turret currentTurret;

    [Header("Chỉ dành cho Combat Slot")]
    public int slotLaneID = -1;

    [Header("Tinh chỉnh vị trí mèo đứng (Offset)")]
    public Vector3 customOffset = new Vector3(0, 0.15f, 0);

    public bool IsEmpty()
    {
        return currentTurret == null;
    }

    // --- THÊM PHẦN NÀY ĐỂ VẼ PREVIEW TRỰC QUAN ---
    private void OnDrawGizmos()
    {
        // 1. Tính toán vị trí con mèo sẽ xuất hiện
        Vector3 previewPosition = transform.position + customOffset;

        // 2. Chọn màu (Màu vàng cho nổi bật)
        Gizmos.color = Color.yellow;

        // 3. Vẽ một vòng tròn nhỏ tượng trưng cho TÂM của con mèo
        Gizmos.DrawWireSphere(previewPosition, 0.2f);

        // 4. Vẽ một đường thẳng nối từ gốc của Slot lên cái vòng tròn đó để dễ nhìn
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, previewPosition);
    }
}