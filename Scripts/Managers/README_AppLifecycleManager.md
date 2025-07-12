# AppLifecycleManager 사용 가이드

## 개요
`AppLifecycleManager`는 모바일 앱의 생명주기를 관리하고 적절한 시점에 데이터를 저장하는 싱글톤 매니저입니다.

## 주요 기능

### 1. 앱 생명주기 관리
- **앱 일시정지/재개**: `OnApplicationPause(bool pauseStatus)`
- **앱 포커스 변경**: `OnApplicationFocus(bool hasFocus)`
- **앱 종료**: `OnApplicationQuit()`
- **메모리 부족**: `OnLowMemory()`
- **배터리 부족**: `OnBatteryLow()`

### 2. 자동 데이터 관리
- 앱 백그라운드 진입 시 자동 저장
- 앱 포그라운드 복귀 시 데이터 복원
- 메모리 부족 시 리소스 정리
- 배터리 절약 모드 자동 적용

### 3. 성능 최적화
- 백그라운드 실행 시 프레임레이트 제한 (30fps)
- 배터리 절약 모드 시 프레임레이트 더욱 제한 (15fps)
- 메모리 사용량 모니터링
- 불필요한 리소스 자동 해제

## 이벤트 시스템

### 사용 가능한 이벤트
```csharp
// 앱 일시정지/재개 이벤트
public static event Action<bool> OnAppPauseChanged;

// 앱 포커스 변경 이벤트
public static event Action<bool> OnAppFocusChanged;

// 앱 종료 이벤트
public static event Action OnAppQuitting;

// 메모리 부족 이벤트
public static event Action OnLowMemory;

// 배터리 부족 이벤트
public static event Action OnBatteryLow;

// 게임 상태 저장 완료 이벤트
public static event Action OnGameStateSaved;

// 게임 상태 복원 완료 이벤트
public static event Action OnGameStateRestored;
```

### 이벤트 구독 예제
```csharp
private void Start()
{
    // 이벤트 구독
    AppLifecycleManager.OnAppPauseChanged += OnAppPauseChanged;
    AppLifecycleManager.OnLowMemory += OnLowMemory;
    AppLifecycleManager.OnGameStateSaved += OnGameStateSaved;
}

private void OnDestroy()
{
    // 이벤트 구독 해제
    AppLifecycleManager.OnAppPauseChanged -= OnAppPauseChanged;
    AppLifecycleManager.OnLowMemory -= OnLowMemory;
    AppLifecycleManager.OnGameStateSaved -= OnGameStateSaved;
}

private void OnAppPauseChanged(bool isPaused)
{
    if (isPaused)
    {
        // 앱이 백그라운드로 진입
        Debug.Log("앱이 백그라운드로 진입했습니다");
    }
    else
    {
        // 앱이 포그라운드로 복귀
        Debug.Log("앱이 포그라운드로 복귀했습니다");
    }
}
```

## 주요 메서드

### 1. 게임 상태 관리
```csharp
// 게임 상태 저장
AppLifecycleManager.Instance.SaveGameState();

// 게임 상태 복원
AppLifecycleManager.Instance.RestoreGameState();

// 빠른 저장 (최소한의 데이터만)
AppLifecycleManager.Instance.QuickSave();
```

### 2. 상태 확인
```csharp
// 앱이 일시정지 상태인지 확인
bool isPaused = AppLifecycleManager.Instance.IsPaused;

// 앱이 포커스를 가지고 있는지 확인
bool hasFocus = AppLifecycleManager.Instance.HasFocus;

// 앱이 종료 중인지 확인
bool isQuitting = AppLifecycleManager.Instance.IsQuitting;
```

### 3. 배터리 부족 처리
```csharp
// 배터리 부족 상황 수동 호출
AppLifecycleManager.Instance.OnBatteryLow();
```

## 설정 옵션

