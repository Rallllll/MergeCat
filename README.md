# 🐱 MergeCat - 2D Casual Merge & Tower Defense

![Unity 6](https://img.shields.io/badge/Unity-6000.3.14f1-black?logo=unity)
![Render Pipeline](https://img.shields.io/badge/Pipeline-Universal%20RP%20(URP)-blue)
![Language](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)
![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20Mobile-green)

> **MergeCat** là một tựa game 2D kết hợp giữa lối chơi gộp thú vị (**Merge**) và thủ thành theo làn (**Lane-based Tower Defense**). Người chơi nuôi dưỡng, nâng cấp và gộp các chú mèo chiến binh để bảo vệ căn cứ trước từng đợt tấn công dồn dập của quái vật.

---

## 🎮 1. Cơ chế Gameplay chính (Core Mechanics)

### 🐾 Cơ chế Gộp Mèo (Merge System)
* **Kéo thả tự do:** Kéo các chú mèo cùng cấp độ vào nhau trên các ô (`Slot`) để tiến hóa thành mèo cấp cao hơn với sức mạnh vượt trội.
* **Cửa hàng trong trận:** Mua thêm mèo bằng Vàng (`Gold`). Cửa hàng tự động nâng cấp sau một số lượng lượt mua để bán mèo cấp cao hơn.
* **Tự động triệu hồi:** Tính năng tự động sinh mèo Level 1 theo chu kỳ thời gian hỗ trợ người chơi.
* **Tái chế / Bán mèo:** Kéo mèo vào thùng rác để hoàn lại một phần vàng tương ứng với cấp độ mèo.

### ⚔️ Chiến đấu & Thủ thành (Tower Defense)
* **Chiến đấu theo làn (Lane Defense):** Đặt mèo vào các vị trí chiến đấu (`Combat Slots`) để ngăn chặn quái vật tràn vào theo làn đường.
* **Tấn công tự động:** Mèo tự động dò tìm mục tiêu bằng Raycast và xả đạn khi phát hiện quái vật trong tầm ngắm.
* **Hiệu ứng sát thương nổi (Floating Damage Text):** Hiển thị trực quan lượng sát thương mỗi khi đạn bắn trúng quái vật bằng TextMeshPro.
* **Căn cứ & Sửa chữa (Base Health):** Căn cứ có lượng máu riêng. Người chơi có thể dùng vàng để sửa chữa và bơm máu tức thì trong lúc chiến đấu.

### 💣 Kỹ năng & Vật phẩm bổ trợ (Skills & Consumables)
* **Bom nổ lan (Bomb):** Đặt bom nổ diện rộng (AOE) tiêu diệt cụm quái vật.
* **Lính cận chiến (Melee Defender):** Triệu hồi lính chặn đường quái vật trực tiếp trên làn.
* **Bẫy (Trap):** Đặt bẫy gây hiệu ứng bất lợi cho quái vật đi qua.

---

## 🏰 2. Hệ thống Sảnh & Nâng cấp (MainHub & Meta Progression)

* **🗺️ Chọn ải & Tiến trình (Level Progression):**
  * Hệ thống mở khóa từng ải tuần tự (Ải 1 ➔ Ải 2 ➔ Ải 3...).
  * Tự động lưu trạng thái màn chơi (*Locked, Unlocked, Finished*) qua `PlayerPrefs`.
* **⭐ Nâng cấp Rank Mèo (Cat Upgrade System):**
  * Nâng cấp tiến trình cho từng giống mèo (từ 0 đến 5 vạch).
  * Hiển thị huy hiệu cấp bậc (`CatRankBadge`) thay đổi theo từng mốc nâng cấp.
* **💎 Cửa hàng & Kinh tế (Economy System):**
  * **Gem Shop:** Nhận Kim cương miễn phí theo thời gian hồi chiêu (Cooldown 60s có hiệu ứng phủ màng đen mờ đếm ngược).
  * **Extra Shop:** Mua các vật phẩm bổ trợ bằng Kim cương và trang bị tối đa 3 món mang vào trận đấu.
* **📑 Menu đa tab:** Chuyển đổi mượt mà giữa các tab Nâng cấp, Cửa hàng, Chọn ải và Cài đặt.

---

## 🎧 3. Hệ thống Âm thanh Toàn diện (Audio System)

Dự án sở hữu một kiến trúc âm thanh trung tâm (`AudioManager`) hoạt động xuyên suốt toàn bộ vòng đời game:

* **Nhạc nền theo từng Scene (Scene-aware BGM):**
  * Scene `Start`: Nhạc mở đầu nhẹ nhàng (`start.wav`).
  * Scene `MainHub`: Giai điệu sảnh thư giãn, vui nhộn (`main_hub.wav`).
  * Scene `Game`: Nhạc chiến đấu sôi động, kịch tính (`in_game_background.wav`).
* **Pool hiệu ứng âm thanh (SFX Pool):**
  * Hỗ trợ 8 kênh phát đồng thời, chống giật rè hoặc ngắt tiếng khi có hàng chục viên đạn và quái nổ cùng lúc.
  * **Biến thiên cao độ (Pitch Randomizer):** Tiếng bắn đạn và tiếng nổ tự động đổi tần số nhẹ (±8%), giúp âm thanh luôn tự nhiên, không gây nhàm chán.
  * Đầy đủ hiệu ứng: Tiếng gộp mèo (*Merge*), nổ quái (*Boom*), tiếng tiền (*Coin*), nhận quà (*Gem Claim*), thắng/thua trận (*Win/Lose*), cảnh báo nhà chính (*Base Hit*), báo lỗi (*Error Locked*).
* **Cài đặt Âm thanh:**
  * Bật/Tắt độc lập giữa Nhạc nền (Music) và Hiệu ứng (Sound).
  * Trạng thái cài đặt được lưu vĩnh viễn vào bộ nhớ máy qua `PlayerPrefs`.

---

## 🛠️ 4. Kiến trúc Kỹ thuật & Design Patterns

| Pattern / Kỹ thuật | Ứng dụng trong dự án |
| :--- | :--- |
| **Singleton Pattern** | `AudioManager`, `UIManager`, `CurrencyManager`, `WaveManager`, `DragManager` |
| **Object Pooling** | Tối ưu hóa hiệu năng, triệt tiêu Garbage Collection cho đạn (`MultiBulletPool`), quái vật (`MultiEnemyPool`), vụ nổ (`ExplosionPool`), số sát thương (`DamageTextPool`), và âm thanh (`SFX Pool`) |
| **Data Persistence** | Lưu trữ vàng, gem, trạng thái ải, cấp bậc mèo qua `PlayerPrefs` an toàn |
| **Decoupled Architecture** | Các module giao tiếp lỏng lẻo, dễ bảo trì và mở rộng tính năng mới |

---

## 📁 5. Cấu trúc thư mục Dự án (Project Structure)

```text
Assets/
├── Animation/           # Animation cho quái vật, đạn và hiệu ứng
├── Asset/               # Sprite đồ họa (UI, nhân vật, bản đồ, nút bấm)
├── Font/                # Font chữ tùy biến (PassionOne-Bold SDF)
├── Prefab/              # Prefabs tái sử dụng (Mèo, Quái, Đạn, FX, Audio)
├── Scenes/              # 3 màn hình chính: Start, MainHub, Game
├── Script/              # Toàn bộ mã nguồn C#
│   ├── AudioManager.cs  # Hệ thống quản lý âm thanh trung tâm
│   ├── Game/            # Logic trong trận (Máu căn cứ, Enemy, Player, Turret, UI)
│   ├── Map+Shop+.._/    # Logic ngoài sảnh (Nâng cấp, Gem Shop, Chọn ải, Menu)
│   ├── Scripts/Player/  # Logic kéo thả, slot, pooling
│   └── Start/           # Chuyển cảnh mở đầu
├── Sound/               # Thư viện âm thanh
│   ├── BGM/             # Nhạc nền các scene
│   └── SFX/             # Hiệu ứng tiếng động gameplay & UI
└── TextMesh Pro/        # Thư viện TMP cho UI & 3D Text
```

---

## 🚀 6. Hướng dẫn Cài đặt & Chạy Game (Getting Started)

1. **Yêu cầu hệ thống:**
   * **Unity Editor:** Phiên bản `Unity 6 (6000.3.14f1)` hoặc mới hơn.
   * **Packages:** Universal RP (URP), TextMeshPro, 2D Animation.
2. **Cách mở dự án:**
   * Mở **Unity Hub** ➔ Chọn **Add** ➔ Trỏ tới thư mục dự án `MergeCat`.
   * Chờ Unity hoàn tất tải và import assets.
3. **Chạy game:**
   * Điều hướng đến thư mục: `Assets/Scenes/`.
   * Mở Scene **`Start.unity`** (hoặc `MainHub.unity` / `Game.unity`).
   * Nhấn nút **Play ▶️** trên thanh công cụ Unity để trải nghiệm!

---

## 👥 Tác giả
* **MergeCat Development Team**
* Dự án được phát triển và tối ưu bằng Unity C#.
