# GameSystemTester 사용 가이드

## 개요
`GameSystemTester`는 지금까지 구현한 모든 게임 시스템을 통합하여 테스트하는 테스터입니다.

## 테스트 대상 시스템

### 1. SettingsManager - 설정 저장/로드
- 설정 데이터 저장/로드 테스트
- 설정 값 변경 및 적용 테스트
- 설정 데이터 검증 테스트

### 2. UserDataManager - 사용자 데이터 관리
- 사용자 데이터 저장/로드 테스트
- 데이터 검증 및 마이그레이션 테스트
- 데이터 손상 복구 테스트

### 3. AudioManager - 오디오 시스템
- 마스터/BGM/SFX 볼륨 설정 테스트
- 음소거/음소거 해제 테스트
- 오디오 상태 검증 테스트

### 4. AutoSaveManager - 자동 저장
- 자동 저장 활성화/비활성화 테스트
- 수동 저장 테스트
- 저장 상태 검증 테스트

### 5. AppLifecycleManager - 앱 생명주기
- 게임 상태 저장/복원 테스트
- 빠른 저장 테스트
- 앱 상태 검증 테스트

### 6. PerformanceMonitor - 성능 모니터링
- 성능 데이터 수집 테스트
- 자동 최적화 테스트
- 성능 측정 검증 테스트

## 테스트 시나리오

### 1. 초기 설정 로드 테스트
- 기본 설정 값 로드
- 설정 파일 존재 여부 확인
- 설정 값 유효성 검증

### 2. 사용자 데이터 저장/로드 테스트
- 테스트 데이터 생성 및 저장
- 데이터 로드 및 검증
- 데이터 일관성 확인

### 3. 설정 변경 및 적용 테스트
- 설정 값 변경
- 변경된 설정 저장
- 설정 적용 확인

### 4. 데이터 손상 복구 테스트
- 손상된 데이터 시뮬레이션
- 자동 복구 기능 테스트
- 복구 결과 검증

### 5. 앱 생명주기 이벤트 테스트
- 앱 일시정지/재개 시뮬레이션
- 데이터 자동 저장 확인
- 상태 복원 검증

### 6. 성능 최적화 테스트
- 성능 데이터 수집
- 자동 최적화 실행
- 최적화 결과 검증

## 사용 방법

### 1. Unity 에디터에서 테스트

#### 메뉴를 통한 테스트
```
Game > Testing > Test All Systems
Game > Testing > Test Settings System
Game > Testing > Test User Data System
Game > Testing > Test Audio System
Game > Testing > Test Auto Save System
Game > Testing > Test App Lifecycle System
Game > Testing > Test Performance Monitor System
```

#### 테스트 환경 설정
```
Game > Testing > Setup Test Environment
```
- 모든 매니저 오브젝트 자동 생성
- 테스트 데이터 초기화

#### 테스트 데이터 정리
```
Game > Testing > Cleanup Test Data
```
- 테스트 중 생성된 데이터 정리
- 실제 데이터와 분리

### 2. 코드에서 테스트

#### 전체 시스템 테스트
```csharp
GameSystemTester.Instance.TestAllSystems();
```

#### 개별 시스템 테스트
```csharp
// 설정 시스템 테스트
GameSystemTester.Instance.TestSettingsSystem();

// 사용자 데이터 시스템 테스트
GameSystemTester.Instance.TestUserDataSystem();

// 오디오 시스템 테스트
GameSystemTester.Instance.TestAudioSystem();

// 자동 저장 시스템 테스트
GameSystemTester.Instance.TestAutoSaveSystem();

// 앱 생명주기 시스템 테스트
GameSystemTester.Instance.TestAppLifecycleSystem();

// 성능 모니터링 시스템 테스트
GameSystemTester.Instance.TestPerformanceMonitorSystem();
```

#### 테스트 결과 확인
```csharp
// 테스트 보고서 생성
GameSystemTester.Instance.GenerateTestReport();

// 테스트 데이터 정리
GameSystemTester.Instance.CleanupTestData();
```

## 테스트 결과

### 테스트 결과 구조
```csharp
public class TestResult
{
    public string testName;        // 테스트 이름
    public bool isSuccess;         // 성공 여부
    public string message;         // 결과 메시지
    public float executionTime;    // 실행 시간
    public DateTime timestamp;     // 타임스탬프
}
```

### 테스트 보고서 구조
```csharp
public class TestReport
{
    public List<TestResult> results;    // 테스트 결과 목록
    public int totalTests;              // 총 테스트 수
    public int passedTests;             // 성공한 테스트 수
    public int failedTests;             // 실패한 테스트 수
    public float totalExecutionTime;    // 총 실행 시간
    public DateTime testStartTime;      // 테스트 시작 시간
    public DateTime testEndTime;        // 테스트 종료 시간
    public float successRate;           // 성공률
}
```

## 테스트 데이터

