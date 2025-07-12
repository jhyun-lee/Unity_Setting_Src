using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUIManager : MonoBehaviour
{
    [Header("Audio Settings UI")]
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle masterAudioToggle;
    
    [Header("Graphics Settings UI")]
    public Dropdown graphicsQualityDropdown;
    public Dropdown frameRateDropdown;
    public Toggle vsyncToggle;
    public Slider brightnessSlider;
    
    [Header("Gameplay Settings UI")]
    public Toggle vibrationToggle;
    public Toggle autoSaveToggle;
    public Slider autoSaveIntervalSlider;
    public TextMeshProUGUI autoSaveIntervalText;
    
    [Header("Language Settings UI")]
    public Dropdown languageDropdown;
    
    [Header("Notification Settings UI")]
    public Toggle pushNotificationToggle;
    public Toggle dailyRewardNotificationToggle;
    public Toggle achievementNotificationToggle;
    
    [Header("Accessibility Settings UI")]
    public Toggle highContrastToggle;
    public Toggle largeTextToggle;
    public Toggle colorBlindToggle;
    public Dropdown colorBlindTypeDropdown;
    
    [Header("Developer Settings UI")]
    public Toggle debugModeToggle;
    public Toggle showFPSToggle;
    public Toggle showDebugInfoToggle;
    
    [Header("Control Buttons")]
    public Button saveButton;
    public Button resetButton;
    public Button closeButton;
    
    [Header("UI References")]
    public GameObject settingsPanel;
    public TextMeshProUGUI settingsTitle;
    
    private SettingsData currentSettings;
    private bool isInitialized = false;
    
    private void Start()
    {
        InitializeUI();
        LoadCurrentSettings();
        SetupEventListeners();
    }
    
    private void OnEnable()
    {
        if (isInitialized)
        {
            LoadCurrentSettings();
        }
    }
    
    private void InitializeUI()
    {
        // 드롭다운 초기화
        InitializeDropdowns();
        
        // 슬라이더 초기화
        InitializeSliders();
        
        // 토글 초기화
        InitializeToggles();
        
        isInitialized = true;
    }
    
    private void InitializeDropdowns()
    {
        // 그래픽 품질 드롭다운
        if (graphicsQualityDropdown != null)
        {
            graphicsQualityDropdown.ClearOptions();
            graphicsQualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "낮음", "보통", "높음" });
        }
        
        // 프레임레이트 드롭다운
        if (frameRateDropdown != null)
        {
            frameRateDropdown.ClearOptions();
            frameRateDropdown.AddOptions(new System.Collections.Generic.List<string> { "30 FPS", "60 FPS", "120 FPS" });
        }
        
        // 언어 드롭다운
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(new System.Collections.Generic.List<string> { "한국어", "English", "日本語", "中文", "Español", "Français", "Deutsch" });
        }
        
        // 색맹 유형 드롭다운
        if (colorBlindTypeDropdown != null)
        {
            colorBlindTypeDropdown.ClearOptions();
            colorBlindTypeDropdown.AddOptions(new System.Collections.Generic.List<string> { "없음", "적색맹", "녹색맹", "청색맹" });
        }
    }
    
    private void InitializeSliders()
    {
        // 볼륨 슬라이더
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
        }
        
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0f;
            bgmVolumeSlider.maxValue = 1f;
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
        }
        
        // 밝기 슬라이더
        if (brightnessSlider != null)
        {
            brightnessSlider.minValue = 0f;
            brightnessSlider.maxValue = 1f;
        }
        
        // 자동 저장 간격 슬라이더
        if (autoSaveIntervalSlider != null)
        {
            autoSaveIntervalSlider.minValue = 10f;
            autoSaveIntervalSlider.maxValue = 300f;
        }
    }
    
    private void InitializeToggles()
    {
        // 모든 토글의 기본값 설정
        if (masterAudioToggle != null) masterAudioToggle.isOn = true;
        if (vibrationToggle != null) vibrationToggle.isOn = true;
        if (autoSaveToggle != null) autoSaveToggle.isOn = true;
        if (pushNotificationToggle != null) pushNotificationToggle.isOn = true;
        if (dailyRewardNotificationToggle != null) dailyRewardNotificationToggle.isOn = true;
        if (achievementNotificationToggle != null) achievementNotificationToggle.isOn = true;
    }
    
    private void SetupEventListeners()
    {
        // 볼륨 슬라이더 이벤트
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        // 마스터 오디오 토글
        if (masterAudioToggle != null)
            masterAudioToggle.onValueChanged.AddListener(OnMasterAudioToggled);
        
        // 그래픽 설정 이벤트
        if (graphicsQualityDropdown != null)
            graphicsQualityDropdown.onValueChanged.AddListener(OnGraphicsQualityChanged);
        if (frameRateDropdown != null)
            frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
        if (vsyncToggle != null)
            vsyncToggle.onValueChanged.AddListener(OnVSyncToggled);
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        
        // 게임플레이 설정 이벤트
        if (vibrationToggle != null)
            vibrationToggle.onValueChanged.AddListener(OnVibrationToggled);
        if (autoSaveToggle != null)
            autoSaveToggle.onValueChanged.AddListener(OnAutoSaveToggled);
        if (autoSaveIntervalSlider != null)
            autoSaveIntervalSlider.onValueChanged.AddListener(OnAutoSaveIntervalChanged);
        
        // 언어 설정 이벤트
        if (languageDropdown != null)
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        
        // 알림 설정 이벤트
        if (pushNotificationToggle != null)
            pushNotificationToggle.onValueChanged.AddListener(OnPushNotificationToggled);
        if (dailyRewardNotificationToggle != null)
            dailyRewardNotificationToggle.onValueChanged.AddListener(OnDailyRewardNotificationToggled);
        if (achievementNotificationToggle != null)
            achievementNotificationToggle.onValueChanged.AddListener(OnAchievementNotificationToggled);
        
        // 접근성 설정 이벤트
        if (highContrastToggle != null)
            highContrastToggle.onValueChanged.AddListener(OnHighContrastToggled);
        if (largeTextToggle != null)
            largeTextToggle.onValueChanged.AddListener(OnLargeTextToggled);
        if (colorBlindToggle != null)
            colorBlindToggle.onValueChanged.AddListener(OnColorBlindToggled);
        if (colorBlindTypeDropdown != null)
            colorBlindTypeDropdown.onValueChanged.AddListener(OnColorBlindTypeChanged);
        
        // 개발자 설정 이벤트
        if (debugModeToggle != null)
            debugModeToggle.onValueChanged.AddListener(OnDebugModeToggled);
        if (showFPSToggle != null)
            showFPSToggle.onValueChanged.AddListener(OnShowFPSToggled);
        if (showDebugInfoToggle != null)
            showDebugInfoToggle.onValueChanged.AddListener(OnShowDebugInfoToggled);
        
        // 버튼 이벤트
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetButtonClicked);
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
    
    private void LoadCurrentSettings()
    {
        if (DataManager.Instance != null && DataManager.Instance.settingsData != null)
        {
            currentSettings = DataManager.Instance.settingsData;
            ApplySettingsToUI();
        }
    }
    
    private void ApplySettingsToUI()
    {
        if (currentSettings == null) return;
        
        // 볼륨 설정
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = currentSettings.masterAudioEnabled ? 1f : 0f;
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.value = currentSettings.bgmVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = currentSettings.sfxVolume;
        if (masterAudioToggle != null)
            masterAudioToggle.isOn = currentSettings.masterAudioEnabled;
        
        // 그래픽 설정
        if (graphicsQualityDropdown != null)
            graphicsQualityDropdown.value = currentSettings.graphicsQuality;
        if (frameRateDropdown != null)
        {
            switch (currentSettings.targetFrameRate)
            {
                case 30: frameRateDropdown.value = 0; break;
                case 60: frameRateDropdown.value = 1; break;
                case 120: frameRateDropdown.value = 2; break;
                default: frameRateDropdown.value = 1; break;
            }
        }
        if (vsyncToggle != null)
            vsyncToggle.isOn = currentSettings.vsyncEnabled;
        if (brightnessSlider != null)
            brightnessSlider.value = currentSettings.screenBrightness;
        
        // 게임플레이 설정
        if (vibrationToggle != null)
            vibrationToggle.isOn = currentSettings.vibrationEnabled;
        if (autoSaveToggle != null)
            autoSaveToggle.isOn = currentSettings.autoSave;
        if (autoSaveIntervalSlider != null)
            autoSaveIntervalSlider.value = currentSettings.autoSaveInterval;
        
        // 언어 설정
        if (languageDropdown != null)
        {
            switch (currentSettings.language)
            {
                case "ko": languageDropdown.value = 0; break;
                case "en": languageDropdown.value = 1; break;
                case "ja": languageDropdown.value = 2; break;
                case "zh": languageDropdown.value = 3; break;
                case "es": languageDropdown.value = 4; break;
                case "fr": languageDropdown.value = 5; break;
                case "de": languageDropdown.value = 6; break;
                default: languageDropdown.value = 0; break;
            }
        }
        
        // 알림 설정
        if (pushNotificationToggle != null)
            pushNotificationToggle.isOn = currentSettings.pushNotifications;
        if (dailyRewardNotificationToggle != null)
            dailyRewardNotificationToggle.isOn = currentSettings.dailyRewardNotification;
        if (achievementNotificationToggle != null)
            achievementNotificationToggle.isOn = currentSettings.achievementNotification;
        
        // 접근성 설정
        if (highContrastToggle != null)
            highContrastToggle.isOn = currentSettings.highContrastMode;
        if (largeTextToggle != null)
            largeTextToggle.isOn = currentSettings.largeTextMode;
        if (colorBlindToggle != null)
            colorBlindToggle.isOn = currentSettings.colorBlindMode;
        if (colorBlindTypeDropdown != null)
        {
            switch (currentSettings.colorBlindType)
            {
                case "none": colorBlindTypeDropdown.value = 0; break;
                case "protanopia": colorBlindTypeDropdown.value = 1; break;
                case "deuteranopia": colorBlindTypeDropdown.value = 2; break;
                case "tritanopia": colorBlindTypeDropdown.value = 3; break;
                default: colorBlindTypeDropdown.value = 0; break;
            }
        }
        
        // 개발자 설정
        if (debugModeToggle != null)
            debugModeToggle.isOn = currentSettings.debugMode;
        if (showFPSToggle != null)
            showFPSToggle.isOn = currentSettings.showFPS;
        if (showDebugInfoToggle != null)
            showDebugInfoToggle.isOn = currentSettings.showDebugInfo;
        
        // 자동 저장 간격 텍스트 업데이트
        UpdateAutoSaveIntervalText();
    }
    
    #region Event Handlers
    
    private void OnMasterVolumeChanged(float value)
    {
        if (currentSettings != null)
        {
            currentSettings.masterAudioEnabled = (value > 0);
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(value);
            }
        }
    }
    
    private void OnBGMVolumeChanged(float value)
    {
        if (currentSettings != null)
        {
            currentSettings.bgmVolume = value;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBGMVolume(value);
            }
        }
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        if (currentSettings != null)
        {
            currentSettings.sfxVolume = value;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(value);
            }
        }
    }
    
    private void OnMasterAudioToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.masterAudioEnabled = isOn;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(isOn ? 1f : 0f);
            }
        }
    }
    
    private void OnGraphicsQualityChanged(int value)
    {
        if (currentSettings != null)
        {
            currentSettings.SetGraphicsQuality(value);
        }
    }
    
    private void OnFrameRateChanged(int value)
    {
        if (currentSettings != null)
        {
            switch (value)
            {
                case 0: currentSettings.targetFrameRate = 30; break;
                case 1: currentSettings.targetFrameRate = 60; break;
                case 2: currentSettings.targetFrameRate = 120; break;
            }
        }
    }
    
    private void OnVSyncToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.vsyncEnabled = isOn;
        }
    }
    
    private void OnBrightnessChanged(float value)
    {
        if (currentSettings != null)
        {
            currentSettings.screenBrightness = value;
        }
    }
    
    private void OnVibrationToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.vibrationEnabled = isOn;
        }
    }
    
    private void OnAutoSaveToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.autoSave = isOn;
        }
    }
    
    private void OnAutoSaveIntervalChanged(float value)
    {
        if (currentSettings != null)
        {
            currentSettings.autoSaveInterval = value;
            UpdateAutoSaveIntervalText();
        }
    }
    
    private void OnLanguageChanged(int value)
    {
        if (currentSettings != null)
        {
            string[] languages = { "ko", "en", "ja", "zh", "es", "fr", "de" };
            if (value < languages.Length)
            {
                currentSettings.SetLanguage(languages[value]);
            }
        }
    }
    
    private void OnPushNotificationToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.pushNotifications = isOn;
        }
    }
    
    private void OnDailyRewardNotificationToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.dailyRewardNotification = isOn;
        }
    }
    
    private void OnAchievementNotificationToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.achievementNotification = isOn;
        }
    }
    
    private void OnHighContrastToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.highContrastMode = isOn;
        }
    }
    
    private void OnLargeTextToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.largeTextMode = isOn;
        }
    }
    
    private void OnColorBlindToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.colorBlindMode = isOn;
        }
    }
    
    private void OnColorBlindTypeChanged(int value)
    {
        if (currentSettings != null)
        {
            string[] types = { "none", "protanopia", "deuteranopia", "tritanopia" };
            if (value < types.Length)
            {
                currentSettings.colorBlindType = types[value];
            }
        }
    }
    
    private void OnDebugModeToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.debugMode = isOn;
        }
    }
    
    private void OnShowFPSToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.showFPS = isOn;
        }
    }
    
    private void OnShowDebugInfoToggled(bool isOn)
    {
        if (currentSettings != null)
        {
            currentSettings.showDebugInfo = isOn;
        }
    }
    
    private void OnSaveButtonClicked()
    {
        SaveSettings();
    }
    
    private void OnResetButtonClicked()
    {
        ResetSettings();
    }
    
    private void OnCloseButtonClicked()
    {
        CloseSettings();
    }
    
    #endregion
    
    private void UpdateAutoSaveIntervalText()
    {
        if (autoSaveIntervalText != null && currentSettings != null)
        {
            int minutes = Mathf.RoundToInt(currentSettings.autoSaveInterval / 60f);
            autoSaveIntervalText.text = $"{minutes}분";
        }
    }
    
    private void SaveSettings()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.settingsData.CopyFrom(currentSettings);
            DataManager.Instance.SaveSettings();
            Debug.Log("설정이 저장되었습니다.");
        }
    }
    
    private void ResetSettings()
    {
        if (currentSettings != null)
        {
            currentSettings.ResetToDefaults();
            ApplySettingsToUI();
            Debug.Log("설정이 기본값으로 초기화되었습니다.");
        }
    }
    
    private void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    public void ShowSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            LoadCurrentSettings();
        }
    }
    
    public void HideSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
} 