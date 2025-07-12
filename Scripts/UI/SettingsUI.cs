using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    #region UI Elements
    
    [Header("Audio Settings")]
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI bgmVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    
    [Header("Graphics Settings")]
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Dropdown frameRateDropdown;
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private TextMeshProUGUI frameRateText;
    
    [Header("Language Settings")]
    [SerializeField] private Dropdown languageDropdown;
    [SerializeField] private TextMeshProUGUI languageText;
    
    [Header("Gameplay Settings")]
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private TextMeshProUGUI vibrationText;
    
    [Header("Control Buttons")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button applyButton;
    
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TextMeshProUGUI settingsTitle;
    [SerializeField] private GameObject confirmResetPanel;
    [SerializeField] private Button confirmResetButton;
    [SerializeField] private Button cancelResetButton;
    
    #endregion
    
    #region Private Variables
    
    private bool isInitialized = false;
    private bool isUpdatingUI = false; // UI 업데이트 중 중복 이벤트 방지
    
    #endregion
    
    #region Unity Lifecycle
    
    private void Start()
    {
        InitializeUI();
        SetupEventListeners();
        LoadCurrentSettings();
    }
    
    private void OnEnable()
    {
        if (isInitialized)
        {
            LoadCurrentSettings();
        }
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeUI()
    {
        // 슬라이더 초기화
        InitializeSliders();
        
        // 드롭다운 초기화
        InitializeDropdowns();
        
        // 토글 초기화
        InitializeToggles();
        
        // 패널 초기화
        InitializePanels();
        
        isInitialized = true;
        Debug.Log("SettingsUI 초기화 완료");
    }
    
    private void InitializeSliders()
    {
        // BGM 볼륨 슬라이더
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0f;
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.value = 0.7f;
        }
        
        // SFX 볼륨 슬라이더
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = 0.8f;
        }
    }
    
    private void InitializeDropdowns()
    {
        // 그래픽 품질 드롭다운
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "낮음", "보통", "높음" });
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
    }
    
    private void InitializeToggles()
    {
        // 진동 토글
        if (vibrationToggle != null)
        {
            vibrationToggle.isOn = true;
        }
    }
    
    private void InitializePanels()
    {
        // 설정 패널 초기화
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        // 리셋 확인 패널 초기화
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(false);
        }
    }
    
    #endregion
    
    #region Event Listeners
    
    private void SetupEventListeners()
    {
        // 볼륨 슬라이더 이벤트
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        // 드롭다운 이벤트
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        if (frameRateDropdown != null)
            frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
        if (languageDropdown != null)
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        
        // 토글 이벤트
        if (vibrationToggle != null)
            vibrationToggle.onValueChanged.AddListener(OnVibrationToggled);
        
        // 버튼 이벤트
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetButtonClicked);
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        if (applyButton != null)
            applyButton.onClick.AddListener(OnApplyButtonClicked);
        
        // 리셋 확인 버튼 이벤트
        if (confirmResetButton != null)
            confirmResetButton.onClick.AddListener(OnConfirmResetClicked);
        if (cancelResetButton != null)
            cancelResetButton.onClick.AddListener(OnCancelResetClicked);
    }
    
    #endregion
    
    #region Settings Loading
    
    private void LoadCurrentSettings()
    {
        if (SettingsManager.Instance == null || SettingsManager.Instance.CurrentSettings == null)
        {
            Debug.LogWarning("SettingsManager가 없거나 설정 데이터가 없습니다.");
            return;
        }
        
        isUpdatingUI = true;
        
        SettingsData settings = SettingsManager.Instance.CurrentSettings;
        
        // 볼륨 설정 로드
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.value = settings.bgmVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = settings.sfxVolume;
        
        // 그래픽 설정 로드
        if (qualityDropdown != null)
            qualityDropdown.value = settings.graphicsQuality;
        if (frameRateDropdown != null)
        {
            switch (settings.targetFrameRate)
            {
                case 30: frameRateDropdown.value = 0; break;
                case 60: frameRateDropdown.value = 1; break;
                case 120: frameRateDropdown.value = 2; break;
                default: frameRateDropdown.value = 1; break;
            }
        }
        
        // 언어 설정 로드
        if (languageDropdown != null)
        {
            switch (settings.language)
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
        
        // 게임플레이 설정 로드
        if (vibrationToggle != null)
            vibrationToggle.isOn = settings.vibrationEnabled;
        
        // UI 텍스트 업데이트
        UpdateUITexts();
        
        isUpdatingUI = false;
        
        Debug.Log("현재 설정을 UI에 로드했습니다.");
    }
    
    #endregion
    
    #region Event Handlers
    
    private void OnBGMVolumeChanged(float value)
    {
        if (isUpdatingUI) return;
        
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetBGMVolume(value);
        }
        
        UpdateVolumeTexts();
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        if (isUpdatingUI) return;
        
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetSFXVolume(value);
        }
        
        UpdateVolumeTexts();
    }
    
    private void OnQualityChanged(int value)
    {
        if (isUpdatingUI) return;
        
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetQualityLevel(value);
        }
    }
    
    private void OnFrameRateChanged(int value)
    {
        if (isUpdatingUI) return;
        
        if (SettingsManager.Instance != null)
        {
            int frameRate = 60;
            switch (value)
            {
                case 0: frameRate = 30; break;
                case 1: frameRate = 60; break;
                case 2: frameRate = 120; break;
            }
            SettingsManager.Instance.SetTargetFrameRate(frameRate);
        }
    }
    
    private void OnLanguageChanged(int value)
    {
        if (isUpdatingUI) return;
        
        if (SettingsManager.Instance != null)
        {
            string[] languages = { "ko", "en", "ja", "zh", "es", "fr", "de" };
            if (value < languages.Length)
            {
                SettingsManager.Instance.SetLanguage(languages[value]);
                UpdateUITexts(); // 언어 변경 시 UI 텍스트 업데이트
            }
        }
    }
    
    private void OnVibrationToggled(bool isOn)
    {
        if (isUpdatingUI) return;
        
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetVibration(isOn);
        }
    }
    
    private void OnResetButtonClicked()
    {
        ShowResetConfirmDialog();
    }
    
    private void OnCloseButtonClicked()
    {
        HideSettings();
    }
    
    private void OnApplyButtonClicked()
    {
        // 설정 적용 (필요시 추가 로직)
        Debug.Log("설정이 적용되었습니다.");
    }
    
    private void OnConfirmResetClicked()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ResetToDefault();
            LoadCurrentSettings(); // UI 다시 로드
            Debug.Log("설정이 기본값으로 초기화되었습니다.");
        }
        
        HideResetConfirmDialog();
    }
    
    private void OnCancelResetClicked()
    {
        HideResetConfirmDialog();
    }
    
    #endregion
    
    #region UI Updates
    
    private void UpdateVolumeTexts()
    {
        if (bgmVolumeText != null && bgmVolumeSlider != null)
        {
            bgmVolumeText.text = $"BGM 볼륨: {bgmVolumeSlider.value:F0}%";
        }
        
        if (sfxVolumeText != null && sfxVolumeSlider != null)
        {
            sfxVolumeText.text = $"SFX 볼륨: {sfxVolumeSlider.value:F0}%";
        }
    }
    
    private void UpdateUITexts()
    {
        // 언어에 따른 UI 텍스트 업데이트
        if (SettingsManager.Instance != null && SettingsManager.Instance.CurrentSettings != null)
        {
            string language = SettingsManager.Instance.CurrentSettings.language;
            
            // 제목 업데이트
            if (settingsTitle != null)
            {
                switch (language)
                {
                    case "ko": settingsTitle.text = "설정"; break;
                    case "en": settingsTitle.text = "Settings"; break;
                    case "ja": settingsTitle.text = "設定"; break;
                    case "zh": settingsTitle.text = "设置"; break;
                    case "es": settingsTitle.text = "Configuración"; break;
                    case "fr": settingsTitle.text = "Paramètres"; break;
                    case "de": settingsTitle.text = "Einstellungen"; break;
                    default: settingsTitle.text = "설정"; break;
                }
            }
            
            // 라벨 텍스트 업데이트
            if (qualityText != null)
            {
                switch (language)
                {
                    case "ko": qualityText.text = "그래픽 품질"; break;
                    case "en": qualityText.text = "Graphics Quality"; break;
                    case "ja": qualityText.text = "グラフィック品質"; break;
                    case "zh": qualityText.text = "图形质量"; break;
                    case "es": qualityText.text = "Calidad Gráfica"; break;
                    case "fr": qualityText.text = "Qualité Graphique"; break;
                    case "de": qualityText.text = "Grafikqualität"; break;
                    default: qualityText.text = "그래픽 품질"; break;
                }
            }
            
            if (frameRateText != null)
            {
                switch (language)
                {
                    case "ko": frameRateText.text = "프레임레이트"; break;
                    case "en": frameRateText.text = "Frame Rate"; break;
                    case "ja": frameRateText.text = "フレームレート"; break;
                    case "zh": frameRateText.text = "帧率"; break;
                    case "es": frameRateText.text = "Tasa de FPS"; break;
                    case "fr": frameRateText.text = "Taux d'Images"; break;
                    case "de": frameRateText.text = "Bildrate"; break;
                    default: frameRateText.text = "프레임레이트"; break;
                }
            }
            
            if (languageText != null)
            {
                switch (language)
                {
                    case "ko": languageText.text = "언어"; break;
                    case "en": languageText.text = "Language"; break;
                    case "ja": languageText.text = "言語"; break;
                    case "zh": languageText.text = "语言"; break;
                    case "es": languageText.text = "Idioma"; break;
                    case "fr": languageText.text = "Langue"; break;
                    case "de": languageText.text = "Sprache"; break;
                    default: languageText.text = "언어"; break;
                }
            }
            
            if (vibrationText != null)
            {
                switch (language)
                {
                    case "ko": vibrationText.text = "진동"; break;
                    case "en": vibrationText.text = "Vibration"; break;
                    case "ja": vibrationText.text = "振動"; break;
                    case "zh": vibrationText.text = "震动"; break;
                    case "es": vibrationText.text = "Vibración"; break;
                    case "fr": vibrationText.text = "Vibration"; break;
                    case "de": vibrationText.text = "Vibration"; break;
                    default: vibrationText.text = "진동"; break;
                }
            }
        }
        
        // 볼륨 텍스트 업데이트
        UpdateVolumeTexts();
    }
    
    #endregion
    
    #region Panel Management
    
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
    
    private void ShowResetConfirmDialog()
    {
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(true);
        }
    }
    
    private void HideResetConfirmDialog()
    {
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(false);
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// 설정 UI 새로고침
    /// </summary>
    public void RefreshSettings()
    {
        LoadCurrentSettings();
    }
    
    /// <summary>
    /// 설정이 변경되었는지 확인
    /// </summary>
    /// <returns>변경 여부</returns>
    public bool HasSettingsChanged()
    {
        // 향후 구현 예정
        return false;
    }
    
    /// <summary>
    /// 설정 변경 사항을 초기화
    /// </summary>
    public void ClearSettingsChanged()
    {
        // 향후 구현 예정
    }
    
    #endregion
} 