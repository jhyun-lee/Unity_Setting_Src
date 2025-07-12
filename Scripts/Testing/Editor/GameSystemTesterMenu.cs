using UnityEngine;
using UnityEditor;
using Game.Testing;

namespace Game.Testing.Editor
{
    /// <summary>
    /// 게임 시스템 테스터 에디터 메뉴
    /// </summary>
    public static class GameSystemTesterMenu
    {
        private const string MENU_BASE = "Game/Testing/";
        private const string MENU_TEST_ALL = MENU_BASE + "Test All Systems";
        private const string MENU_TEST_SETTINGS = MENU_BASE + "Test Settings System";
        private const string MENU_TEST_USER_DATA = MENU_BASE + "Test User Data System";
        private const string MENU_TEST_AUDIO = MENU_BASE + "Test Audio System";
        private const string MENU_TEST_AUTO_SAVE = MENU_BASE + "Test Auto Save System";
        private const string MENU_TEST_LIFECYCLE = MENU_BASE + "Test App Lifecycle System";
        private const string MENU_TEST_PERFORMANCE = MENU_BASE + "Test Performance Monitor System";
        private const string MENU_GENERATE_REPORT = MENU_BASE + "Generate Test Report";
        private const string MENU_CLEANUP_DATA = MENU_BASE + "Cleanup Test Data";

        #region Menu Items
        [MenuItem(MENU_TEST_ALL)]
        public static void TestAllSystems()
        {
            if (GameSystemTester.Instance != null)
            {
                GameSystemTester.Instance.TestAllSystems();
                Debug.Log("전체 시스템 테스트 시작");
            }
            else
            {
                Debug.LogError("GameSystemTester를 찾을 수 없습니다. 씬에 GameSystemTester를 추가해주세요.");
            }
        }

        [MenuItem(MENU_TEST_SETTINGS)]
        public static void TestSettingsSystem()
        {
            if (GameSystemTester.Instance != null)
            {
                GameSystemTester.Instance.TestSettingsSystem();
                Debug.Log("설정 시스템 테스트 시작");
            }
            else
            {
                Debug.LogError("GameSystemTester를 찾을 수 없습니다.");
            }
        }

        [MenuItem(MENU_TEST_USER_DATA)]
        public static void TestUserDataSystem()
        {
            if (GameSystemTester.Instance != null)
            {
                GameSystemTester.Instance.TestUserDataSystem();
                Debug.Log("사용자 데이터 시스템 테스트 시작");
            }
            else
            {
                Debug.LogError("GameSystemTester를 찾을 수 없습니다.");
            }
        }

        [MenuItem(MENU_TEST_AUDIO)]
        public static void TestAudioSystem()
        {
            if (GameSystemTester.Instance != null)
            {
                GameSystemTester.Instance.TestAudioSystem();
                Debug.Log("오디오 시스템 테스트 시작");
            }
            else
            {
                Debug.LogError("GameSystemTester를 찾을 수 없습니다.");
            }
        }

        [MenuItem(MENU_TEST_AUTO_SAVE)]
        public static void TestAutoSaveSystem()
        {
            if (GameSystemTester.Instance != null)
            {
                GameSystemTester.Instance.TestAutoSaveSystem();
                Debug.Log("자동 저장 시스템 테스트 시작");
            }
            else
            {
                Debug.LogError("GameSystemTester를 찾을 수 없습니다.");
            }
        }

        [MenuItem(MENU_TEST_LIFECYCLE)]
        public static void TestAppLifecycleSystem()
        {
            if (GameSystemTester.Instance != null)
            {
                GameSystemTester.Instance.TestAppLifecycleSystem();
                Debug.Log("앱 생명주기 시스템 테스트 시작");
            }
            else
            {
                Debug.LogError("GameSystemTester를 찾을 수 없습니다.");
            }
        }

        [MenuItem(MENU_TEST_PERFORMANCE)]
        public static void TestPerformanceMonitorSystem()
        {
            if (GameSystemTester.Instance != null)
            {
                GameSystemTester.Instance.TestPerformanceMonitorSystem();
                Debug.Log("성능 모니터링 시스템 테스트 시작");
            }
            else
            {
                Debug.LogError("GameSystemTester를 찾을 수 없습니다.");
            }
        }

