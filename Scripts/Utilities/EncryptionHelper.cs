using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 사용자 데이터 보안을 위한 간단한 암호화 시스템
/// </summary>
public static class EncryptionHelper
{
    private static readonly string SALT = "UnityGame2024";
    private static readonly int KEY_SIZE = 32; // 256-bit
    private static readonly int IV_SIZE = 16;  // 128-bit
    private static readonly string ENCRYPTION_PREFIX = "ENC:";
    
    /// <summary>
    /// 문자열 암호화
    /// </summary>
    /// <param name="plainText">암호화할 텍스트</param>
    /// <returns>암호화된 텍스트 (실패 시 원본 반환)</returns>
    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            Debug.LogWarning("[EncryptionHelper] 암호화할 텍스트가 비어있습니다");
            return plainText;
        }
        
        try
        {
            // 이미 암호화된 텍스트인지 확인
            if (IsEncrypted(plainText))
            {
                Debug.LogWarning("[EncryptionHelper] 이미 암호화된 텍스트입니다");
                return plainText;
            }
            
            byte[] key = GenerateKey();
            byte[] iv = GenerateIV();
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    
                    // IV와 암호화된 데이터를 결합
                    byte[] combined = new byte[iv.Length + encryptedBytes.Length];
                    Array.Copy(iv, 0, combined, 0, iv.Length);
                    Array.Copy(encryptedBytes, 0, combined, iv.Length, encryptedBytes.Length);
                    
                    string encryptedText = Convert.ToBase64String(combined);
                    string result = ENCRYPTION_PREFIX + encryptedText;
                    
                    Debug.Log($"[EncryptionHelper] 암호화 성공: {plainText.Length} → {result.Length} bytes");
                    return result;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[EncryptionHelper] 암호화 실패: {e.Message}");
            return plainText; // 실패 시 원본 반환
        }
    }
    
    /// <summary>
    /// 문자열 복호화
    /// </summary>
    /// <param name="encryptedText">복호화할 텍스트</param>
    /// <returns>복호화된 텍스트 (실패 시 원본 반환)</returns>
    public static string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            Debug.LogWarning("[EncryptionHelper] 복호화할 텍스트가 비어있습니다");
            return encryptedText;
        }
        
        try
        {
            // 암호화된 텍스트인지 확인
            if (!IsEncrypted(encryptedText))
            {
                Debug.LogWarning("[EncryptionHelper] 암호화되지 않은 텍스트입니다");
                return encryptedText;
            }
            
            // 암호화 접두사 제거
            string encryptedData = encryptedText.Substring(ENCRYPTION_PREFIX.Length);
            byte[] combined = Convert.FromBase64String(encryptedData);
            
            // IV와 암호화된 데이터 분리
            byte[] iv = new byte[IV_SIZE];
            byte[] encryptedBytes = new byte[combined.Length - IV_SIZE];
            Array.Copy(combined, 0, iv, 0, IV_SIZE);
            Array.Copy(combined, IV_SIZE, encryptedBytes, 0, encryptedBytes.Length);
            
            byte[] key = GenerateKey();
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                    
                    Debug.Log($"[EncryptionHelper] 복호화 성공: {encryptedText.Length} → {decryptedText.Length} bytes");
                    return decryptedText;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[EncryptionHelper] 복호화 실패: {e.Message}");
            return encryptedText; // 실패 시 원본 반환
        }
    }
    
    /// <summary>
    /// 암호화 키 생성
    /// </summary>
    /// <returns>암호화 키 (32바이트)</returns>
    public static byte[] GenerateKey()
    {
        try
        {
            // 디바이스 고유 정보를 기반으로 키 생성
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            string deviceModel = SystemInfo.deviceModel;
            string osVersion = SystemInfo.operatingSystem;
            
            // 고유 문자열 생성
            string uniqueString = $"{deviceId}_{deviceModel}_{osVersion}_{SALT}";
            
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(uniqueString));
                
                // 32바이트 키 생성
                byte[] key = new byte[KEY_SIZE];
                Array.Copy(hash, key, Math.Min(hash.Length, KEY_SIZE));
                
                return key;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[EncryptionHelper] 키 생성 실패: {e.Message}");
            
            // 폴백: 기본 키 반환
            byte[] fallbackKey = new byte[KEY_SIZE];
            for (int i = 0; i < KEY_SIZE; i++)
            {
                fallbackKey[i] = (byte)(i * 7 + 13);
            }
            return fallbackKey;
        }
    }
    
    /// <summary>
    /// 초기화 벡터(IV) 생성
    /// </summary>
    /// <returns>초기화 벡터 (16바이트)</returns>
    private static byte[] GenerateIV()
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[EncryptionHelper] IV 생성 실패: {e.Message}");
            
            // 폴백: 기본 IV 반환
            byte[] fallbackIV = new byte[IV_SIZE];
            for (int i = 0; i < IV_SIZE; i++)
            {
                fallbackIV[i] = (byte)(i * 11 + 17);
            }
            return fallbackIV;
        }
    }
    
    /// <summary>
    /// 암호화된 텍스트인지 확인
    /// </summary>
    /// <param name="text">확인할 텍스트</param>
    /// <returns>암호화된 텍스트 여부</returns>
    public static bool IsEncrypted(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;
        
        return text.StartsWith(ENCRYPTION_PREFIX);
    }
    
    /// <summary>
    /// 암호화 키 정보 출력 (디버그용)
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void PrintKeyInfo()
    {
        try
        {
            byte[] key = GenerateKey();
            string keyHex = BitConverter.ToString(key).Replace("-", "");
            
            Debug.Log($"[EncryptionHelper] 키 정보:");
            Debug.Log($"  - 키 길이: {key.Length} bytes");
            Debug.Log($"  - 키 (Hex): {keyHex}");
            Debug.Log($"  - 디바이스 ID: {SystemInfo.deviceUniqueIdentifier}");
            Debug.Log($"  - 디바이스 모델: {SystemInfo.deviceModel}");
            Debug.Log($"  - OS 버전: {SystemInfo.operatingSystem}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[EncryptionHelper] 키 정보 출력 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 암호화 테스트 (디버그용)
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void TestEncryption()
    {
        try
        {
            string originalText = "Hello, World! This is a test message for encryption.";
            
            Debug.Log($"[EncryptionHelper] 암호화 테스트 시작");
            Debug.Log($"  - 원본 텍스트: {originalText}");
            
            string encrypted = Encrypt(originalText);
            Debug.Log($"  - 암호화된 텍스트: {encrypted}");
            
            string decrypted = Decrypt(encrypted);
            Debug.Log($"  - 복호화된 텍스트: {decrypted}");
            
            bool success = originalText.Equals(decrypted);
            Debug.Log($"  - 테스트 결과: {(success ? "성공" : "실패")}");
            
            if (!success)
            {
                Debug.LogError("암호화/복호화 테스트 실패!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[EncryptionHelper] 암호화 테스트 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 암호화 강도 테스트
    /// </summary>
    /// <param name="dataSize">테스트할 데이터 크기 (KB)</param>
    /// <returns>성능 정보</returns>
    public static string PerformanceTest(int dataSize = 1)
    {
        try
        {
            // 테스트 데이터 생성
            string testData = GenerateTestData(dataSize * 1024);
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // 암호화 성능 테스트
            stopwatch.Restart();
            string encrypted = Encrypt(testData);
            long encryptTime = stopwatch.ElapsedMilliseconds;
            
            // 복호화 성능 테스트
            stopwatch.Restart();
            string decrypted = Decrypt(encrypted);
            long decryptTime = stopwatch.ElapsedMilliseconds;
            
            // 검증
            bool isValid = testData.Equals(decrypted);
            
            string result = $"[EncryptionHelper] 성능 테스트 결과:\n" +
                          $"  - 데이터 크기: {dataSize} KB\n" +
                          $"  - 암호화 시간: {encryptTime} ms\n" +
                          $"  - 복호화 시간: {decryptTime} ms\n" +
                          $"  - 검증 결과: {(isValid ? "성공" : "실패")}";
            
            Debug.Log(result);
            return result;
        }
        catch (Exception e)
        {
            string error = $"[EncryptionHelper] 성능 테스트 실패: {e.Message}";
            Debug.LogError(error);
            return error;
        }
    }
    
    /// <summary>
    /// 테스트 데이터 생성
    /// </summary>
    /// <param name="size">데이터 크기 (바이트)</param>
    /// <returns>테스트 데이터</returns>
    private static string GenerateTestData(int size)
    {
        StringBuilder sb = new StringBuilder();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();
        
        for (int i = 0; i < size; i++)
        {
            sb.Append(chars[random.Next(chars.Length)]);
        }
        
        return sb.ToString();
    }
} 