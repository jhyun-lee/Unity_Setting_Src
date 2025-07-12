using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using Game.UI;

namespace Game.Testing
{
    /// <summary>
    /// 테스트 결과 구조
    /// </summary>
    [System.Serializable]
    public class TestResult
    {
        public string testName;
        public bool isSuccess;
        public string message;
        public float executionTime;
        public System.DateTime timestamp;

        public TestResult(string name, bool success, string msg, float time)
        {
            testName = name;
            isSuccess = success;
            message = msg;
            executionTime = time;
            timestamp = System.DateTime.Now;
        }
    }

    /// <summary>
    /// 통합 테스트 보고서
    /// </summary>
    [System.Serializable]
    public class TestReport
    {
        public List<TestResult> results = new List<TestResult>();
        public int totalTests;
        public int passedTests;
        public int failedTests;
        public float totalExecutionTime;
        public System.DateTime testStartTime;
        public System.DateTime testEndTime;

        public float successRate => totalTests > 0 ? (float)passedTests / totalTests * 100f : 0f;
    }

    /// <summary>
    /// 게임 시스템 통합 테스터
    /// </summary>
    public class GameSystemTester : MonoBehaviour
    {
        #region Singleton
        private static GameSystemTester _instance;
        public static GameSystemTester Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameSystemTester>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameSystemTester");
                        _instance = go.AddComponent<GameSystemTester>();
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
                InitializeTester();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Properties
        [Header("Test Settings")]
        [SerializeField] private bool _enableAutoTest = false;
        [SerializeField] private bool _enableDetailedLogs = true;
        [SerializeField] private float _testDelay = 0.5f;
        
        [Header("Test Data")]
        [SerializeField] private string _testSettingsKey = "TestSettings";
        [SerializeField] private string _testUserDataKey = "TestUserData";
        
        [Header("Test Results")]
        [SerializeField] private TestReport _currentReport;
        [SerializeField] private bool _isTestRunning = false;
        
        // 테스트 데이터
        private Dictionary<string, object> _testSettings;
        private Dictionary<string, object> _testUserData;
        
        // 이벤트
        public static event Action<TestReport> OnTestCompleted;
        public static event Action<TestResult> OnTestResult;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            if (_enableAutoTest)
            {
                StartCoroutine(AutoTestCoroutine());
            }
        }
        #endregion

        #region Initialization
        private void InitializeTester()
        {
            _currentReport = new TestReport();
            _isTestRunning = false;
            
            // 테스트 데이터 초기화
            InitializeTestData();
            
            if (_enableDetailedLogs)
                GameLogger.Log("GameSystemTester 초기화 완료", LogType.Info);
        }

        private void InitializeTestData()
        {
            // 테스트 설정 데이터
            _testSettings = new Dictionary<string, object>
            {
                { "masterVolume", 0.8f },
                { "bgmVolume", 0.7f },
                { "sfxVolume", 0.9f },
                { "qualityLevel", 2 },
                { "targetFrameRate", 60 },
                { "autoSave", true },
                { "performanceMonitoring", true }
            };

            // 테스트 사용자 데이터
            _testUserData = new Dictionary<string, object>
            {
                { "playerName", "TestPlayer" },
                { "level", 5 },
                { "experience", 1250 },
                { "coins", 500 },
                { "highScore", 8500 },
                { "playTime", 3600f },
                { "achievements", new List<string> { "FirstWin", "CoinCollector" } },
                { "settings", _testSettings }
            };
        }
        #endregion

        #region Public Test Methods
        /// <summary>
        /// 모든 시스템 테스트
        /// </summary>
        [ContextMenu("Test All Systems")]
        public void TestAllSystems()
        {
            if (_isTestRunning)
            {
                GameLogger.LogWarning("테스트가 이미 실행 중입니다");
                return;
            }

            StartCoroutine(TestAllSystemsCoroutine());
        }

