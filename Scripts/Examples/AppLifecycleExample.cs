using UnityEngine;
using UnityEngine.UI;
using Game.Managers;

namespace Game.Examples
{
    /// <summary>
    /// AppLifecycleManager 사용 예제
    /// </summary>
    public class AppLifecycleExample : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text statusText;
        [SerializeField] private Text memoryText;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button restoreButton;
        
        [Header("Settings")]
        [SerializeField] private bool enableAutoSave = true;
        [SerializeField] private bool showDebugInfo = true;
        
        private void Start()
        {
            // UI 버튼 이벤트 연결
            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveButtonClicked);
            
            if (restoreButton != null)
                restoreButton.onClick.AddListener(OnRestoreButtonClicked);
            
            // AppLifecycleManager 이벤트 구독
            SubscribeToEvents();
            
            // 초기 상태 업데이트
            UpdateStatusText();
        }
        
        private void OnDestroy()
        {
            // 이벤트 구독 해제
            UnsubscribeFromEvents();
        }
        
        private void Update()
        {
            // 메모리 사용량 업데이트
            if (memoryText != null)
            {
                long memoryMB = System.GC.GetTotalMemory(false) / 1024 / 1024;
                memoryText.text = $"메모리: {memoryMB}MB";
            }
        }
        
        #region Event Subscriptions
        private void SubscribeToEvents()
        {
            AppLifecycleManager.OnAppPauseChanged += OnAppPauseChanged;
            AppLifecycleManager.OnAppFocusChanged += OnAppFocusChanged;
            AppLifecycleManager.OnAppQuitting += OnAppQuitting;
            AppLifecycleManager.OnLowMemory += OnLowMemory;
            AppLifecycleManager.OnBatteryLow += OnBatteryLow;
            AppLifecycleManager.OnGameStateSaved += OnGameStateSaved;
            AppLifecycleManager.OnGameStateRestored += OnGameStateRestored;
        }
        
        private void UnsubscribeFromEvents()
        {
            AppLifecycleManager.OnAppPauseChanged -= OnAppPauseChanged;
            AppLifecycleManager.OnAppFocusChanged -= OnAppFocusChanged;
            AppLifecycleManager.OnAppQuitting -= OnAppQuitting;
            AppLifecycleManager.OnLowMemory -= OnLowMemory;
            AppLifecycleManager.OnBatteryLow -= OnBatteryLow;
            AppLifecycleManager.OnGameStateSaved -= OnGameStateSaved;
            AppLifecycleManager.OnGameStateRestored -= OnGameStateRestored;
        }
        #endregion
        
        #region Event Handlers
        private void OnAppPauseChanged(bool isPaused)
        {
            if (showDebugInfo)
                GameLogger.Log($"앱 일시정지 상태 변경: {(isPaused ? "일시정지" : "재개")}", LogType.Info);
            
            UpdateStatusText();
        }
        
        private void OnAppFocusChanged(bool hasFocus)
        {
            if (showDebugInfo)
                GameLogger.Log($"앱 포커스 변경: {(hasFocus ? "포커스 획득" : "포커스 해제")}", LogType.Info);
            
            UpdateStatusText();
        }
        
        private void OnAppQuitting()
        {
            if (showDebugInfo)
                GameLogger.Log("앱 종료 시작", LogType.Warning);
            
            // 최종 정리 작업
            PerformFinalCleanup();
        }
        
        private void OnLowMemory()
        {
            if (showDebugInfo)
                GameLogger.Log("메모리 부족 감지 - 리소스 정리", LogType.Warning);
            
            // 불필요한 리소스 해제
            CleanupResources();
        }
        
        private void OnBatteryLow()
        {
            if (showDebugInfo)
                GameLogger.Log("배터리 부족 감지 - 성능 최적화", LogType.Warning);
            
            // 배터리 절약 모드 적용
            ApplyBatterySavingMode();
        }
        
        private void OnGameStateSaved()
        {
            if (showDebugInfo)
                GameLogger.Log("게임 상태 저장 완료", LogType.Info);
            
            UpdateStatusText();
        }
        
        private void OnGameStateRestored()
        {
            if (showDebugInfo)
                GameLogger.Log("게임 상태 복원 완료", LogType.Info);
            
            UpdateStatusText();
        }
        #endregion
        
        #region UI Methods
        private void OnSaveButtonClicked()
        {
            if (AppLifecycleManager.Instance != null)
            {
                AppLifecycleManager.Instance.SaveGameState();
                GameLogger.Log("수동 저장 요청", LogType.Info);
            }
        }
        
        private void OnRestoreButtonClicked()
        {
            if (AppLifecycleManager.Instance != null)
            {
                AppLifecycleManager.Instance.RestoreGameState();
                GameLogger.Log("수동 복원 요청", LogType.Info);
            }
        }
        
        private void UpdateStatusText()
        {
            if (statusText == null) return;
            
            string status = "앱 상태: ";
            
            if (AppLifecycleManager.Instance != null)
            {
                status += $"일시정지: {(AppLifecycleManager.Instance.IsPaused ? "예" : "아니오")}, ";
                status += $"포커스: {(AppLifecycleManager.Instance.HasFocus ? "예" : "아니오")}, ";
                status += $"종료중: {(AppLifecycleManager.Instance.IsQuitting ? "예" : "아니오")}";
            }
            else
            {
                status += "AppLifecycleManager 없음";
            }
            
            statusText.text = status;
        }
        #endregion
        
        #region Utility Methods
        private void PerformFinalCleanup()
        {
            // 최종 정리 작업
            GameLogger.Log("최종 정리 작업 수행", LogType.Info);
            
            // 로그 파일 저장
            GameLogger.SaveLogToFile();
            
            // 임시 파일 정리
            CleanupTempFiles();
        }
        
        private void CleanupResources()
        {
            GameLogger.Log("리소스 정리 시작", LogType.Info);
            
            // 사용하지 않는 에셋 해제
            Resources.UnloadUnusedAssets();
            
            // 가비지 컬렉션 강제 실행
            System.GC.Collect();
            
            // 캐시 정리
            if (Caching.CleanCache())
            {
                GameLogger.Log("캐시 정리 완료", LogType.Info);
            }
        }
        
        private void ApplyBatterySavingMode()
        {
            GameLogger.Log("배터리 절약 모드 적용", LogType.Info);
            
            // 프레임레이트 제한
            Application.targetFrameRate = 15;
            
            // 품질 설정 최소화
            QualitySettings.SetQualityLevel(0);
            QualitySettings.vSyncCount = 0;
            
            // 오디오 볼륨 낮춤
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(0.3f);
            }
        }
        
        private void CleanupTempFiles()
        {
            // 임시 파일 정리 로직
            GameLogger.Log("임시 파일 정리", LogType.Info);
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// 수동 저장 실행
        /// </summary>
        [ContextMenu("Manual Save")]
        public void ManualSave()
        {
            if (AppLifecycleManager.Instance != null)
            {
                AppLifecycleManager.Instance.SaveGameState();
                GameLogger.Log("수동 저장 실행", LogType.Info);
            }
        }
        
        /// <summary>
        /// 수동 복원 실행
        /// </summary>
        [ContextMenu("Manual Restore")]
        public void ManualRestore()
        {
            if (AppLifecycleManager.Instance != null)
            {
                AppLifecycleManager.Instance.RestoreGameState();
                GameLogger.Log("수동 복원 실행", LogType.Info);
            }
        }
        
        /// <summary>
        /// 빠른 저장 실행
        /// </summary>
        [ContextMenu("Quick Save")]
        public void QuickSave()
        {
            if (AppLifecycleManager.Instance != null)
            {
                AppLifecycleManager.Instance.QuickSave();
                GameLogger.Log("빠른 저장 실행", LogType.Info);
            }
        }
        
        /// <summary>
        /// 배터리 부족 상황 시뮬레이션
        /// </summary>
        [ContextMenu("Simulate Battery Low")]
        public void SimulateBatteryLow()
        {
            if (AppLifecycleManager.Instance != null)
            {
                AppLifecycleManager.Instance.OnBatteryLow();
                GameLogger.Log("배터리 부족 상황 시뮬레이션", LogType.Warning);
            }
        }
        #endregion
        
        #region Debug Methods
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUILayout.BeginArea(new Rect(10, 250, 300, 200));
            GUILayout.Label("AppLifecycle Example Debug:");
            
            if (AppLifecycleManager.Instance != null)
            {
                GUILayout.Label($"Paused: {AppLifecycleManager.Instance.IsPaused}");
                GUILayout.Label($"Has Focus: {AppLifecycleManager.Instance.HasFocus}");
                GUILayout.Label($"Is Quitting: {AppLifecycleManager.Instance.IsQuitting}");
            }
            else
            {
                GUILayout.Label("AppLifecycleManager: NULL");
            }
            
            if (GUILayout.Button("Manual Save"))
                ManualSave();
            
            if (GUILayout.Button("Manual Restore"))
                ManualRestore();
            
            if (GUILayout.Button("Quick Save"))
                QuickSave();
            
            if (GUILayout.Button("Simulate Battery Low"))
                SimulateBatteryLow();
            
            GUILayout.EndArea();
        }
        #endregion
    }
} 