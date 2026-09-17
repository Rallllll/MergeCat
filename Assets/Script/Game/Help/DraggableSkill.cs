using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Cài đặt Vật phẩm")]
    public GameObject skillPrefab;
    public int skillCount = 5;

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
        if (skillCount <= 0) return;

        // CỘNG THÊM DRAG OFFSET NGAY LÚC ĐẺ RA
        ghostItem = Instantiate(skillPrefab, GetMouseWorldPosition() + dragOffset, Quaternion.identity);
        ghostSr = ghostItem.GetComponent<SpriteRenderer>();

        Collider2D col = ghostItem.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Melee meleeScript = ghostItem.GetComponent<Melee>();
        if (meleeScript != null) meleeScript.enabled = false;

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
            // CỘNG THÊM DRAG OFFSET LÚC KÉO ĐI
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
                // CỘNG THÊM SNAP OFFSET VÀO TRỤC Y KHI THẢ XUỐNG
                Vector3 snappedPos = ghostItem.transform.position;
                snappedPos.y = hit.transform.position.y + snapOffset.y;
                ghostItem.transform.position = snappedPos;

                ghostSr.color = Color.white;
                Collider2D col = ghostItem.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;

                Melee meleeScript = ghostItem.GetComponent<Melee>();
                if (meleeScript != null) meleeScript.enabled = true;

                skillCount--;
            }
            else
            {
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