### 테스트 설정 데이터
```csharp
private Dictionary<string, object> _testSettings = new Dictionary<string, object>
{
    { "masterVolume", 0.8f },
    { "bgmVolume", 0.7f },
    { "sfxVolume", 0.9f },
    { "qualityLevel", 2 },
    { "targetFrameRate", 60 },
    { "autoSave", true },
    { "performanceMonitoring", true }
};
```

### 테스트 사용자 데이터
```csharp
private Dictionary<string, object> _testUserData = new Dictionary<string, object>
{
    { "playerName", "TestPlayer" },
    { "level", 5 },
    { "experience", 1250 },
    { "coins", 500 },
    { "highScore", 8500 },
    { "playTime", 3600f },
    { "achievements", new List<string> { "FirstWin", "CoinCollector" } },
    { "settings", _testSettings }
};
```

## 이벤트 시스템

### 사용 가능한 이벤트
```csharp
// 테스트 완료 이벤트
public static event Action<TestReport> OnTestCompleted;

// 개별 테스트 결과 이벤트
public static event Action<TestResult> OnTestResult;
```

### 이벤트 구독 예제
```csharp
private void Start()
{
    // 이벤트 구독
    GameSystemTester.OnTestCompleted += OnTestCompleted;
    GameSystemTester.OnTestResult += OnTestResult;
}

private void OnDestroy()
{
    // 이벤트 구독 해제
    GameSystemTester.OnTestCompleted -= OnTestCompleted;
    GameSystemTester.OnTestResult -= OnTestResult;
}

private void OnTestCompleted(TestReport report)
{
    Debug.Log($"테스트 완료: 성공률 {report.successRate:F1}%");
}

private void OnTestResult(TestResult result)
{
    Debug.Log($"{result.testName}: {(result.isSuccess ? "성공" : "실패")}");
}
```

## 테스트 규칙

### 1. 독립적 실행
- 각 테스트는 독립적으로 실행 가능
- 테스트 간 의존성 최소화
- 테스트 순서에 관계없이 동일한 결과

### 2. 오류 처리
- 테스트 실패 시 구체적인 오류 메시지 출력
- 예외 상황에 대한 적절한 처리
- 테스트 중단 없이 계속 진행

### 3. 데이터 분리
- 테스트 데이터는 실제 데이터와 분리
- 테스트 전용 키 사용
- 테스트 후 데이터 정리

### 4. 정리 작업
- 모든 테스트 완료 후 정리 작업 수행
- 테스트 데이터 삭제
- 시스템 상태 복원

## 디버그 기능

### OnGUI 디버그 정보
- 테스트 실행 상태 표시
- 테스트 결과 요약
- 성공률 및 실행 시간 표시

### 로그 출력
- 각 테스트 단계별 상세 로그
- 성공/실패 원인 분석
- 실행 시간 측정

## 주의사항

### 1. 테스트 환경
- 플레이 모드에서만 테스트 실행 가능
- 모든 매니저가 씬에 존재해야 함
- 테스트 전 환경 설정 필요

### 2. 데이터 백업
- 테스트 전 중요 데이터 백업
- 테스트 데이터와 실제 데이터 분리
- 테스트 후 데이터 정리 필수

### 3. 성능 고려사항
- 대량 테스트 시 성능 영향 고려
- 테스트 간 적절한 지연 시간 설정
- 메모리 사용량 모니터링

### 4. 에러 처리
- 테스트 실패 시 시스템 복구
- 부분 실패 시에도 계속 진행
- 상세한 오류 로그 기록

## 사용 예제

### 기본 테스트 실행
```csharp
public class TestRunner : MonoBehaviour
{
    private void Start()
    {
        // 전체 시스템 테스트
        GameSystemTester.Instance.TestAllSystems();
    }
}
```

### 개별 테스트 실행
```csharp
public class CustomTester : MonoBehaviour
{
    public void RunCustomTests()
    {
        // 설정 시스템만 테스트
        GameSystemTester.Instance.TestSettingsSystem();
        
        // 사용자 데이터 시스템만 테스트
        GameSystemTester.Instance.TestUserDataSystem();
    }
}
```

### 테스트 결과 분석
```csharp
public class TestAnalyzer : MonoBehaviour
{
    private void Start()
    {
        // 이벤트 구독
        GameSystemTester.OnTestCompleted += AnalyzeTestResults;
    }
    
    private void AnalyzeTestResults(TestReport report)
    {
        Debug.Log($"총 테스트: {report.totalTests}");
        Debug.Log($"성공: {report.passedTests}");
        Debug.Log($"실패: {report.failedTests}");
        Debug.Log($"성공률: {report.successRate:F1}%");
        
        foreach (var result in report.results)
        {
            Debug.Log($"{result.testName}: {result.message}");
        }
    }
}
```

이 시스템을 통해 모든 게임 시스템의 정상 동작을 확인하고, 문제점을 사전에 발견하여 안정적인 게임 개발을 보장할 수 있습니다. 