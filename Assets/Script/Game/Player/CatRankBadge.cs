using UnityEngine;

public class CatRankBadge : MonoBehaviour
{
    public string catID = "Cat_01"; // Nhớ gõ cho khớp với ngoài Hub

    [Tooltip("Kéo 4 ảnh huy hiệu vào đây: 0(Gốc), 1(Lần 1-2), 2(Lần 3-4), 3(Lần 5)")]
    public Sprite[] rankSprites;

    private SpriteRenderer badgeRenderer;

    void Start()
    {
        badgeRenderer = GetComponent<SpriteRenderer>();
        UpdateRankVisual();
    }

    public void UpdateRankVisual()
    {
        if (badgeRenderer == null || rankSprites == null || rankSprites.Length == 0) return;

        int clickCount = PlayerPrefs.GetInt("UpgradeCount_" + catID, 0);
        int spriteIndex = 0;

        // Bấm lần 1, lần 2 -> Đổi ảnh 1
        if (clickCount == 1 || clickCount == 2)
        {
            spriteIndex = 1;
        }
        // Bấm lần 3, lần 4 -> Đổi ảnh 2
        else if (clickCount == 3 || clickCount == 4)
        {
            spriteIndex = 2;
        }
        // Bấm lần 5 -> Đổi ảnh 3
        else if (clickCount >= 5)
        {
            spriteIndex = 3;
        }

        spriteIndex = Mathf.Clamp(spriteIndex, 0, rankSprites.Length - 1);
        badgeRenderer.sprite = rankSprites[spriteIndex];
    }
}