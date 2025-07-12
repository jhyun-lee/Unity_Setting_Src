using UnityEngine;
using UnityEngine.UI;
using Game.Managers;

namespace Game.Examples
{
    /// <summary>
    /// PerformanceMonitor 사용 예제
    /// </summary>
    public class PerformanceMonitorExample : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _startMonitoringButton;
        [SerializeField] private Button _stopMonitoringButton;
        [SerializeField] private Button _autoOptimizeButton;
        [SerializeField] private Button _resetOptimizationButton;
        [SerializeField] private Button _toggleUIButton;
        
        [Header("Performance Display")]
        [SerializeField] private Text _currentFPS;
        [SerializeField] private Text _currentMemory;
        [SerializeField] private Text _currentBattery;
        [SerializeField] private Text _currentCPU;
        [SerializeField] private Text _currentGPU;
        [SerializeField] private Text _currentTemperature;
        [SerializeField] private Text _optimizationLevel;
        [SerializeField] private Text _warningStatus;
        
        [Header("Settings")]
        [SerializeField] private bool _enableAutoOptimization = true;
        [SerializeField] private bool _showPerformanceWarnings = true;
        [SerializeField] private float _updateInterval = 1f;
        
        private bool _isMonitoring = false;
        private PerformanceData _lastPerformanceData;
        
        private void Start()
        {
            // UI 버튼 이벤트 연결
            SetupUIButtons();
            
            // 이벤트 구독
            SubscribeToEvents();
            
            // 초기 상태 설정
            UpdateUI();
        }
        
        private void OnDestroy()
        {
            // 이벤트 구독 해제
            UnsubscribeFromEvents();
        }
        
        private void Update()
        {
            // 주기적으로 UI 업데이트
            if (Time.time % _updateInterval < Time.deltaTime)
            {
                UpdatePerformanceDisplay();
            }
        }
        
        #region UI Setup
        private void SetupUIButtons()
        {
            if (_startMonitoringButton != null)
                _startMonitoringButton.onClick.AddListener(OnStartMonitoringClicked);
            
            if (_stopMonitoringButton != null)
                _stopMonitoringButton.onClick.AddListener(OnStopMonitoringClicked);
            
            if (_autoOptimizeButton != null)
                _autoOptimizeButton.onClick.AddListener(OnAutoOptimizeClicked);
            
            if (_resetOptimizationButton != null)
                _resetOptimizationButton.onClick.AddListener(OnResetOptimizationClicked);
            
            if (_toggleUIButton != null)
                _toggleUIButton.onClick.AddListener(OnToggleUIClicked);
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
            _lastPerformanceData = data;
            UpdatePerformanceDisplay();
        }
        
        private void OnPerformanceWarning(PerformanceData data)
        {
            if (_showPerformanceWarnings)
            {
                GameLogger.LogWarning($"성능 경고 감지: FPS={data.fps:F1}, 메모리={data.memoryUsage:P1}, 배터리={data.batteryLevel:P1}");
                UpdateWarningStatus("성능 경고 감지!");
            }
        }
        
        private void OnAutoOptimization(string message)
        {
            GameLogger.LogInfo($"자동 최적화 실행: {message}");
            UpdateOptimizationLevel();
        }
        #endregion
        
        #region Button Handlers
        private void OnStartMonitoringClicked()
        {
            if (PerformanceMonitor.Instance != null)
            {
                PerformanceMonitor.Instance.StartMonitoring();
                _isMonitoring = true;
                UpdateUI();
                GameLogger.Log("성능 모니터링 시작", LogType.Info);
            }
        }
        
        private void OnStopMonitoringClicked()
        {
            if (PerformanceMonitor.Instance != null)
            {
                PerformanceMonitor.Instance.StopMonitoring();
                _isMonitoring = false;
                UpdateUI();
                GameLogger.Log("성능 모니터링 중지", LogType.Info);
            }
        }
        
        private void OnAutoOptimizeClicked()
        {
            if (PerformanceMonitor.Instance != null)
            {
                PerformanceMonitor.Instance.AutoOptimize();
                UpdateOptimizationLevel();
                GameLogger.Log("수동 최적화 실행", LogType.Info);
            }
        }
        
