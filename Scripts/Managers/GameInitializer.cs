using UnityEngine;
using System.Collections;

/// <summary>
/// 게임 초기화 관리자
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("Initialization Settings")]
    [SerializeField] private bool autoOptimizeOnStart = true;
    [SerializeField] private bool showDeviceInfo = true;
    [SerializeField] private bool showRecommendedSettings = true;
    
    [Header("Optimization Settings")]
    [SerializeField] private bool applyDeviceOptimization = true;
    [SerializeField] private bool allowManualOverride = true;
    
    // 초기화 상태
    private bool isInitialized = false;
    
    // 이벤트
    public static System.Action OnGameInitialized;
    public static System.Action OnDeviceOptimized;
    
    private void Awake()
    {
        // 싱글톤 패턴
        if (FindObjectsOfType<GameInitializer>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitializeGame());
    }
    
    /// <summary>
    /// 게임 초기화 코루틴
    /// </summary>
    private IEnumerator InitializeGame()
    {
        GameLogger.LogInfo("게임 초기화 시작");
        
        // 1. 디바이스 정보 수집
        yield return StartCoroutine(InitializeDeviceInfo());
        
        // 2. 디바이스 최적화
        if (applyDeviceOptimization)
        {
            yield return StartCoroutine(OptimizeDevice());
        }
        
        // 3. 매니저 초기화
        yield return StartCoroutine(InitializeManagers());
        
        // 4. 초기화 완료
        isInitialized = true;
        OnGameInitialized?.Invoke();
        
        GameLogger.LogInfo("게임 초기화 완료");
    }
    
    /// <summary>
    /// 디바이스 정보 초기화
    /// </summary>
    private IEnumerator InitializeDeviceInfo()
    {
        GameLogger.LogInfo("디바이스 정보 수집 시작");
        
        // 디바이스 정보 수집
        DeviceInfo.Initialize();
        
        // 디바이스 정보 출력
        if (showDeviceInfo)
        {
            DeviceInfo.PrintDeviceInfo();
        }
        
        // 권장 설정 출력
        if (showRecommendedSettings)
        {
            DeviceInfo.PrintRecommendedSettings();
        }
        
        // 성능 점수 출력
        int performanceScore = DeviceInfo.GetPerformanceScore();
        GameLogger.LogInfo($"디바이스 성능 점수: {performanceScore}/100");
        
        // 배터리 및 네트워크 정보
        string batteryInfo = DeviceInfo.GetBatteryInfo();
        string networkInfo = DeviceInfo.GetNetworkInfo();
        GameLogger.LogInfo(batteryInfo);
        GameLogger.LogInfo(networkInfo);
        
        yield return new WaitForEndOfFrame();
    }
    
    /// <summary>
    /// 디바이스 최적화
    /// </summary>
    private IEnumerator OptimizeDevice()
    {
        GameLogger.LogInfo("디바이스 최적화 시작");
        
        // 자동 최적화 적용
        if (autoOptimizeOnStart)
        {
            DeviceInfo.OptimizeForDevice();
            OnDeviceOptimized?.Invoke();
        }
        
        // 사용자 설정 확인
        if (allowManualOverride)
        {
            CheckUserSettings();
        }
        
        yield return new WaitForEndOfFrame();
    }
    
    /// <summary>
    /// 매니저 초기화
    /// </summary>
    private IEnumerator InitializeManagers()
    {
        GameLogger.LogInfo("매니저 초기화 시작");
        
        // UserDataManager 초기화 확인
        if (UserDataManager.Instance == null)
        {
            GameLogger.LogWarning("UserDataManager가 없습니다");
        }
        else
        {
            GameLogger.LogInfo("UserDataManager 초기화 확인");
        }
        
        // AutoSaveManager 초기화 확인
        if (AutoSaveManager.Instance == null)
        {
            GameLogger.LogWarning("AutoSaveManager가 없습니다");
        }
        else
        {
            GameLogger.LogInfo("AutoSaveManager 초기화 확인");
        }
        
        // AudioManager 초기화 확인
        if (AudioManager.Instance == null)
        {
            GameLogger.LogWarning("AudioManager가 없습니다");
        }
        else
        {
            GameLogger.LogInfo("AudioManager 초기화 확인");
        }
        
        // AppLifecycleManager 초기화 확인
        if (AppLifecycleManager.Instance == null)
        {
            GameLogger.LogWarning("AppLifecycleManager가 없습니다");
        }
        else
        {
            GameLogger.LogInfo("AppLifecycleManager 초기화 확인");
        }
        
        // PerformanceMonitor 초기화 확인
        if (PerformanceMonitor.Instance == null)
        {
            GameLogger.LogWarning("PerformanceMonitor가 없습니다");
        }
        else
        {
            GameLogger.LogInfo("PerformanceMonitor 초기화 확인");
        }
        
        yield return new WaitForEndOfFrame();
    }
    
    /// <summary>
    /// 사용자 설정 확인
    /// </summary>
    private void CheckUserSettings()
    {
        // PlayerPrefs에서 사용자 설정 확인
        bool userOptimizationEnabled = PlayerPrefs.GetInt("UserOptimizationEnabled", -1);
        
        if (userOptimizationEnabled == -1)
        {
            // 첫 실행 시 자동 최적화 적용
            GameLogger.LogInfo("첫 실행 - 자동 최적화 적용");
            PlayerPrefs.SetInt("UserOptimizationEnabled", 1);
            PlayerPrefs.Save();
        }
        else if (userOptimizationEnabled == 0)
        {
            // 사용자가 자동 최적화를 비활성화한 경우
            GameLogger.LogInfo("사용자 설정 - 자동 최적화 비활성화");
        }
        else
        {
            // 사용자가 자동 최적화를 활성화한 경우
            GameLogger.LogInfo("사용자 설정 - 자동 최적화 활성화");
        }
    }
    
    /// <summary>
    /// 초기화 상태 확인
    /// </summary>
    /// <returns>초기화 완료 여부</returns>
    public bool IsInitialized()
    {
        return isInitialized;
    }
    
    /// <summary>
    /// 수동 최적화 적용
    /// </summary>
    public void ApplyManualOptimization()
    {
        if (!isInitialized)
        {
            GameLogger.LogWarning("게임이 아직 초기화되지 않았습니다");
            return;
        }
        
        GameLogger.LogInfo("수동 최적화 적용");
        DeviceInfo.OptimizeForDevice();
        OnDeviceOptimized?.Invoke();
    }
    
    /// <summary>
    /// 사용자 설정 변경
    /// </summary>
    /// <param name="enabled">자동 최적화 활성화 여부</param>
    public void SetUserOptimization(bool enabled)
    {
        PlayerPrefs.SetInt("UserOptimizationEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
        
        GameLogger.LogInfo($"사용자 최적화 설정 변경: {(enabled ? "활성화" : "비활성화")}");
    }
    
    /// <summary>
    /// 디바이스 정보 새로고침
    /// </summary>
    public void RefreshDeviceInfo()
    {
        if (!isInitialized)
        {
            GameLogger.LogWarning("게임이 아직 초기화되지 않았습니다");
            return;
        }
        
        GameLogger.LogInfo("디바이스 정보 새로고침");
        DeviceInfo.PrintDeviceInfo();
        DeviceInfo.PrintRecommendedSettings();
    }
    
    /// <summary>
    /// 성능 테스트 실행
    /// </summary>
    public void RunPerformanceTest()
    {
        if (!isInitialized)
        {
            GameLogger.LogWarning("게임이 아직 초기화되지 않았습니다");
            return;
        }
        
        GameLogger.LogInfo("성능 테스트 시작");
        
        // FPS 측정
        StartCoroutine(MeasureFPS());
        
        // 메모리 사용량 측정
        MeasureMemoryUsage();
        
        // 배터리 상태 확인
        string batteryInfo = DeviceInfo.GetBatteryInfo();
        GameLogger.LogInfo(batteryInfo);
    }
    
    /// <summary>
    /// FPS 측정
    /// </summary>
    private IEnumerator MeasureFPS()
    {
        int frameCount = 0;
        float timeElapsed = 0f;
        float targetTime = 5f; // 5초간 측정
        
        while (timeElapsed < targetTime)
        {
            frameCount++;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        float fps = frameCount / timeElapsed;
        GameLogger.LogInfo($"FPS 측정 결과: {fps:F1}");
    }
    
    /// <summary>
    /// 메모리 사용량 측정
    /// </summary>
    private void MeasureMemoryUsage()
    {
        long totalMemory = SystemInfo.systemMemorySize;
        long usedMemory = System.GC.GetTotalMemory(false);
        
        GameLogger.LogInfo($"메모리 사용량: {usedMemory / 1024 / 1024}MB / {totalMemory}MB");
    }
    
    /// <summary>
    /// 디버그 정보 출력
    /// </summary>
    [ContextMenu("Print Debug Info")]
    public void PrintDebugInfo()
    {
        if (!isInitialized)
        {
            GameLogger.LogWarning("게임이 아직 초기화되지 않았습니다");
            return;
        }
        
        GameLogger.LogInfo("=== 디버그 정보 ===");
        
        // 디바이스 정보
        DeviceInfo.PrintDeviceInfo();
        
        // 성능 점수
        int score = DeviceInfo.GetPerformanceScore();
        GameLogger.LogInfo($"성능 점수: {score}/100");
        
        // 매니저 상태
        GameLogger.LogInfo($"UserDataManager: {(UserDataManager.Instance != null ? "활성" : "비활성")}");
        GameLogger.LogInfo($"AutoSaveManager: {(AutoSaveManager.Instance != null ? "활성" : "비활성")}");
        GameLogger.LogInfo($"AudioManager: {(AudioManager.Instance != null ? "활성" : "비활성")}");
        GameLogger.LogInfo($"AppLifecycleManager: {(AppLifecycleManager.Instance != null ? "활성" : "비활성")}");
        GameLogger.LogInfo($"PerformanceMonitor: {(PerformanceMonitor.Instance != null ? "활성" : "비활성")}");
        
        // 현재 설정
        GameLogger.LogInfo($"목표 프레임레이트: {Application.targetFrameRate}");
        GameLogger.LogInfo($"품질 레벨: {QualitySettings.GetQualityLevel()}");
        GameLogger.LogInfo($"그림자 품질: {QualitySettings.shadows}");
        GameLogger.LogInfo($"안티앨리어싱: {QualitySettings.antiAliasing}");
        
        GameLogger.LogInfo("==================");
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isInitialized)
        {
            GameLogger.LogInfo("앱 일시정지 - 성능 정보 저장");
            // 성능 정보를 로그에 저장
            GameLogger.SaveLogToFile();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && isInitialized)
        {
            GameLogger.LogInfo("앱 포커스 해제 - 성능 정보 저장");
            GameLogger.SaveLogToFile();
        }
    }
} 