        /// <summary>
        /// 설정 시스템 테스트
        /// </summary>
        [ContextMenu("Test Settings System")]
        public void TestSettingsSystem()
        {
            StartCoroutine(TestSettingsSystemCoroutine());
        }

        /// <summary>
        /// 사용자 데이터 시스템 테스트
        /// </summary>
        [ContextMenu("Test User Data System")]
        public void TestUserDataSystem()
        {
            StartCoroutine(TestUserDataSystemCoroutine());
        }

        /// <summary>
        /// 오디오 시스템 테스트
        /// </summary>
        [ContextMenu("Test Audio System")]
        public void TestAudioSystem()
        {
            StartCoroutine(TestAudioSystemCoroutine());
        }

        /// <summary>
        /// 자동 저장 시스템 테스트
        /// </summary>
        [ContextMenu("Test Auto Save System")]
        public void TestAutoSaveSystem()
        {
            StartCoroutine(TestAutoSaveSystemCoroutine());
        }

        /// <summary>
        /// 앱 생명주기 시스템 테스트
        /// </summary>
        [ContextMenu("Test App Lifecycle System")]
        public void TestAppLifecycleSystem()
        {
            StartCoroutine(TestAppLifecycleSystemCoroutine());
        }

        /// <summary>
        /// 성능 모니터링 시스템 테스트
        /// </summary>
        [ContextMenu("Test Performance Monitor System")]
        public void TestPerformanceMonitorSystem()
        {
            StartCoroutine(TestPerformanceMonitorSystemCoroutine());
        }

        /// <summary>
        /// 테스트 결과 보고서 생성
        /// </summary>
        [ContextMenu("Generate Test Report")]
        public void GenerateTestReport()
        {
            if (_currentReport == null || _currentReport.results.Count == 0)
            {
                GameLogger.LogWarning("테스트 결과가 없습니다");
                return;
            }

            PrintTestReport();
        }
        #endregion

        #region Test Coroutines
        private IEnumerator TestAllSystemsCoroutine()
        {
            _isTestRunning = true;
            _currentReport = new TestReport();
            _currentReport.testStartTime = System.DateTime.Now;

            GameLogger.LogInfo("=== 전체 시스템 테스트 시작 ===");

            // 1. 설정 시스템 테스트
            yield return StartCoroutine(TestSettingsSystemCoroutine());

            // 2. 사용자 데이터 시스템 테스트
            yield return StartCoroutine(TestUserDataSystemCoroutine());

            // 3. 오디오 시스템 테스트
            yield return StartCoroutine(TestAudioSystemCoroutine());

            // 4. 자동 저장 시스템 테스트
            yield return StartCoroutine(TestAutoSaveSystemCoroutine());

            // 5. 앱 생명주기 시스템 테스트
            yield return StartCoroutine(TestAppLifecycleSystemCoroutine());

            // 6. 성능 모니터링 시스템 테스트
            yield return StartCoroutine(TestPerformanceMonitorSystemCoroutine());

            // 테스트 완료
            _currentReport.testEndTime = System.DateTime.Now;
            _currentReport.totalTests = _currentReport.results.Count;
            _currentReport.passedTests = _currentReport.results.FindAll(r => r.isSuccess).Count;
            _currentReport.failedTests = _currentReport.totalTests - _currentReport.passedTests;

            foreach (var result in _currentReport.results)
            {
                _currentReport.totalExecutionTime += result.executionTime;
            }

            GameLogger.LogInfo("=== 전체 시스템 테스트 완료 ===");
            PrintTestReport();

            _isTestRunning = false;
            OnTestCompleted?.Invoke(_currentReport);
        }