        private void OnResetOptimizationClicked()
        {
            if (PerformanceMonitor.Instance != null)
            {
                PerformanceMonitor.Instance.ResetOptimization();
                UpdateOptimizationLevel();
                GameLogger.Log("최적화 상태 초기화", LogType.Info);
            }
        }
        
        private void OnToggleUIClicked()
        {
            if (PerformanceMonitor.Instance != null)
            {
                PerformanceMonitor.Instance.ShowPerformanceUI();
                GameLogger.Log("성능 UI 토글", LogType.Info);
            }
        }
        #endregion
        
        #region UI Updates
        private void UpdateUI()
        {
            // 버튼 상태 업데이트
            if (_startMonitoringButton != null)
                _startMonitoringButton.interactable = !_isMonitoring;
            
            if (_stopMonitoringButton != null)
                _stopMonitoringButton.interactable = _isMonitoring;
            
            // 모니터링 상태 표시
            UpdateMonitoringStatus();
        }
        
        private void UpdateMonitoringStatus()
        {
            if (_warningStatus != null)
            {
                if (_isMonitoring)
                {
                    _warningStatus.text = "모니터링 중";
                    _warningStatus.color = Color.green;
                }
                else
                {
                    _warningStatus.text = "모니터링 중지";
                    _warningStatus.color = Color.gray;
                }
            }
        }
        
        private void UpdatePerformanceDisplay()
        {
            if (_lastPerformanceData == null) return;
            
            // FPS 표시
            if (_currentFPS != null)
            {
                _currentFPS.text = $"FPS: {_lastPerformanceData.fps:F1}";
                _currentFPS.color = GetPerformanceColor(_lastPerformanceData.fps, 30f, 45f);
            }
            
            // 메모리 표시
            if (_currentMemory != null)
            {
                float memoryMB = _lastPerformanceData.memoryUsage * SystemInfo.systemMemorySize;
                _currentMemory.text = $"메모리: {memoryMB:F0}MB ({_lastPerformanceData.memoryUsage:P1})";
                _currentMemory.color = GetPerformanceColor(_lastPerformanceData.memoryUsage, 0.6f, 0.8f, true);
            }
            
            // 배터리 표시
            if (_currentBattery != null)
            {
                _currentBattery.text = $"배터리: {_lastPerformanceData.batteryLevel:P0}";
                _currentBattery.color = GetPerformanceColor(_lastPerformanceData.batteryLevel, 0.2f, 0.5f);
            }
            
            // CPU 표시
            if (_currentCPU != null)
            {
                _currentCPU.text = $"CPU: {_lastPerformanceData.cpuUsage:P0}";
                _currentCPU.color = GetPerformanceColor(_lastPerformanceData.cpuUsage, 0.7f, 0.9f, true);
            }
            
            // GPU 표시
            if (_currentGPU != null)
            {
                _currentGPU.text = $"GPU: {_lastPerformanceData.gpuUsage:P0}";
                _currentGPU.color = GetPerformanceColor(_lastPerformanceData.gpuUsage, 0.7f, 0.9f, true);
            }
            
            // 온도 표시
            if (_currentTemperature != null)
            {
                _currentTemperature.text = $"온도: {_lastPerformanceData.temperature:F1}°C";
                _currentTemperature.color = GetPerformanceColor(_lastPerformanceData.temperature, 45f, 60f, true);
            }
        }
        
        private void UpdateOptimizationLevel()
        {
            if (_optimizationLevel != null && PerformanceMonitor.Instance != null)
            {
                // 최적화 레벨은 PerformanceMonitor에서 직접 접근할 수 없으므로
                // 이벤트를 통해 전달받은 정보를 사용
                _optimizationLevel.text = "최적화 레벨: 확인 중";
                _optimizationLevel.color = Color.yellow;
            }
        }
        
        private void UpdateWarningStatus(string message)
        {
            if (_warningStatus != null)
            {
                _warningStatus.text = message;
                _warningStatus.color = Color.red;
            }
        }
        
