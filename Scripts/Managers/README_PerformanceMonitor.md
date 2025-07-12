# PerformanceMonitor 사용 가이드

## 개요
`PerformanceMonitor`는 게임 성능을 실시간으로 모니터링하고 필요시 자동으로 최적화하는 싱글톤 매니저입니다.

## 주요 기능

### 1. 실시간 성능 모니터링
- **FPS (초당 프레임 수)**: 실시간 FPS 측정
- **메모리 사용량**: 메모리 사용률 및 사용량 모니터링
- **배터리 사용량**: 배터리 레벨 모니터링
- **CPU 사용률**: CPU 사용률 추정
- **GPU 사용률**: GPU 사용률 추정
- **온도**: 디바이스 온도 추정

### 2. 자동 최적화
- **FPS 30 이하**: 품질 레벨 하향 조정
- **메모리 사용량 80% 이상**: 리소스 해제
- **배터리 20% 이하**: 절약 모드 활성화
- **과열 감지**: 프레임레이트 제한

### 3. 성능 데이터 관리
- 순환 버퍼로 1분간의 성능 데이터 저장
- 평균 성능 데이터 계산
- 성능 히스토리 관리

## 성능 임계값

```csharp
public static class PerformanceThresholds
{
    public const float LOW_FPS = 30f;           // 낮은 FPS 임계값
    public const float HIGH_MEMORY = 0.8f;      // 높은 메모리 사용률 임계값
    public const float LOW_BATTERY = 0.2f;      // 낮은 배터리 임계값
    public const float HIGH_CPU = 0.9f;         // 높은 CPU 사용률 임계값
    public const float HIGH_GPU = 0.9f;         // 높은 GPU 사용률 임계값
    public const float OVERHEAT_TEMPERATURE = 60f; // 과열 온도 임계값
}
```

## 이벤트 시스템

### 사용 가능한 이벤트
```csharp
// 성능 저하 감지 이벤트
public static event Action<PerformanceData> OnPerformanceWarning;

// 자동 최적화 실행 이벤트
public static event Action<string> OnAutoOptimization;

// 성능 데이터 업데이트 이벤트
public static event Action<PerformanceData> OnPerformanceDataUpdated;
```

### 이벤트 구독 예제
```csharp
private void Start()
{
    // 이벤트 구독
    PerformanceMonitor.OnPerformanceDataUpdated += OnPerformanceDataUpdated;
    PerformanceMonitor.OnPerformanceWarning += OnPerformanceWarning;
    PerformanceMonitor.OnAutoOptimization += OnAutoOptimization;
}

private void OnDestroy()
{
    // 이벤트 구독 해제
    PerformanceMonitor.OnPerformanceDataUpdated -= OnPerformanceDataUpdated;
    PerformanceMonitor.OnPerformanceWarning -= OnPerformanceWarning;
    PerformanceMonitor.OnAutoOptimization -= OnAutoOptimization;
}

private void OnPerformanceDataUpdated(PerformanceData data)
{
    Debug.Log($"FPS: {data.fps}, 메모리: {data.memoryUsage:P1}");
}

private void OnPerformanceWarning(PerformanceData data)
{
    Debug.LogWarning($"성능 경고: FPS={data.fps}, 메모리={data.memoryUsage:P1}");
}
```

## 주요 메서드

### 1. 모니터링 제어
```csharp
// 모니터링 시작
PerformanceMonitor.Instance.StartMonitoring();

// 모니터링 중지
PerformanceMonitor.Instance.StopMonitoring();

// 성능 체크
PerformanceData data = PerformanceMonitor.Instance.CheckPerformance();
```

### 2. 성능 데이터 조회
```csharp
// 현재 FPS 반환
float fps = PerformanceMonitor.Instance.GetCurrentFPS();

// 메모리 사용량 반환 (MB)
float memoryMB = PerformanceMonitor.Instance.GetMemoryUsage();

// 메모리 사용률 반환 (0-1)
float memoryRatio = PerformanceMonitor.Instance.GetMemoryUsageRatio();

// 배터리 레벨 반환 (0-1)
float batteryLevel = PerformanceMonitor.Instance.GetBatteryLevel();
```

### 3. 자동 최적화
```csharp
// 자동 최적화 실행
PerformanceMonitor.Instance.AutoOptimize();

// 최적화 상태 초기화
PerformanceMonitor.Instance.ResetOptimization();
```

### 4. 성능 데이터 히스토리
```csharp
// 성능 데이터 히스토리 반환
List<PerformanceData> history = PerformanceMonitor.Instance.GetPerformanceHistory();

// 평균 성능 데이터 반환
PerformanceData avgData = PerformanceMonitor.Instance.GetAveragePerformance();
```

## 자동 최적화 규칙

### 1. FPS 최적화
- **레벨 1**: 목표 FPS 45, 품질 레벨 1
- **레벨 2**: 목표 FPS 30, 품질 레벨 0
- **레벨 3+**: 목표 FPS 15, 품질 레벨 0, vSync 비활성화

### 2. 메모리 최적화
- 불필요한 리소스 해제 (`Resources.UnloadUnusedAssets()`)
- 가비지 컬렉션 강제 실행 (`GC.Collect()`)
- 캐시 정리 (`Caching.CleanCache()`)

### 3. 배터리 최적화
- 프레임레이트 30fps로 제한
- 품질 설정 최소화
- 오디오 볼륨 50%로 낮춤

