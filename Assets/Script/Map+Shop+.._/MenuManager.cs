using UnityEngine;
using UnityEngine.UI; // Bắt buộc phải có để điều khiển Image của nút

public class MenuManager : MonoBehaviour
{
    [Header("Cài đặt Tab & Panel (Phải xếp cùng thứ tự)")]
    public GameObject[] allPanels;    // Chứa các màn hình (Panel)
    public Image[] allTabImages;      // Chứa component Image của các nút bấm

    [Header("Hình ảnh trạng thái")]
    public Sprite activeSprite;       // Kéo hình nút màu xanh vào đây (BtnTab_Ac)
    public Sprite inactiveSprite;     // Kéo hình nút màu xám vào đây (BtnTab_on)

    // Hàm này sẽ nhận vào Số thứ tự (Index) của nút được bấm (0, 1, 2, 3...)
    public void OpenTab(int tabIndex)
    {
        for (int i = 0; i < allPanels.Length; i++)
        {
            if (i == tabIndex)
            {
                // Bật Panel hiện tại và đổi nút thành màu xanh
                allPanels[i].SetActive(true);
                allTabImages[i].sprite = activeSprite;
            }
            else
            {
                // Tắt các Panel khác và đổi nút thành màu xám
                allPanels[i].SetActive(false);
                allTabImages[i].sprite = inactiveSprite;
            }
        }
    }
}