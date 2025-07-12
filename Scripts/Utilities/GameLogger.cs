using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// 게임 로그 시스템
/// </summary>
public static class GameLogger
{
    [System.Serializable]
    public enum LogType
    {
        Info,
        Warning,
        Error,
        Debug
    }
    
    [System.Serializable]
    public class LogEntry
    {
        public string timestamp;
        public LogType type;
        public string message;
        public string stackTrace;
        
        public LogEntry(LogType type, string message, string stackTrace = "")
        {
            this.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            this.type = type;
            this.message = message;
            this.stackTrace = stackTrace;
        }
        
        public override string ToString()
        {
            return $"[{timestamp}] [{type}] {message}";
        }
    }
    
    // 로그 설정
    private static readonly int MAX_LOG_ENTRIES = 1000;
    private static readonly int MAX_LOG_FILE_SIZE = 5 * 1024 * 1024; // 5MB
    private static readonly string LOG_FILE_NAME = "game_log.txt";
    private static readonly string LOG_FILE_PATH = Path.Combine(Application.persistentDataPath, LOG_FILE_NAME);
    
    // 로그 저장소
    private static readonly List<LogEntry> logEntries = new List<LogEntry>();
    private static readonly object logLock = new object();
    
    // 로그 레벨 설정
    private static LogType minimumLogLevel = LogType.Info;
    
    /// <summary>
    /// 로그 출력
    /// </summary>
    /// <param name="message">로그 메시지</param>
    /// <param name="type">로그 타입</param>
    public static void Log(string message, LogType type = LogType.Info)
    {
        if (type < minimumLogLevel)
            return;
        
        lock (logLock)
        {
            LogEntry entry = new LogEntry(type, message);
            logEntries.Add(entry);
            
            // 최대 로그 개수 제한
            if (logEntries.Count > MAX_LOG_ENTRIES)
            {
                logEntries.RemoveAt(0);
            }
            
            // Unity 콘솔에 출력
            OutputToUnityConsole(entry);
        }
    }
    
    /// <summary>
    /// 에러 로그
    /// </summary>
    /// <param name="error">에러 메시지</param>
    public static void LogError(string error)
    {
        Log(error, LogType.Error);
    }
    
    /// <summary>
    /// 경고 로그
    /// </summary>
    /// <param name="warning">경고 메시지</param>
    public static void LogWarning(string warning)
    {
        Log(warning, LogType.Warning);
    }
    
    /// <summary>
    /// 정보 로그
    /// </summary>
    /// <param name="info">정보 메시지</param>
    public static void LogInfo(string info)
    {
        Log(info, LogType.Info);
    }
    
