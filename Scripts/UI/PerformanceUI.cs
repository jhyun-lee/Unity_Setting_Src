using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using System.Collections.Generic;

namespace Game.UI
{
    /// <summary>
    /// 성능 모니터링 UI
    /// </summary>
    public class PerformanceUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _performancePanel;
        [SerializeField] private Text _fpsText;
        [SerializeField] private Text _memoryText;
        [SerializeField] private Text _batteryText;
        [SerializeField] private Text _cpuText;
        [SerializeField] private Text _gpuText;
        [SerializeField] private Text _temperatureText;
        [SerializeField] private Text _optimizationText;
        [SerializeField] private Text _warningText;
        
        [Header("Performance Bars")]
        [SerializeField] private Slider _fpsSlider;
        [SerializeField] private Slider _memorySlider;
        [SerializeField] private Slider _batterySlider;
        [SerializeField] private Slider _cpuSlider;
        [SerializeField] private Slider _gpuSlider;
        
        [Header("Settings")]
        [SerializeField] private bool _showInEditor = true;
        [SerializeField] private bool _showInDevelopmentBuild = true;
        [SerializeField] private bool _showInReleaseBuild = false;
        [SerializeField] private float _updateInterval = 0.5f;
        
        [Header("Colors")]
        [SerializeField] private Color _goodColor = Color.green;
        [SerializeField] private Color _warningColor = Color.yellow;
        [SerializeField] private Color _dangerColor = Color.red;
        
        private PerformanceData _currentData;
        private bool _isVisible = false;
        private float _lastUpdateTime = 0f;
        private List<PerformanceData> _performanceHistory = new List<PerformanceData>();
        
        private void Start()
        {
            // UI 초기화
            InitializeUI();
            
            // 이벤트 구독
            SubscribeToEvents();
            
            // 초기 상태 설정
            UpdateVisibility();
        }
        
        private void OnDestroy()
        {
            // 이벤트 구독 해제
            UnsubscribeFromEvents();
        }
        
        private void Update()
        {
            // 주기적으로 UI 업데이트
            if (Time.time - _lastUpdateTime >= _updateInterval)
            {
                UpdatePerformanceUI();
                _lastUpdateTime = Time.time;
            }
        }
        
        #region Initialization
        private void InitializeUI()
        {
            if (_performancePanel != null)
                _performancePanel.SetActive(false);
                
            // 슬라이더 초기화
            InitializeSliders();
            
            if (_enableDebugLogs)
                GameLogger.Log("PerformanceUI 초기화 완료", LogType.Info);
        }
        
        private void InitializeSliders()
        {
            if (_fpsSlider != null)
            {
                _fpsSlider.minValue = 0;
                _fpsSlider.maxValue = 60;
                _fpsSlider.value = 0;
            }
            
            if (_memorySlider != null)
            {
                _memorySlider.minValue = 0;
                _memorySlider.maxValue = 1;
                _memorySlider.value = 0;
            }
            
            if (_batterySlider != null)
            {
                _batterySlider.minValue = 0;
                _batterySlider.maxValue = 1;
                _batterySlider.value = 1;
            }
            
            if (_cpuSlider != null)
            {
                _cpuSlider.minValue = 0;
                _cpuSlider.maxValue = 1;
                _cpuSlider.value = 0;
            }
            
            if (_gpuSlider != null)
            {
                _gpuSlider.minValue = 0;
                _gpuSlider.maxValue = 1;
                _gpuSlider.value = 0;
            }
        }
        
        private void SubscribeToEvents()
        {
            PerformanceMonitor.OnPerformanceDataUpdated += OnPerformanceDataUpdated;
            PerformanceMonitor.OnPerformanceWarning += OnPerformanceWarning;
            PerformanceMonitor.OnAutoOptimization += OnAutoOptimization;
        }
        
        private void UnsubscribeFromEvents()
        {
            PerformanceMonitor.OnPerformanceDataUpdated -= OnPerformanceDataUpdated;
            PerformanceMonitor.OnPerformanceWarning -= OnPerformanceWarning;
            PerformanceMonitor.OnAutoOptimization -= OnAutoOptimization;
        }
        #endregion
        
        #region Event Handlers
        private void OnPerformanceDataUpdated(PerformanceData data)
        {
            _currentData = data;
            
            // 성능 히스토리에 추가
            _performanceHistory.Add(data);
            if (_performanceHistory.Count > 100) // 최대 100개 데이터 유지
            {
                _performanceHistory.RemoveAt(0);
            }
        }
        
