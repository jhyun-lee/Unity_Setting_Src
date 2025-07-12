using UnityEngine;
using System;

public class UserDataManager : MonoBehaviour
{
    #region Singleton Pattern
    
    public static UserDataManager Instance { get; private set; }
    
    #endregion
    
    #region Properties
    
    [Header("User Data")]
    public UserData CurrentData { get; private set; }
    
    #endregion
    
    #region Events
    
    // 사용자 데이터 변경 이벤트
    public static event Action OnUserDataChanged;
    public static event Action OnUserDataLoaded;
    public static event Action OnUserDataSaved;
    
    #endregion
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        InitializeSingleton();
        InitializeUserData();
    }
    
    private void Start()
    {
        // 추가 초기화 로직이 필요한 경우 여기에 구현
    }
    
    private void OnDestroy()
    {
        // 정리 작업이 필요한 경우 여기에 구현
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeSingleton()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("UserDataManager 초기화 완료");
        }
        else
        {
            // 이미 인스턴스가 존재하는 경우 중복 생성 방지
            Debug.LogWarning("UserDataManager가 이미 존재합니다. 중복 생성 방지.");
            Destroy(gameObject);
        }
    }
    
    private void InitializeUserData()
    {
        // 기본 사용자 데이터 초기화
        CurrentData = UserData.CreateNewUser(System.Guid.NewGuid().ToString(), "Player");
        
        // 데이터 검증
        CurrentData.ValidateData();
        
        Debug.Log("사용자 데이터 초기화 완료");
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// 현재 사용자 데이터를 새로운 데이터로 업데이트
    /// </summary>
    /// <param name="newData">새로운 사용자 데이터</param>
    public void UpdateUserData(UserData newData)
    {
        if (newData != null)
        {
            CurrentData = newData;
            CurrentData.ValidateData();
            OnUserDataChanged?.Invoke();
            Debug.Log("사용자 데이터가 업데이트되었습니다.");
        }
        else
        {
            Debug.LogError("새로운 사용자 데이터가 null입니다.");
        }
    }
    
    /// <summary>
    /// 사용자 데이터를 기본값으로 초기화
    /// </summary>
    public void ResetUserData()
    {
        CurrentData.ResetUserData();
        OnUserDataChanged?.Invoke();
        Debug.Log("사용자 데이터가 초기화되었습니다.");
    }
    
    /// <summary>
    /// 사용자 데이터가 유효한지 확인
    /// </summary>
    /// <returns>유효한 경우 true</returns>
    public bool IsValid()
    {
        return CurrentData != null && !string.IsNullOrEmpty(CurrentData.userId);
    }
    
    /// <summary>
    /// 현재 사용자 정보를 문자열로 반환
    /// </summary>
    /// <returns>사용자 정보 문자열</returns>
    public string GetUserDataInfo()
    {
        if (CurrentData == null)
            return "사용자 데이터가 없습니다.";
        
        return $"사용자 ID: {CurrentData.userId}\n" +
               $"닉네임: {CurrentData.nickname}\n" +
               $"현재 레벨: {CurrentData.currentLevel}\n" +
               $"최고 점수: {CurrentData.highScore:N0}\n" +
               $"총 코인: {CurrentData.totalCoins:N0}\n" +
               $"총 플레이 시간: {CurrentData.totalPlayTimeSeconds / 3600:F1}시간\n" +
               $"마지막 플레이: {CurrentData.lastPlayTime:yyyy-MM-dd HH:mm}";
    }
    
    #endregion
    
    #region User Data Access Methods
    
    /// <summary>
    /// 사용자 닉네임 설정
    /// </summary>
    /// <param name="nickname">새로운 닉네임</param>
    public void SetNickname(string nickname)
    {
        if (CurrentData != null && !string.IsNullOrEmpty(nickname))
        {
            CurrentData.nickname = nickname;
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 코인 추가
    /// </summary>
    /// <param name="amount">추가할 코인 수량</param>
    public void AddCoins(int amount)
    {
        if (CurrentData != null && amount > 0)
        {
            CurrentData.AddCoins(amount);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 코인 사용
    /// </summary>
    /// <param name="amount">사용할 코인 수량</param>
    /// <returns>사용 성공 여부</returns>
    public bool SpendCoins(int amount)
    {
        if (CurrentData != null && amount > 0)
        {
            bool success = CurrentData.SpendCoins(amount);
            if (success)
            {
                OnUserDataChanged?.Invoke();
            }
            return success;
        }
        return false;
    }
    
    /// <summary>
    /// 점수 업데이트
    /// </summary>
    /// <param name="newScore">새로운 점수</param>
    public void UpdateScore(long newScore)
    {
        if (CurrentData != null && newScore >= 0)
        {
            CurrentData.UpdateScore(newScore);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 레벨 업데이트
    /// </summary>
    /// <param name="newLevel">새로운 레벨</param>
    public void UpdateLevel(int newLevel)
    {
        if (CurrentData != null && newLevel > 0)
        {
            CurrentData.UpdateLevel(newLevel);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 아이템 해금
    /// </summary>
    /// <param name="itemId">아이템 ID</param>
    public void UnlockItem(string itemId)
    {
        if (CurrentData != null && !string.IsNullOrEmpty(itemId))
        {
            CurrentData.UnlockItem(itemId);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 아이템 장착
    /// </summary>
    /// <param name="itemId">아이템 ID</param>
    public void EquipItem(string itemId)
    {
        if (CurrentData != null && !string.IsNullOrEmpty(itemId))
        {
            CurrentData.EquipItem(itemId);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 아이템 해제
    /// </summary>
    /// <param name="itemId">아이템 ID</param>
    public void UnequipItem(string itemId)
    {
        if (CurrentData != null && !string.IsNullOrEmpty(itemId))
        {
            CurrentData.UnequipItem(itemId);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 아이템 개수 추가
    /// </summary>
    /// <param name="itemId">아이템 ID</param>
    /// <param name="count">추가할 개수</param>
    public void AddItemCount(string itemId, int count)
    {
        if (CurrentData != null && !string.IsNullOrEmpty(itemId) && count > 0)
        {
            CurrentData.AddItemCount(itemId, count);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 업적 해금
    /// </summary>
    /// <param name="achievementId">업적 ID</param>
    public void UnlockAchievement(string achievementId)
    {
        if (CurrentData != null && !string.IsNullOrEmpty(achievementId))
        {
            CurrentData.UnlockAchievement(achievementId);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 보상 수령
    /// </summary>
    /// <param name="rewardId">보상 ID</param>
    public void ClaimReward(string rewardId)
    {
        if (CurrentData != null && !string.IsNullOrEmpty(rewardId))
        {
            CurrentData.ClaimReward(rewardId);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 튜토리얼 완료
    /// </summary>
    /// <param name="tutorialId">튜토리얼 ID</param>
    public void CompleteTutorial(string tutorialId)
    {
        if (CurrentData != null && !string.IsNullOrEmpty(tutorialId))
        {
            CurrentData.CompleteTutorial(tutorialId);
            OnUserDataChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// 플레이 시간 업데이트
    /// </summary>
    /// <param name="playTimeSeconds">플레이 시간 (초)</param>
    public void UpdatePlayTime(float playTimeSeconds)
    {
        if (CurrentData != null && playTimeSeconds > 0)
        {
            CurrentData.UpdatePlayTime(playTimeSeconds);
            OnUserDataChanged?.Invoke();
        }
    }
    
    #endregion
    
    #region Game Statistics Methods
    
    /// <summary>
    /// 게임 결과 추가
    /// </summary>
    /// <param name="score">점수</param>
    /// <param name="playTime">플레이 시간</param>
    /// <param name="levelCleared">클리어한 레벨</param>
    public void AddGameResult(long score, float playTime, int levelCleared = 0)
    {
        if (CurrentData != null && CurrentData.gameStats != null)
        {
            CurrentData.gameStats.AddGameResult(score, playTime, levelCleared);
            UpdatePlayTime(playTime);
            UpdateScore(score);
            
            if (levelCleared > 0)
            {
                UpdateLevel(levelCleared);
            }
        }
    }
    
    /// <summary>
    /// 게임 통계 정보 반환
    /// </summary>
    /// <returns>통계 정보 문자열</returns>
    public string GetGameStatsInfo()
    {
        if (CurrentData?.gameStats == null)
            return "게임 통계가 없습니다.";
        
        GameStats stats = CurrentData.gameStats;
        return $"총 플레이 시간: {stats.totalPlayTime / 3600:F1}시간\n" +
               $"게임 플레이 횟수: {stats.totalGamesPlayed}회\n" +
               $"최고 점수: {stats.highScore:N0}\n" +
               $"평균 점수: {stats.averageScore:N0}\n" +
               $"업적 수: {stats.achievements.Count}개\n" +
               $"클리어한 레벨: {stats.totalLevelsCleared}개";
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// 사용자 데이터가 변경되었는지 확인 (향후 구현 예정)
    /// </summary>
    /// <returns>변경 여부</returns>
    public bool HasUserDataChanged()
    {
        // 향후 구현 예정
        return false;
    }
    
    /// <summary>
    /// 사용자 데이터 변경 사항을 초기화 (향후 구현 예정)
    /// </summary>
    public void ClearUserDataChanged()
    {
        // 향후 구현 예정
    }
    
    /// <summary>
    /// 새로운 사용자 생성
    /// </summary>
    /// <param name="nickname">닉네임</param>
    public void CreateNewUser(string nickname = "Player")
    {
        CurrentData = UserData.CreateNewUser(System.Guid.NewGuid().ToString(), nickname);
        OnUserDataChanged?.Invoke();
        Debug.Log($"새로운 사용자가 생성되었습니다: {nickname}");
    }
    
    #endregion
} 