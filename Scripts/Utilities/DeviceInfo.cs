using UnityEngine;
using System;

/// <summary>
/// 디바이스 성능 등급
/// </summary>
public enum DevicePerformance
{
    Low,    // 저사양
    Medium, // 중사양
    High    // 고사양
}

/// <summary>
/// 디바이스 정보 및 최적화 시스템
/// </summary>
public static class DeviceInfo
{
    [System.Serializable]
    public class DeviceData
    {
        public string deviceId;
        public string deviceModel;
        public string deviceName;
        public string operatingSystem;
        public string processorType;
        public int processorCount;
        public int systemMemorySize;
        public string graphicsDeviceName;
        public int graphicsMemorySize;
        public int screenWidth;
        public int screenHeight;
        public float screenDpi;
        public DevicePerformance performance;
        public bool isLowEndDevice;
        public bool isMidEndDevice;
        public bool isHighEndDevice;
        public DateTime collectedAt;
    }
    
    [System.Serializable]
    public class OptimizationSettings
    {
        public int targetFrameRate;
        public int qualityLevel;
        public bool enableShadows;
        public bool enableParticles;
        public bool enablePostProcessing;
        public bool enableAntiAliasing;
        public int textureQuality;
        public int anisotropicFiltering;
        public bool enableVSync;
        public float audioQuality;
        public bool enableRealTimeReflections;
        public bool enableSoftParticles;
        public bool enableHDR;
        public bool enableBloom;
        public bool enableMotionBlur;
        public bool enableDepthOfField;
    }
    
    // 디바이스 정보 (한 번만 수집)
    private static DeviceData deviceData;
    private static bool isInitialized = false;
    
    // 성능 기준값
    private static readonly int LOW_MEMORY_THRESHOLD = 2048; // 2GB
    private static readonly int HIGH_MEMORY_THRESHOLD = 6144; // 6GB
    private static readonly int LOW_CPU_CORES = 4;
    private static readonly int HIGH_CPU_CORES = 8;
    
    /// <summary>
    /// 디바이스 정보 초기화
    /// </summary>
    public static void Initialize()
    {
        if (isInitialized)
        {
            GameLogger.LogWarning("DeviceInfo가 이미 초기화되었습니다");
            return;
        }
        
        deviceData = CollectDeviceInfo();
        isInitialized = true;
        
        GameLogger.LogInfo("디바이스 정보 수집 완료");
        GameLogger.LogInfo($"디바이스: {deviceData.deviceModel}");
        GameLogger.LogInfo($"성능 등급: {deviceData.performance}");
        GameLogger.LogInfo($"메모리: {deviceData.systemMemorySize}MB");
        GameLogger.LogInfo($"프로세서: {deviceData.processorType} ({deviceData.processorCount}코어)");
    }
    
    /// <summary>
    /// 디바이스 고유 ID 반환
    /// </summary>
    /// <returns>디바이스 고유 ID</returns>
    public static string GetDeviceId()
    {
        EnsureInitialized();
        return deviceData.deviceId;
    }
    
    /// <summary>
    /// 디바이스 정보 수집
    /// </summary>
    /// <returns>디바이스 정보</returns>
    public static DeviceData GetDeviceInfo()
    {
        EnsureInitialized();
        return deviceData;
    }
    
    /// <summary>
    /// 저사양 디바이스 판별
    /// </summary>
    /// <returns>저사양 여부</returns>
    public static bool IsLowEndDevice()
    {
        EnsureInitialized();
        return deviceData.isLowEndDevice;
    }
    
    /// <summary>
    /// 중사양 디바이스 판별
    /// </summary>
    /// <returns>중사양 여부</returns>
    public static bool IsMidEndDevice()
    {
        EnsureInitialized();
        return deviceData.isMidEndDevice;
    }
    
    /// <summary>
    /// 고사양 디바이스 판별
    /// </summary>
    /// <returns>고사양 여부</returns>
    public static bool IsHighEndDevice()
    {
        EnsureInitialized();
        return deviceData.isHighEndDevice;
    }
    
    /// <summary>
    /// 디바이스별 최적화
    /// </summary>
    public static void OptimizeForDevice()
    {
        EnsureInitialized();
        
        GameLogger.LogInfo($"디바이스 최적화 시작: {deviceData.performance}");
        
        OptimizationSettings settings = GetRecommendedSettings();
        ApplyOptimizationSettings(settings);
        
        GameLogger.LogInfo("디바이스 최적화 완료");
    }
    