        private void OnPerformanceWarning(PerformanceData data)
        {
            if (_warningText != null)
            {
                _warningText.text = "성능 경고 감지!";
                _warningText.color = _dangerColor;
            }
        }
        
        private void OnAutoOptimization(string message)
        {
            if (_optimizationText != null)
            {
                _optimizationText.text = $"자동 최적화: {message}";
                _optimizationText.color = _warningColor;
            }
        }
        #endregion
        
        #region UI Updates
        private void UpdatePerformanceUI()
        {
            if (!_isVisible || _currentData == null) return;
            
            // FPS 업데이트
            UpdateFPSText();
            UpdateFPSSlider();
            
            // 메모리 업데이트
            UpdateMemoryText();
            UpdateMemorySlider();
            
            // 배터리 업데이트
            UpdateBatteryText();
            UpdateBatterySlider();
            
            // CPU 업데이트
            UpdateCPUText();
            UpdateCPUSlider();
            
            // GPU 업데이트
            UpdateGPUText();
            UpdateGPUSlider();
            
            // 온도 업데이트
            UpdateTemperatureText();
            
            // 경고 텍스트 업데이트
            UpdateWarningText();
        }
        
        private void UpdateFPSText()
        {
            if (_fpsText != null)
            {
                _fpsText.text = $"FPS: {_currentData.fps:F1}";
                _fpsText.color = GetPerformanceColor(_currentData.fps, 30f, 45f);
            }
        }
        
        private void UpdateFPSSlider()
        {
            if (_fpsSlider != null)
            {
                _fpsSlider.value = _currentData.fps;
                _fpsSlider.fillRect.GetComponent<Image>().color = GetPerformanceColor(_currentData.fps, 30f, 45f);
            }
        }
        
        private void UpdateMemoryText()
        {
            if (_memoryText != null)
            {
                float memoryMB = _currentData.memoryUsage * SystemInfo.systemMemorySize;
                _memoryText.text = $"메모리: {memoryMB:F0}MB ({_currentData.memoryUsage:P1})";
                _memoryText.color = GetPerformanceColor(_currentData.memoryUsage, 0.6f, 0.8f, true);
            }
        }
        
        private void UpdateMemorySlider()
        {
            if (_memorySlider != null)
            {
                _memorySlider.value = _currentData.memoryUsage;
                _memorySlider.fillRect.GetComponent<Image>().color = GetPerformanceColor(_currentData.memoryUsage, 0.6f, 0.8f, true);
            }
        }
        
        private void UpdateBatteryText()
        {
            if (_batteryText != null)
            {
                _batteryText.text = $"배터리: {_currentData.batteryLevel:P0}";
                _batteryText.color = GetPerformanceColor(_currentData.batteryLevel, 0.2f, 0.5f);
            }
        }
        
        private void UpdateBatterySlider()
        {
            if (_batterySlider != null)
            {
                _batterySlider.value = _currentData.batteryLevel;
                _batterySlider.fillRect.GetComponent<Image>().color = GetPerformanceColor(_currentData.batteryLevel, 0.2f, 0.5f);
            }
        }
        
        private void UpdateCPUText()
        {
            if (_cpuText != null)
            {
                _cpuText.text = $"CPU: {_currentData.cpuUsage:P0}";
                _cpuText.color = GetPerformanceColor(_currentData.cpuUsage, 0.7f, 0.9f, true);
            }
        }
        
        private void UpdateCPUSlider()
        {
            if (_cpuSlider != null)
            {
                _cpuSlider.value = _currentData.cpuUsage;
                _cpuSlider.fillRect.GetComponent<Image>().color = GetPerformanceColor(_currentData.cpuUsage, 0.7f, 0.9f, true);
            }
        }
        
        private void UpdateGPUText()
        {
            if (_gpuText != null)
            {
                _gpuText.text = $"GPU: {_currentData.gpuUsage:P0}";
                _gpuText.color = GetPerformanceColor(_currentData.gpuUsage, 0.7f, 0.9f, true);
            }
        }
        
        private void UpdateGPUSlider()
        {
            if (_gpuSlider != null)
            {
                _gpuSlider.value = _currentData.gpuUsage;
                _gpuSlider.fillRect.GetComponent<Image>().color = GetPerformanceColor(_currentData.gpuUsage, 0.7f, 0.9f, true);
            }
        }
        
