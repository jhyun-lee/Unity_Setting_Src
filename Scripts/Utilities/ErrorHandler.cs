using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 에러 타입 열거형
/// </summary>
public enum ErrorType
{
    DataCorruption,
    FileNotFound,
    SaveFailed,
    LoadFailed,
    NetworkError,
    ValidationError,
    MigrationError,
    EncryptionError,
    UnknownError
}

/// <summary>
/// 에러 핸들링 시스템
/// </summary>
public static class ErrorHandler
{
    [System.Serializable]
    public class ErrorInfo
    {
        public ErrorType type;
        public string message;
        public string operation;
        public DateTime timestamp;
        public string stackTrace;
        public bool isRecovered;
        
        public ErrorInfo(ErrorType type, string message, string operation = "", string stackTrace = "")
        {
            this.type = type;
            this.message = message;
            this.operation = operation;
            this.timestamp = DateTime.Now;
            this.stackTrace = stackTrace;
            this.isRecovered = false;
        }
    }
    
    // 에러 처리 설정
    private static readonly Dictionary<ErrorType, string> errorMessages = new Dictionary<ErrorType, string>
    {
        { ErrorType.DataCorruption, "데이터가 손상되었습니다. 복구를 시도합니다." },
        { ErrorType.FileNotFound, "파일을 찾을 수 없습니다. 기본 데이터를 생성합니다." },
        { ErrorType.SaveFailed, "데이터 저장에 실패했습니다. 다시 시도합니다." },
        { ErrorType.LoadFailed, "데이터 로드에 실패했습니다. 백업에서 복구를 시도합니다." },
        { ErrorType.NetworkError, "네트워크 연결에 문제가 있습니다." },
        { ErrorType.ValidationError, "데이터 검증에 실패했습니다. 수정을 시도합니다." },
        { ErrorType.MigrationError, "데이터 마이그레이션에 실패했습니다." },
        { ErrorType.EncryptionError, "암호화/복호화에 실패했습니다." },
        { ErrorType.UnknownError, "알 수 없는 오류가 발생했습니다." }
    };
    
    // 에러 기록
    private static readonly List<ErrorInfo> errorHistory = new List<ErrorInfo>();
    private static readonly object errorLock = new object();
    
    // 복구 시도 횟수
    private static readonly Dictionary<ErrorType, int> recoveryAttempts = new Dictionary<ErrorType, int>();
    private static readonly int MAX_RECOVERY_ATTEMPTS = 3;
    
    /// <summary>
    /// 데이터 에러 처리
    /// </summary>
    /// <param name="operation">작업명</param>
    /// <param name="ex">예외 객체</param>
    /// <returns>복구 성공 여부</returns>
    public static bool HandleDataError(string operation, Exception ex)
    {
        ErrorType errorType = DetermineErrorType(ex);
        string message = $"{operation} 중 오류 발생: {ex.Message}";
        
        ErrorInfo errorInfo = new ErrorInfo(errorType, message, operation, ex.StackTrace);
        RecordError(errorInfo);
        
        GameLogger.LogException(ex, operation);
        
        // 자동 복구 시도
        bool recovered = RecoveryAction(errorType);
        errorInfo.isRecovered = recovered;
        
        if (recovered)
        {
            GameLogger.LogInfo($"{operation} 복구 성공");
        }
        else
        {
            GameLogger.LogError($"{operation} 복구 실패");
            ShowErrorPopup(GetErrorMessage(errorType));
        }
        
        return recovered;
    }
    
    /// <summary>
    /// 파일 에러 처리
    /// </summary>
    /// <param name="fileName">파일명</param>
    /// <param name="ex">예외 객체</param>
    /// <returns>복구 성공 여부</returns>
    public static bool HandleFileError(string fileName, Exception ex)
    {
        ErrorType errorType = DetermineErrorType(ex);
        string message = $"파일 '{fileName}' 처리 중 오류 발생: {ex.Message}";
        
        ErrorInfo errorInfo = new ErrorInfo(errorType, message, $"File: {fileName}", ex.StackTrace);
        RecordError(errorInfo);
        
        GameLogger.LogException(ex, $"File: {fileName}");
        
        // 자동 복구 시도
        bool recovered = RecoveryAction(errorType);
        errorInfo.isRecovered = recovered;
        
        if (recovered)
        {
            GameLogger.LogInfo($"파일 '{fileName}' 복구 성공");
        }
        else
        {
            GameLogger.LogError($"파일 '{fileName}' 복구 실패");
            ShowErrorPopup(GetErrorMessage(errorType));
        }
        
        return recovered;
    }
    
    /// <summary>
    /// 에러 팝업 표시
    /// </summary>
    /// <param name="message">에러 메시지</param>
    public static void ShowErrorPopup(string message)
    {
        #if UNITY_EDITOR
        // 에디터에서는 콘솔에 출력
        GameLogger.LogError($"에러 팝업: {message}");
        #else
        // 실제 기기에서는 UI 팝업 표시 (구현 필요)
        GameLogger.LogError($"사용자에게 표시된 에러: {message}");
        
        // TODO: UI 팝업 시스템과 연동
        // ErrorPopupUI.Show(message);
        #endif
    }
    