    /// <summary>
    /// 권장 설정 반환
    /// </summary>
    /// <returns>최적화 설정</returns>
    public static OptimizationSettings GetRecommendedSettings()
    {
        EnsureInitialized();
        
        OptimizationSettings settings = new OptimizationSettings();
        
        switch (deviceData.performance)
        {
            case DevicePerformance.Low:
                settings = GetLowEndSettings();
                break;
                
            case DevicePerformance.Medium:
                settings = GetMidEndSettings();
                break;
                
            case DevicePerformance.High:
                settings = GetHighEndSettings();
                break;
        }
        
        return settings;
    }
    
    /// <summary>
    /// 디바이스 정보 수집
    /// </summary>
    /// <returns>수집된 디바이스 정보</returns>
    private static DeviceData CollectDeviceInfo()
    {
        DeviceData data = new DeviceData
        {
            deviceId = SystemInfo.deviceUniqueIdentifier,
            deviceModel = SystemInfo.deviceModel,
            deviceName = SystemInfo.deviceName,
            operatingSystem = SystemInfo.operatingSystem,
            processorType = SystemInfo.processorType,
            processorCount = SystemInfo.processorCount,
            systemMemorySize = SystemInfo.systemMemorySize,
            graphicsDeviceName = SystemInfo.graphicsDeviceName,
            graphicsMemorySize = SystemInfo.graphicsMemorySize,
            screenWidth = Screen.width,
            screenHeight = Screen.height,
            screenDpi = Screen.dpi,
            collectedAt = DateTime.Now
        };
        
        // 성능 등급 판별
        data.performance = DeterminePerformance(data);
        
        // 성능별 플래그 설정
        data.isLowEndDevice = data.performance == DevicePerformance.Low;
        data.isMidEndDevice = data.performance == DevicePerformance.Medium;
        data.isHighEndDevice = data.performance == DevicePerformance.High;
        
        return data;
    }
    
    /// <summary>
    /// 성능 등급 판별
    /// </summary>
    /// <param name="data">디바이스 정보</param>
    /// <returns>성능 등급</returns>
    private static DevicePerformance DeterminePerformance(DeviceData data)
    {
        int score = 0;
        
        // 메모리 점수 (최대 40점)
        if (data.systemMemorySize >= HIGH_MEMORY_THRESHOLD)
            score += 40;
        else if (data.systemMemorySize >= LOW_MEMORY_THRESHOLD)
            score += 20;
        else
            score += 10;
        
        // 프로세서 점수 (최대 30점)
        if (data.processorCount >= HIGH_CPU_CORES)
            score += 30;
        else if (data.processorCount >= LOW_CPU_CORES)
            score += 20;
        else
            score += 10;
        
        // 그래픽 메모리 점수 (최대 20점)
        if (data.graphicsMemorySize >= 4096)
            score += 20;
        else if (data.graphicsMemorySize >= 2048)
            score += 15;
        else if (data.graphicsMemorySize >= 1024)
            score += 10;
        else
            score += 5;
        
        // 해상도 점수 (최대 10점)
        int totalPixels = data.screenWidth * data.screenHeight;
        if (totalPixels >= 2073600) // 1920x1080 이상
            score += 10;
        else if (totalPixels >= 921600) // 1280x720 이상
            score += 7;
        else
            score += 5;
        
        // 성능 등급 결정
        if (score >= 80)
            return DevicePerformance.High;
        else if (score >= 50)
            return DevicePerformance.Medium;
        else
            return DevicePerformance.Low;
    }
    
    /// <summary>
    /// 저사양 설정
    /// </summary>
    /// <returns>저사양 최적화 설정</returns>
    private static OptimizationSettings GetLowEndSettings()
    {
        return new OptimizationSettings
        {
            targetFrameRate = 30,
            qualityLevel = 0, // Low
            enableShadows = false,
            enableParticles = false,
            enablePostProcessing = false,
            enableAntiAliasing = false,
            textureQuality = 0, // Full Res
            anisotropicFiltering = 0, // Disabled
            enableVSync = false,
            audioQuality = 0.5f,
            enableRealTimeReflections = false,
            enableSoftParticles = false,
            enableHDR = false,
            enableBloom = false,
            enableMotionBlur = false,
            enableDepthOfField = false
        };
    }
    
    /// <summary>
    /// 중사양 설정
    /// </summary>
    /// <returns>중사양 최적화 설정</returns>
    private static OptimizationSettings GetMidEndSettings()
    {
        return new OptimizationSettings
        {
            targetFrameRate = 60,
            qualityLevel = 2, // Medium
            enableShadows = true,
            enableParticles = true,
            enablePostProcessing = true,
            enableAntiAliasing = true,
            textureQuality = 1, // Half Res
            anisotropicFiltering = 1, // Per Texture
            enableVSync = true,
            audioQuality = 0.8f,
            enableRealTimeReflections = false,
            enableSoftParticles = false,
            enableHDR = true,
            enableBloom = true,
            enableMotionBlur = false,
            enableDepthOfField = false
        };
    }
    
