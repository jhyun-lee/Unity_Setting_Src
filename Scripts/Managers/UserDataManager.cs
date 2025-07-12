using UnityEngine;
using System;
using System.IO;

public class UserDataManager : MonoBehaviour
{
    [Header("Data Settings")]
    [SerializeField] private string dataFileName = "userdata.json";
    [SerializeField] private string backupFileName = "userdata_backup.json";
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    [Header("Data Validation")]
    [SerializeField] private bool enableAutoRepair = true;
    [SerializeField] private bool enableAutoMigration = true;
    
    [Header("Security")]
    [SerializeField] private bool enableEncryption = true;
    
    // 싱글톤 인스턴스
    public static UserDataManager Instance { get; private set; }
    
    // 현재 사용자 데이터
    public UserData CurrentUserData { get; private set; }
    
    // 데이터 버전 관리
    private const int CURRENT_DATA_VERSION = 4;
    private const int MIN_SUPPORTED_VERSION = 1;
    
    // 데이터 변경 이벤트
    public event Action<UserData> OnDataChanged;
    public event Action<UserData> OnDataLoaded;
    public event Action<UserData> OnDataSaved;
    public event Action<string> OnDataError;
    public event Action<UserData> OnDataMigrated;
    public event Action<UserData> OnDataRepaired;
    
    // 파일 경로
    private string DataFilePath => Path.Combine(Application.persistentDataPath, dataFileName);
    private string BackupFilePath => Path.Combine(Application.persistentDataPath, backupFileName);
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeManager()
    {
        LogDebug("UserDataManager 초기화 시작");
        
        // 데이터 로드 시도
        if (!LoadData())
        {
            LogDebug("데이터 로드 실패, 기본 데이터 생성");
            CreateDefaultData();
        }
        
        LogDebug("UserDataManager 초기화 완료");
    }
    
    /// <summary>
    /// JSON 파일에서 사용자 데이터 로드
    /// </summary>
    public bool LoadData()
    {
        try
        {
            if (!File.Exists(DataFilePath))
            {
                LogDebug("데이터 파일이 존재하지 않음: " + DataFilePath);
                return false;
            }
            
            string jsonData = File.ReadAllText(DataFilePath);
            
            // 암호화된 데이터 복호화
            if (enableEncryption && EncryptionHelper.IsEncrypted(jsonData))
            {
                jsonData = EncryptionHelper.Decrypt(jsonData);
                LogDebug("암호화된 데이터 복호화 완료");
            }
            
            UserData loadedData = JsonUtility.FromJson<UserData>(jsonData);
            
            // 데이터 버전 체크 및 마이그레이션
            if (loadedData.dataVersion < CURRENT_DATA_VERSION)
            {
                LogDebug($"데이터 버전이 낮습니다: {loadedData.dataVersion} → {CURRENT_DATA_VERSION}");
                
                if (enableAutoMigration)
                {
                    if (!MigrateData(loadedData, loadedData.dataVersion, CURRENT_DATA_VERSION))
                    {
                        LogError("데이터 마이그레이션 실패");
                        return TryRestoreFromBackup();
                    }
                }
                else
                {
                    LogError("자동 마이그레이션이 비활성화되어 있습니다");
                    return false;
                }
            }
            else if (loadedData.dataVersion > CURRENT_DATA_VERSION)
            {
                LogError($"지원하지 않는 데이터 버전: {loadedData.dataVersion} (현재 버전: {CURRENT_DATA_VERSION})");
                return TryRestoreFromBackup();
            }
            
            // 데이터 검증 및 복구
            if (!ValidateUserData(loadedData))
            {
                LogDebug("데이터 검증 실패, 복구 시도");
                
                if (enableAutoRepair)
                {
                    if (!RepairData(loadedData))
                    {
                        LogError("데이터 복구 실패");
                        return TryRestoreFromBackup();
                    }
                }
                else
                {
                    LogError("자동 복구가 비활성화되어 있습니다");
                    return TryRestoreFromBackup();
                }
            }
            
            CurrentUserData = loadedData;
            LogDebug("데이터 로드 성공");
            OnDataLoaded?.Invoke(CurrentUserData);
            return true;
        }
        catch (Exception e)
        {
            LogError("데이터 로드 중 오류 발생: " + e.Message);
            OnDataError?.Invoke("데이터 로드 실패: " + e.Message);
            
            // 에러 핸들링 시스템 사용
            if (!ErrorHandler.HandleDataError("데이터 로드", e))
            {
                return TryRestoreFromBackup();
            }
            
            return false;
        }
    }
    