### 4. 온도 최적화
- 프레임레이트 20fps로 제한
- 품질 설정 최소화
- 오디오 볼륨 30%로 낮춤

## 설정 옵션

### Inspector에서 설정 가능한 옵션
- **Enable Monitoring**: 모니터링 활성화 여부
- **Monitoring Interval**: 모니터링 간격 (초)
- **Data Buffer Size**: 성능 데이터 버퍼 크기
- **Enable Auto Optimization**: 자동 최적화 활성화 여부
- **Show Performance UI**: 성능 UI 표시 여부

### 최적화 설정
- **Enable FPS Optimization**: FPS 최적화 활성화 여부
- **Enable Memory Optimization**: 메모리 최적화 활성화 여부
- **Enable Battery Optimization**: 배터리 최적화 활성화 여부
- **Enable Thermal Optimization**: 온도 최적화 활성화 여부

## 성능 데이터 구조

```csharp
[System.Serializable]
public class PerformanceData
{
    public float fps;              // FPS
    public float memoryUsage;      // 메모리 사용률 (0-1)
    public float batteryLevel;     // 배터리 레벨 (0-1)
    public float cpuUsage;         // CPU 사용률 (0-1)
    public float gpuUsage;         // GPU 사용률 (0-1)
    public float temperature;      // 온도 (섭씨)
    public long timestamp;         // 타임스탬프
    public string deviceInfo;      // 디바이스 정보
}
```

## 사용 예제

### 기본 사용법
```csharp
public class GameController : MonoBehaviour
{
    private void Start()
    {
        // 모니터링 시작
        PerformanceMonitor.Instance.StartMonitoring();
        
        // 이벤트 구독
        PerformanceMonitor.OnPerformanceWarning += OnPerformanceWarning;
    }
    
    private void OnPerformanceWarning(PerformanceData data)
    {
        if (data.fps < 30)
        {
            Debug.LogWarning("FPS가 낮습니다!");
        }
        
        if (data.memoryUsage > 0.8f)
        {
            Debug.LogWarning("메모리 사용량이 높습니다!");
        }
    }
}
```

### 수동 최적화
```csharp
public class PerformanceController : MonoBehaviour
{
    public void ManualOptimize()
    {
        PerformanceMonitor.Instance.AutoOptimize();
    }
    
    public void ResetOptimization()
    {
        PerformanceMonitor.Instance.ResetOptimization();
    }
    
    public void CheckPerformance()
    {
        var data = PerformanceMonitor.Instance.CheckPerformance();
        Debug.Log($"현재 성능: FPS={data.fps}, 메모리={data.memoryUsage:P1}");
    }
}
```

### 성능 히스토리 분석
```csharp
public class PerformanceAnalyzer : MonoBehaviour
{
    public void AnalyzePerformance()
    {
        var history = PerformanceMonitor.Instance.GetPerformanceHistory();
        var avgData = PerformanceMonitor.Instance.GetAveragePerformance();
        
        Debug.Log($"평균 FPS: {avgData.fps:F1}");
        Debug.Log($"평균 메모리 사용률: {avgData.memoryUsage:P1}");
        Debug.Log($"데이터 포인트 수: {history.Count}");
    }
}
```

## UI 연동

### PerformanceUI 사용
```csharp
// UI 컴포넌트 참조
[SerializeField] private PerformanceUI performanceUI;

private void Start()
{
    // UI 표시 설정
    performanceUI.SetUIVisibility(true, true, false);
    
    // 업데이트 간격 설정
    performanceUI.SetUpdateInterval(0.5f);
    
    // 색상 설정
    performanceUI.SetColors(Color.green, Color.yellow, Color.red);
}
```

## 디버그 기능

### OnGUI 디버그 정보
- 실시간 성능 데이터 표시
- 최적화 레벨 표시
- 성능 경고 상태 표시
- 수동 최적화 버튼

### 로그 출력
- 모든 성능 이벤트에 대한 상세 로그
- 최적화 적용/해제 로그
- 성능 경고 로그

## 주의사항

### 1. 성능 측정 정확도
- CPU/GPU 사용률은 추정값입니다
- 온도는 디바이스별로 다를 수 있습니다
- 실제 구현에서는 디바이스별 API 사용을 권장합니다

### 2. 최적화 쿨다운
- 자동 최적화는 30초 쿨다운이 있습니다
- 연속적인 최적화를 방지합니다

### 3. 메모리 사용량
- 성능 데이터 버퍼는 메모리를 사용합니다
- 기본값은 1분간의 데이터를 저장합니다

### 4. 배터리 영향
- 지속적인 모니터링은 배터리를 소모할 수 있습니다
- 필요시 모니터링을 중지하는 것을 권장합니다

## 연동 시스템

### 기존 매니저들과의 연동
- **AppLifecycleManager**: 앱 생명주기에 따른 모니터링 제어
- **AudioManager**: 배터리 절약 시 오디오 볼륨 조절
- **DeviceInfo**: 디바이스별 최적화 설정
- **GameLogger**: 성능 이벤트 로깅

### 에러 처리
- **ErrorHandler**: 성능 관련 에러 처리
- **GameLogger**: 상세한 성능 로그 출력

이 시스템을 통해 게임의 성능을 실시간으로 모니터링하고, 필요시 자동으로 최적화하여 안정적인 게임 플레이를 보장할 수 있습니다. 