        [MenuItem(MENU_GENERATE_REPORT)]
        public static void GenerateTestReport()
        {
            if (GameSystemTester.Instance != null)
            {
                GameSystemTester.Instance.GenerateTestReport();
                Debug.Log("테스트 보고서 생성");
            }
            else
            {
                Debug.LogError("GameSystemTester를 찾을 수 없습니다.");
            }
        }

        [MenuItem(MENU_CLEANUP_DATA)]
        public static void CleanupTestData()
        {
            if (GameSystemTester.Instance != null)
            {
                GameSystemTester.Instance.CleanupTestData();
                Debug.Log("테스트 데이터 정리 완료");
            }
            else
            {
                Debug.LogError("GameSystemTester를 찾을 수 없습니다.");
            }
        }
        #endregion

        #region Menu Validation
        [MenuItem(MENU_TEST_ALL, true)]
        [MenuItem(MENU_TEST_SETTINGS, true)]
        [MenuItem(MENU_TEST_USER_DATA, true)]
        [MenuItem(MENU_TEST_AUDIO, true)]
        [MenuItem(MENU_TEST_AUTO_SAVE, true)]
        [MenuItem(MENU_TEST_LIFECYCLE, true)]
        [MenuItem(MENU_TEST_PERFORMANCE, true)]
        [MenuItem(MENU_GENERATE_REPORT, true)]
        [MenuItem(MENU_CLEANUP_DATA, true)]
        public static bool ValidateTestMenu()
        {
            return Application.isPlaying && GameSystemTester.Instance != null;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// GameSystemTester 오브젝트 생성
        /// </summary>
        [MenuItem("Game/Testing/Create GameSystemTester")]
        public static void CreateGameSystemTester()
        {
            if (GameSystemTester.Instance != null)
            {
                Debug.LogWarning("GameSystemTester가 이미 존재합니다.");
                Selection.activeGameObject = GameSystemTester.Instance.gameObject;
                return;
            }

            GameObject testerObject = new GameObject("GameSystemTester");
            testerObject.AddComponent<GameSystemTester>();
            
            Selection.activeGameObject = testerObject;
            Debug.Log("GameSystemTester 오브젝트가 생성되었습니다.");
        }

        /// <summary>
        /// 모든 매니저 오브젝트 생성
        /// </summary>
        [MenuItem("Game/Testing/Create All Managers")]
        public static void CreateAllManagers()
        {
            CreateManagerIfNotExists<SettingsManager>("SettingsManager");
            CreateManagerIfNotExists<UserDataManager>("UserDataManager");
            CreateManagerIfNotExists<AudioManager>("AudioManager");
            CreateManagerIfNotExists<AutoSaveManager>("AutoSaveManager");
            CreateManagerIfNotExists<AppLifecycleManager>("AppLifecycleManager");
            CreateManagerIfNotExists<PerformanceMonitor>("PerformanceMonitor");
            CreateManagerIfNotExists<GameSystemTester>("GameSystemTester");
            
            Debug.Log("모든 매니저 오브젝트가 생성되었습니다.");
        }

        private static void CreateManagerIfNotExists<T>(string name) where T : MonoBehaviour
        {
            if (Object.FindObjectOfType<T>() == null)
            {
                GameObject managerObject = new GameObject(name);
                managerObject.AddComponent<T>();
                Debug.Log($"{name} 오브젝트가 생성되었습니다.");
            }
            else
            {
                Debug.Log($"{name}가 이미 존재합니다.");
            }
        }

        /// <summary>
        /// 테스트 환경 설정
        /// </summary>
        [MenuItem("Game/Testing/Setup Test Environment")]
        public static void SetupTestEnvironment()
        {
            // 모든 매니저 생성
            CreateAllManagers();
            
            // 테스트 데이터 초기화
            if (GameSystemTester.Instance != null)
            {
                Debug.Log("테스트 환경 설정이 완료되었습니다.");
            }
        }
        #endregion
    }
} 