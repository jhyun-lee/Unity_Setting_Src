using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    [Header("Data Settings")]
    public bool autoSave = true;
    public float autoSaveInterval = 30f; // 자동 저장 간격 (초)
    
    [Header("Current Data")]
    public SettingsData settingsData;
    public UserData userData;
    
    // 이벤트
    public static event Action OnDataLoaded;
    public static event Action OnDataSaved;
    public static event Action OnSettingsChanged;
    public static event Action OnUserDataChanged;
    
    // 파일 경로
    private string settingsFilePath;
    private string userDataFilePath;
    private string backupFilePath;
    
    // 자동 저장 타이머
    private float autoSaveTimer = 0f;
    private bool dataChanged = false;
    
    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDataManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadAllData();
    }
    
    private void Update()
    {
        // 자동 저장 처리
        if (autoSave && dataChanged)
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                SaveAllData();
                autoSaveTimer = 0f;
                dataChanged = false;
            }
        }
    }
    
    private void InitializeDataManager()
    {
        // 파일 경로 설정
        settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
        userDataFilePath = Path.Combine(Application.persistentDataPath, "userdata.json");
        backupFilePath = Path.Combine(Application.persistentDataPath, "backup.json");
        
        // 데이터 초기화
        settingsData = new SettingsData();
        userData = new UserData();
    }
    
    #region Settings Data Management
    
    public void LoadSettings()
    {
        try
        {
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                settingsData = JsonUtility.FromJson<SettingsData>(json);
                settingsData.ValidateVolumeSettings();
                Debug.Log("설정 데이터 로드 완료");
            }
            else
            {
                settingsData = new SettingsData();
                SaveSettings();
                Debug.Log("새로운 설정 데이터 생성");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"설정 데이터 로드 실패: {e.Message}");
            settingsData = new SettingsData();
        }
    }
    
    public void SaveSettings()
    {
        try
        {
            settingsData.ValidateVolumeSettings();
            string json = JsonUtility.ToJson(settingsData, true);
            File.WriteAllText(settingsFilePath, json);
            Debug.Log("설정 데이터 저장 완료");
            OnSettingsChanged?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"설정 데이터 저장 실패: {e.Message}");
        }
    }
    
    public void ResetSettings()
    {
        settingsData.ResetToDefaults();
        SaveSettings();
        Debug.Log("설정이 기본값으로 초기화되었습니다.");
    }
    
    #endregion
    
    #region User Data Management
    
    public void LoadUserData()
    {
        try
        {
            if (File.Exists(userDataFilePath))
            {
                string json = File.ReadAllText(userDataFilePath);
                userData = JsonUtility.FromJson<UserData>(json);
                userData.ValidateData();
                Debug.Log("사용자 데이터 로드 완료");
            }
            else
            {
                userData = UserData.CreateNewUser(System.Guid.NewGuid().ToString());
                SaveUserData();
                Debug.Log("새로운 사용자 데이터 생성");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"사용자 데이터 로드 실패: {e.Message}");
            userData = UserData.CreateNewUser(System.Guid.NewGuid().ToString());
        }
    }
    
    public void SaveUserData()
    {
        try
        {
            userData.ValidateData();
            userData.dataLastModifiedTime = DateTime.Now;
            string json = JsonUtility.ToJson(userData, true);
            File.WriteAllText(userDataFilePath, json);
            Debug.Log("사용자 데이터 저장 완료");
            OnUserDataChanged?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"사용자 데이터 저장 실패: {e.Message}");
        }
    }
    
    public void ResetUserData()
    {
        userData.ResetUserData();
        SaveUserData();
        Debug.Log("사용자 데이터가 초기화되었습니다.");
    }
    
    public void CreateBackup()
    {
        try
        {
            string backupJson = JsonUtility.ToJson(userData, true);
            File.WriteAllText(backupFilePath, backupJson);
            Debug.Log("백업 데이터 생성 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"백업 생성 실패: {e.Message}");
        }
    }
    
    public bool RestoreFromBackup()
    {
        try
        {
            if (File.Exists(backupFilePath))
            {
                string json = File.ReadAllText(backupFilePath);
                userData = JsonUtility.FromJson<UserData>(json);
                userData.ValidateData();
                SaveUserData();
                Debug.Log("백업에서 데이터 복원 완료");
                return true;
            }
            else
            {
                Debug.LogWarning("백업 파일이 존재하지 않습니다.");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"백업 복원 실패: {e.Message}");
            return false;
        }
    }
    
    #endregion
    
    #region Combined Data Management
    
    public void LoadAllData()
    {
        LoadSettings();
        LoadUserData();
        OnDataLoaded?.Invoke();
    }
    
    public void SaveAllData()
    {
        SaveSettings();
        SaveUserData();
        OnDataSaved?.Invoke();
    }
    
    public void MarkDataAsChanged()
    {
        dataChanged = true;
    }
    
    #endregion
    
    #region Utility Methods
    
    public bool HasValidUserData()
    {
        return userData != null && !string.IsNullOrEmpty(userData.userId);
    }
    
    public bool HasValidSettings()
    {
        return settingsData != null;
    }
    
    public void DeleteAllData()
    {
        try
        {
            if (File.Exists(settingsFilePath))
                File.Delete(settingsFilePath);
            if (File.Exists(userDataFilePath))
                File.Delete(userDataFilePath);
            if (File.Exists(backupFilePath))
                File.Delete(backupFilePath);
            
            settingsData = new SettingsData();
            userData = new UserData();
            
            Debug.Log("모든 데이터가 삭제되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 삭제 실패: {e.Message}");
        }
    }
    
    public string GetDataInfo()
    {
        if (userData == null) return "데이터 없음";
        
        return $"사용자: {userData.nickname}\n" +
               $"레벨: {userData.currentLevel}\n" +
               $"코인: {userData.totalCoins}\n" +
               $"최고점수: {userData.highScore}\n" +
               $"마지막 수정: {userData.dataLastModifiedTime}";
    }
    
    #endregion
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveAllData();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveAllData();
        }
    }
    
    private void OnApplicationQuit()
    {
        SaveAllData();
    }
} 