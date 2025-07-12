using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Managers
{
    /// <summary>
    /// 모바일 앱의 생명주기를 관리하고 적절한 시점에 데이터를 저장하는 매니저
    /// </summary>
    public class AppLifecycleManager : MonoBehaviour
    {
        #region Singleton
        private static AppLifecycleManager _instance;
        public static AppLifecycleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AppLifecycleManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AppLifecycleManager");
                        _instance = go.AddComponent<AppLifecycleManager>();
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
                InitializeManager();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// 앱 일시정지/재개 이벤트
        /// </summary>
        public static event Action<bool> OnAppPauseChanged;

        /// <summary>
        /// 앱 포커스 변경 이벤트
        /// </summary>
        public static event Action<bool> OnAppFocusChanged;

        /// <summary>
        /// 앱 종료 이벤트
        /// </summary>
        public static event Action OnAppQuitting;

        /// <summary>
        /// 메모리 부족 이벤트
        /// </summary>
        public static event Action OnLowMemory;

        /// <summary>
        /// 배터리 부족 이벤트
        /// </summary>
        public static event Action OnBatteryLow;

        /// <summary>
        /// 게임 상태 저장 완료 이벤트
        /// </summary>
        public static event Action OnGameStateSaved;

        /// <summary>
        /// 게임 상태 복원 완료 이벤트
        /// </summary>
        public static event Action OnGameStateRestored;
        #endregion

        #region Properties
        [Header("Lifecycle Settings")]
        [SerializeField] private bool _enableAutoSave = true;
        [SerializeField] private float _saveDelay = 0.5f;
        [SerializeField] private bool _enableBatteryOptimization = true;
        [SerializeField] private bool _enableMemoryOptimization = true;

        [Header("Debug")]
        [SerializeField] private bool _enableDebugLogs = true;

        private bool _isPaused = false;
        private bool _hasFocus = true;
        private bool _isQuitting = false;
        private bool _isSaving = false;
        private bool _isRestoring = false;
        private Coroutine _saveCoroutine;
        private Coroutine _restoreCoroutine;

        // 성능 모니터링
        private float _lastMemoryCheck = 0f;
        private float _memoryCheckInterval = 5f;
        private long _lastMemoryUsage = 0;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            if (_enableDebugLogs)
                GameLogger.Log("AppLifecycleManager 초기화 완료", LogType.Info);
        }

        private void Update()
        {
            // 메모리 사용량 모니터링
            if (_enableMemoryOptimization && Time.time - _lastMemoryCheck > _memoryCheckInterval)
            {
                CheckMemoryUsage();
                _lastMemoryCheck = Time.time;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (_isPaused == pauseStatus) return;

            _isPaused = pauseStatus;
            
            if (_enableDebugLogs)
                GameLogger.Log($"앱 일시정지 상태 변경: {pauseStatus}", LogType.Info);

            if (pauseStatus)
            {
                // 앱이 백그라운드로 진입
                HandleAppBackground();
            }
            else
            {
                // 앱이 포그라운드로 복귀
                HandleAppForeground();
            }

            OnAppPauseChanged?.Invoke(pauseStatus);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (_hasFocus == hasFocus) return;

            _hasFocus = hasFocus;
            
            if (_enableDebugLogs)
                GameLogger.Log($"앱 포커스 변경: {hasFocus}", LogType.Info);

            if (!hasFocus)
            {
                // 포커스 잃음 - 빠른 저장
                QuickSave();
            }
            else
            {
                // 포커스 획득 - 상태 복원
                RestoreGameState();
            }

            OnAppFocusChanged?.Invoke(hasFocus);
        }

        private void OnApplicationQuit()
        {
            if (_isQuitting) return;

            _isQuitting = true;
            
            if (_enableDebugLogs)
                GameLogger.Log("앱 종료 시작", LogType.Warning);

            // 최종 저장 수행
            FinalSave();
            
            OnAppQuitting?.Invoke();
        }

        private void OnLowMemory()
        {
            if (_enableDebugLogs)
                GameLogger.Log("메모리 부족 감지", LogType.Warning);

            HandleLowMemory();
            OnLowMemory?.Invoke();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 게임 상태 저장
        /// </summary>
        public void SaveGameState()
        {
            if (_isSaving) return;

            if (_saveCoroutine != null)
                StopCoroutine(_saveCoroutine);

            _saveCoroutine = StartCoroutine(SaveGameStateCoroutine());
        }

        /// <summary>
        /// 게임 상태 복원
        /// </summary>
        public void RestoreGameState()
        {
            if (_isRestoring) return;

            if (_restoreCoroutine != null)
                StopCoroutine(_restoreCoroutine);

            _restoreCoroutine = StartCoroutine(RestoreGameStateCoroutine());
        }

        /// <summary>
        /// 빠른 저장 (최소한의 데이터만)
        /// </summary>
        public void QuickSave()
        {
            if (_isSaving) return;

            StartCoroutine(QuickSaveCoroutine());
        }

        /// <summary>
        /// 배터리 부족 처리
        /// </summary>
        public void OnBatteryLow()
        {
            if (_enableDebugLogs)
                GameLogger.Log("배터리 부족 감지", LogType.Warning);

            HandleBatteryLow();
            OnBatteryLow?.Invoke();
        }

        /// <summary>
        /// 앱이 일시정지 상태인지 확인
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// 앱이 포커스를 가지고 있는지 확인
        /// </summary>
        public bool HasFocus => _hasFocus;

        /// <summary>
        /// 앱이 종료 중인지 확인
        /// </summary>
        public bool IsQuitting => _isQuitting;
        #endregion

        #region Private Methods
        private void InitializeManager()
        {
            _isPaused = false;
            _hasFocus = true;
            _isQuitting = false;
            _isSaving = false;
            _isRestoring = false;
            _lastMemoryCheck = Time.time;
            _lastMemoryUsage = GC.GetTotalMemory(false);

            if (_enableDebugLogs)
                GameLogger.Log("AppLifecycleManager 초기화 완료", LogType.Info);
        }

        private void HandleAppBackground()
        {
            if (_enableDebugLogs)
                GameLogger.Log("앱 백그라운드 진입 처리", LogType.Info);

            // 음악 정지
            if (AudioManager.Instance != null)
                AudioManager.Instance.PauseAllAudio();

            // 자동 저장
            if (_enableAutoSave)
                SaveGameState();

            // 성능 최적화
            if (_enableBatteryOptimization)
                OptimizeForBackground();
        }

        private void HandleAppForeground()
        {
            if (_enableDebugLogs)
                GameLogger.Log("앱 포그라운드 복귀 처리", LogType.Info);

            // 음악 재개
            if (AudioManager.Instance != null)
                AudioManager.Instance.ResumeAllAudio();

            // 게임 상태 복원
            RestoreGameState();

            // 성능 최적화 해제
            if (_enableBatteryOptimization)
                RestorePerformanceSettings();
        }

        private void HandleLowMemory()
        {
            if (_enableDebugLogs)
                GameLogger.Log("메모리 부족 상황 처리", LogType.Warning);

            // 불필요한 리소스 해제
            Resources.UnloadUnusedAssets();
            GC.Collect();

            // 캐시 정리
            if (Caching.CleanCache())
            {
                if (_enableDebugLogs)
                    GameLogger.Log("캐시 정리 완료", LogType.Info);
            }

            // 임시 저장
            QuickSave();
        }

        private void HandleBatteryLow()
        {
            if (_enableDebugLogs)
                GameLogger.Log("배터리 절약 모드 활성화", LogType.Warning);

            // 성능 최적화
            OptimizeForBattery();

            // 자동 저장
            if (_enableAutoSave)
                SaveGameState();
        }

        private void FinalSave()
        {
            if (_enableDebugLogs)
                GameLogger.Log("최종 저장 시작", LogType.Info);

            // 모든 매니저들에게 저장 요청
            if (UserDataManager.Instance != null)
                UserDataManager.Instance.SaveUserData();

            if (SettingsManager.Instance != null)
                SettingsManager.Instance.SaveSettings();

            if (AutoSaveManager.Instance != null)
                AutoSaveManager.Instance.ForceSave();

            if (_enableDebugLogs)
                GameLogger.Log("최종 저장 완료", LogType.Info);
        }

        private void OptimizeForBackground()
        {
            // 프레임레이트 제한
            Application.targetFrameRate = 30;

            // 품질 설정 낮춤
            QualitySettings.SetQualityLevel(0);

            if (_enableDebugLogs)
                GameLogger.Log("백그라운드 성능 최적화 적용", LogType.Info);
        }

        private void OptimizeForBattery()
        {
            // 프레임레이트 더욱 제한
            Application.targetFrameRate = 15;

            // 품질 설정 최소화
            QualitySettings.SetQualityLevel(0);
            QualitySettings.vSyncCount = 0;

            if (_enableDebugLogs)
                GameLogger.Log("배터리 절약 모드 적용", LogType.Info);
        }

        private void RestorePerformanceSettings()
        {
            // 디바이스 정보에 따른 최적 설정 적용
            if (DeviceInfo.Instance != null)
            {
                var recommendedSettings = DeviceInfo.Instance.GetRecommendedSettings();
                Application.targetFrameRate = recommendedSettings.targetFrameRate;
                QualitySettings.SetQualityLevel(recommendedSettings.qualityLevel);
            }
            else
            {
                // 기본 설정
                Application.targetFrameRate = 60;
                QualitySettings.SetQualityLevel(2);
            }

            if (_enableDebugLogs)
                GameLogger.Log("성능 설정 복원", LogType.Info);
        }

        private void CheckMemoryUsage()
        {
            long currentMemory = GC.GetTotalMemory(false);
            long memoryIncrease = currentMemory - _lastMemoryUsage;
            
            if (_enableDebugLogs)
                GameLogger.Log($"메모리 사용량: {currentMemory / 1024 / 1024}MB (증가: {memoryIncrease / 1024 / 1024}MB)", LogType.Debug);

            // 메모리 사용량이 급격히 증가하면 경고
            if (memoryIncrease > 50 * 1024 * 1024) // 50MB 이상 증가
            {
                GameLogger.Log("메모리 사용량 급증 감지", LogType.Warning);
            }

            _lastMemoryUsage = currentMemory;
        }
        #endregion

        #region Coroutines
        private IEnumerator SaveGameStateCoroutine()
        {
            if (_isSaving) yield break;

            _isSaving = true;

            if (_enableDebugLogs)
                GameLogger.Log("게임 상태 저장 시작", LogType.Info);

            try
            {
                // 저장 지연
                yield return new WaitForSeconds(_saveDelay);

                // 사용자 데이터 저장
                if (UserDataManager.Instance != null)
                    UserDataManager.Instance.SaveUserData();

                // 설정 저장
                if (SettingsManager.Instance != null)
                    SettingsManager.Instance.SaveSettings();

                // 자동 저장 매니저에 알림
                if (AutoSaveManager.Instance != null)
                    AutoSaveManager.Instance.OnManualSave();

                if (_enableDebugLogs)
                    GameLogger.Log("게임 상태 저장 완료", LogType.Info);

                OnGameStateSaved?.Invoke();
            }
            catch (System.Exception e)
            {
                GameLogger.LogError($"게임 상태 저장 실패: {e.Message}");
                ErrorHandler.HandleError(ErrorType.DataSave, e);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private IEnumerator RestoreGameStateCoroutine()
        {
            if (_isRestoring) yield break;

            _isRestoring = true;

            if (_enableDebugLogs)
                GameLogger.Log("게임 상태 복원 시작", LogType.Info);

            try
            {
                // 복원 지연
                yield return new WaitForSeconds(_saveDelay);

                // 사용자 데이터 로드
                if (UserDataManager.Instance != null)
                    UserDataManager.Instance.LoadUserData();

                // 설정 로드
                if (SettingsManager.Instance != null)
                    SettingsManager.Instance.LoadSettings();

                if (_enableDebugLogs)
                    GameLogger.Log("게임 상태 복원 완료", LogType.Info);

                OnGameStateRestored?.Invoke();
            }
            catch (System.Exception e)
            {
                GameLogger.LogError($"게임 상태 복원 실패: {e.Message}");
                ErrorHandler.HandleError(ErrorType.DataLoad, e);
            }
            finally
            {
                _isRestoring = false;
            }
        }

        private IEnumerator QuickSaveCoroutine()
        {
            if (_enableDebugLogs)
                GameLogger.Log("빠른 저장 시작", LogType.Info);

            try
            {
                // 최소한의 데이터만 저장
                if (UserDataManager.Instance != null)
                    UserDataManager.Instance.SaveUserData();

                if (_enableDebugLogs)
                    GameLogger.Log("빠른 저장 완료", LogType.Info);
            }
            catch (System.Exception e)
            {
                GameLogger.LogError($"빠른 저장 실패: {e.Message}");
                ErrorHandler.HandleError(ErrorType.DataSave, e);
            }
        }
        #endregion

        #region Debug Methods
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnGUI()
        {
            if (!_enableDebugLogs) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"App Lifecycle Debug Info:");
            GUILayout.Label($"Paused: {_isPaused}");
            GUILayout.Label($"Has Focus: {_hasFocus}");
            GUILayout.Label($"Is Quitting: {_isQuitting}");
            GUILayout.Label($"Is Saving: {_isSaving}");
            GUILayout.Label($"Is Restoring: {_isRestoring}");
            GUILayout.Label($"Memory: {GC.GetTotalMemory(false) / 1024 / 1024}MB");
            GUILayout.EndArea();
        }
        #endregion
    }
} 