### Inspector에서 설정 가능한 옵션
- **Enable Auto Save**: 자동 저장 활성화 여부
- **Save Delay**: 저장 지연 시간 (초)
- **Enable Battery Optimization**: 배터리 최적화 활성화 여부
- **Enable Memory Optimization**: 메모리 최적화 활성화 여부
- **Enable Debug Logs**: 디버그 로그 출력 여부

## 생명주기 이벤트 처리

### 1. 앱 백그라운드 진입 시
- 음악 정지
- 자동 저장 실행
- 성능 최적화 적용 (프레임레이트 30fps로 제한)

### 2. 앱 포그라운드 복귀 시
- 음악 재개
- 게임 상태 복원
- 성능 설정 복원

### 3. 앱 종료 시
- 최종 데이터 저장
- 모든 매니저들에게 저장 요청

### 4. 메모리 부족 시
- 불필요한 리소스 해제
- 가비지 컬렉션 강제 실행
- 캐시 정리
- 임시 저장 실행

### 5. 배터리 부족 시
- 성능 최적화 (프레임레이트 15fps로 제한)
- 자동 저장 실행

## 사용 예제

### 기본 사용법
```csharp
public class GameController : MonoBehaviour
{
    private void Start()
    {
        // 이벤트 구독
        AppLifecycleManager.OnAppPauseChanged += OnAppPauseChanged;
        AppLifecycleManager.OnLowMemory += OnLowMemory;
    }
    
    private void OnAppPauseChanged(bool isPaused)
    {
        if (isPaused)
        {
            // 게임 일시정지 처리
            PauseGame();
        }
        else
        {
            // 게임 재개 처리
            ResumeGame();
        }
    }
    
    private void OnLowMemory()
    {
        // 메모리 부족 시 처리
        CleanupUnusedResources();
    }
}
```

### 수동 저장/복원
```csharp
public class SaveSystem : MonoBehaviour
{
    public void ManualSave()
    {
        AppLifecycleManager.Instance.SaveGameState();
    }
    
    public void ManualRestore()
    {
        AppLifecycleManager.Instance.RestoreGameState();
    }
    
    public void QuickSave()
    {
        AppLifecycleManager.Instance.QuickSave();
    }
}
```

## 디버그 기능

### OnGUI 디버그 정보
- 앱 일시정지 상태
- 앱 포커스 상태
- 앱 종료 상태
- 저장/복원 상태
- 메모리 사용량

### 로그 출력
- 모든 생명주기 이벤트에 대한 상세 로그
- 성능 최적화 적용/해제 로그
- 저장/복원 상태 로그

## 주의사항

### 1. 초기화 순서
- `AppLifecycleManager`는 `GameInitializer`에서 자동으로 초기화됩니다
- 다른 매니저들보다 먼저 초기화되어야 합니다

### 2. 이벤트 구독 해제
- 반드시 `OnDestroy()`에서 이벤트 구독을 해제해야 합니다
- 메모리 누수를 방지하기 위해 중요합니다

### 3. 성능 고려사항
- 메모리 부족 시 자동으로 리소스 정리가 실행됩니다
- 배터리 절약 모드 시 성능이 크게 저하될 수 있습니다

### 4. 데이터 저장
- 자동 저장은 백그라운드 진입 시에만 실행됩니다
- 중요한 데이터는 수동으로 저장하는 것을 권장합니다

## 연동 시스템

### 기존 매니저들과의 연동
- **UserDataManager**: 사용자 데이터 저장/로드
- **SettingsManager**: 설정 데이터 저장/로드
- **AudioManager**: 오디오 재생/정지
- **AutoSaveManager**: 자동 저장 시스템
- **DeviceInfo**: 디바이스별 최적화 설정

### 에러 처리
- **GameLogger**: 모든 이벤트에 대한 로깅
- **ErrorHandler**: 에러 발생 시 자동 처리

이 시스템을 통해 모바일 앱의 생명주기를 안전하게 관리하고, 사용자 데이터를 적절한 시점에 저장할 수 있습니다. 