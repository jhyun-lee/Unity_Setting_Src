using System;

[Serializable]
public class SettingsData
{
    // 오디오 설정
    [Header("Audio Settings")]
    public float bgmVolume = 0.7f;        // 배경음악 볼륨 (0.0 ~ 1.0)
    public float sfxVolume = 0.8f;        // 효과음 볼륨 (0.0 ~ 1.0)
    public bool masterAudioEnabled = true; // 마스터 오디오 활성화
    
    // 언어 설정
    [Header("Language Settings")]
    public string language = "ko";         // 언어 코드 (ko, en, ja, zh 등)
    
    // 알림 설정
    [Header("Notification Settings")]
    public bool pushNotifications = true;  // 푸시 알림 활성화
    public bool dailyRewardNotification = true; // 일일 보상 알림
    public bool achievementNotification = true;  // 업적 알림
    
    // 그래픽 설정
    [Header("Graphics Settings")]
    public int graphicsQuality = 2;        // 그래픽 품질 레벨 (0: Low, 1: Medium, 2: High)
    public int targetFrameRate = 60;       // 목표 프레임레이트
    public bool vsyncEnabled = false;      // 수직 동기화
    public float screenBrightness = 1.0f;  // 화면 밝기 (0.0 ~ 1.0)
    
    // 게임플레이 설정
    [Header("Gameplay Settings")]
    public bool vibrationEnabled = true;   // 진동 설정
    public bool autoSave = true;           // 자동 저장
    public float autoSaveInterval = 30f;   // 자동 저장 간격 (초)
    public bool tutorialCompleted = false; // 튜토리얼 완료 여부
    
    // 조작 설정
    [Header("Control Settings")]
    public float touchSensitivity = 1.0f;  // 터치 감도
    public bool invertControls = false;    // 조작 반전
    public bool showTouchIndicator = true; // 터치 표시기
    
    // 접근성 설정
    [Header("Accessibility Settings")]
    public bool highContrastMode = false;  // 고대비 모드
    public bool largeTextMode = false;     // 큰 텍스트 모드
    public bool colorBlindMode = false;    // 색맹 모드
    public string colorBlindType = "none"; // 색맹 유형 (none, protanopia, deuteranopia, tritanopia)
    
    // 개발자 설정
    [Header("Developer Settings")]
    public bool debugMode = false;         // 디버그 모드
    public bool showFPS = false;           // FPS 표시
    public bool showDebugInfo = false;     // 디버그 정보 표시
    
    // 생성자
    public SettingsData()
    {
        // 기본값들은 이미 위에서 설정됨
    }
    
    // 볼륨 설정 검증
    public void ValidateVolumeSettings()
    {
        bgmVolume = Mathf.Clamp01(bgmVolume);
        sfxVolume = Mathf.Clamp01(sfxVolume);
        screenBrightness = Mathf.Clamp01(screenBrightness);
        touchSensitivity = Mathf.Clamp(touchSensitivity, 0.1f, 3.0f);
    }
    
    // 그래픽 품질 설정
    public void SetGraphicsQuality(int quality)
    {
        graphicsQuality = Mathf.Clamp(quality, 0, 2);
        
        // 품질에 따른 자동 설정
        switch (graphicsQuality)
        {
            case 0: // Low
                targetFrameRate = 30;
                vsyncEnabled = false;
                break;
            case 1: // Medium
                targetFrameRate = 60;
                vsyncEnabled = false;
                break;
            case 2: // High
                targetFrameRate = 60;
                vsyncEnabled = true;
                break;
        }
    }
    
    // 언어 설정
    public void SetLanguage(string langCode)
    {
        string[] supportedLanguages = { "ko", "en", "ja", "zh", "es", "fr", "de" };
        
        if (System.Array.Exists(supportedLanguages, lang => lang == langCode))
        {
            language = langCode;
        }
        else
        {
            language = "en"; // 기본값
        }
    }
    
    // 모든 설정을 기본값으로 초기화
    public void ResetToDefaults()
    {
        bgmVolume = 0.7f;
        sfxVolume = 0.8f;
        masterAudioEnabled = true;
        language = "ko";
        pushNotifications = true;
        dailyRewardNotification = true;
        achievementNotification = true;
        graphicsQuality = 2;
        targetFrameRate = 60;
        vsyncEnabled = false;
        screenBrightness = 1.0f;
        vibrationEnabled = true;
        autoSave = true;
        autoSaveInterval = 30f;
        tutorialCompleted = false;
        touchSensitivity = 1.0f;
        invertControls = false;
        showTouchIndicator = true;
        highContrastMode = false;
        largeTextMode = false;
        colorBlindMode = false;
        colorBlindType = "none";
        debugMode = false;
        showFPS = false;
        showDebugInfo = false;
    }
    
    // 설정 복사
    public void CopyFrom(SettingsData other)
    {
        bgmVolume = other.bgmVolume;
        sfxVolume = other.sfxVolume;
        masterAudioEnabled = other.masterAudioEnabled;
        language = other.language;
        pushNotifications = other.pushNotifications;
        dailyRewardNotification = other.dailyRewardNotification;
        achievementNotification = other.achievementNotification;
        graphicsQuality = other.graphicsQuality;
        targetFrameRate = other.targetFrameRate;
        vsyncEnabled = other.vsyncEnabled;
        screenBrightness = other.screenBrightness;
        vibrationEnabled = other.vibrationEnabled;
        autoSave = other.autoSave;
        autoSaveInterval = other.autoSaveInterval;
        tutorialCompleted = other.tutorialCompleted;
        touchSensitivity = other.touchSensitivity;
        invertControls = other.invertControls;
        showTouchIndicator = other.showTouchIndicator;
        highContrastMode = other.highContrastMode;
        largeTextMode = other.largeTextMode;
        colorBlindMode = other.colorBlindMode;
        colorBlindType = other.colorBlindType;
        debugMode = other.debugMode;
        showFPS = other.showFPS;
        showDebugInfo = other.showDebugInfo;
    }
} 