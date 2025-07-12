using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 게임 이벤트 열거형
/// </summary>
public enum GameEvent
{
    ScoreChanged,
    CoinEarned,
    LevelComplete,
    SettingsChanged,
    AppPaused,
    DataChanged,
    AchievementUnlocked,
    ItemPurchased,
    GameStarted,
    GameEnded
}

/// <summary>
/// 자동 저장 관리자
/// </summary>
public class AutoSaveManager : MonoBehaviour
{
    [Header("Auto Save Settings")]
    [SerializeField] private bool enableAutoSave = true;
    [SerializeField] private float autoSaveInterval = 30f; // 30초
    [SerializeField] private float saveCooldown = 5f; // 5초 쿨다운
    [SerializeField] private int maxRetryAttempts = 3;
    [SerializeField] private float retryDelay = 2f;
    
    [Header("Save Conditions")]
    [SerializeField] private bool saveOnScoreChange = true;
    [SerializeField] private bool saveOnCoinEarned = true;
    [SerializeField] private bool saveOnLevelComplete = true;
    [SerializeField] private bool saveOnSettingsChange = true;
    [SerializeField] private bool saveOnAppPaused = true;
    [SerializeField] private bool saveOnDataChanged = true;
    
    [Header("UI Settings")]
    [SerializeField] private bool showSaveIndicator = true;
    [SerializeField] private float saveIndicatorDuration = 2f;
    
    // 싱글톤 인스턴스
    public static AutoSaveManager Instance { get; private set; }
    
    // 자동 저장 상태
    private bool isAutoSaveRunning = false;
    private bool isSaving = false;
    private float lastSaveTime = 0f;
    private int currentRetryAttempts = 0;
    
    // 이벤트
    public event Action OnAutoSaveStarted;
    public event Action OnAutoSaveCompleted;
    public event Action OnAutoSaveFailed;
    public event Action<bool> OnSaveStatusChanged;
    
    // 코루틴 참조
    private Coroutine autoSaveCoroutine;
    private Coroutine retryCoroutine;
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAutoSave();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeAutoSave()
    {
        GameLogger.LogInfo("AutoSaveManager 초기화");
        
        // PlayerPrefs에서 자동 저장 설정 로드
        enableAutoSave = PlayerPrefs.GetInt("AutoSaveEnabled", 1) == 1;
        
        if (enableAutoSave)
        {
            StartAutoSave();
        }
    }
    
    /// <summary>
    /// 자동 저장 시작
    /// </summary>
    public void StartAutoSave()
    {
        if (isAutoSaveRunning)
        {
            GameLogger.LogWarning("자동 저장이 이미 실행 중입니다");
            return;
        }
        
        if (!enableAutoSave)
        {
            GameLogger.LogWarning("자동 저장이 비활성화되어 있습니다");
            return;
        }
        
        isAutoSaveRunning = true;
        autoSaveCoroutine = StartCoroutine(AutoSaveCoroutine());
        
        GameLogger.LogInfo("자동 저장 시작");
        OnAutoSaveStarted?.Invoke();
    }
    
    /// <summary>
    /// 자동 저장 중지
    /// </summary>
    public void StopAutoSave()
    {
        if (!isAutoSaveRunning)
        {
            GameLogger.LogWarning("자동 저장이 실행되지 않고 있습니다");
            return;
        }
        
        isAutoSaveRunning = false;
        
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
            autoSaveCoroutine = null;
        }
        
        if (retryCoroutine != null)
        {
            StopCoroutine(retryCoroutine);
            retryCoroutine = null;
        }
        
