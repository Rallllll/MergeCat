using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Bắt buộc thêm dòng này để check trạng thái Button

public class DraggableSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Liên kết Kho Đồ (Đồng bộ Hub)")]
    public GameExtraButton extraButtonRef; // Kéo script GameExtraButton vào đây

    [Header("Cài đặt Vật phẩm")]
    public GameObject skillPrefab;

    [Header("Tinh chỉnh vị trí (Offset)")]
    [Tooltip("Chỉnh số này để mũi tên chuột nằm ngay giữa thân lúc đang kéo")]
    public Vector3 dragOffset = new Vector3(0, -0.5f, 0);

    [Tooltip("Chỉnh độ cao thấp của lính khi thả xuống bám vào Lane")]
    public Vector3 snapOffset = new Vector3(0, 0.25f, 0);

    [Header("Màu sắc hiển thị (UI)")]
    public Color validColor = new Color(0f, 1f, 0f, 0.5f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.5f);
    public Color laneHighlightColor = new Color(0f, 0.5f, 1f, 0.3f);

    private GameObject ghostItem;
    private Camera mainCam;
    private SpriteRenderer ghostSr;
    private GameObject[] lanes;
    private Color[] originalLaneColors;

    void Start()
    {
        mainCam = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // CHẶN KÉO NẾU HẾT HÀNG (Nút đã bị mờ đi)
        Button btn = GetComponent<Button>();
        if (btn != null && !btn.interactable) return;

        ghostItem = Instantiate(skillPrefab, GetMouseWorldPosition() + dragOffset, Quaternion.identity);
        ghostSr = ghostItem.GetComponent<SpriteRenderer>();

        Collider2D col = ghostItem.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Tắt logic hoạt động lúc đang kéo lơ lửng
        Melee meleeScript = ghostItem.GetComponent<Melee>();
        if (meleeScript != null) meleeScript.enabled = false;

        Bomb bombScript = ghostItem.GetComponent<Bomb>();
        if (bombScript != null) bombScript.enabled = false;

        lanes = GameObject.FindGameObjectsWithTag("Road");
        originalLaneColors = new Color[lanes.Length];

        for (int i = 0; i < lanes.Length; i++)
        {
            SpriteRenderer laneSr = lanes[i].GetComponent<SpriteRenderer>();
            if (laneSr != null)
            {
                originalLaneColors[i] = laneSr.color;
                laneSr.color = laneHighlightColor;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostItem != null)
        {
            ghostItem.transform.position = GetMouseWorldPosition() + dragOffset;
            RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero, 0f, LayerMask.GetMask("Road"));

            if (hit.collider != null) ghostSr.color = validColor;
            else ghostSr.color = invalidColor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostItem != null)
        {
            // Trả lại màu gốc cho Road
            for (int i = 0; i < lanes.Length; i++)
            {
                if (lanes[i] != null)
                {
                    SpriteRenderer laneSr = lanes[i].GetComponent<SpriteRenderer>();
                    if (laneSr != null) laneSr.color = originalLaneColors[i];
                }
            }

            RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero, 0f, LayerMask.GetMask("Road"));

            if (hit.collider != null)
            {
                // KIỂM TRA KHO VÀ TRỪ DATA TRƯỚC KHI CHO PHÉP THẢ
                if (extraButtonRef != null && extraButtonRef.TryUseItem())
                {
                    // Kéo thả thành công -> Trừ tiền/số lượng thành công -> Sinh ra thật
                    Vector3 snappedPos = ghostItem.transform.position;
                    snappedPos.y = hit.transform.position.y + snapOffset.y;
                    ghostItem.transform.position = snappedPos;

                    ghostSr.color = Color.white;
                    Collider2D col = ghostItem.GetComponent<Collider2D>();
                    if (col != null) col.enabled = true;

                    // Bật lại logic hoạt động cho con lính hoặc quả bom
                    Melee meleeScript = ghostItem.GetComponent<Melee>();
                    if (meleeScript != null) meleeScript.enabled = true;

                    Bomb bombScript = ghostItem.GetComponent<Bomb>();
                    if (bombScript != null) bombScript.enabled = true;
                }
                else
                {
                    // Lỗi gì đó (chưa kéo ExtraButtonRef vào) hoặc lách luật -> Xóa bóng
                    Destroy(ghostItem);
                }
            }
            else
            {
                // Thả sai chỗ (không phải Road) -> Xóa bóng (chưa gọi TryUseItem nên không bị mất đồ)
                Destroy(ghostItem);
            }

            ghostItem = null;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCam.transform.position.z);
        return mainCam.ScreenToWorldPoint(mousePos);
    }
}