        private Color GetPerformanceColor(float value, float warningThreshold, float dangerThreshold, bool inverse = false)
        {
            if (inverse)
            {
                // 높을수록 나쁜 경우 (메모리, CPU, GPU, 온도)
                if (value >= dangerThreshold) return Color.red;
                if (value >= warningThreshold) return Color.yellow;
                return Color.green;
            }
            else
            {
                // 높을수록 좋은 경우 (FPS, 배터리)
                if (value <= dangerThreshold) return Color.red;
                if (value <= warningThreshold) return Color.yellow;
                return Color.green;
            }
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// 성능 테스트 실행
        /// </summary>
        [ContextMenu("Run Performance Test")]
        public void RunPerformanceTest()
        {
            if (PerformanceMonitor.Instance != null)
            {
                GameLogger.LogInfo("성능 테스트 시작");
                
                // 현재 성능 데이터 출력
                var data = PerformanceMonitor.Instance.CheckPerformance();
                GameLogger.LogInfo($"현재 FPS: {data.fps:F1}");
                GameLogger.LogInfo($"메모리 사용률: {data.memoryUsage:P1}");
                GameLogger.LogInfo($"배터리 레벨: {data.batteryLevel:P1}");
                GameLogger.LogInfo($"CPU 사용률: {data.cpuUsage:P1}");
                GameLogger.LogInfo($"GPU 사용률: {data.gpuUsage:P1}");
                GameLogger.LogInfo($"온도: {data.temperature:F1}°C");
                
                // 평균 성능 데이터 출력
                var avgData = PerformanceMonitor.Instance.GetAveragePerformance();
                GameLogger.LogInfo($"평균 FPS: {avgData.fps:F1}");
                GameLogger.LogInfo($"평균 메모리 사용률: {avgData.memoryUsage:P1}");
            }
        }
        
        /// <summary>
        /// 성능 히스토리 출력
        /// </summary>
        [ContextMenu("Print Performance History")]
        public void PrintPerformanceHistory()
        {
            if (PerformanceMonitor.Instance != null)
            {
                var history = PerformanceMonitor.Instance.GetPerformanceHistory();
                GameLogger.LogInfo($"성능 히스토리 ({history.Count}개 데이터)");
                
                for (int i = 0; i < Mathf.Min(history.Count, 10); i++)
                {
                    var data = history[history.Count - 1 - i];
                    GameLogger.LogInfo($"데이터 {i + 1}: FPS={data.fps:F1}, 메모리={data.memoryUsage:P1}, 배터리={data.batteryLevel:P1}");
                }
            }
        }
        
        /// <summary>
        /// 자동 최적화 설정 변경
        /// </summary>
        public void SetAutoOptimization(bool enabled)
        {
            _enableAutoOptimization = enabled;
            GameLogger.LogInfo($"자동 최적화 설정: {(enabled ? "활성화" : "비활성화")}");
        }
        
        /// <summary>
        /// 성능 경고 표시 설정 변경
        /// </summary>
        public void SetPerformanceWarnings(bool enabled)
        {
            _showPerformanceWarnings = enabled;
            GameLogger.LogInfo($"성능 경고 표시 설정: {(enabled ? "활성화" : "비활성화")}");
        }
        #endregion
        
        #region Debug Methods
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 300, 300, 200));
            GUILayout.Label("Performance Monitor Example Debug:");
            
            if (PerformanceMonitor.Instance != null)
            {
                GUILayout.Label($"모니터링 상태: {(_isMonitoring ? "활성" : "비활성")}");
                GUILayout.Label($"현재 FPS: {PerformanceMonitor.Instance.GetCurrentFPS():F1}");
                GUILayout.Label($"메모리 사용량: {PerformanceMonitor.Instance.GetMemoryUsage():F1}MB");
                GUILayout.Label($"배터리 레벨: {PerformanceMonitor.Instance.GetBatteryLevel():P1}");
            }
            else
            {
                GUILayout.Label("PerformanceMonitor: NULL");
            }
            
            if (GUILayout.Button("Run Performance Test"))
                RunPerformanceTest();
            
            if (GUILayout.Button("Print History"))
                PrintPerformanceHistory();
            
            if (GUILayout.Button("Toggle Auto Optimization"))
                SetAutoOptimization(!_enableAutoOptimization);
            
            GUILayout.EndArea();
        }
        #endregion
    }
} 