    /// <summary>
    /// 고사양 설정
    /// </summary>
    /// <returns>고사양 최적화 설정</returns>
    private static OptimizationSettings GetHighEndSettings()
    {
        return new OptimizationSettings
        {
            targetFrameRate = 60,
            qualityLevel = 4, // High
            enableShadows = true,
            enableParticles = true,
            enablePostProcessing = true,
            enableAntiAliasing = true,
            textureQuality = 2, // Quarter Res
            anisotropicFiltering = 2, // Force Enable
            enableVSync = true,
            audioQuality = 1.0f,
            enableRealTimeReflections = true,
            enableSoftParticles = true,
            enableHDR = true,
            enableBloom = true,
            enableMotionBlur = true,
            enableDepthOfField = true
        };
    }
    
    /// <summary>
    /// 최적화 설정 적용
    /// </summary>
    /// <param name="settings">최적화 설정</param>
    private static void ApplyOptimizationSettings(OptimizationSettings settings)
    {
        try
        {
            // 프레임레이트 설정
            Application.targetFrameRate = settings.targetFrameRate;
            
            // 품질 레벨 설정
            QualitySettings.SetQualityLevel(settings.qualityLevel, true);
            
            // 그림자 설정
            QualitySettings.shadows = settings.enableShadows ? ShadowQuality.All : ShadowQuality.Disable;
            
            // 안티앨리어싱 설정
            QualitySettings.antiAliasing = settings.enableAntiAliasing ? 4 : 0;
            
            // 텍스처 품질 설정
            QualitySettings.masterTextureLimit = settings.textureQuality;
            
            // 이방성 필터링 설정
            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)settings.anisotropicFiltering;
            
            // VSync 설정
            QualitySettings.vSyncCount = settings.enableVSync ? 1 : 0;
            
            // 오디오 품질 설정
            AudioConfiguration config = AudioSettings.GetConfiguration();
            config.sampleRate = settings.audioQuality >= 1.0f ? 48000 : 22050;
            AudioSettings.Reset(config);
            
            // 파티클 시스템 설정
            SetParticleSettings(settings.enableParticles);
            
            // 포스트 프로세싱 설정
            SetPostProcessingSettings(settings);
            
            GameLogger.LogInfo("최적화 설정 적용 완료");
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, "최적화 설정 적용 실패");
        }
    }
    
    /// <summary>
    /// 파티클 시스템 설정
    /// </summary>
    /// <param name="enable">활성화 여부</param>
    private static void SetParticleSettings(bool enable)
    {
        // 모든 파티클 시스템 찾기
        ParticleSystem[] particleSystems = FindObjectsOfType<ParticleSystem>();
        
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null)
            {
                var main = ps.main;
                main.playOnAwake = enable;
                
                if (!enable)
                {
                    ps.Stop();
                }
            }
        }
    }
    
    /// <summary>
    /// 포스트 프로세싱 설정
    /// </summary>
    /// <param name="settings">최적화 설정</param>
    private static void SetPostProcessingSettings(OptimizationSettings settings)
    {
        // TODO: Post Processing Stack과 연동
        // 실제 구현에서는 Post Processing Stack의 Volume 컴포넌트를 찾아서 설정
        
        GameLogger.LogInfo("포스트 프로세싱 설정 적용");
    }
    
    /// <summary>
    /// 초기화 확인
    /// </summary>
    private static void EnsureInitialized()
    {
        if (!isInitialized)
        {
            Initialize();
        }
    }
    
    /// <summary>
    /// 디바이스 정보 출력
    /// </summary>
    public static void PrintDeviceInfo()
    {
        EnsureInitialized();
        
        string info = $"[DeviceInfo] 디바이스 정보:\n" +
                     $"  - 디바이스 ID: {deviceData.deviceId}\n" +
                     $"  - 디바이스 모델: {deviceData.deviceModel}\n" +
                     $"  - 디바이스 이름: {deviceData.deviceName}\n" +
                     $"  - OS: {deviceData.operatingSystem}\n" +
                     $"  - 프로세서: {deviceData.processorType}\n" +
                     $"  - 코어 수: {deviceData.processorCount}\n" +
                     $"  - 시스템 메모리: {deviceData.systemMemorySize}MB\n" +
                     $"  - 그래픽 카드: {deviceData.graphicsDeviceName}\n" +
                     $"  - 그래픽 메모리: {deviceData.graphicsMemorySize}MB\n" +
                     $"  - 화면 해상도: {deviceData.screenWidth}x{deviceData.screenHeight}\n" +
                     $"  - DPI: {deviceData.screenDpi}\n" +
                     $"  - 성능 등급: {deviceData.performance}\n" +
                     $"  - 수집 시간: {deviceData.collectedAt}";
        
        GameLogger.LogInfo(info);
    }
    
    /// <summary>
    /// 성능 점수 계산
    /// </summary>
    /// <returns>성능 점수 (0-100)</returns>
    public static int GetPerformanceScore()
    {
        EnsureInitialized();
        
        int score = 0;
        
        // 메모리 점수 (최대 40점)
        if (deviceData.systemMemorySize >= HIGH_MEMORY_THRESHOLD)
            score += 40;
        else if (deviceData.systemMemorySize >= LOW_MEMORY_THRESHOLD)
            score += 20;
        else
            score += 10;
        
        // 프로세서 점수 (최대 30점)
        if (deviceData.processorCount >= HIGH_CPU_CORES)
            score += 30;
        else if (deviceData.processorCount >= LOW_CPU_CORES)
            score += 20;
        else
            score += 10;
        
        // 그래픽 메모리 점수 (최대 20점)
        if (deviceData.graphicsMemorySize >= 4096)
            score += 20;
        else if (deviceData.graphicsMemorySize >= 2048)
            score += 15;
        else if (deviceData.graphicsMemorySize >= 1024)
            score += 10;
        else
            score += 5;
        
        // 해상도 점수 (최대 10점)
        int totalPixels = deviceData.screenWidth * deviceData.screenHeight;
        if (totalPixels >= 2073600)
            score += 10;
        else if (totalPixels >= 921600)
            score += 7;
        else
            score += 5;
        
        return score;
    }
    
    /// <summary>
    /// 배터리 상태 확인
    /// </summary>
    /// <returns>배터리 정보</returns>
    public static string GetBatteryInfo()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject batteryManager = context.Call<AndroidJavaObject>("getSystemService", "batterymanager");
            
            int level = batteryManager.Call<int>("getIntProperty", 2); // BATTERY_PROPERTY_CAPACITY
            int scale = batteryManager.Call<int>("getIntProperty", 1); // BATTERY_PROPERTY_CHARGE_COUNTER
            
            float batteryLevel = (float)level / scale * 100f;
            
            return $"배터리: {batteryLevel:F1}%";
        }
        catch (Exception ex)
        {
            GameLogger.LogException(ex, "배터리 정보 수집 실패");
            return "배터리 정보 없음";
        }
        #else
        return "배터리 정보 (에디터)";
        #endif
    }
    
    /// <summary>
    /// 네트워크 상태 확인
    /// </summary>
    /// <returns>네트워크 정보</returns>
    public static string GetNetworkInfo()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return "네트워크: 연결 없음";
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            return "네트워크: 모바일 데이터";
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            return "네트워크: Wi-Fi";
        }
        else
        {
            return "네트워크: 알 수 없음";
        }
    }
    
    /// <summary>
    /// 권장 설정 정보 출력
    /// </summary>
    public static void PrintRecommendedSettings()
    {
        EnsureInitialized();
        
        OptimizationSettings settings = GetRecommendedSettings();
        
        string info = $"[DeviceInfo] 권장 설정 ({deviceData.performance}):\n" +
                     $"  - 목표 프레임레이트: {settings.targetFrameRate}fps\n" +
                     $"  - 품질 레벨: {settings.qualityLevel}\n" +
                     $"  - 그림자: {(settings.enableShadows ? "활성화" : "비활성화")}\n" +
                     $"  - 파티클: {(settings.enableParticles ? "활성화" : "비활성화")}\n" +
                     $"  - 포스트 프로세싱: {(settings.enablePostProcessing ? "활성화" : "비활성화")}\n" +
                     $"  - 안티앨리어싱: {(settings.enableAntiAliasing ? "활성화" : "비활성화")}\n" +
                     $"  - 텍스처 품질: {settings.textureQuality}\n" +
                     $"  - 이방성 필터링: {settings.anisotropicFiltering}\n" +
                     $"  - VSync: {(settings.enableVSync ? "활성화" : "비활성화")}\n" +
                     $"  - 오디오 품질: {settings.audioQuality:F1}";
        
        GameLogger.LogInfo(info);
    }
} 