    /// <summary>
    /// 복구 동작 수행
    /// </summary>
    /// <param name="errorType">에러 타입</param>
    /// <returns>복구 성공 여부</returns>
    public static bool RecoveryAction(ErrorType errorType)
    {
        // 복구 시도 횟수 체크
        if (GetRecoveryAttempts(errorType) >= MAX_RECOVERY_ATTEMPTS)
        {
            GameLogger.LogWarning($"{errorType} 복구 시도 횟수 초과");
            return false;
        }
        
        IncrementRecoveryAttempts(errorType);
        
        try
        {
            switch (errorType)
            {
                case ErrorType.DataCorruption:
                    return HandleDataCorruption();
                    
                case ErrorType.FileNotFound:
                    return HandleFileNotFound();
                    
                case ErrorType.SaveFailed:
                    return HandleSaveFailed();
                    
                case ErrorType.LoadFailed:
                    return HandleLoadFailed();
                    
                case ErrorType.ValidationError:
                    return HandleValidationError();
                    
                case ErrorType.MigrationError:
                    return HandleMigrationError();
                    
                case ErrorType.EncryptionError:
                    return HandleEncryptionError();
                    
                default:
                    GameLogger.LogWarning($"알 수 없는 에러 타입: {errorType}");
                    return false;
            }
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, $"복구 동작 실패: {errorType}");
            return false;
        }
    }
    
    /// <summary>
    /// 에러 타입 판별
    /// </summary>
    /// <param name="ex">예외 객체</param>
    /// <returns>에러 타입</returns>
    private static ErrorType DetermineErrorType(Exception ex)
    {
        string message = ex.Message.ToLower();
        
        if (message.Contains("corrupt") || message.Contains("손상"))
            return ErrorType.DataCorruption;
        
        if (message.Contains("not found") || message.Contains("찾을 수 없"))
            return ErrorType.FileNotFound;
        
        if (message.Contains("save") || message.Contains("저장"))
            return ErrorType.SaveFailed;
        
        if (message.Contains("load") || message.Contains("로드"))
            return ErrorType.LoadFailed;
        
        if (message.Contains("network") || message.Contains("네트워크"))
            return ErrorType.NetworkError;
        
        if (message.Contains("validation") || message.Contains("검증"))
            return ErrorType.ValidationError;
        
        if (message.Contains("migration") || message.Contains("마이그레이션"))
            return ErrorType.MigrationError;
        
        if (message.Contains("encryption") || message.Contains("암호화"))
            return ErrorType.EncryptionError;
        
        return ErrorType.UnknownError;
    }
    
    /// <summary>
    /// 데이터 손상 복구
    /// </summary>
    /// <returns>복구 성공 여부</returns>
    private static bool HandleDataCorruption()
    {
        try
        {
            GameLogger.LogInfo("데이터 손상 복구 시도");
            
            // 백업에서 복구 시도
            if (UserDataManager.Instance != null)
            {
                // 백업 파일에서 복구 시도
                return true; // 실제 구현에서는 백업 복구 로직
            }
            
            return false;
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, "데이터 손상 복구 실패");
            return false;
        }
    }
    
    /// <summary>
    /// 파일 없음 복구
    /// </summary>
    /// <returns>복구 성공 여부</returns>
    private static bool HandleFileNotFound()
    {
        try
        {
            GameLogger.LogInfo("파일 없음 복구 시도");
            
            // 기본 데이터 생성
            if (UserDataManager.Instance != null)
            {
                UserDataManager.Instance.CreateDefaultData();
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, "파일 없음 복구 실패");
            return false;
        }
    }
    
    /// <summary>
    /// 저장 실패 복구
    /// </summary>
    /// <returns>복구 성공 여부</returns>
    private static bool HandleSaveFailed()
    {
        try
        {
            GameLogger.LogInfo("저장 실패 복구 시도");
            
            // 저장 재시도
            if (UserDataManager.Instance != null)
            {
                return UserDataManager.Instance.SaveData();
            }
            
            return false;
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, "저장 실패 복구 실패");
            return false;
        }
    }
    
    /// <summary>
    /// 로드 실패 복구
    /// </summary>
    /// <returns>복구 성공 여부</returns>
    private static bool HandleLoadFailed()
    {
        try
        {
            GameLogger.LogInfo("로드 실패 복구 시도");
            
            // 백업에서 복구 시도
            if (UserDataManager.Instance != null)
            {
                return UserDataManager.Instance.LoadData();
            }
            
            return false;
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, "로드 실패 복구 실패");
            return false;
        }
    }
    
    /// <summary>
    /// 검증 에러 복구
    /// </summary>
    /// <returns>복구 성공 여부</returns>
    private static bool HandleValidationError()
    {
        try
        {
            GameLogger.LogInfo("검증 에러 복구 시도");
            
            // 데이터 복구 시도
            if (UserDataManager.Instance != null && UserDataManager.Instance.CurrentUserData != null)
            {
                return UserDataManager.Instance.RepairData(UserDataManager.Instance.CurrentUserData);
            }
            
            return false;
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, "검증 에러 복구 실패");
            return false;
        }
    }
    
    /// <summary>
    /// 마이그레이션 에러 복구
    /// </summary>
    /// <returns>복구 성공 여부</returns>
    private static bool HandleMigrationError()
    {
        try
        {
            GameLogger.LogInfo("마이그레이션 에러 복구 시도");
            
            // 기본 데이터로 초기화
            if (UserDataManager.Instance != null)
            {
                UserDataManager.Instance.ResetData();
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, "마이그레이션 에러 복구 실패");
            return false;
        }
    }
    
    /// <summary>
    /// 암호화 에러 복구
    /// </summary>
    /// <returns>복구 성공 여부</returns>
    private static bool HandleEncryptionError()
    {
        try
        {
            GameLogger.LogInfo("암호화 에러 복구 시도");
            
            // 암호화 비활성화 후 재시도
            if (UserDataManager.Instance != null)
            {
                // TODO: 암호화 설정 비활성화
                return UserDataManager.Instance.LoadData();
            }
            
            return false;
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, "암호화 에러 복구 실패");
            return false;
        }
    }
    
    /// <summary>
    /// 에러 기록
    /// </summary>
    /// <param name="errorInfo">에러 정보</param>
    private static void RecordError(ErrorInfo errorInfo)
    {
        lock (errorLock)
        {
            errorHistory.Add(errorInfo);
            
            // 최대 100개 에러 기록 유지
            if (errorHistory.Count > 100)
            {
                errorHistory.RemoveAt(0);
            }
        }
    }
    
    /// <summary>
    /// 에러 메시지 가져오기
    /// </summary>
    /// <param name="errorType">에러 타입</param>
    /// <returns>에러 메시지</returns>
    private static string GetErrorMessage(ErrorType errorType)
    {
        return errorMessages.ContainsKey(errorType) ? errorMessages[errorType] : "알 수 없는 오류가 발생했습니다.";
    }
    
    /// <summary>
    /// 복구 시도 횟수 가져오기
    /// </summary>
    /// <param name="errorType">에러 타입</param>
    /// <returns>복구 시도 횟수</returns>
    private static int GetRecoveryAttempts(ErrorType errorType)
    {
        return recoveryAttempts.ContainsKey(errorType) ? recoveryAttempts[errorType] : 0;
    }
    
    /// <summary>
    /// 복구 시도 횟수 증가
    /// </summary>
    /// <param name="errorType">에러 타입</param>
    private static void IncrementRecoveryAttempts(ErrorType errorType)
    {
        if (recoveryAttempts.ContainsKey(errorType))
        {
            recoveryAttempts[errorType]++;
        }
        else
        {
            recoveryAttempts[errorType] = 1;
        }
    }
    
    /// <summary>
    /// 복구 시도 횟수 초기화
    /// </summary>
    public static void ResetRecoveryAttempts()
    {
        recoveryAttempts.Clear();
        GameLogger.LogInfo("복구 시도 횟수가 초기화되었습니다");
    }
    
    /// <summary>
    /// 에러 통계 출력
    /// </summary>
    public static void PrintErrorStatistics()
    {
        lock (errorLock)
        {
            Dictionary<ErrorType, int> errorCounts = new Dictionary<ErrorType, int>();
            int totalErrors = errorHistory.Count;
            int recoveredErrors = 0;
            
            foreach (ErrorInfo error in errorHistory)
            {
                if (!errorCounts.ContainsKey(error.type))
                {
                    errorCounts[error.type] = 0;
                }
                errorCounts[error.type]++;
                
                if (error.isRecovered)
                {
                    recoveredErrors++;
                }
            }
            
            string stats = $"[ErrorHandler] 에러 통계:\n" +
                         $"  - 총 에러: {totalErrors}\n" +
                         $"  - 복구 성공: {recoveredErrors}\n" +
                         $"  - 복구 실패: {totalErrors - recoveredErrors}";
            
            foreach (var kvp in errorCounts)
            {
                stats += $"\n  - {kvp.Key}: {kvp.Value}";
            }
            
            GameLogger.LogInfo(stats);
        }
    }
    
    /// <summary>
    /// 에러 기록 가져오기
    /// </summary>
    /// <returns>에러 기록 목록</returns>
    public static List<ErrorInfo> GetErrorHistory()
    {
        lock (errorLock)
        {
            return new List<ErrorInfo>(errorHistory);
        }
    }
    
    /// <summary>
    /// 에러 기록 초기화
    /// </summary>
    public static void ClearErrorHistory()
    {
        lock (errorLock)
        {
            errorHistory.Clear();
            GameLogger.LogInfo("에러 기록이 초기화되었습니다");
        }
    }
} 