        GameLogger.LogInfo("자동 저장 중지");
    }
    
    /// <summary>
    /// 즉시 저장
    /// </summary>
    public void SaveNow()
    {
        if (isSaving)
        {
            GameLogger.LogWarning("저장이 이미 진행 중입니다");
            return;
        }
        
        if (Time.time - lastSaveTime < saveCooldown)
        {
            GameLogger.LogWarning("저장 쿨다운 중입니다");
            return;
        }
        
        StartCoroutine(SaveCoroutine());
    }
    
    /// <summary>
    /// 게임 이벤트 처리
    /// </summary>
    /// <param name="gameEvent">게임 이벤트</param>
    public void OnGameEvent(GameEvent gameEvent)
    {
        if (!enableAutoSave || isSaving)
            return;
        
        bool shouldSave = CheckSaveConditions(gameEvent);
        
        if (shouldSave)
        {
            GameLogger.LogInfo($"게임 이벤트로 인한 자동 저장: {gameEvent}");
            SaveNow();
        }
    }
    
    /// <summary>
    /// 저장 조건 확인
    /// </summary>
    /// <param name="gameEvent">게임 이벤트</param>
    /// <returns>저장 여부</returns>
    public bool CheckSaveConditions(GameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case GameEvent.ScoreChanged:
                return saveOnScoreChange;
                
            case GameEvent.CoinEarned:
                return saveOnCoinEarned;
                
            case GameEvent.LevelComplete:
                return saveOnLevelComplete;
                
            case GameEvent.SettingsChanged:
                return saveOnSettingsChange;
                
            case GameEvent.AppPaused:
                return saveOnAppPaused;
                
            case GameEvent.DataChanged:
                return saveOnDataChanged;
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// 자동 저장 코루틴
    /// </summary>
    private IEnumerator AutoSaveCoroutine()
    {
        while (isAutoSaveRunning)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            
            if (isAutoSaveRunning && !isSaving)
            {
                GameLogger.LogInfo("정기 자동 저장 실행");
                yield return StartCoroutine(SaveCoroutine());
            }
        }
    }
    
    /// <summary>
    /// 저장 코루틴
    /// </summary>
    private IEnumerator SaveCoroutine()
    {
        if (isSaving)
        {
            yield break;
        }
        
        isSaving = true;
        currentRetryAttempts = 0;
        
        // 저장 상태 UI 표시
        OnSaveStatusChanged?.Invoke(true);
        
        if (showSaveIndicator)
        {
            ShowSaveIndicator(true);
        }
        
        bool saveSuccess = false;
        
        while (currentRetryAttempts < maxRetryAttempts && !saveSuccess)
        {
            try
            {
                if (UserDataManager.Instance != null)
                {
                    saveSuccess = UserDataManager.Instance.SaveData();
                }
                else
                {
                    GameLogger.LogError("UserDataManager가 없습니다");
                    break;
                }
            }
            catch (Exception ex)
            {
                GameLogger.LogException(ex, "자동 저장 중 오류");
                ErrorHandler.HandleDataError("자동 저장", ex);
            }
            
            if (!saveSuccess)
            {
                currentRetryAttempts++;
                GameLogger.LogWarning($"저장 실패, 재시도 {currentRetryAttempts}/{maxRetryAttempts}");
                
                if (currentRetryAttempts < maxRetryAttempts)
                {
                    yield return new WaitForSeconds(retryDelay);
                }
            }
        }
        
        // 저장 완료 처리
        lastSaveTime = Time.time;
        isSaving = false;
        
        if (saveSuccess)
        {
            GameLogger.LogInfo("자동 저장 완료");
            OnAutoSaveCompleted?.Invoke();
        }
        else
        {
            GameLogger.LogError("자동 저장 실패");
            OnAutoSaveFailed?.Invoke();
        }
        
        // 저장 상태 UI 숨김
        OnSaveStatusChanged?.Invoke(false);
        
        if (showSaveIndicator)
        {
            ShowSaveIndicator(false);
        }
    }
    
    /// <summary>
    /// 저장 표시기 표시/숨김
    /// </summary>
    /// <param name="show">표시 여부</param>
    private void ShowSaveIndicator(bool show)
    {
        // TODO: UI 매니저와 연동하여 저장 표시기 구현
        if (show)
        {
            GameLogger.LogInfo("저장 중...");
            // SaveIndicatorUI.Show();
        }
        else
        {
            GameLogger.LogInfo("저장 완료");
            // SaveIndicatorUI.Hide();
        }
    }
    
    /// <summary>
    /// 자동 저장 설정 변경
    /// </summary>
    /// <param name="enabled">활성화 여부</param>
    public void SetAutoSaveEnabled(bool enabled)
    {
        enableAutoSave = enabled;
        
        // PlayerPrefs에 설정 저장
        PlayerPrefs.SetInt("AutoSaveEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
        
        if (enabled && !isAutoSaveRunning)
        {
            StartAutoSave();
        }
        else if (!enabled && isAutoSaveRunning)
        {
            StopAutoSave();
        }
        
        GameLogger.LogInfo($"자동 저장 {(enabled ? "활성화" : "비활성화")}");
    }
    
    /// <summary>
    /// 자동 저장 설정 상태 확인
    /// </summary>
    /// <returns>자동 저장 활성화 여부</returns>
    public bool GetAutoSaveEnabled()
    {
        return enableAutoSave;
    }
    
    /// <summary>
    /// 자동 저장 간격 설정
    /// </summary>
    /// <param name="interval">간격 (초)</param>
    public void SetAutoSaveInterval(float interval)
    {
        autoSaveInterval = Mathf.Max(10f, interval); // 최소 10초
        
        if (isAutoSaveRunning)
        {
            StopAutoSave();
            StartAutoSave();
        }
        
        GameLogger.LogInfo($"자동 저장 간격 변경: {autoSaveInterval}초");
    }
    
    /// <summary>
    /// 저장 쿨다운 설정
    /// </summary>
    /// <param name="cooldown">쿨다운 시간 (초)</param>
    public void SetSaveCooldown(float cooldown)
    {
        saveCooldown = Mathf.Max(1f, cooldown); // 최소 1초
        GameLogger.LogInfo($"저장 쿨다운 변경: {saveCooldown}초");
    }
    
    /// <summary>
    /// 현재 저장 상태 확인
    /// </summary>
    /// <returns>저장 중 여부</returns>
    public bool IsSaving()
    {
        return isSaving;
    }
    
    /// <summary>
    /// 자동 저장 상태 확인
    /// </summary>
    /// <returns>자동 저장 실행 중 여부</returns>
    public bool IsAutoSaveRunning()
    {
        return isAutoSaveRunning;
    }
    
    /// <summary>
    /// 마지막 저장 시간 확인
    /// </summary>
    /// <returns>마지막 저장 시간</returns>
    public float GetLastSaveTime()
    {
        return lastSaveTime;
    }
    
    /// <summary>
    /// 저장 통계 정보 출력
    /// </summary>
    public void PrintSaveStatistics()
    {
        string stats = $"[AutoSaveManager] 저장 통계:\n" +
                     $"  - 자동 저장 활성화: {enableAutoSave}\n" +
                     $"  - 자동 저장 실행 중: {isAutoSaveRunning}\n" +
                     $"  - 현재 저장 중: {isSaving}\n" +
                     $"  - 저장 간격: {autoSaveInterval}초\n" +
                     $"  - 쿨다운 시간: {saveCooldown}초\n" +
                     $"  - 최대 재시도 횟수: {maxRetryAttempts}\n" +
                     $"  - 마지막 저장 시간: {Time.time - lastSaveTime:F1}초 전";
        
        GameLogger.LogInfo(stats);
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && enableAutoSave && saveOnAppPaused)
        {
            GameLogger.LogInfo("앱 일시정지 - 자동 저장 실행");
            OnGameEvent(GameEvent.AppPaused);
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && enableAutoSave && saveOnAppPaused)
        {
            GameLogger.LogInfo("앱 포커스 해제 - 자동 저장 실행");
            OnGameEvent(GameEvent.AppPaused);
        }
    }
    
    private void OnDestroy()
    {
        if (isAutoSaveRunning)
        {
            StopAutoSave();
        }
        
        GameLogger.LogInfo("AutoSaveManager 종료");
    }
    
    /// <summary>
    /// 편의 메서드들
    /// </summary>
    public void OnScoreChanged() => OnGameEvent(GameEvent.ScoreChanged);
    public void OnCoinEarned() => OnGameEvent(GameEvent.CoinEarned);
    public void OnLevelComplete() => OnGameEvent(GameEvent.LevelComplete);
    public void OnSettingsChanged() => OnGameEvent(GameEvent.SettingsChanged);
    public void OnDataChanged() => OnGameEvent(GameEvent.DataChanged);
    public void OnAchievementUnlocked() => OnGameEvent(GameEvent.AchievementUnlocked);
    public void OnItemPurchased() => OnGameEvent(GameEvent.ItemPurchased);
    public void OnGameStarted() => OnGameEvent(GameEvent.GameStarted);
    public void OnGameEnded() => OnGameEvent(GameEvent.GameEnded);
} 