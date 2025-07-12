using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game.Managers
{
    /// <summary>
    /// 성능 임계값 정의
    /// </summary>
    public static class PerformanceThresholds
    {
        public const float LOW_FPS = 30f;
        public const float HIGH_MEMORY = 0.8f;
        public const float LOW_BATTERY = 0.2f;
        public const float HIGH_CPU = 0.9f;
        public const float HIGH_GPU = 0.9f;
        public const float OVERHEAT_TEMPERATURE = 60f; // 섭씨
    }

    /// <summary>
    /// 성능 데이터 구조
    /// </summary>
    [System.Serializable]
    public class PerformanceData
    {
        public float fps;
        public float memoryUsage;
        public float batteryLevel;
        public float cpuUsage;
        public float gpuUsage;
        public float temperature;
        public long timestamp;
        public string deviceInfo;

        public PerformanceData()
        {
            timestamp = DateTime.Now.Ticks;
            deviceInfo = SystemInfo.deviceModel;
        }
    }

    /// <summary>
    /// 성능 모니터링 및 자동 최적화 매니저
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        #region Singleton
        private static PerformanceMonitor _instance;
        public static PerformanceMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PerformanceMonitor>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("PerformanceMonitor");
                        _instance = go.AddComponent<PerformanceMonitor>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeMonitor();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// 성능 저하 감지 이벤트
        /// </summary>
        public static event Action<PerformanceData> OnPerformanceWarning;

        /// <summary>
        /// 자동 최적화 실행 이벤트
        /// </summary>
        public static event Action<string> OnAutoOptimization;

        /// <summary>
        /// 성능 데이터 업데이트 이벤트
        /// </summary>
        public static event Action<PerformanceData> OnPerformanceDataUpdated;
        #endregion

        #region Properties
        [Header("Monitoring Settings")]
        [SerializeField] private bool _enableMonitoring = true;
        [SerializeField] private float _monitoringInterval = 1f;
        [SerializeField] private int _dataBufferSize = 60; // 1분간의 데이터
        [SerializeField] private bool _enableAutoOptimization = true;
        [SerializeField] private bool _showPerformanceUI = true;

        [Header("Optimization Settings")]
        [SerializeField] private bool _enableFPSOptimization = true;
        [SerializeField] private bool _enableMemoryOptimization = true;
        [SerializeField] private bool _enableBatteryOptimization = true;
        [SerializeField] private bool _enableThermalOptimization = true;

        [Header("Debug")]
        [SerializeField] private bool _enableDebugLogs = true;
        [SerializeField] private bool _showDetailedInfo = false;

        // 성능 데이터 버퍼
        private Queue<PerformanceData> _performanceDataBuffer;
        private PerformanceData _currentData;
        private PerformanceData _lastData;

        // 모니터링 상태
        private bool _isMonitoring = false;
        private Coroutine _monitoringCoroutine;
        private float _lastMonitoringTime = 0f;

        // FPS 계산
        private int _frameCount = 0;
        private float _fpsUpdateTime = 0f;
        private float _currentFPS = 0f;

        // 최적화 상태
        private bool _isOptimized = false;
        private int _optimizationLevel = 0;
        private float _lastOptimizationTime = 0f;
        private const float OPTIMIZATION_COOLDOWN = 30f; // 30초 쿨다운

        // 성능 경고 상태
        private bool _hasPerformanceWarning = false;
        private string _lastWarningMessage = "";
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            if (_enableMonitoring)
                StartMonitoring();
        }

        private void Update()
        {
            // FPS 계산
            _frameCount++;
            if (Time.time - _fpsUpdateTime >= 1f)
            {
                _currentFPS = _frameCount;
                _frameCount = 0;
                _fpsUpdateTime = Time.time;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // 앱이 백그라운드로 진입 시 모니터링 일시 중지
                if (_isMonitoring)
                    StopMonitoring();
            }
            else
            {
                // 앱이 포그라운드로 복귀 시 모니터링 재개
                if (_enableMonitoring && !_isMonitoring)
                    StartMonitoring();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 모니터링 시작
        /// </summary>
        public void StartMonitoring()
        {
            if (_isMonitoring) return;

            _isMonitoring = true;
            _performanceDataBuffer = new Queue<PerformanceData>();
            _currentData = new PerformanceData();
            _lastData = new PerformanceData();

            if (_monitoringCoroutine != null)
                StopCoroutine(_monitoringCoroutine);

            _monitoringCoroutine = StartCoroutine(MonitoringCoroutine());

            if (_enableDebugLogs)
                GameLogger.Log("성능 모니터링 시작", LogType.Info);
        }

        /// <summary>
        /// 모니터링 중지
        /// </summary>
        public void StopMonitoring()
        {
            if (!_isMonitoring) return;

            _isMonitoring = false;
            if (_monitoringCoroutine != null)
            {
                StopCoroutine(_monitoringCoroutine);
                _monitoringCoroutine = null;
            }

            if (_enableDebugLogs)
                GameLogger.Log("성능 모니터링 중지", LogType.Info);
        }

        /// <summary>
        /// 현재 FPS 반환
        /// </summary>
        public float GetCurrentFPS()
        {
            return _currentFPS;
        }

        /// <summary>
        /// 메모리 사용량 반환 (MB)
        /// </summary>
        public float GetMemoryUsage()
        {
            return System.GC.GetTotalMemory(false) / 1024f / 1024f;
        }

        /// <summary>
        /// 메모리 사용률 반환 (0-1)
        /// </summary>
        public float GetMemoryUsageRatio()
        {
            long totalMemory = SystemInfo.systemMemorySize * 1024L * 1024L;
            long usedMemory = System.GC.GetTotalMemory(false);
            return (float)usedMemory / totalMemory;
        }

        /// <summary>
        /// 배터리 레벨 반환 (0-1)
        /// </summary>
        public float GetBatteryLevel()
        {
            return SystemInfo.batteryLevel;
        }

        /// <summary>
        /// 성능 체크
        /// </summary>
        public PerformanceData CheckPerformance()
        {
            _currentData = new PerformanceData
            {
                fps = _currentFPS,
                memoryUsage = GetMemoryUsageRatio(),
                batteryLevel = GetBatteryLevel(),
                cpuUsage = GetCPUUsage(),
                gpuUsage = GetGPUUsage(),
                temperature = GetDeviceTemperature()
            };

            // 성능 데이터 버퍼에 추가
            _performanceDataBuffer.Enqueue(_currentData);
            if (_performanceDataBuffer.Count > _dataBufferSize)
            {
                _performanceDataBuffer.Dequeue();
            }

            // 성능 경고 체크
            CheckPerformanceWarnings();

            // 자동 최적화 체크
            if (_enableAutoOptimization && Time.time - _lastOptimizationTime > OPTIMIZATION_COOLDOWN)
            {
                CheckAutoOptimization();
            }

            OnPerformanceDataUpdated?.Invoke(_currentData);

            return _currentData;
        }

        /// <summary>
        /// 자동 최적화 실행
        /// </summary>
        public void AutoOptimize()
        {
            if (Time.time - _lastOptimizationTime < OPTIMIZATION_COOLDOWN)
            {
                if (_enableDebugLogs)
                    GameLogger.Log("최적화 쿨다운 중", LogType.Warning);
                return;
            }

            _lastOptimizationTime = Time.time;
            _optimizationLevel++;

            if (_enableDebugLogs)
                GameLogger.Log($"자동 최적화 실행 (레벨: {_optimizationLevel})", LogType.Info);

            // FPS 최적화
            if (_enableFPSOptimization && _currentData.fps < PerformanceThresholds.LOW_FPS)
            {
                OptimizeFPS();
            }

            // 메모리 최적화
            if (_enableMemoryOptimization && _currentData.memoryUsage > PerformanceThresholds.HIGH_MEMORY)
            {
                OptimizeMemory();
            }

            // 배터리 최적화
            if (_enableBatteryOptimization && _currentData.batteryLevel < PerformanceThresholds.LOW_BATTERY)
            {
                OptimizeBattery();
            }

            // 온도 최적화
            if (_enableThermalOptimization && _currentData.temperature > PerformanceThresholds.OVERHEAT_TEMPERATURE)
            {
                OptimizeThermal();
            }

            _isOptimized = true;
            OnAutoOptimization?.Invoke($"최적화 레벨 {_optimizationLevel} 적용");
        }

        /// <summary>
        /// 성능 정보 UI 표시
        /// </summary>
        public void ShowPerformanceUI()
        {
            if (!_showPerformanceUI) return;

            // 개발 빌드에서만 UI 표시
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            // UI 표시 로직은 별도 UI 스크립트에서 처리
            #endif
        }

        /// <summary>
        /// 성능 데이터 히스토리 반환
        /// </summary>
        public List<PerformanceData> GetPerformanceHistory()
        {
            return _performanceDataBuffer.ToList();
        }

        /// <summary>
        /// 평균 성능 데이터 반환
        /// </summary>
        public PerformanceData GetAveragePerformance()
        {
            if (_performanceDataBuffer.Count == 0)
                return new PerformanceData();

            var avgData = new PerformanceData();
            int count = _performanceDataBuffer.Count;

            foreach (var data in _performanceDataBuffer)
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

        /// <summary>
        /// 최적화 상태 초기화
        /// </summary>
        public void ResetOptimization()
        {
            _isOptimized = false;
            _optimizationLevel = 0;
            _lastOptimizationTime = 0f;

            // 기본 설정으로 복원
            Application.targetFrameRate = 60;
            QualitySettings.SetQualityLevel(2);

            if (_enableDebugLogs)
                GameLogger.Log("최적화 상태 초기화", LogType.Info);
        }
        #endregion

        #region Private Methods
        private void InitializeMonitor()
        {
            _performanceDataBuffer = new Queue<PerformanceData>();
            _currentData = new PerformanceData();
            _lastData = new PerformanceData();
            _isMonitoring = false;
            _isOptimized = false;
            _optimizationLevel = 0;
            _hasPerformanceWarning = false;

            if (_enableDebugLogs)
                GameLogger.Log("PerformanceMonitor 초기화 완료", LogType.Info);
        }

        private IEnumerator MonitoringCoroutine()
        {
            while (_isMonitoring)
            {
                CheckPerformance();
                yield return new WaitForSeconds(_monitoringInterval);
            }
        }

        private void CheckPerformanceWarnings()
        {
            bool hasWarning = false;
            string warningMessage = "";

            // FPS 경고
            if (_currentData.fps < PerformanceThresholds.LOW_FPS)
            {
                hasWarning = true;
                warningMessage += $"FPS 낮음: {_currentData.fps:F1} ";
            }

            // 메모리 경고
            if (_currentData.memoryUsage > PerformanceThresholds.HIGH_MEMORY)
            {
                hasWarning = true;
                warningMessage += $"메모리 사용량 높음: {_currentData.memoryUsage:P1} ";
            }

            // 배터리 경고
            if (_currentData.batteryLevel < PerformanceThresholds.LOW_BATTERY)
            {
                hasWarning = true;
                warningMessage += $"배터리 부족: {_currentData.batteryLevel:P1} ";
            }

            // CPU 경고
            if (_currentData.cpuUsage > PerformanceThresholds.HIGH_CPU)
            {
                hasWarning = true;
                warningMessage += $"CPU 사용률 높음: {_currentData.cpuUsage:P1} ";
            }

            // GPU 경고
            if (_currentData.gpuUsage > PerformanceThresholds.HIGH_GPU)
            {
                hasWarning = true;
                warningMessage += $"GPU 사용률 높음: {_currentData.gpuUsage:P1} ";
            }

            // 온도 경고
            if (_currentData.temperature > PerformanceThresholds.OVERHEAT_TEMPERATURE)
            {
                hasWarning = true;
                warningMessage += $"과열 감지: {_currentData.temperature:F1}°C ";
            }

            if (hasWarning && !_hasPerformanceWarning)
            {
                _hasPerformanceWarning = true;
                _lastWarningMessage = warningMessage;
                OnPerformanceWarning?.Invoke(_currentData);

                if (_enableDebugLogs)
                    GameLogger.LogWarning($"성능 경고: {warningMessage}");
            }
            else if (!hasWarning && _hasPerformanceWarning)
            {
                _hasPerformanceWarning = false;
                if (_enableDebugLogs)
                    GameLogger.Log("성능 경고 해제", LogType.Info);
            }
        }

        private void CheckAutoOptimization()
        {
            bool needsOptimization = false;

            if (_currentData.fps < PerformanceThresholds.LOW_FPS) needsOptimization = true;
            if (_currentData.memoryUsage > PerformanceThresholds.HIGH_MEMORY) needsOptimization = true;
            if (_currentData.batteryLevel < PerformanceThresholds.LOW_BATTERY) needsOptimization = true;
            if (_currentData.temperature > PerformanceThresholds.OVERHEAT_TEMPERATURE) needsOptimization = true;

            if (needsOptimization)
            {
                AutoOptimize();
            }
        }

        private float GetCPUUsage()
        {
            // CPU 사용률은 모바일에서 정확히 측정하기 어려우므로 추정값 사용
            return Mathf.Clamp01(Time.deltaTime * 60f / Application.targetFrameRate);
        }

        private float GetGPUUsage()
        {
            // GPU 사용률은 모바일에서 정확히 측정하기 어려우므로 추정값 사용
            return Mathf.Clamp01(QualitySettings.GetQualityLevel() / 5f);
        }

        private float GetDeviceTemperature()
        {
            // 실제 온도는 디바이스별로 다르므로 추정값 사용
            // 실제 구현에서는 디바이스별 API 사용 필요
            return 25f + (SystemInfo.processorFrequency / 1000f) * 0.1f;
        }

        private void OptimizeFPS()
        {
            if (_optimizationLevel == 1)
            {
                Application.targetFrameRate = 45;
                QualitySettings.SetQualityLevel(1);
            }
            else if (_optimizationLevel == 2)
            {
                Application.targetFrameRate = 30;
                QualitySettings.SetQualityLevel(0);
            }
            else
            {
                Application.targetFrameRate = 15;
                QualitySettings.SetQualityLevel(0);
                QualitySettings.vSyncCount = 0;
            }

            if (_enableDebugLogs)
                GameLogger.Log($"FPS 최적화 적용 - 목표 FPS: {Application.targetFrameRate}", LogType.Info);
        }

        private void OptimizeMemory()
        {
            // 불필요한 리소스 해제
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            // 캐시 정리
            if (Caching.CleanCache())
            {
                if (_enableDebugLogs)
                    GameLogger.Log("메모리 최적화 - 캐시 정리 완료", LogType.Info);
            }

            if (_enableDebugLogs)
                GameLogger.Log("메모리 최적화 적용", LogType.Info);
        }

        private void OptimizeBattery()
        {
            // 프레임레이트 제한
            Application.targetFrameRate = 30;

            // 품질 설정 최소화
            QualitySettings.SetQualityLevel(0);
            QualitySettings.vSyncCount = 0;

            // 오디오 볼륨 낮춤
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(0.5f);
            }

            if (_enableDebugLogs)
                GameLogger.Log("배터리 절약 모드 적용", LogType.Info);
        }

        private void OptimizeThermal()
        {
            // 프레임레이트 더욱 제한
            Application.targetFrameRate = 20;

            // 품질 설정 최소화
            QualitySettings.SetQualityLevel(0);
            QualitySettings.vSyncCount = 0;

            // 오디오 볼륨 더욱 낮춤
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(0.3f);
            }

            if (_enableDebugLogs)
                GameLogger.Log("과열 방지 모드 적용", LogType.Warning);
        }
        #endregion

        #region Debug Methods
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnGUI()
        {
            if (!_showPerformanceUI) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 250));
            GUILayout.Label("Performance Monitor Debug:");
            
            if (_currentData != null)
            {
                GUILayout.Label($"FPS: {_currentData.fps:F1}");
                GUILayout.Label($"Memory: {_currentData.memoryUsage:P1}");
                GUILayout.Label($"Battery: {_currentData.batteryLevel:P1}");
                GUILayout.Label($"CPU: {_currentData.cpuUsage:P1}");
                GUILayout.Label($"GPU: {_currentData.gpuUsage:P1}");
                GUILayout.Label($"Temperature: {_currentData.temperature:F1}°C");
                GUILayout.Label($"Optimization Level: {_optimizationLevel}");
                GUILayout.Label($"Is Optimized: {_isOptimized}");
            }

            if (_hasPerformanceWarning)
            {
                GUI.color = Color.red;
                GUILayout.Label($"Warning: {_lastWarningMessage}");
                GUI.color = Color.white;
            }

            if (GUILayout.Button("Auto Optimize"))
                AutoOptimize();

            if (GUILayout.Button("Reset Optimization"))
                ResetOptimization();

            if (GUILayout.Button(_isMonitoring ? "Stop Monitoring" : "Start Monitoring"))
            {
                if (_isMonitoring)
                    StopMonitoring();
                else
                    StartMonitoring();
            }

            GUILayout.EndArea();
        }
        #endregion
    }
} 