        private IEnumerator TestSettingsSystemCoroutine()
        {
            float startTime = Time.realtimeSinceStartup;
            bool success = true;
            string message = "";

            try
            {
                GameLogger.LogInfo("설정 시스템 테스트 시작");

                // SettingsManager 존재 확인
                if (SettingsManager.Instance == null)
                {
                    success = false;
                    message = "SettingsManager가 없습니다";
                    yield break;
                }

                // 테스트 설정 저장
                foreach (var setting in _testSettings)
                {
                    SettingsManager.Instance.SetSetting(setting.Key, setting.Value);
                }

                yield return new WaitForSeconds(_testDelay);

                // 설정 저장
                SettingsManager.Instance.SaveSettings();

                yield return new WaitForSeconds(_testDelay);

                // 설정 로드
                SettingsManager.Instance.LoadSettings();

                yield return new WaitForSeconds(_testDelay);

                // 설정 값 검증
                foreach (var setting in _testSettings)
                {
                    var loadedValue = SettingsManager.Instance.GetSetting(setting.Key);
                    if (!AreValuesEqual(loadedValue, setting.Value))
                    {
                        success = false;
                        message = $"설정 값 불일치: {setting.Key}";
                        break;
                    }
                }

                if (success)
                {
                    message = "설정 시스템 테스트 성공";
                }
            }
            catch (System.Exception e)
            {
                success = false;
                message = $"설정 시스템 테스트 실패: {e.Message}";
            }

            float executionTime = Time.realtimeSinceStartup - startTime;
            var result = new TestResult("Settings System", success, message, executionTime);
            _currentReport.results.Add(result);
            OnTestResult?.Invoke(result);

            GameLogger.LogInfo($"설정 시스템 테스트 완료: {(success ? "성공" : "실패")} - {message}");
        }

        private IEnumerator TestUserDataSystemCoroutine()
        {
            float startTime = Time.realtimeSinceStartup;
            bool success = true;
            string message = "";

            try
            {
                GameLogger.LogInfo("사용자 데이터 시스템 테스트 시작");

                // UserDataManager 존재 확인
                if (UserDataManager.Instance == null)
                {
                    success = false;
                    message = "UserDataManager가 없습니다";
                    yield break;
                }

                // 테스트 데이터 설정
                foreach (var data in _testUserData)
                {
                    UserDataManager.Instance.SetUserData(data.Key, data.Value);
                }

                yield return new WaitForSeconds(_testDelay);

                // 사용자 데이터 저장
                UserDataManager.Instance.SaveUserData();

                yield return new WaitForSeconds(_testDelay);

                // 사용자 데이터 로드
                UserDataManager.Instance.LoadUserData();

                yield return new WaitForSeconds(_testDelay);

                // 데이터 값 검증
                foreach (var data in _testUserData)
                {
                    var loadedValue = UserDataManager.Instance.GetUserData(data.Key);
                    if (!AreValuesEqual(loadedValue, data.Value))
                    {
                        success = false;
                        message = $"사용자 데이터 값 불일치: {data.Key}";
                        break;
                    }
                }

                if (success)
                {
                    message = "사용자 데이터 시스템 테스트 성공";
                }
            }
            catch (System.Exception e)
            {
                success = false;
                message = $"사용자 데이터 시스템 테스트 실패: {e.Message}";
            }

            float executionTime = Time.realtimeSinceStartup - startTime;
            var result = new TestResult("User Data System", success, message, executionTime);
            _currentReport.results.Add(result);
            OnTestResult?.Invoke(result);

            GameLogger.LogInfo($"사용자 데이터 시스템 테스트 완료: {(success ? "성공" : "실패")} - {message}");
        }