        private void UpdateTemperatureText()
        {
            if (_temperatureText != null)
            {
                _temperatureText.text = $"온도: {_currentData.temperature:F1}°C";
                _temperatureText.color = GetPerformanceColor(_currentData.temperature, 45f, 60f, true);
            }
        }
        
        private void UpdateWarningText()
        {
            if (_warningText != null)
            {
                // 경고가 없으면 텍스트 숨김
                if (!HasPerformanceWarning())
                {
                    _warningText.text = "";
                    _warningText.color = Color.white;
                }
            }
        }
        
        private bool HasPerformanceWarning()
        {
            if (_currentData == null) return false;
            
            return _currentData.fps < PerformanceThresholds.LOW_FPS ||
                   _currentData.memoryUsage > PerformanceThresholds.HIGH_MEMORY ||
                   _currentData.batteryLevel < PerformanceThresholds.LOW_BATTERY ||
                   _currentData.cpuUsage > PerformanceThresholds.HIGH_CPU ||
                   _currentData.gpuUsage > PerformanceThresholds.HIGH_GPU ||
                   _currentData.temperature > PerformanceThresholds.OVERHEAT_TEMPERATURE;
        }
        #endregion
        
        #region Utility Methods
        private Color GetPerformanceColor(float value, float warningThreshold, float dangerThreshold, bool inverse = false)
        {
            if (inverse)
            {
                // 높을수록 나쁜 경우 (메모리, CPU, GPU, 온도)
                if (value >= dangerThreshold) return _dangerColor;
                if (value >= warningThreshold) return _warningColor;
                return _goodColor;
            }
            else
            {
                // 높을수록 좋은 경우 (FPS, 배터리)
                if (value <= dangerThreshold) return _dangerColor;
                if (value <= warningThreshold) return _warningColor;
                return _goodColor;
            }
        }
        
        private void UpdateVisibility()
        {
            bool shouldShow = false;
            
            #if UNITY_EDITOR
            shouldShow = _showInEditor;
            #elif DEVELOPMENT_BUILD
            shouldShow = _showInDevelopmentBuild;
            #else
            shouldShow = _showInReleaseBuild;
            #endif
            
            if (_performancePanel != null)
            {
                _performancePanel.SetActive(shouldShow);
                _isVisible = shouldShow;
            }
        }
        
        /// <summary>
        /// UI 표시/숨김 토글
        /// </summary>
        public void ToggleUI()
        {
            if (_performancePanel != null)
            {
                _isVisible = !_isVisible;
                _performancePanel.SetActive(_isVisible);
            }
        }
        
        /// <summary>
        /// 성능 히스토리 반환
        /// </summary>
        public List<PerformanceData> GetPerformanceHistory()
        {
            return new List<PerformanceData>(_performanceHistory);
        }
        
        /// <summary>
        /// 평균 성능 데이터 반환
        /// </summary>
        public PerformanceData GetAveragePerformance()
        {
            if (_performanceHistory.Count == 0)
                return new PerformanceData();
                
            var avgData = new PerformanceData();
            int count = _performanceHistory.Count;
            
            foreach (var data in _performanceHistory)
            {
                avgData.fps += data.fps;
                avgData.memoryUsage += data.memoryUsage;
                avgData.batteryLevel += data.batteryLevel;
                avgData.cpuUsage += data.cpuUsage;
                avgData.gpuUsage += data.gpuUsage;
                avgData.temperature += data.temperature;
            }
            
            avgData.fps /= count;
            avgData.memoryUsage /= count;
            avgData.batteryLevel /= count;
            avgData.cpuUsage /= count;
            avgData.gpuUsage /= count;
            avgData.temperature /= count;
            
            return avgData;
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// UI 표시 설정
        /// </summary>
        public void SetUIVisibility(bool showInEditor, bool showInDevelopment, bool showInRelease)
        {
            _showInEditor = showInEditor;
            _showInDevelopmentBuild = showInDevelopment;
            _showInReleaseBuild = showInRelease;
            UpdateVisibility();
        }
        
        /// <summary>
        /// 업데이트 간격 설정
        /// </summary>
        public void SetUpdateInterval(float interval)
        {
            _updateInterval = Mathf.Clamp(interval, 0.1f, 5f);
        }
        
        /// <summary>
        /// 색상 설정
        /// </summary>
        public void SetColors(Color good, Color warning, Color danger)
        {
            _goodColor = good;
            _warningColor = warning;
            _dangerColor = danger;
        }
        #endregion
    }
} 