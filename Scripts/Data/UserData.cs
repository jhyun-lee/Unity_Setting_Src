using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    // 기본 사용자 정보
    [Header("User Information")]
    public string userId = "";              // 유저 ID
    public string nickname = "Player";      // 닉네임
    public string email = "";               // 이메일 (선택사항)
    
    // 게임 진행 정보
    [Header("Game Progress")]
    public long highScore = 0;              // 최고 점수
    public long totalCoins = 0;             // 총 코인
    public int currentLevel = 1;            // 현재 레벨
    public int maxLevelUnlocked = 1;        // 최대 해금된 레벨
    
    // 게임 통계
    [Header("Game Statistics")]
    public GameStats gameStats;             // 게임 통계 데이터
    
    // 튜토리얼 및 학습 정보
    [Header("Tutorial & Learning")]
    public bool tutorialCompleted = false;   // 튜토리얼 완료 상태
    public List<string> completedTutorials = new List<string>(); // 완료된 튜토리얼 목록
    
    // 시간 정보
    [Header("Time Information")]
    public DateTime firstPlayTime;           // 첫 플레이 시간
    public DateTime lastPlayTime;            // 마지막 플레이 시간
    public float totalPlayTimeSeconds = 0f;  // 총 플레이 시간 (초)
    
    // 인벤토리 및 아이템
    [Header("Inventory & Items")]
    public List<string> unlockedItems = new List<string>();     // 해금된 아이템들
    public List<string> equippedItems = new List<string>();     // 장착된 아이템들
    public Dictionary<string, int> itemCounts = new Dictionary<string, int>(); // 아이템 개수
    
    // 업적 및 보상
    [Header("Achievements & Rewards")]
    public List<string> unlockedAchievements = new List<string>(); // 해금된 업적들
    public List<string> claimedRewards = new List<string>();       // 수령한 보상들
    
    // 설정 및 선호도
    [Header("Preferences")]
    public string preferredLanguage = "ko";  // 선호 언어
    public bool soundEnabled = true;         // 사운드 활성화
    public bool vibrationEnabled = true;     // 진동 활성화
    
    // 버전 정보
    [Header("Version Information")]
    public string appVersion = "1.0.0";     // 앱 버전
    public string dataVersion = "1.0";      // 데이터 버전
    public DateTime dataCreatedTime;         // 데이터 생성 시간
    public DateTime dataLastModifiedTime;    // 데이터 마지막 수정 시간
    
    // 생성자
    public UserData()
    {
        gameStats = new GameStats();
        dataCreatedTime = DateTime.Now;
        dataLastModifiedTime = DateTime.Now;
        firstPlayTime = DateTime.Now;
        lastPlayTime = DateTime.Now;
    }
    
    // 새로운 사용자 생성
    public static UserData CreateNewUser(string newUserId, string newNickname = "Player")
    {
        UserData newUser = new UserData();
        newUser.userId = newUserId;
        newUser.nickname = newNickname;
        newUser.dataCreatedTime = DateTime.Now;
        newUser.dataLastModifiedTime = DateTime.Now;
        newUser.firstPlayTime = DateTime.Now;
        newUser.lastPlayTime = DateTime.Now;
        
        return newUser;
    }
    
    // 코인 추가
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateLastModifiedTime();
    }
    
    // 코인 사용
    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            UpdateLastModifiedTime();
            return true;
        }
        return false;
    }
    
    // 점수 업데이트
    public void UpdateScore(long newScore)
    {
        if (newScore > highScore)
        {
            highScore = newScore;
        }
        UpdateLastModifiedTime();
    }
    
    // 레벨 업데이트
    public void UpdateLevel(int newLevel)
    {
        currentLevel = newLevel;
        if (newLevel > maxLevelUnlocked)
        {
            maxLevelUnlocked = newLevel;
        }
        UpdateLastModifiedTime();
    }
    
    // 아이템 해금
    public void UnlockItem(string itemId)
    {
        if (!unlockedItems.Contains(itemId))
        {
            unlockedItems.Add(itemId);
            UpdateLastModifiedTime();
        }
    }
    
    // 아이템 장착
    public void EquipItem(string itemId)
    {
        if (unlockedItems.Contains(itemId) && !equippedItems.Contains(itemId))
        {
            equippedItems.Add(itemId);
            UpdateLastModifiedTime();
        }
    }
    
    // 아이템 해제
    public void UnequipItem(string itemId)
    {
        equippedItems.Remove(itemId);
        UpdateLastModifiedTime();
    }
    
    // 아이템 개수 추가
    public void AddItemCount(string itemId, int count)
    {
        if (itemCounts.ContainsKey(itemId))
        {
            itemCounts[itemId] += count;
        }
        else
        {
            itemCounts[itemId] = count;
        }
        UpdateLastModifiedTime();
    }
    
    // 업적 해금
    public void UnlockAchievement(string achievementId)
    {
        if (!unlockedAchievements.Contains(achievementId))
        {
            unlockedAchievements.Add(achievementId);
            gameStats.AddAchievement(achievementId);
            UpdateLastModifiedTime();
        }
    }
    
    // 보상 수령
    public void ClaimReward(string rewardId)
    {
        if (!claimedRewards.Contains(rewardId))
        {
            claimedRewards.Add(rewardId);
            UpdateLastModifiedTime();
        }
    }
    
    // 튜토리얼 완료
    public void CompleteTutorial(string tutorialId)
    {
        if (!completedTutorials.Contains(tutorialId))
        {
            completedTutorials.Add(tutorialId);
            UpdateLastModifiedTime();
        }
    }
    
    // 플레이 시간 업데이트
    public void UpdatePlayTime(float playTimeSeconds)
    {
        totalPlayTimeSeconds += playTimeSeconds;
        lastPlayTime = DateTime.Now;
        gameStats.totalPlayTime += playTimeSeconds;
        UpdateLastModifiedTime();
    }
    
    // 마지막 수정 시간 업데이트
    private void UpdateLastModifiedTime()
    {
        dataLastModifiedTime = DateTime.Now;
    }
    
    // 사용자 데이터 검증
    public void ValidateData()
    {
        // 기본값 검증
        if (string.IsNullOrEmpty(userId))
        {
            userId = System.Guid.NewGuid().ToString();
        }
        
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = "Player";
        }
        
        // 음수 값 방지
        if (totalCoins < 0) totalCoins = 0;
        if (highScore < 0) highScore = 0;
        if (currentLevel < 1) currentLevel = 1;
        if (maxLevelUnlocked < 1) maxLevelUnlocked = 1;
        if (totalPlayTimeSeconds < 0) totalPlayTimeSeconds = 0;
        
        // 게임 통계 검증
        if (gameStats == null)
        {
            gameStats = new GameStats();
        }
    }
    
    // 사용자 데이터 초기화 (주의: 모든 데이터 삭제)
    public void ResetUserData()
    {
        highScore = 0;
        totalCoins = 0;
        currentLevel = 1;
        maxLevelUnlocked = 1;
        gameStats.ResetStats();
        tutorialCompleted = false;
        completedTutorials.Clear();
        unlockedItems.Clear();
        equippedItems.Clear();
        itemCounts.Clear();
        unlockedAchievements.Clear();
        claimedRewards.Clear();
        totalPlayTimeSeconds = 0f;
        firstPlayTime = DateTime.Now;
        lastPlayTime = DateTime.Now;
        UpdateLastModifiedTime();
    }
    
    // 사용자 데이터 복사
    public void CopyFrom(UserData other)
    {
        userId = other.userId;
        nickname = other.nickname;
        email = other.email;
        highScore = other.highScore;
        totalCoins = other.totalCoins;
        currentLevel = other.currentLevel;
        maxLevelUnlocked = other.maxLevelUnlocked;
        gameStats = other.gameStats;
        tutorialCompleted = other.tutorialCompleted;
        completedTutorials = new List<string>(other.completedTutorials);
        firstPlayTime = other.firstPlayTime;
        lastPlayTime = other.lastPlayTime;
        totalPlayTimeSeconds = other.totalPlayTimeSeconds;
        unlockedItems = new List<string>(other.unlockedItems);
        equippedItems = new List<string>(other.equippedItems);
        itemCounts = new Dictionary<string, int>(other.itemCounts);
        unlockedAchievements = new List<string>(other.unlockedAchievements);
        claimedRewards = new List<string>(other.claimedRewards);
        preferredLanguage = other.preferredLanguage;
        soundEnabled = other.soundEnabled;
        vibrationEnabled = other.vibrationEnabled;
        appVersion = other.appVersion;
        dataVersion = other.dataVersion;
        dataCreatedTime = other.dataCreatedTime;
        dataLastModifiedTime = other.dataLastModifiedTime;
    }
} 