    /// <summary>
    /// JSON 파일에 사용자 데이터 저장
    /// </summary>
    public bool SaveData()
    {
        try
        {
            if (CurrentUserData == null)
            {
                LogError("저장할 데이터가 없습니다");
                return false;
            }
            
            // 백업 생성
            CreateBackup();
            
            // 데이터 검증
            if (!ValidateData(CurrentUserData))
            {
                LogError("저장할 데이터가 유효하지 않습니다");
                return false;
            }
            
            // JSON 직렬화
            string jsonData = JsonUtility.ToJson(CurrentUserData, true);
            
            // 암호화 적용
            if (enableEncryption)
            {
                jsonData = EncryptionHelper.Encrypt(jsonData);
                LogDebug("데이터 암호화 적용");
            }
            
            // 파일 저장
            File.WriteAllText(DataFilePath, jsonData);
            
            LogDebug("데이터 저장 성공: " + DataFilePath);
            OnDataSaved?.Invoke(CurrentUserData);
            return true;
        }
        catch (Exception e)
        {
            LogError("데이터 저장 중 오류 발생: " + e.Message);
            OnDataError?.Invoke("데이터 저장 실패: " + e.Message);
            
            // 에러 핸들링 시스템 사용
            ErrorHandler.HandleDataError("데이터 저장", e);
            return false;
        }
    }
    
    /// <summary>
    /// 기본 사용자 데이터 생성
    /// </summary>
    public void CreateDefaultData()
    {
        CurrentUserData = new UserData
        {
            playerName = "Player",
            level = 1,
            experience = 0,
            coins = 100,
            gems = 10,
            totalPlayTime = 0,
            lastLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            isFirstTime = true,
            dataVersion = CURRENT_DATA_VERSION,
            achievements = new string[0],
            gameStats = new GameStats()
        };
        
        LogDebug("기본 데이터 생성 완료 (버전 " + CURRENT_DATA_VERSION + ")");
        SaveData();
        OnDataChanged?.Invoke(CurrentUserData);
    }
    
    /// <summary>
    /// 사용자 데이터 유효성 검사
    /// </summary>
    public bool ValidateData(UserData data)
    {
        return ValidateUserData(data);
    }
    