        private IEnumerator TestAudioSystemCoroutine()
        {
            float startTime = Time.realtimeSinceStartup;
            bool success = true;
            string message = "";

            try
            {
                GameLogger.LogInfo("오디오 시스템 테스트 시작");

                // AudioManager 존재 확인
                if (AudioManager.Instance == null)
                {
                    success = false;
                    message = "AudioManager가 없습니다";
                    yield break;
                }

                // 마스터 볼륨 테스트
                AudioManager.Instance.SetMasterVolume(0.5f);
                yield return new WaitForSeconds(0.1f);

                if (Mathf.Abs(AudioManager.Instance.GetMasterVolume() - 0.5f) > 0.01f)
                {
                    success = false;
                    message = "마스터 볼륨 설정 실패";
                    yield break;
                }

                // BGM 볼륨 테스트
                AudioManager.Instance.SetBGMVolume(0.7f);
                yield return new WaitForSeconds(0.1f);

                if (Mathf.Abs(AudioManager.Instance.GetBGMVolume() - 0.7f) > 0.01f)
                {
                    success = false;
                    message = "BGM 볼륨 설정 실패";
                    yield break;
                }

                // SFX 볼륨 테스트
                AudioManager.Instance.SetSFXVolume(0.8f);
                yield return new WaitForSeconds(0.1f);

                if (Mathf.Abs(AudioManager.Instance.GetSFXVolume() - 0.8f) > 0.01f)
                {
                    success = false;
                    message = "SFX 볼륨 설정 실패";
                    yield break;
                }

                // 음소거 테스트
                AudioManager.Instance.MuteAll();
                yield return new WaitForSeconds(0.1f);

                if (!AudioManager.Instance.IsMuted())
                {
                    success = false;
                    message = "음소거 기능 실패";
                    yield break;
                }

                // 음소거 해제 테스트
                AudioManager.Instance.UnmuteAll();
                yield return new WaitForSeconds(0.1f);

                if (AudioManager.Instance.IsMuted())
                {
                    success = false;
                    message = "음소거 해제 기능 실패";
                    yield break;
                }

                if (success)
                {
                    message = "오디오 시스템 테스트 성공";
                }
            }
            catch (System.Exception e)
            {
                success = false;
                message = $"오디오 시스템 테스트 실패: {e.Message}";
            }

            float executionTime = Time.realtimeSinceStartup - startTime;
            var result = new TestResult("Audio System", success, message, executionTime);
            _currentReport.results.Add(result);
            OnTestResult?.Invoke(result);

            GameLogger.LogInfo($"오디오 시스템 테스트 완료: {(success ? "성공" : "실패")} - {message}");
        }

        private IEnumerator TestAutoSaveSystemCoroutine()
        {
            float startTime = Time.realtimeSinceStartup;
            bool success = true;
            string message = "";

            try
            {
                GameLogger.LogInfo("자동 저장 시스템 테스트 시작");

                // AutoSaveManager 존재 확인
                if (AutoSaveManager.Instance == null)
                {
                    success = false;
                    message = "AutoSaveManager가 없습니다";
                    yield break;
                }

                // 자동 저장 활성화
                AutoSaveManager.Instance.EnableAutoSave();
                yield return new WaitForSeconds(0.1f);

                if (!AutoSaveManager.Instance.IsAutoSaveEnabled())
                {
                    success = false;
                    message = "자동 저장 활성화 실패";
                    yield break;
                }

                // 수동 저장 테스트
                AutoSaveManager.Instance.ForceSave();
                yield return new WaitForSeconds(0.1f);

                // 자동 저장 비활성화
                AutoSaveManager.Instance.DisableAutoSave();
                yield return new WaitForSeconds(0.1f);

                if (AutoSaveManager.Instance.IsAutoSaveEnabled())
                {
                    success = false;
                    message = "자동 저장 비활성화 실패";
                    yield break;
                }

                if (success)
                {
                    message = "자동 저장 시스템 테스트 성공";
                }
            }
            catch (System.Exception e)
            {
                success = false;
                message = $"자동 저장 시스템 테스트 실패: {e.Message}";
            }

            float executionTime = Time.realtimeSinceStartup - startTime;
            var result = new TestResult("Auto Save System", success, message, executionTime);
            _currentReport.results.Add(result);
            OnTestResult?.Invoke(result);

            GameLogger.LogInfo($"자동 저장 시스템 테스트 완료: {(success ? "성공" : "실패")} - {message}");
        }

