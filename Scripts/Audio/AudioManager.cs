using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton Pattern
    
    public static AudioManager Instance { get; private set; }
    
    #endregion
    
    #region Audio Sources
    
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    
    #endregion
    
    #region Audio Settings
    
    [Header("Audio Settings")]
    public float bgmVolume = 0.7f;
    public float sfxVolume = 0.8f;
    
    #endregion
    
    #region Private Variables
    
    private AudioClip currentBGMClip;
    private bool isBGMPlaying = false;
    
    #endregion
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        InitializeSingleton();
        InitializeAudioSources();
    }
    
    private void Start()
    {
        // SettingsManager에서 볼륨 설정 로드
        LoadVolumeFromSettings();
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeSingleton()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager 초기화 완료");
        }
        else
        {
            // 이미 인스턴스가 존재하는 경우 중복 생성 방지
            Debug.LogWarning("AudioManager가 이미 존재합니다. 중복 생성 방지.");
            Destroy(gameObject);
        }
    }
    
    private void InitializeAudioSources()
    {
        // BGM AudioSource 초기화
        if (bgmSource == null)
        {
            GameObject bgmObject = new GameObject("BGM Source");
            bgmObject.transform.SetParent(transform);
            bgmSource = bgmObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.volume = bgmVolume;
        }
        
        // SFX AudioSource 초기화
        if (sfxSource == null)
        {
            GameObject sfxObject = new GameObject("SFX Source");
            sfxObject.transform.SetParent(transform);
            sfxSource = sfxObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }
        
        Debug.Log("AudioSource 초기화 완료");
    }
    
    #endregion
    
    #region Volume Control
    
    /// <summary>
    /// BGM 볼륨 설정
    /// </summary>
    /// <param name="volume">볼륨 값 (0.0 ~ 1.0)</param>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
        
        // SettingsManager에 반영
        if (SettingsManager.Instance != null && SettingsManager.Instance.CurrentSettings != null)
        {
            SettingsManager.Instance.CurrentSettings.bgmVolume = bgmVolume;
        }
        
        Debug.Log($"BGM 볼륨이 {bgmVolume:F2}로 설정되었습니다.");
    }
    
    /// <summary>
    /// SFX 볼륨 설정
    /// </summary>
    /// <param name="volume">볼륨 값 (0.0 ~ 1.0)</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        
        // SettingsManager에 반영
        if (SettingsManager.Instance != null && SettingsManager.Instance.CurrentSettings != null)
        {
            SettingsManager.Instance.CurrentSettings.sfxVolume = sfxVolume;
        }
        
        Debug.Log($"SFX 볼륨이 {sfxVolume:F2}로 설정되었습니다.");
    }
    
    /// <summary>
    /// SettingsManager에서 볼륨 설정 로드
    /// </summary>
    public void LoadVolumeFromSettings()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.CurrentSettings != null)
        {
            SettingsData settings = SettingsManager.Instance.CurrentSettings;
            
            SetBGMVolume(settings.bgmVolume);
            SetSFXVolume(settings.sfxVolume);
            
            Debug.Log("SettingsManager에서 볼륨 설정을 로드했습니다.");
        }
        else
        {
            Debug.LogWarning("SettingsManager가 없거나 설정 데이터가 없습니다.");
        }
    }
    
    #endregion
    
    #region BGM Control
    
    /// <summary>
    /// BGM 재생
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("BGM 클립이 null입니다.");
            return;
        }
        
        if (bgmSource == null)
        {
            Debug.LogError("BGM AudioSource가 없습니다.");
            return;
        }
        
        // 현재 BGM과 다른 경우에만 재생
        if (currentBGMClip != clip)
        {
            currentBGMClip = clip;
            bgmSource.clip = clip;
            bgmSource.Play();
            isBGMPlaying = true;
            
            Debug.Log($"BGM 재생: {clip.name}");
        }
        else if (!bgmSource.isPlaying)
        {
            // 같은 클립이지만 재생 중이 아닌 경우 재생
            bgmSource.Play();
            isBGMPlaying = true;
            
            Debug.Log($"BGM 재개: {clip.name}");
        }
    }
    
    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
            isBGMPlaying = false;
            Debug.Log("BGM이 정지되었습니다.");
        }
    }
    
    /// <summary>
    /// BGM 일시정지
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Pause();
            isBGMPlaying = false;
            Debug.Log("BGM이 일시정지되었습니다.");
        }
    }
    
    /// <summary>
    /// BGM 재개
    /// </summary>
    public void ResumeBGM()
    {
        if (bgmSource != null && !bgmSource.isPlaying && currentBGMClip != null)
        {
            bgmSource.UnPause();
            isBGMPlaying = true;
            Debug.Log("BGM이 재개되었습니다.");
        }
    }
    
    #endregion
    
    #region SFX Control
    
    /// <summary>
    /// 효과음 재생
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SFX 클립이 null입니다.");
            return;
        }
        
        if (sfxSource == null)
        {
            Debug.LogError("SFX AudioSource가 없습니다.");
            return;
        }
        
        // OneShot으로 재생
        sfxSource.PlayOneShot(clip, sfxVolume);
        
        Debug.Log($"SFX 재생: {clip.name}");
    }
    
    /// <summary>
    /// 효과음 재생 (볼륨 지정)
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    /// <param name="volume">볼륨 값 (0.0 ~ 1.0)</param>
    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            Debug.LogWarning("SFX 클립이 null입니다.");
            return;
        }
        
        if (sfxSource == null)
        {
            Debug.LogError("SFX AudioSource가 없습니다.");
            return;
        }
        
        // 볼륨 범위 제한
        volume = Mathf.Clamp01(volume);
        
        // OneShot으로 재생
        sfxSource.PlayOneShot(clip, volume);
        
        Debug.Log($"SFX 재생: {clip.name} (볼륨: {volume:F2})");
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// BGM이 재생 중인지 확인
    /// </summary>
    /// <returns>재생 중이면 true</returns>
    public bool IsBGMPlaying()
    {
        return bgmSource != null && bgmSource.isPlaying;
    }
    
    /// <summary>
    /// 현재 재생 중인 BGM 클립 반환
    /// </summary>
    /// <returns>현재 BGM 클립</returns>
    public AudioClip GetCurrentBGMClip()
    {
        return currentBGMClip;
    }
    
    /// <summary>
    /// 현재 BGM 클립 이름 반환
    /// </summary>
    /// <returns>BGM 클립 이름</returns>
    public string GetCurrentBGMName()
    {
        return currentBGMClip != null ? currentBGMClip.name : "None";
    }
    
    /// <summary>
    /// 모든 오디오 일시정지
    /// </summary>
    public void PauseAllAudio()
    {
        if (bgmSource != null)
            bgmSource.Pause();
        if (sfxSource != null)
            sfxSource.Pause();
        
        isBGMPlaying = false;
        Debug.Log("모든 오디오가 일시정지되었습니다.");
    }
    
    /// <summary>
    /// 모든 오디오 재개
    /// </summary>
    public void ResumeAllAudio()
    {
        if (bgmSource != null)
            bgmSource.UnPause();
        if (sfxSource != null)
            bgmSource.UnPause();
        
        isBGMPlaying = bgmSource != null && bgmSource.isPlaying;
        Debug.Log("모든 오디오가 재개되었습니다.");
    }
    
    /// <summary>
    /// 모든 오디오 정지
    /// </summary>
    public void StopAllAudio()
    {
        if (bgmSource != null)
            bgmSource.Stop();
        if (sfxSource != null)
            sfxSource.Stop();
        
        isBGMPlaying = false;
        currentBGMClip = null;
        Debug.Log("모든 오디오가 정지되었습니다.");
    }
    
    /// <summary>
    /// 오디오 정보 반환
    /// </summary>
    /// <returns>오디오 정보 문자열</returns>
    public string GetAudioInfo()
    {
        return $"BGM 볼륨: {bgmVolume:F2}\n" +
               $"SFX 볼륨: {sfxVolume:F2}\n" +
               $"현재 BGM: {GetCurrentBGMName()}\n" +
               $"BGM 재생 중: {(IsBGMPlaying() ? "예" : "아니오")}";
    }
    
    #endregion
    
    #region Settings Integration
    
    /// <summary>
    /// SettingsManager와 연동하여 볼륨 설정 적용
    /// </summary>
    public void ApplySettingsVolume()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.CurrentSettings != null)
        {
            SettingsData settings = SettingsManager.Instance.CurrentSettings;
            
            SetBGMVolume(settings.bgmVolume);
            SetSFXVolume(settings.sfxVolume);
            
            Debug.Log("SettingsManager에서 볼륨 설정을 적용했습니다.");
        }
    }
    
    #endregion
} 