    /// <summary>
    /// 전체 사용자 데이터 검증
    /// </summary>
    public bool ValidateUserData(UserData data)
    {
        if (data == null)
        {
            LogError("데이터가 null입니다");
            return false;
        }
        
        // 기본 필드 검증
        if (string.IsNullOrEmpty(data.playerName))
        {
            LogError("플레이어 이름이 비어있습니다");
            if (enableAutoRepair)
            {
                data.playerName = "Player";
                LogDebug("플레이어 이름을 기본값으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        if (data.level < 1)
        {
            LogError("레벨이 유효하지 않습니다: " + data.level);
            if (enableAutoRepair)
            {
                data.level = 1;
                LogDebug("레벨을 1로 수정");
            }
            else
            {
                return false;
            }
        }
        
        if (data.experience < 0)
        {
            LogError("경험치가 음수입니다: " + data.experience);
            if (enableAutoRepair)
            {
                data.experience = 0;
                LogDebug("경험치를 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        if (data.coins < 0)
        {
            LogError("코인이 음수입니다: " + data.coins);
            if (enableAutoRepair)
            {
                data.coins = 0;
                LogDebug("코인을 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        if (data.gems < 0)
        {
            LogError("젬이 음수입니다: " + data.gems);
            if (enableAutoRepair)
            {
                data.gems = 0;
                LogDebug("젬을 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        if (data.totalPlayTime < 0)
        {
            LogError("총 플레이 시간이 음수입니다: " + data.totalPlayTime);
            if (enableAutoRepair)
            {
                data.totalPlayTime = 0;
                LogDebug("총 플레이 시간을 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        // GameStats 검증
        if (!ValidateGameStats(data.gameStats))
        {
            return false;
        }
        
        // 업적 배열 검증
        if (data.achievements == null)
        {
            LogError("업적 배열이 null입니다");
            if (enableAutoRepair)
            {
                data.achievements = new string[0];
                LogDebug("업적 배열을 빈 배열로 수정");
            }
            else
            {
                return false;
            }
        }
        
        // 논리적 일관성 검증
        if (data.gameStats.totalGames != (data.gameStats.wins + data.gameStats.losses))
        {
            LogError("게임 통계가 일관되지 않습니다");
            if (enableAutoRepair)
            {
                data.gameStats.totalGames = data.gameStats.wins + data.gameStats.losses;
                LogDebug("게임 통계를 일관되게 수정");
            }
            else
            {
                return false;
            }
        }
        
        LogDebug("데이터 검증 성공");
        return true;
    }
    
    /// <summary>
    /// 게임 통계 검증
    /// </summary>
    public bool ValidateGameStats(GameStats stats)
    {
        if (stats == null)
        {
            LogError("게임 통계가 null입니다");
            if (enableAutoRepair)
            {
                stats = new GameStats();
                LogDebug("게임 통계를 새로 생성");
            }
            else
            {
                return false;
            }
        }
        
        if (stats.totalGames < 0)
        {
            LogError("총 게임 수가 음수입니다: " + stats.totalGames);
            if (enableAutoRepair)
            {
                stats.totalGames = 0;
                LogDebug("총 게임 수를 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        if (stats.wins < 0)
        {
            LogError("승리 수가 음수입니다: " + stats.wins);
            if (enableAutoRepair)
            {
                stats.wins = 0;
                LogDebug("승리 수를 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        if (stats.losses < 0)
        {
            LogError("패배 수가 음수입니다: " + stats.losses);
            if (enableAutoRepair)
            {
                stats.losses = 0;
                LogDebug("패배 수를 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        if (stats.highestScore < 0)
        {
            LogError("최고 점수가 음수입니다: " + stats.highestScore);
            if (enableAutoRepair)
            {
                stats.highestScore = 0;
                LogDebug("최고 점수를 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        // 추가 통계 필드 검증 (버전 4부터)
        if (stats.averageScore < 0)
        {
            LogError("평균 점수가 음수입니다: " + stats.averageScore);
            if (enableAutoRepair)
            {
                stats.averageScore = 0;
                LogDebug("평균 점수를 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        if (stats.totalPlayTime < 0)
        {
            LogError("총 플레이 시간이 음수입니다: " + stats.totalPlayTime);
            if (enableAutoRepair)
            {
                stats.totalPlayTime = 0;
                LogDebug("총 플레이 시간을 0으로 수정");
            }
            else
            {
                return false;
            }
        }
        
        LogDebug("게임 통계 검증 성공");
        return true;
    }
    
    /// <summary>
    /// 점수 업데이트
    /// </summary>
    public void UpdateScore(int newScore)
    {
        if (CurrentUserData == null)
        {
            LogError("업데이트할 데이터가 없습니다");
            return;
        }
        
        CurrentUserData.gameStats.totalGames++;
        
        if (newScore > CurrentUserData.gameStats.highestScore)
        {
            CurrentUserData.gameStats.highestScore = newScore;
            LogDebug("새로운 최고 점수 달성: " + newScore);
        }
        
        // 승리/패배 판정 (임시 로직 - 실제 게임에 맞게 수정 필요)
        if (newScore > 0)
        {
            CurrentUserData.gameStats.wins++;
        }
        else
        {
            CurrentUserData.gameStats.losses++;
        }
        
        LogDebug("점수 업데이트: " + newScore);
        SaveData();
        OnDataChanged?.Invoke(CurrentUserData);
        
        // 자동 저장 이벤트 발생
        if (AutoSaveManager.Instance != null)
        {
            AutoSaveManager.Instance.OnScoreChanged();
        }
    }
    
    /// <summary>
    /// 코인 업데이트
    /// </summary>
    public void UpdateCoins(int amount)
    {
        if (CurrentUserData == null)
        {
            LogError("업데이트할 데이터가 없습니다");
            return;
        }
        
        int newAmount = CurrentUserData.coins + amount;
        if (newAmount < 0)
        {
            LogError("코인 부족: 현재 " + CurrentUserData.coins + ", 요청 " + amount);
            return;
        }
        
        CurrentUserData.coins = newAmount;
        LogDebug("코인 업데이트: " + amount + " (총 " + newAmount + ")");
        SaveData();
        OnDataChanged?.Invoke(CurrentUserData);
        
        // 자동 저장 이벤트 발생
        if (AutoSaveManager.Instance != null)
        {
            AutoSaveManager.Instance.OnCoinEarned();
        }
    }
    
    /// <summary>
    /// 젬 업데이트
    /// </summary>
    public void UpdateGems(int amount)
    {
        if (CurrentUserData == null)
        {
            LogError("업데이트할 데이터가 없습니다");
            return;
        }
        
        int newAmount = CurrentUserData.gems + amount;
        if (newAmount < 0)
        {
            LogError("젬 부족: 현재 " + CurrentUserData.gems + ", 요청 " + amount);
            return;
        }
        
        CurrentUserData.gems = newAmount;
        LogDebug("젬 업데이트: " + amount + " (총 " + newAmount + ")");
        SaveData();
        OnDataChanged?.Invoke(CurrentUserData);
        
        // 자동 저장 이벤트 발생
        if (AutoSaveManager.Instance != null)
        {
            AutoSaveManager.Instance.OnDataChanged();
        }
    }
    
    /// <summary>
    /// 경험치 업데이트
    /// </summary>
    public void UpdateExperience(int amount)
    {
        if (CurrentUserData == null)
        {
            LogError("업데이트할 데이터가 없습니다");
            return;
        }
        
        CurrentUserData.experience += amount;
        
        // 레벨업 체크 (임시 로직 - 실제 게임에 맞게 수정 필요)
        int requiredExp = CurrentUserData.level * 100;
        if (CurrentUserData.experience >= requiredExp)
        {
            CurrentUserData.level++;
            CurrentUserData.experience -= requiredExp;
            LogDebug("레벨업! 새로운 레벨: " + CurrentUserData.level);
        }
        
        LogDebug("경험치 업데이트: " + amount + " (총 " + CurrentUserData.experience + ")");
        SaveData();
        OnDataChanged?.Invoke(CurrentUserData);
        
        // 자동 저장 이벤트 발생
        if (AutoSaveManager.Instance != null)
        {
            AutoSaveManager.Instance.OnDataChanged();
        }
    }
    
    /// <summary>
    /// 게임 통계 업데이트
    /// </summary>
    public void UpdateGameStats()
    {
        if (CurrentUserData == null)
        {
            LogError("업데이트할 데이터가 없습니다");
            return;
        }
        
        // 마지막 로그인 시간 업데이트
        CurrentUserData.lastLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        // 첫 로그인 체크
        if (CurrentUserData.isFirstTime)
        {
            CurrentUserData.isFirstTime = false;
            LogDebug("첫 로그인 완료");
        }
        
        LogDebug("게임 통계 업데이트 완료");
        SaveData();
        OnDataChanged?.Invoke(CurrentUserData);
        
        // 자동 저장 이벤트 발생
        if (AutoSaveManager.Instance != null)
        {
            AutoSaveManager.Instance.OnDataChanged();
        }
    }
    
    /// <summary>
    /// 데이터 초기화
    /// </summary>
    public void ResetData()
    {
        LogDebug("데이터 초기화 시작");
        
        // 백업 파일 삭제
        if (File.Exists(BackupFilePath))
        {
            File.Delete(BackupFilePath);
            LogDebug("백업 파일 삭제");
        }
        
        // 메인 데이터 파일 삭제
        if (File.Exists(DataFilePath))
        {
            File.Delete(DataFilePath);
            LogDebug("메인 데이터 파일 삭제");
        }
        
        // 기본 데이터 생성
        CreateDefaultData();
        
        LogDebug("데이터 초기화 완료");
    }
    
    /// <summary>
    /// 현재 앱의 데이터 버전 반환
    /// </summary>
    public int GetCurrentDataVersion()
    {
        return CURRENT_DATA_VERSION;
    }
    
    /// <summary>
    /// 데이터 마이그레이션
    /// </summary>
    public bool MigrateData(UserData data, int fromVersion, int toVersion)
    {
        if (!enableAutoMigration)
        {
            LogDebug("자동 마이그레이션이 비활성화되어 있습니다");
            return false;
        }
        
        if (fromVersion >= toVersion)
        {
            LogDebug("마이그레이션이 필요하지 않습니다");
            return true;
        }
        
        LogDebug($"데이터 마이그레이션 시작: 버전 {fromVersion} → {toVersion}");
        
        try
        {
            // 마이그레이션 전 백업 생성
            CreateBackup();
            
            // 단계별 마이그레이션 수행
            for (int version = fromVersion; version < toVersion; version++)
            {
                if (!PerformMigrationStep(data, version, version + 1))
                {
                    LogError($"버전 {version} → {version + 1} 마이그레이션 실패");
                    return false;
                }
            }
            
            // 최신 버전으로 설정
            data.dataVersion = toVersion;
            
            LogDebug("데이터 마이그레이션 완료");
            OnDataMigrated?.Invoke(data);
            return true;
        }
        catch (Exception e)
        {
            LogError($"마이그레이션 중 오류 발생: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 단계별 마이그레이션 수행
    /// </summary>
    private bool PerformMigrationStep(UserData data, int fromVersion, int toVersion)
    {
        LogDebug($"마이그레이션 단계: {fromVersion} → {toVersion}");
        
        switch (fromVersion)
        {
            case 1:
                return MigrateFromV1ToV2(data);
            case 2:
                return MigrateFromV2ToV3(data);
            case 3:
                return MigrateFromV3ToV4(data);
            default:
                LogError($"지원하지 않는 마이그레이션: {fromVersion} → {toVersion}");
                return false;
        }
    }
    
    /// <summary>
    /// 버전 1 → 2 마이그레이션 (GameStats 추가)
    /// </summary>
    private bool MigrateFromV1ToV2(UserData data)
    {
        try
        {
            LogDebug("버전 1 → 2 마이그레이션: GameStats 추가");
            
            // GameStats가 없으면 새로 생성
            if (data.gameStats == null)
            {
                data.gameStats = new GameStats();
                LogDebug("GameStats 객체 생성");
            }
            
            return true;
        }
        catch (Exception e)
        {
            LogError($"버전 1 → 2 마이그레이션 실패: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 버전 2 → 3 마이그레이션 (업적 시스템 추가)
    /// </summary>
    private bool MigrateFromV2ToV3(UserData data)
    {
        try
        {
            LogDebug("버전 2 → 3 마이그레이션: 업적 시스템 추가");
            
            // 업적 배열이 없으면 새로 생성
            if (data.achievements == null)
            {
                data.achievements = new string[0];
                LogDebug("업적 배열 생성");
            }
            
            return true;
        }
        catch (Exception e)
        {
            LogError($"버전 2 → 3 마이그레이션 실패: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 버전 3 → 4 마이그레이션 (새로운 통계 필드 추가)
    /// </summary>
    private bool MigrateFromV3ToV4(UserData data)
    {
        try
        {
            LogDebug("버전 3 → 4 마이그레이션: 새로운 통계 필드 추가");
            
            // GameStats에 새로운 필드 추가
            if (data.gameStats != null)
            {
                // averageScore 필드가 없으면 기본값 설정
                if (data.gameStats.averageScore < 0)
                {
                    data.gameStats.averageScore = 0;
                    LogDebug("평균 점수 필드 추가");
                }
                
                // totalPlayTime 필드가 없으면 기본값 설정
                if (data.gameStats.totalPlayTime < 0)
                {
                    data.gameStats.totalPlayTime = 0;
                    LogDebug("총 플레이 시간 필드 추가");
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            LogError($"버전 3 → 4 마이그레이션 실패: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 손상된 데이터 수정
    /// </summary>
    public bool RepairData(UserData data)
    {
        if (!enableAutoRepair)
        {
            LogDebug("자동 복구가 비활성화되어 있습니다");
            return false;
        }
        
        LogDebug("데이터 복구 시작");
        
        try
        {
            bool repaired = false;
            
            // 기본 필드 복구
            if (string.IsNullOrEmpty(data.playerName))
            {
                data.playerName = "Player";
                repaired = true;
                LogDebug("플레이어 이름 복구");
            }
            
            if (data.level < 1)
            {
                data.level = 1;
                repaired = true;
                LogDebug("레벨 복구");
            }
            
            if (data.experience < 0)
            {
                data.experience = 0;
                repaired = true;
                LogDebug("경험치 복구");
            }
            
            if (data.coins < 0)
            {
                data.coins = 0;
                repaired = true;
                LogDebug("코인 복구");
            }
            
            if (data.gems < 0)
            {
                data.gems = 0;
                repaired = true;
                LogDebug("젬 복구");
            }
            
            if (data.totalPlayTime < 0)
            {
                data.totalPlayTime = 0;
                repaired = true;
                LogDebug("총 플레이 시간 복구");
            }
            
            // GameStats 복구
            if (data.gameStats == null)
            {
                data.gameStats = new GameStats();
                repaired = true;
                LogDebug("GameStats 객체 복구");
            }
            else
            {
                if (data.gameStats.totalGames < 0)
                {
                    data.gameStats.totalGames = 0;
                    repaired = true;
                    LogDebug("총 게임 수 복구");
                }
                
                if (data.gameStats.wins < 0)
                {
                    data.gameStats.wins = 0;
                    repaired = true;
                    LogDebug("승리 수 복구");
                }
                
                if (data.gameStats.losses < 0)
                {
                    data.gameStats.losses = 0;
                    repaired = true;
                    LogDebug("패배 수 복구");
                }
                
                if (data.gameStats.highestScore < 0)
                {
                    data.gameStats.highestScore = 0;
                    repaired = true;
                    LogDebug("최고 점수 복구");
                }
                
                // 버전 4 필드 복구
                if (data.gameStats.averageScore < 0)
                {
                    data.gameStats.averageScore = 0;
                    repaired = true;
                    LogDebug("평균 점수 복구");
                }
                
                if (data.gameStats.totalPlayTime < 0)
                {
                    data.gameStats.totalPlayTime = 0;
                    repaired = true;
                    LogDebug("게임 통계 총 플레이 시간 복구");
                }
            }
            
            // 업적 배열 복구
            if (data.achievements == null)
            {
                data.achievements = new string[0];
                repaired = true;
                LogDebug("업적 배열 복구");
            }
            
            // 논리적 일관성 복구
            if (data.gameStats != null && 
                data.gameStats.totalGames != (data.gameStats.wins + data.gameStats.losses))
            {
                data.gameStats.totalGames = data.gameStats.wins + data.gameStats.losses;
                repaired = true;
                LogDebug("게임 통계 일관성 복구");
            }
            
            if (repaired)
            {
                LogDebug("데이터 복구 완료");
                OnDataRepaired?.Invoke(data);
            }
            else
            {
                LogDebug("복구할 데이터가 없습니다");
            }
            
            return true;
        }
        catch (Exception e)
        {
            LogError($"데이터 복구 중 오류 발생: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 백업 생성
    /// </summary>
    private void CreateBackup()
    {
        try
        {
            if (File.Exists(DataFilePath))
            {
                File.Copy(DataFilePath, BackupFilePath, true);
                LogDebug("백업 생성 완료");
            }
        }
        catch (Exception e)
        {
            LogError("백업 생성 실패: " + e.Message);
        }
    }
    
    /// <summary>
    /// 백업에서 복구 시도
    /// </summary>
    private bool TryRestoreFromBackup()
    {
        try
        {
            if (!File.Exists(BackupFilePath))
            {
                LogDebug("백업 파일이 존재하지 않음");
                return false;
            }
            
            string backupJson = File.ReadAllText(BackupFilePath);
            UserData backupData = JsonUtility.FromJson<UserData>(backupJson);
            
            if (ValidateData(backupData))
            {
                CurrentUserData = backupData;
                LogDebug("백업에서 복구 성공");
                OnDataLoaded?.Invoke(CurrentUserData);
                return true;
            }
            else
            {
                LogError("백업 데이터도 유효하지 않음");
                return false;
            }
        }
        catch (Exception e)
        {
            LogError("백업 복구 실패: " + e.Message);
            return false;
        }
    }
    
    /// <summary>
    /// 디버그 로그 출력
    /// </summary>
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            GameLogger.LogDebug("[UserDataManager] " + message);
        }
    }
    
    /// <summary>
    /// 에러 로그 출력
    /// </summary>
    private void LogError(string message)
    {
        GameLogger.LogError("[UserDataManager] " + message);
    }
    
    /// <summary>
    /// 현재 데이터 상태 출력 (디버그용)
    /// </summary>
    [ContextMenu("Print Current Data")]
    public void PrintCurrentData()
    {
        if (CurrentUserData != null)
        {
            string json = JsonUtility.ToJson(CurrentUserData, true);
            Debug.Log("[UserDataManager] Current Data:\n" + json);
        }
        else
        {
            Debug.Log("[UserDataManager] No data loaded");
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            LogDebug("앱 일시정지 - 데이터 저장");
            SaveData();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            LogDebug("앱 포커스 해제 - 데이터 저장");
            SaveData();
        }
    }
    
    private void OnDestroy()
    {
        LogDebug("앱 종료 - 데이터 저장");
        SaveData();
    }
} 