        private IEnumerator TestAppLifecycleSystemCoroutine()
        {
            float startTime = Time.realtimeSinceStartup;
            bool success = true;
            string message = "";

            try
            {
                GameLogger.LogInfo("앱 생명주기 시스템 테스트 시작");

                // AppLifecycleManager 존재 확인
                if (AppLifecycleManager.Instance == null)
                {
                    success = false;
                    message = "AppLifecycleManager가 없습니다";
                    yield break;
                }

                // 게임 상태 저장 테스트
                AppLifecycleManager.Instance.SaveGameState();
                yield return new WaitForSeconds(0.1f);

                // 게임 상태 복원 테스트
                AppLifecycleManager.Instance.RestoreGameState();
                yield return new WaitForSeconds(0.1f);

                // 빠른 저장 테스트
                AppLifecycleManager.Instance.QuickSave();
                yield return new WaitForSeconds(0.1f);

                // 상태 확인
                if (AppLifecycleManager.Instance.IsQuitting)
                {
                    success = false;
                    message = "앱 종료 상태 확인 실패";
                    yield break;
                }

                if (success)
                {
                    message = "앱 생명주기 시스템 테스트 성공";
                }
            }
            catch (System.Exception e)
            {
                success = false;
                message = $"앱 생명주기 시스템 테스트 실패: {e.Message}";
            }

            float executionTime = Time.realtimeSinceStartup - startTime;
            var result = new TestResult("App Lifecycle System", success, message, executionTime);
            _currentReport.results.Add(result);
            OnTestResult?.Invoke(result);

            GameLogger.LogInfo($"앱 생명주기 시스템 테스트 완료: {(success ? "성공" : "실패")} - {message}");
        }

        private IEnumerator TestPerformanceMonitorSystemCoroutine()
        {
            float startTime = Time.realtimeSinceStartup;
            bool success = true;
            string message = "";

            try
            {
                GameLogger.LogInfo("성능 모니터링 시스템 테스트 시작");

                // PerformanceMonitor 존재 확인
                if (PerformanceMonitor.Instance == null)
                {
                    success = false;
                    message = "PerformanceMonitor가 없습니다";
                    yield break;
                }

                // 모니터링 시작
                PerformanceMonitor.Instance.StartMonitoring();
                yield return new WaitForSeconds(0.1f);

                // 성능 체크
                var performanceData = PerformanceMonitor.Instance.CheckPerformance();
                yield return new WaitForSeconds(0.1f);

                if (performanceData == null)
                {
                    success = false;
                    message = "성능 데이터 수집 실패";
                    yield break;
                }

                // FPS 확인
                float fps = PerformanceMonitor.Instance.GetCurrentFPS();
                if (fps < 0)
                {
                    success = false;
                    message = "FPS 측정 실패";
                    yield break;
                }

                // 메모리 사용량 확인
                float memoryUsage = PerformanceMonitor.Instance.GetMemoryUsage();
                if (memoryUsage < 0)
                {
                    success = false;
                    message = "메모리 사용량 측정 실패";
                    yield break;
                }

                // 배터리 레벨 확인
                float batteryLevel = PerformanceMonitor.Instance.GetBatteryLevel();
                if (batteryLevel < 0 || batteryLevel > 1)
                {
                    success = false;
                    message = "배터리 레벨 측정 실패";
                    yield break;
                }

                // 자동 최적화 테스트
                PerformanceMonitor.Instance.AutoOptimize();
                yield return new WaitForSeconds(0.1f);

                // 최적화 초기화
                PerformanceMonitor.Instance.ResetOptimization();
                yield return new WaitForSeconds(0.1f);

                // 모니터링 중지
                PerformanceMonitor.Instance.StopMonitoring();
                yield return new WaitForSeconds(0.1f);

                if (success)
                {
                    message = "성능 모니터링 시스템 테스트 성공";
                }
            }
            catch (System.Exception e)
            {
                success = false;
                message = $"성능 모니터링 시스템 테스트 실패: {e.Message}";
            }

            float executionTime = Time.realtimeSinceStartup - startTime;
            var result = new TestResult("Performance Monitor System", success, message, executionTime);
            _currentReport.results.Add(result);
            OnTestResult?.Invoke(result);

            GameLogger.LogInfo($"성능 모니터링 시스템 테스트 완료: {(success ? "성공" : "실패")} - {message}");
        }

