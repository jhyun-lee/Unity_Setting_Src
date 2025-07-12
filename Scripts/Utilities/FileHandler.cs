using UnityEngine;
using System;
using System.IO;

/// <summary>
/// 안전한 파일 저장/로드를 위한 유틸리티 클래스
/// </summary>
public static class FileHandler
{
    /// <summary>
    /// 파일 저장
    /// </summary>
    /// <param name="fileName">파일명</param>
    /// <param name="jsonData">저장할 JSON 데이터</param>
    /// <returns>저장 성공 여부</returns>
    public static bool SaveToFile(string fileName, string jsonData)
    {
        try
        {
            string filePath = GetFilePath(fileName);
            
            // 디렉토리가 없으면 생성
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Debug.Log($"[FileHandler] 디렉토리 생성: {directory}");
            }
            
            // 백업 생성
            CreateBackup(fileName);
            
            // 파일 저장
            File.WriteAllText(filePath, jsonData);
            
            Debug.Log($"[FileHandler] 파일 저장 성공: {filePath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 파일 저장 실패 ({fileName}): {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 파일 로드
    /// </summary>
    /// <param name="fileName">파일명</param>
    /// <returns>로드된 데이터 (실패 시 null)</returns>
    public static string LoadFromFile(string fileName)
    {
        try
        {
            string filePath = GetFilePath(fileName);
            
            if (!File.Exists(filePath))
            {
                Debug.Log($"[FileHandler] 파일이 존재하지 않음: {filePath}");
                return null;
            }
            
            string data = File.ReadAllText(filePath);
            Debug.Log($"[FileHandler] 파일 로드 성공: {filePath}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 파일 로드 실패 ({fileName}): {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 파일 존재 확인
    /// </summary>
    /// <param name="fileName">파일명</param>
    /// <returns>파일 존재 여부</returns>
    public static bool FileExists(string fileName)
    {
        try
        {
            string filePath = GetFilePath(fileName);
            bool exists = File.Exists(filePath);
            
            Debug.Log($"[FileHandler] 파일 존재 확인 ({fileName}): {exists}");
            return exists;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 파일 존재 확인 실패 ({fileName}): {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 파일 삭제
    /// </summary>
    /// <param name="fileName">파일명</param>
    /// <returns>삭제 성공 여부</returns>
    public static bool DeleteFile(string fileName)
    {
        try
        {
            string filePath = GetFilePath(fileName);
            
            if (!File.Exists(filePath))
            {
                Debug.Log($"[FileHandler] 삭제할 파일이 존재하지 않음: {filePath}");
                return true; // 이미 없으면 성공으로 처리
            }
            
            File.Delete(filePath);
            Debug.Log($"[FileHandler] 파일 삭제 성공: {filePath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 파일 삭제 실패 ({fileName}): {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 백업 파일 생성
    /// </summary>
    /// <param name="fileName">원본 파일명</param>
    /// <returns>백업 생성 성공 여부</returns>
    public static bool CreateBackup(string fileName)
    {
        try
        {
            string filePath = GetFilePath(fileName);
            string backupPath = GetBackupFilePath(fileName);
            
            if (!File.Exists(filePath))
            {
                Debug.Log($"[FileHandler] 백업할 파일이 존재하지 않음: {filePath}");
                return true; // 원본이 없으면 백업 성공으로 처리
            }
            
            // 백업 디렉토리 생성
            string backupDirectory = Path.GetDirectoryName(backupPath);
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }
            
            // 백업 파일 생성
            File.Copy(filePath, backupPath, true);
            Debug.Log($"[FileHandler] 백업 파일 생성 성공: {backupPath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 백업 파일 생성 실패 ({fileName}): {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 백업에서 복구
    /// </summary>
    /// <param name="fileName">복구할 파일명</param>
    /// <returns>복구 성공 여부</returns>
    public static bool RestoreFromBackup(string fileName)
    {
        try
        {
            string filePath = GetFilePath(fileName);
            string backupPath = GetBackupFilePath(fileName);
            
            if (!File.Exists(backupPath))
            {
                Debug.Log($"[FileHandler] 백업 파일이 존재하지 않음: {backupPath}");
                return false;
            }
            
            // 백업에서 복구
            File.Copy(backupPath, filePath, true);
            Debug.Log($"[FileHandler] 백업에서 복구 성공: {filePath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 백업에서 복구 실패 ({fileName}): {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 백업 파일 존재 확인
    /// </summary>
    /// <param name="fileName">원본 파일명</param>
    /// <returns>백업 파일 존재 여부</returns>
    public static bool BackupExists(string fileName)
    {
        try
        {
            string backupPath = GetBackupFilePath(fileName);
            bool exists = File.Exists(backupPath);
            
            Debug.Log($"[FileHandler] 백업 파일 존재 확인 ({fileName}): {exists}");
            return exists;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 백업 파일 존재 확인 실패 ({fileName}): {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 백업 파일 삭제
    /// </summary>
    /// <param name="fileName">원본 파일명</param>
    /// <returns>삭제 성공 여부</returns>
    public static bool DeleteBackup(string fileName)
    {
        try
        {
            string backupPath = GetBackupFilePath(fileName);
            
            if (!File.Exists(backupPath))
            {
                Debug.Log($"[FileHandler] 삭제할 백업 파일이 존재하지 않음: {backupPath}");
                return true; // 이미 없으면 성공으로 처리
            }
            
            File.Delete(backupPath);
            Debug.Log($"[FileHandler] 백업 파일 삭제 성공: {backupPath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 백업 파일 삭제 실패 ({fileName}): {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 파일 크기 확인
    /// </summary>
    /// <param name="fileName">파일명</param>
    /// <returns>파일 크기 (바이트, 실패 시 -1)</returns>
    public static long GetFileSize(string fileName)
    {
        try
        {
            string filePath = GetFilePath(fileName);
            
            if (!File.Exists(filePath))
            {
                Debug.Log($"[FileHandler] 파일이 존재하지 않음: {filePath}");
                return -1;
            }
            
            FileInfo fileInfo = new FileInfo(filePath);
            long size = fileInfo.Length;
            
            Debug.Log($"[FileHandler] 파일 크기 ({fileName}): {size} bytes");
            return size;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 파일 크기 확인 실패 ({fileName}): {e.Message}");
            return -1;
        }
    }
    
    /// <summary>
    /// 파일 수정 시간 확인
    /// </summary>
    /// <param name="fileName">파일명</param>
    /// <returns>파일 수정 시간 (실패 시 DateTime.MinValue)</returns>
    public static DateTime GetFileModifiedTime(string fileName)
    {
        try
        {
            string filePath = GetFilePath(fileName);
            
            if (!File.Exists(filePath))
            {
                Debug.Log($"[FileHandler] 파일이 존재하지 않음: {filePath}");
                return DateTime.MinValue;
            }
            
            FileInfo fileInfo = new FileInfo(filePath);
            DateTime modifiedTime = fileInfo.LastWriteTime;
            
            Debug.Log($"[FileHandler] 파일 수정 시간 ({fileName}): {modifiedTime}");
            return modifiedTime;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 파일 수정 시간 확인 실패 ({fileName}): {e.Message}");
            return DateTime.MinValue;
        }
    }
    
    /// <summary>
    /// 파일 경로 생성
    /// </summary>
    /// <param name="fileName">파일명</param>
    /// <returns>전체 파일 경로</returns>
    private static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
    
    /// <summary>
    /// 백업 파일 경로 생성
    /// </summary>
    /// <param name="fileName">원본 파일명</param>
    /// <returns>백업 파일 경로</returns>
    private static string GetBackupFilePath(string fileName)
    {
        string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);
        string backupFileName = $"{nameWithoutExt}_backup{extension}";
        
        return Path.Combine(Application.persistentDataPath, backupFileName);
    }
    
    /// <summary>
    /// 디렉토리 내 모든 파일 삭제
    /// </summary>
    /// <param name="directoryName">디렉토리명 (null이면 persistentDataPath)</param>
    /// <returns>삭제 성공 여부</returns>
    public static bool ClearDirectory(string directoryName = null)
    {
        try
        {
            string directoryPath = string.IsNullOrEmpty(directoryName) 
                ? Application.persistentDataPath 
                : Path.Combine(Application.persistentDataPath, directoryName);
            
            if (!Directory.Exists(directoryPath))
            {
                Debug.Log($"[FileHandler] 삭제할 디렉토리가 존재하지 않음: {directoryPath}");
                return true;
            }
            
            string[] files = Directory.GetFiles(directoryPath);
            int deletedCount = 0;
            
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                    deletedCount++;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[FileHandler] 파일 삭제 실패: {file} - {e.Message}");
                }
            }
            
            Debug.Log($"[FileHandler] 디렉토리 정리 완료: {directoryPath} ({deletedCount}개 파일 삭제)");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 디렉토리 정리 실패 ({directoryName}): {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 사용 가능한 디스크 공간 확인 (MB 단위)
    /// </summary>
    /// <returns>사용 가능한 공간 (MB, 실패 시 -1)</returns>
    public static long GetAvailableDiskSpace()
    {
        try
        {
            string path = Application.persistentDataPath;
            DriveInfo drive = new DriveInfo(Path.GetPathRoot(path));
            long availableSpace = drive.AvailableFreeSpace / (1024 * 1024); // MB 단위
            
            Debug.Log($"[FileHandler] 사용 가능한 디스크 공간: {availableSpace} MB");
            return availableSpace;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileHandler] 디스크 공간 확인 실패: {e.Message}");
            return -1;
        }
    }
} 