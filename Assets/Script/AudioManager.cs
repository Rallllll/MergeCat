using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Quản lý toàn bộ hệ thống âm thanh (BGM & SFX) trong MergeCat.
/// Tự động tồn tại xuyên suốt các Scene, phát nhạc nền tương ứng từng Scene và hỗ trợ Pool SFX.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Nhạc Nền (BGM)")]
    public AudioClip bgmStart;
    public AudioClip bgmMainHub;
    public AudioClip bgmGame;

    [Header("Hiệu Ứng Âm Thanh (SFX)")]
    public AudioClip sfxBaseHit;
    public AudioClip sfxBoom;
    public AudioClip sfxClick;
    public AudioClip sfxCoin;
    public AudioClip sfxErrorLocked;
    public AudioClip sfxGemClaim;
    public AudioClip sfxLose;
    public AudioClip sfxMerge;
    public AudioClip sfxShoot;
    public AudioClip sfxUpgrade;
    public AudioClip sfxWin;

    [Header("Cài đặt Âm lượng Mặc định")]
    [Range(0f, 1f)] public float defaultMusicVolume = 0.6f;
    [Range(0f, 1f)] public float defaultSoundVolume = 1f;

    private AudioSource bgmSource;
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private const int SFX_POOL_SIZE = 8;
    private int currentSfxIndex = 0;

    private const string PREF_MUSIC = "MergeCat_MusicEnabled";
    private const string PREF_SOUND = "MergeCat_SoundEnabled";

    // Tự động khởi tạo AudioManager ngay trước khi bất kỳ Scene nào tải lên
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null)
        {
#if UNITY_EDITOR
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/AudioManager.prefab");
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab);
                go.name = "AudioManager";
                DontDestroyOnLoad(go);
                return;
            }
#endif
            GameObject audioObj = new GameObject("AudioManager");
            audioObj.AddComponent<AudioManager>();
            DontDestroyOnLoad(audioObj);
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        LoadClipsFromAssetsIfEmpty();
#endif

        InitAudioSources();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Khi game vừa bật, phát nhạc tương ứng Scene hiện tại
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    private void InitAudioSources()
    {
        // 1. Tạo AudioSource cho Nhạc nền
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = defaultMusicVolume;
        bgmSource.mute = !IsMusicEnabled();

        // 2. Tạo Pool AudioSource cho SFX (tránh cắt tiếng khi bắn nhiều quái cùng lúc)
        for (int i = 0; i < SFX_POOL_SIZE; i++)
        {
            AudioSource sfxSrc = gameObject.AddComponent<AudioSource>();
            sfxSrc.loop = false;
            sfxSrc.playOnAwake = false;
            sfxSrc.volume = defaultSoundVolume;
            sfxSrc.mute = !IsSoundEnabled();
            sfxPool.Add(sfxSrc);
        }
    }

    // Tự động chuyển BGM khi chuyển Scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    public void PlayBGMForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Start":
                PlayBGM(bgmStart);
                break;
            case "MainHub":
                PlayBGM(bgmMainHub);
                break;
            case "Game":
                PlayBGM(bgmGame);
                break;
            default:
                break;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.mute = !IsMusicEnabled();
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    // Phát âm thanh SFX chung có hỗ trợ ngẫu nhiên Pitch (đổi tần số nhẹ để âm thanh tự nhiên)
    public void PlaySFX(AudioClip clip, float volume = 1f, bool randomizePitch = false)
    {
        if (clip == null || !IsSoundEnabled()) return;

        AudioSource availableSource = GetAvailableSFXSource();
        if (availableSource != null)
        {
            availableSource.pitch = randomizePitch ? Random.Range(0.92f, 1.08f) : 1f;
            availableSource.volume = defaultSoundVolume * volume;
            availableSource.PlayOneShot(clip);
        }
    }

    private AudioSource GetAvailableSFXSource()
    {
        for (int i = 0; i < sfxPool.Count; i++)
        {
            if (!sfxPool[i].isPlaying) return sfxPool[i];
        }

        // Nếu tất cả source đều bận, lấy xoay vòng
        currentSfxIndex = (currentSfxIndex + 1) % sfxPool.Count;
        return sfxPool[currentSfxIndex];
    }

    // ============================================
    // CÁC HÀM TIỆN ÍCH GỌI NHANH CHO GAME
    // ============================================

    public void PlayClick() => PlaySFX(sfxClick, 0.85f, false);
    public void PlayTabSwitch() => PlayClick(); // Chuyển tab dùng chung tiếng click như yêu cầu
    public void PlayShoot() => PlaySFX(sfxShoot, 0.75f, true);
    public void PlayBaseHit() => PlaySFX(sfxBaseHit, 1f, false);
    public void PlayBoom() => PlaySFX(sfxBoom, 0.9f, true);
    public void PlayCoin() => PlaySFX(sfxCoin, 0.9f, true);
    public void PlayError() => PlaySFX(sfxErrorLocked, 0.85f, false);
    public void PlayGemClaim() => PlaySFX(sfxGemClaim, 0.95f, false);
    public void PlayLose() => PlaySFX(sfxLose, 1f, false);
    public void PlayMerge() => PlaySFX(sfxMerge, 1.1f, false);
    public void PlayUpgrade() => PlaySFX(sfxUpgrade, 0.95f, false);
    public void PlayWin() => PlaySFX(sfxWin, 1f, false);

    // ============================================
    // BẬT / TẮT ÂM THANH (SETTING)
    // ============================================

    public bool IsMusicEnabled() => PlayerPrefs.GetInt(PREF_MUSIC, 1) == 1;

    public void SetMusicEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(PREF_MUSIC, enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (bgmSource != null)
        {
            bgmSource.mute = !enabled;
            if (enabled && !bgmSource.isPlaying && bgmSource.clip != null)
            {
                bgmSource.Play();
            }
        }
    }

    public void ToggleMusic() => SetMusicEnabled(!IsMusicEnabled());

    public bool IsSoundEnabled() => PlayerPrefs.GetInt(PREF_SOUND, 1) == 1;

    public void SetSoundEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(PREF_SOUND, enabled ? 1 : 0);
        PlayerPrefs.Save();

        foreach (var src in sfxPool)
        {
            if (src != null) src.mute = !enabled;
        }
    }

    public void ToggleSound() => SetSoundEnabled(!IsSoundEnabled());

#if UNITY_EDITOR
    // Tự động load các file âm thanh từ thư mục Assets/Sound vào nếu chưa được gán Inspector
    public void LoadClipsFromAssetsIfEmpty()
    {
        if (bgmStart == null) bgmStart = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/BGM/start.wav");
        if (bgmMainHub == null) bgmMainHub = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/BGM/main_hub.wav");
        if (bgmGame == null) bgmGame = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/BGM/in_game_background.wav");

        if (sfxBaseHit == null) sfxBaseHit = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_base_hit.mp3");
        if (sfxBoom == null) sfxBoom = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_boom.wav");
        if (sfxClick == null) sfxClick = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_click.ogg");
        if (sfxCoin == null) sfxCoin = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_coin.ogg");
        if (sfxErrorLocked == null) sfxErrorLocked = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_error_locked.ogg");
        if (sfxGemClaim == null) sfxGemClaim = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_gem_claim.wav");
        if (sfxLose == null) sfxLose = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_lose.mp3");
        if (sfxMerge == null) sfxMerge = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_merge.ogg");
        if (sfxShoot == null) sfxShoot = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_shoot.mp3");
        if (sfxUpgrade == null) sfxUpgrade = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_upgrade.ogg");
        if (sfxWin == null) sfxWin = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/sfx_win.mp3");
    }
#endif
}