        private IEnumerator AutoTestCoroutine()
        {
            yield return new WaitForSeconds(2f); // 초기화 대기
            TestAllSystems();
        }
        #endregion

        #region Utility Methods
        private bool AreValuesEqual(object value1, object value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null) return false;

            if (value1.GetType() != value2.GetType()) return false;

            if (value1 is float float1 && value2 is float float2)
            {
                return Mathf.Abs(float1 - float2) < 0.001f;
            }

            if (value1 is int int1 && value2 is int int2)
            {
                return int1 == int2;
            }

            if (value1 is string str1 && value2 is string str2)
            {
                return str1 == str2;
            }

            if (value1 is bool bool1 && value2 is bool bool2)
            {
                return bool1 == bool2;
            }

            return value1.Equals(value2);
        }

        private void PrintTestReport()
        {
            if (_currentReport == null || _currentReport.results.Count == 0)
            {
                GameLogger.LogWarning("테스트 결과가 없습니다");
                return;
            }

            GameLogger.LogInfo("=== 테스트 결과 보고서 ===");
            GameLogger.LogInfo($"총 테스트 수: {_currentReport.totalTests}");
            GameLogger.LogInfo($"성공: {_currentReport.passedTests}");
            GameLogger.LogInfo($"실패: {_currentReport.failedTests}");
            GameLogger.LogInfo($"성공률: {_currentReport.successRate:F1}%");
            GameLogger.LogInfo($"총 실행 시간: {_currentReport.totalExecutionTime:F2}초");
            GameLogger.LogInfo($"테스트 시작: {_currentReport.testStartTime}");
            GameLogger.LogInfo($"테스트 완료: {_currentReport.testEndTime}");

            GameLogger.LogInfo("--- 상세 결과 ---");
            foreach (var result in _currentReport.results)
            {
                string status = result.isSuccess ? "성공" : "실패";
                GameLogger.LogInfo($"{result.testName}: {status} ({result.executionTime:F2}초) - {result.message}");
            }

            GameLogger.LogInfo("==================");
        }

        /// <summary>
        /// 테스트 데이터 정리
        /// </summary>
        [ContextMenu("Cleanup Test Data")]
        public void CleanupTestData()
        {
            try
            {
                // 테스트 설정 삭제
                if (SettingsManager.Instance != null)
                {
                    foreach (var setting in _testSettings.Keys)
                    {
                        SettingsManager.Instance.RemoveSetting(setting);
                    }
                }

                // 테스트 사용자 데이터 삭제
                if (UserDataManager.Instance != null)
                {
                    foreach (var data in _testUserData.Keys)
                    {
                        UserDataManager.Instance.RemoveUserData(data);
                    }
                }

                GameLogger.LogInfo("테스트 데이터 정리 완료");
            }
            catch (System.Exception e)
            {
                GameLogger.LogError($"테스트 데이터 정리 실패: {e.Message}");
            }
        }
        #endregion

        #region Debug Methods
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 500, 300, 200));
            GUILayout.Label("Game System Tester Debug:");
            
            GUILayout.Label($"테스트 실행 중: {_isTestRunning}");
            
            if (_currentReport != null)
            {
                GUILayout.Label($"총 테스트: {_currentReport.totalTests}");
                GUILayout.Label($"성공: {_currentReport.passedTests}");
                GUILayout.Label($"실패: {_currentReport.failedTests}");
                GUILayout.Label($"성공률: {_currentReport.successRate:F1}%");
            }
            
            if (GUILayout.Button("Test All Systems"))
                TestAllSystems();
            
            if (GUILayout.Button("Generate Report"))
                GenerateTestReport();
            
            if (GUILayout.Button("Cleanup Data"))
                CleanupTestData();
            
            GUILayout.EndArea();
        }
        #endregion
    }
} 