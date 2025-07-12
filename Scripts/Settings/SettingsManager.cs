using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    #region Singleton Pattern
    
    public static SettingsManager Instance { get; private set; }
    
    #endregion
    
    #region Properties
    
    [Header("Settings Data")]
    public SettingsData CurrentSettings { get; private set; }
    
    #endregion
    
    #region PlayerPrefs Keys
    
    // PlayerPrefs 키 상수
    private const string KEY_BGM_VOLUME = "BGMVolume";
    private const string KEY_SFX_VOLUME = "SFXVolume";
    private const string KEY_MASTER_AUDIO = "MasterAudio";
    private const string KEY_LANGUAGE = "Language";
    private const string KEY_GRAPHICS_QUALITY = "GraphicsQuality";
    private const string KEY_TARGET_FRAME_RATE = "TargetFrameRate";
    private const string KEY_VSYNC = "VSync";
    private const string KEY_SCREEN_BRIGHTNESS = "ScreenBrightness";
    private const string KEY_VIBRATION = "Vibration";
    private const string KEY_AUTO_SAVE = "AutoSave";
    private const string KEY_AUTO_SAVE_INTERVAL = "AutoSaveInterval";
    private const string KEY_TUTORIAL_COMPLETED = "TutorialCompleted";
    private const string KEY_TOUCH_SENSITIVITY = "TouchSensitivity";
    private const string KEY_INVERT_CONTROLS = "InvertControls";
    private const string KEY_SHOW_TOUCH_INDICATOR = "ShowTouchIndicator";
    private const string KEY_HIGH_CONTRAST = "HighContrast";
    private const string KEY_LARGE_TEXT = "LargeText";
    private const string KEY_COLOR_BLIND = "ColorBlind";
    private const string KEY_COLOR_BLIND_TYPE = "ColorBlindType";
    private const string KEY_DEBUG_MODE = "DebugMode";
    private const string KEY_SHOW_FPS = "ShowFPS";
    private const string KEY_SHOW_DEBUG_INFO = "ShowDebugInfo";
    private const string KEY_PUSH_NOTIFICATIONS = "PushNotifications";
    private const string KEY_DAILY_REWARD_NOTIFICATION = "DailyRewardNotification";
    private const string KEY_ACHIEVEMENT_NOTIFICATION = "AchievementNotification";
    
    #endregion
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        InitializeSingleton();
        InitializeSettings();
    }
    
    private void Start()
    {
        // 설정 로드 및 적용
        LoadSettings();
        ApplySettingsToSystem();
    }
    
    private void OnDestroy()
    {
        // 종료 시 설정 저장
        SaveSettings();
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
            Debug.Log("SettingsManager 초기화 완료");
        }
        else
        {
            // 이미 인스턴스가 존재하는 경우 중복 생성 방지
            Debug.LogWarning("SettingsManager가 이미 존재합니다. 중복 생성 방지.");
            Destroy(gameObject);
        }
    }
    
    private void InitializeSettings()
    {
        // 기본 설정 데이터 초기화
        CurrentSettings = new SettingsData();
        
        // 설정 데이터 검증
        CurrentSettings.ValidateVolumeSettings();
        
        Debug.Log("설정 데이터 초기화 완료");
    }
    
    #endregion
    
    #region Settings Load/Save
    
    /// <summary>
    /// PlayerPrefs에서 설정 로드
    /// </summary>
    public void LoadSettings()
    {
        if (CurrentSettings == null)
        {
            CurrentSettings = new SettingsData();
        }
        
        // 오디오 설정 로드
        CurrentSettings.bgmVolume = PlayerPrefs.GetFloat(KEY_BGM_VOLUME, CurrentSettings.bgmVolume);
        CurrentSettings.sfxVolume = PlayerPrefs.GetFloat(KEY_SFX_VOLUME, CurrentSettings.sfxVolume);
        CurrentSettings.masterAudioEnabled = PlayerPrefs.GetInt(KEY_MASTER_AUDIO, CurrentSettings.masterAudioEnabled ? 1 : 0) == 1;
        
        // 언어 설정 로드
        CurrentSettings.language = PlayerPrefs.GetString(KEY_LANGUAGE, CurrentSettings.language);
        
        // 그래픽 설정 로드
        CurrentSettings.graphicsQuality = PlayerPrefs.GetInt(KEY_GRAPHICS_QUALITY, CurrentSettings.graphicsQuality);
        CurrentSettings.targetFrameRate = PlayerPrefs.GetInt(KEY_TARGET_FRAME_RATE, CurrentSettings.targetFrameRate);
        CurrentSettings.vsyncEnabled = PlayerPrefs.GetInt(KEY_VSYNC, CurrentSettings.vsyncEnabled ? 1 : 0) == 1;
        CurrentSettings.screenBrightness = PlayerPrefs.GetFloat(KEY_SCREEN_BRIGHTNESS, CurrentSettings.screenBrightness);
        
        // 게임플레이 설정 로드
        CurrentSettings.vibrationEnabled = PlayerPrefs.GetInt(KEY_VIBRATION, CurrentSettings.vibrationEnabled ? 1 : 0) == 1;
        CurrentSettings.autoSave = PlayerPrefs.GetInt(KEY_AUTO_SAVE, CurrentSettings.autoSave ? 1 : 0) == 1;
        CurrentSettings.autoSaveInterval = PlayerPrefs.GetFloat(KEY_AUTO_SAVE_INTERVAL, CurrentSettings.autoSaveInterval);
        CurrentSettings.tutorialCompleted = PlayerPrefs.GetInt(KEY_TUTORIAL_COMPLETED, CurrentSettings.tutorialCompleted ? 1 : 0) == 1;
        
        // 조작 설정 로드
        CurrentSettings.touchSensitivity = PlayerPrefs.GetFloat(KEY_TOUCH_SENSITIVITY, CurrentSettings.touchSensitivity);
        CurrentSettings.invertControls = PlayerPrefs.GetInt(KEY_INVERT_CONTROLS, CurrentSettings.invertControls ? 1 : 0) == 1;
        CurrentSettings.showTouchIndicator = PlayerPrefs.GetInt(KEY_SHOW_TOUCH_INDICATOR, CurrentSettings.showTouchIndicator ? 1 : 0) == 1;
        
        // 접근성 설정 로드
        CurrentSettings.highContrastMode = PlayerPrefs.GetInt(KEY_HIGH_CONTRAST, CurrentSettings.highContrastMode ? 1 : 0) == 1;
        CurrentSettings.largeTextMode = PlayerPrefs.GetInt(KEY_LARGE_TEXT, CurrentSettings.largeTextMode ? 1 : 0) == 1;
        CurrentSettings.colorBlindMode = PlayerPrefs.GetInt(KEY_COLOR_BLIND, CurrentSettings.colorBlindMode ? 1 : 0) == 1;
        CurrentSettings.colorBlindType = PlayerPrefs.GetString(KEY_COLOR_BLIND_TYPE, CurrentSettings.colorBlindType);
        
        // 개발자 설정 로드
        CurrentSettings.debugMode = PlayerPrefs.GetInt(KEY_DEBUG_MODE, CurrentSettings.debugMode ? 1 : 0) == 1;
        CurrentSettings.showFPS = PlayerPrefs.GetInt(KEY_SHOW_FPS, CurrentSettings.showFPS ? 1 : 0) == 1;
        CurrentSettings.showDebugInfo = PlayerPrefs.GetInt(KEY_SHOW_DEBUG_INFO, CurrentSettings.showDebugInfo ? 1 : 0) == 1;
        
        // 알림 설정 로드
        CurrentSettings.pushNotifications = PlayerPrefs.GetInt(KEY_PUSH_NOTIFICATIONS, CurrentSettings.pushNotifications ? 1 : 0) == 1;
        CurrentSettings.dailyRewardNotification = PlayerPrefs.GetInt(KEY_DAILY_REWARD_NOTIFICATION, CurrentSettings.dailyRewardNotification ? 1 : 0) == 1;
        CurrentSettings.achievementNotification = PlayerPrefs.GetInt(KEY_ACHIEVEMENT_NOTIFICATION, CurrentSettings.achievementNotification ? 1 : 0) == 1;
        
        // 설정 데이터 검증
        CurrentSettings.ValidateVolumeSettings();
        
        Debug.Log("설정 로드 완료");
    }
    
    /// <summary>
    /// PlayerPrefs에 설정 저장
    /// </summary>
    public void SaveSettings()
    {
        if (CurrentSettings == null) return;
        
        // 오디오 설정 저장
        PlayerPrefs.SetFloat(KEY_BGM_VOLUME, CurrentSettings.bgmVolume);
        PlayerPrefs.SetFloat(KEY_SFX_VOLUME, CurrentSettings.sfxVolume);
        PlayerPrefs.SetInt(KEY_MASTER_AUDIO, CurrentSettings.masterAudioEnabled ? 1 : 0);
        
        // 언어 설정 저장
        PlayerPrefs.SetString(KEY_LANGUAGE, CurrentSettings.language);
        
        // 그래픽 설정 저장
        PlayerPrefs.SetInt(KEY_GRAPHICS_QUALITY, CurrentSettings.graphicsQuality);
        PlayerPrefs.SetInt(KEY_TARGET_FRAME_RATE, CurrentSettings.targetFrameRate);
        PlayerPrefs.SetInt(KEY_VSYNC, CurrentSettings.vsyncEnabled ? 1 : 0);
        PlayerPrefs.SetFloat(KEY_SCREEN_BRIGHTNESS, CurrentSettings.screenBrightness);
        
        // 게임플레이 설정 저장
        PlayerPrefs.SetInt(KEY_VIBRATION, CurrentSettings.vibrationEnabled ? 1 : 0);
        PlayerPrefs.SetInt(KEY_AUTO_SAVE, CurrentSettings.autoSave ? 1 : 0);
        PlayerPrefs.SetFloat(KEY_AUTO_SAVE_INTERVAL, CurrentSettings.autoSaveInterval);
        PlayerPrefs.SetInt(KEY_TUTORIAL_COMPLETED, CurrentSettings.tutorialCompleted ? 1 : 0);
        
        // 조작 설정 저장
        PlayerPrefs.SetFloat(KEY_TOUCH_SENSITIVITY, CurrentSettings.touchSensitivity);
        PlayerPrefs.SetInt(KEY_INVERT_CONTROLS, CurrentSettings.invertControls ? 1 : 0);
        PlayerPrefs.SetInt(KEY_SHOW_TOUCH_INDICATOR, CurrentSettings.showTouchIndicator ? 1 : 0);
        
        // 접근성 설정 저장
        PlayerPrefs.SetInt(KEY_HIGH_CONTRAST, CurrentSettings.highContrastMode ? 1 : 0);
        PlayerPrefs.SetInt(KEY_LARGE_TEXT, CurrentSettings.largeTextMode ? 1 : 0);
        PlayerPrefs.SetInt(KEY_COLOR_BLIND, CurrentSettings.colorBlindMode ? 1 : 0);
        PlayerPrefs.SetString(KEY_COLOR_BLIND_TYPE, CurrentSettings.colorBlindType);
        
        // 개발자 설정 저장
        PlayerPrefs.SetInt(KEY_DEBUG_MODE, CurrentSettings.debugMode ? 1 : 0);
        PlayerPrefs.SetInt(KEY_SHOW_FPS, CurrentSettings.showFPS ? 1 : 0);
        PlayerPrefs.SetInt(KEY_SHOW_DEBUG_INFO, CurrentSettings.showDebugInfo ? 1 : 0);
        
        // 알림 설정 저장
        PlayerPrefs.SetInt(KEY_PUSH_NOTIFICATIONS, CurrentSettings.pushNotifications ? 1 : 0);
        PlayerPrefs.SetInt(KEY_DAILY_REWARD_NOTIFICATION, CurrentSettings.dailyRewardNotification ? 1 : 0);
        PlayerPrefs.SetInt(KEY_ACHIEVEMENT_NOTIFICATION, CurrentSettings.achievementNotification ? 1 : 0);
        
        // PlayerPrefs 저장
        PlayerPrefs.Save();
        
        Debug.Log("설정 저장 완료");
    }
    
    #endregion
    
    #region Settings Application
    
    /// <summary>
    /// 설정을 시스템에 실시간 적용
    /// </summary>
    public void ApplySettingsToSystem()
    {
        if (CurrentSettings == null) return;
        
        // 오디오 설정 적용
        ApplyAudioSettings();
        
        // 그래픽 설정 적용
        ApplyGraphicsSettings();
        
        // 게임플레이 설정 적용
        ApplyGameplaySettings();
        
        // 접근성 설정 적용
        ApplyAccessibilitySettings();
        
        // 개발자 설정 적용
        ApplyDeveloperSettings();
        
        Debug.Log("설정이 시스템에 적용되었습니다.");
    }
    
    private void ApplyAudioSettings()
    {
        // AudioManager가 있는 경우 오디오 설정 적용
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(CurrentSettings.masterAudioEnabled ? 1f : 0f);
            AudioManager.Instance.SetBGMVolume(CurrentSettings.bgmVolume);
            AudioManager.Instance.SetSFXVolume(CurrentSettings.sfxVolume);
        }
    }
    
    private void ApplyGraphicsSettings()
    {
        // 프레임레이트 설정
        Application.targetFrameRate = CurrentSettings.targetFrameRate;
        
        // 수직동기화 설정
        QualitySettings.vSyncCount = CurrentSettings.vsyncEnabled ? 1 : 0;
        
        // 그래픽 품질 설정
        QualitySettings.SetQualityLevel(CurrentSettings.graphicsQuality, true);
    }
    
    private void ApplyGameplaySettings()
    {
        // 진동 설정 (모바일에서만 적용)
        #if UNITY_ANDROID || UNITY_IOS
        if (CurrentSettings.vibrationEnabled)
        {
            // 진동 활성화 로직 (필요시 구현)
        }
        #endif
    }
    
    private void ApplyAccessibilitySettings()
    {
        // 고대비 모드 적용
        if (CurrentSettings.highContrastMode)
        {
            // 고대비 모드 로직 (필요시 구현)
        }
        
        // 큰 텍스트 모드 적용
        if (CurrentSettings.largeTextMode)
        {
            // 큰 텍스트 모드 로직 (필요시 구현)
        }
        
        // 색맹 모드 적용
        if (CurrentSettings.colorBlindMode)
        {
            // 색맹 모드 로직 (필요시 구현)
        }
    }
    
    private void ApplyDeveloperSettings()
    {
        // 디버그 모드 설정
        if (CurrentSettings.debugMode)
        {
            // 디버그 모드 로직 (필요시 구현)
        }
        
        // FPS 표시 설정
        if (CurrentSettings.showFPS)
        {
            // FPS 표시 로직 (필요시 구현)
        }
    }
    
    #endregion
    
    #region Public Settings Methods
    
    /// <summary>
    /// BGM 볼륨 설정
    /// </summary>
    /// <param name="volume">볼륨 값 (0.0 ~ 1.0)</param>
    public void SetBGMVolume(float volume)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.bgmVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(KEY_BGM_VOLUME, CurrentSettings.bgmVolume);
            PlayerPrefs.Save();
            
            // 실시간 적용
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBGMVolume(CurrentSettings.bgmVolume);
            }
            
            Debug.Log($"BGM 볼륨이 {CurrentSettings.bgmVolume:F2}로 설정되었습니다.");
        }
    }
    
    /// <summary>
    /// SFX 볼륨 설정
    /// </summary>
    /// <param name="volume">볼륨 값 (0.0 ~ 1.0)</param>
    public void SetSFXVolume(float volume)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(KEY_SFX_VOLUME, CurrentSettings.sfxVolume);
            PlayerPrefs.Save();
            
            // 실시간 적용
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(CurrentSettings.sfxVolume);
            }
            
            Debug.Log($"SFX 볼륨이 {CurrentSettings.sfxVolume:F2}로 설정되었습니다.");
        }
    }
    
    /// <summary>
    /// 언어 설정
    /// </summary>
    /// <param name="language">언어 코드</param>
    public void SetLanguage(string language)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.SetLanguage(language);
            PlayerPrefs.SetString(KEY_LANGUAGE, CurrentSettings.language);
            PlayerPrefs.Save();
            
            Debug.Log($"언어가 {CurrentSettings.language}로 설정되었습니다.");
        }
    }
    
    /// <summary>
    /// 그래픽 품질 설정
    /// </summary>
    /// <param name="level">품질 레벨 (0: Low, 1: Medium, 2: High)</param>
    public void SetQualityLevel(int level)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.SetGraphicsQuality(level);
            PlayerPrefs.SetInt(KEY_GRAPHICS_QUALITY, CurrentSettings.graphicsQuality);
            PlayerPrefs.Save();
            
            // 실시간 적용
            QualitySettings.SetQualityLevel(CurrentSettings.graphicsQuality, true);
            
            Debug.Log($"그래픽 품질이 {CurrentSettings.graphicsQuality}로 설정되었습니다.");
        }
    }
    
    /// <summary>
    /// 목표 프레임레이트 설정
    /// </summary>
    /// <param name="frameRate">프레임레이트</param>
    public void SetTargetFrameRate(int frameRate)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.targetFrameRate = frameRate;
            PlayerPrefs.SetInt(KEY_TARGET_FRAME_RATE, CurrentSettings.targetFrameRate);
            PlayerPrefs.Save();
            
            // 실시간 적용
            Application.targetFrameRate = CurrentSettings.targetFrameRate;
            
            Debug.Log($"목표 프레임레이트가 {CurrentSettings.targetFrameRate}로 설정되었습니다.");
        }
    }
    
    /// <summary>
    /// 진동 설정
    /// </summary>
    /// <param name="enabled">활성화 여부</param>
    public void SetVibration(bool enabled)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.vibrationEnabled = enabled;
            PlayerPrefs.SetInt(KEY_VIBRATION, enabled ? 1 : 0);
            PlayerPrefs.Save();
            
            Debug.Log($"진동이 {(enabled ? "활성화" : "비활성화")}되었습니다.");
        }
    }
    
    /// <summary>
    /// 마스터 오디오 활성화/비활성화
    /// </summary>
    /// <param name="enabled">활성화 여부</param>
    public void SetMasterAudioEnabled(bool enabled)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.masterAudioEnabled = enabled;
            PlayerPrefs.SetInt(KEY_MASTER_AUDIO, enabled ? 1 : 0);
            PlayerPrefs.Save();
            
            // 실시간 적용
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(enabled ? 1f : 0f);
            }
            
            Debug.Log($"마스터 오디오가 {(enabled ? "활성화" : "비활성화")}되었습니다.");
        }
    }
    
    /// <summary>
    /// 수직동기화 설정
    /// </summary>
    /// <param name="enabled">활성화 여부</param>
    public void SetVSyncEnabled(bool enabled)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.vsyncEnabled = enabled;
            PlayerPrefs.SetInt(KEY_VSYNC, enabled ? 1 : 0);
            PlayerPrefs.Save();
            
            // 실시간 적용
            QualitySettings.vSyncCount = enabled ? 1 : 0;
            
            Debug.Log($"수직동기화가 {(enabled ? "활성화" : "비활성화")}되었습니다.");
        }
    }
    
    /// <summary>
    /// 화면 밝기 설정
    /// </summary>
    /// <param name="brightness">밝기 값 (0.0 ~ 1.0)</param>
    public void SetScreenBrightness(float brightness)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.screenBrightness = Mathf.Clamp01(brightness);
            PlayerPrefs.SetFloat(KEY_SCREEN_BRIGHTNESS, CurrentSettings.screenBrightness);
            PlayerPrefs.Save();
            
            Debug.Log($"화면 밝기가 {CurrentSettings.screenBrightness:F2}로 설정되었습니다.");
        }
    }
    
    /// <summary>
    /// 자동 저장 설정
    /// </summary>
    /// <param name="enabled">활성화 여부</param>
    public void SetAutoSaveEnabled(bool enabled)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.autoSave = enabled;
            PlayerPrefs.SetInt(KEY_AUTO_SAVE, enabled ? 1 : 0);
            PlayerPrefs.Save();
            
            Debug.Log($"자동 저장이 {(enabled ? "활성화" : "비활성화")}되었습니다.");
        }
    }
    
    /// <summary>
    /// 튜토리얼 완료 상태 설정
    /// </summary>
    /// <param name="completed">완료 여부</param>
    public void SetTutorialCompleted(bool completed)
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.tutorialCompleted = completed;
            PlayerPrefs.SetInt(KEY_TUTORIAL_COMPLETED, completed ? 1 : 0);
            PlayerPrefs.Save();
            
            Debug.Log($"튜토리얼 완료 상태가 {(completed ? "완료" : "미완료")}로 설정되었습니다.");
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// 현재 설정을 새로운 설정으로 업데이트
    /// </summary>
    /// <param name="newSettings">새로운 설정 데이터</param>
    public void UpdateSettings(SettingsData newSettings)
    {
        if (newSettings != null)
        {
            CurrentSettings = newSettings;
            CurrentSettings.ValidateVolumeSettings();
            SaveSettings();
            ApplySettingsToSystem();
            Debug.Log("설정이 업데이트되었습니다.");
        }
        else
        {
            Debug.LogError("새로운 설정 데이터가 null입니다.");
        }
    }
    
    /// <summary>
    /// 설정을 기본값으로 초기화
    /// </summary>
    public void ResetToDefault()
    {
        if (CurrentSettings != null)
        {
            CurrentSettings.ResetToDefaults();
            SaveSettings();
            ApplySettingsToSystem();
            Debug.Log("설정이 기본값으로 초기화되었습니다.");
        }
    }
    
    /// <summary>
    /// 설정이 유효한지 확인
    /// </summary>
    /// <returns>유효한 경우 true</returns>
    public bool IsValid()
    {
        return CurrentSettings != null;
    }
    
    /// <summary>
    /// 현재 설정 정보를 문자열로 반환
    /// </summary>
    /// <returns>설정 정보 문자열</returns>
    public string GetSettingsInfo()
    {
        if (CurrentSettings == null)
            return "설정 데이터가 없습니다.";
        
        return $"언어: {CurrentSettings.language}\n" +
               $"BGM 볼륨: {CurrentSettings.bgmVolume:F2}\n" +
               $"SFX 볼륨: {CurrentSettings.sfxVolume:F2}\n" +
               $"그래픽 품질: {CurrentSettings.graphicsQuality}\n" +
               $"프레임레이트: {CurrentSettings.targetFrameRate} FPS\n" +
               $"진동: {(CurrentSettings.vibrationEnabled ? "활성화" : "비활성화")}\n" +
               $"자동 저장: {(CurrentSettings.autoSave ? "활성화" : "비활성화")}";
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// 모든 PlayerPrefs 설정 삭제
    /// </summary>
    public void ClearAllSettings()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("모든 설정이 삭제되었습니다.");
    }
    
    /// <summary>
    /// 특정 설정 삭제
    /// </summary>
    /// <param name="key">삭제할 설정 키</param>
    public void ClearSetting(string key)
    {
        PlayerPrefs.DeleteKey(key);
        Debug.Log($"설정 '{key}'가 삭제되었습니다.");
    }
    
    #endregion
} 