    /// <summary>
    /// 디버그 로그 (개발 빌드에서만)
    /// </summary>
    /// <param name="debug">디버그 메시지</param>
    public static void LogDebug(string debug)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Log(debug, LogType.Debug);
        #endif
    }
    
    /// <summary>
    /// 예외 로그
    /// </summary>
    /// <param name="ex">예외 객체</param>
    /// <param name="context">컨텍스트 메시지</param>
    public static void LogException(Exception ex, string context = "")
    {
        string message = string.IsNullOrEmpty(context) ? ex.Message : $"{context}: {ex.Message}";
        LogEntry entry = new LogEntry(LogType.Error, message, ex.StackTrace);
        
        lock (logLock)
        {
            logEntries.Add(entry);
            
            if (logEntries.Count > MAX_LOG_ENTRIES)
            {
                logEntries.RemoveAt(0);
            }
        }
        
        OutputToUnityConsole(entry);
    }
    
    /// <summary>
    /// 로그 파일 저장
    /// </summary>
    /// <returns>저장 성공 여부</returns>
    public static bool SaveLogToFile()
    {
        try
        {
            // 로그 파일 크기 체크
            if (File.Exists(LOG_FILE_PATH))
            {
                FileInfo fileInfo = new FileInfo(LOG_FILE_PATH);
                if (fileInfo.Length > MAX_LOG_FILE_SIZE)
                {
                    File.Delete(LOG_FILE_PATH);
                    LogInfo("로그 파일이 너무 커서 삭제되었습니다");
                }
            }
            
            // 디렉토리 생성
            string directory = Path.GetDirectoryName(LOG_FILE_PATH);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // 로그 파일 작성
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== Game Log ===");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Unity Version: {Application.unityVersion}");
            sb.AppendLine($"Platform: {Application.platform}");
            sb.AppendLine($"Device: {SystemInfo.deviceModel}");
            sb.AppendLine($"OS: {SystemInfo.operatingSystem}");
            sb.AppendLine("================\n");
            
            lock (logLock)
            {
                foreach (LogEntry entry in logEntries)
                {
                    sb.AppendLine(entry.ToString());
                    if (!string.IsNullOrEmpty(entry.stackTrace))
                    {
                        sb.AppendLine($"StackTrace: {entry.stackTrace}");
                    }
                    sb.AppendLine();
                }
            }
            
            File.WriteAllText(LOG_FILE_PATH, sb.ToString());
            LogInfo($"로그 파일 저장 완료: {LOG_FILE_PATH}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameLogger] 로그 파일 저장 실패: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 로그 초기화
    /// </summary>
    public static void ClearLogs()
    {
        lock (logLock)
        {
            logEntries.Clear();
            LogInfo("로그가 초기화되었습니다");
        }
    }
    
    /// <summary>
    /// 로그 파일 경로 반환
    /// </summary>
    /// <returns>로그 파일 경로</returns>
    public static string GetLogFilePath()
    {
        return LOG_FILE_PATH;
    }
    
    /// <summary>
    /// 로그 개수 반환
    /// </summary>
    /// <returns>현재 로그 개수</returns>
    public static int GetLogCount()
    {
        lock (logLock)
        {
            return logEntries.Count;
        }
    }
    
    /// <summary>
    /// 최소 로그 레벨 설정
    /// </summary>
    /// <param name="level">최소 로그 레벨</param>
    public static void SetMinimumLogLevel(LogType level)
    {
        minimumLogLevel = level;
        LogInfo($"최소 로그 레벨이 {level}로 설정되었습니다");
    }
    
    /// <summary>
    /// 특정 타입의 로그 필터링
    /// </summary>
    /// <param name="type">로그 타입</param>
    /// <returns>필터링된 로그 목록</returns>
    public static List<LogEntry> GetLogsByType(LogType type)
    {
        List<LogEntry> filteredLogs = new List<LogEntry>();
        
        lock (logLock)
        {
            foreach (LogEntry entry in logEntries)
            {
                if (entry.type == type)
                {
                    filteredLogs.Add(entry);
                }
            }
        }
        
        return filteredLogs;
    }
    
    /// <summary>
    /// 최근 로그 가져오기
    /// </summary>
    /// <param name="count">가져올 로그 개수</param>
    /// <returns>최근 로그 목록</returns>
    public static List<LogEntry> GetRecentLogs(int count = 10)
    {
        List<LogEntry> recentLogs = new List<LogEntry>();
        
        lock (logLock)
        {
            int startIndex = Math.Max(0, logEntries.Count - count);
            for (int i = startIndex; i < logEntries.Count; i++)
            {
                recentLogs.Add(logEntries[i]);
            }
        }
        
        return recentLogs;
    }
    
    /// <summary>
    /// Unity 콘솔에 출력
    /// </summary>
    /// <param name="entry">로그 엔트리</param>
    private static void OutputToUnityConsole(LogEntry entry)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // 개발 빌드에서는 모든 로그 출력
        switch (entry.type)
        {
            case LogType.Error:
                Debug.LogError($"[GameLogger] {entry.message}");
                break;
            case LogType.Warning:
                Debug.LogWarning($"[GameLogger] {entry.message}");
                break;
            case LogType.Debug:
                Debug.Log($"[GameLogger] [DEBUG] {entry.message}");
                break;
            default:
                Debug.Log($"[GameLogger] {entry.message}");
                break;
        }
        #else
        // 릴리스 빌드에서는 에러만 출력
        if (entry.type == LogType.Error)
        {
            Debug.LogError($"[GameLogger] {entry.message}");
        }
        #endif
    }
    
    /// <summary>
    /// 로그 통계 정보 출력
    /// </summary>
    public static void PrintLogStatistics()
    {
        lock (logLock)
        {
            int totalLogs = logEntries.Count;
            int errorCount = 0;
            int warningCount = 0;
            int infoCount = 0;
            int debugCount = 0;
            
            foreach (LogEntry entry in logEntries)
            {
                switch (entry.type)
                {
                    case LogType.Error:
                        errorCount++;
                        break;
                    case LogType.Warning:
                        warningCount++;
                        break;
                    case LogType.Info:
                        infoCount++;
                        break;
                    case LogType.Debug:
                        debugCount++;
                        break;
                }
            }
            
            string stats = $"[GameLogger] 로그 통계:\n" +
                         $"  - 총 로그: {totalLogs}\n" +
                         $"  - 에러: {errorCount}\n" +
                         $"  - 경고: {warningCount}\n" +
                         $"  - 정보: {infoCount}\n" +
                         $"  - 디버그: {debugCount}";
            
            Debug.Log(stats);
        }
    }
    
    /// <summary>
    /// 앱 종료 시 로그 저장
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void OnApplicationQuit()
    {
        SaveLogToFile();
    }
} 