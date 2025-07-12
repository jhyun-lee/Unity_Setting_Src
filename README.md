# Unity 모바일 게임 시스템 통합 패키지

## 개요
이 패키지는 Unity로 모바일 게임을 개발할 때 필요한 핵심 시스템들을 통합하여 제공합니다. 데이터 관리, 설정 관리, 오디오 시스템, 자동 저장, 앱 생명주기 관리, 성능 모니터링, 그리고 통합 테스트 시스템까지 모든 기능을 포함합니다.

## 🎯 주요 기능

### 📁 데이터 관리 시스템
- **UserDataManager**: 사용자 데이터 저장/로드, 검증, 마이그레이션
- **FileHandler**: 안전한 파일 저장/로드, 백업 관리
- **EncryptionHelper**: AES 암호화/복호화로 데이터 보안
- **AutoSaveManager**: 자동 저장 시스템 (30초 주기)

### ⚙️ 설정 관리 시스템
- **SettingsManager**: 게임 설정 저장/로드
- **SettingsUI**: 설정 UI 연동

### 🔊 오디오 시스템
- **AudioManager**: 마스터/BGM/SFX 볼륨 관리
- **오디오 재생/정지/일시정지 기능**

### 📱 앱 생명주기 관리
- **AppLifecycleManager**: 앱 포커스, 일시정지, 종료 처리
- **자동 데이터 저장/복원**
- **배터리 절약 모드 대응**

### 📊 성능 모니터링
- **PerformanceMonitor**: 실시간 FPS, 메모리, 배터리 모니터링
- **PerformanceUI**: 성능 정보 시각화
- **자동 최적화 시스템**

### 🧪 통합 테스트 시스템
- **GameSystemTester**: 모든 시스템 통합 테스트
- **Unity 에디터 메뉴 연동**

### 🔧 유틸리티 시스템
- **GameLogger**: 로그 관리 시스템
- **ErrorHandler**: 에러 처리 시스템
- **DeviceInfo**: 디바이스 정보 수집 및 최적화
- **GameInitializer**: 게임 초기화 관리

## 📁 프로젝트 구조

```
Scripts/
├── Managers/                    # 핵심 매니저 클래스들
│   ├── SettingsManager.cs      # 설정 관리
│   ├── UserDataManager.cs      # 사용자 데이터 관리
│   ├── AudioManager.cs         # 오디오 관리
│   ├── AutoSaveManager.cs      # 자동 저장
│   ├── AppLifecycleManager.cs  # 앱 생명주기
│   ├── PerformanceMonitor.cs   # 성능 모니터링
│   └── GameInitializer.cs      # 게임 초기화
├── Utilities/                   # 유틸리티 클래스들
│   ├── FileHandler.cs          # 파일 처리
│   ├── EncryptionHelper.cs     # 암호화
│   ├── GameLogger.cs           # 로그 관리
│   ├── ErrorHandler.cs         # 에러 처리
│   └── DeviceInfo.cs           # 디바이스 정보
├── UI/                         # UI 관련 클래스들
│   ├── SettingsUI.cs           # 설정 UI
│   └── PerformanceUI.cs        # 성능 UI
├── Testing/                    # 테스트 시스템
│   ├── GameSystemTester.cs     # 통합 테스터
│   ├── Editor/                 # 에디터 스크립트
│   │   └── GameSystemTesterMenu.cs
│   └── README_GameSystemTester.md
├── Examples/                   # 사용 예제
│   ├── AppLifecycleExample.cs  # 앱 생명주기 예제
│   └── PerformanceMonitorExample.cs
└── README_*.md                 # 각 시스템별 문서
```

## 🚀 빠른 시작

### 1. 기본 설정

#### Unity 에디터에서 자동 설정
```
Game > Testing > Setup Test Environment
```
이 메뉴를 통해 모든 필요한 매니저 오브젝트가 자동으로 생성됩니다.

#### 수동 설정
```csharp
// GameInitializer를 씬에 추가
GameObject initializer = new GameObject("GameInitializer");
initializer.AddComponent<GameInitializer>();
```

### 2. 기본 사용법

#### 게임 시작 시 초기화
```csharp
public class GameController : MonoBehaviour
{
    private void Start()
    {
        // 게임 초기화 (자동으로 모든 매니저 초기화)
        if (GameInitializer.Instance != null)
        {
            GameInitializer.OnGameInitialized += OnGameInitialized;
        }
    }
    
    private void OnGameInitialized()
    {
        Debug.Log("게임 초기화 완료!");
        
        // 설정 로드
        SettingsManager.Instance.LoadSettings();
        
        // 사용자 데이터 로드
        UserDataManager.Instance.LoadUserData();
        
        // 자동 저장 시작
        AutoSaveManager.Instance.EnableAutoSave();
        
        // 성능 모니터링 시작
        PerformanceMonitor.Instance.StartMonitoring();
    }
}
```

#### 설정 관리
```csharp
// 설정 저장
SettingsManager.Instance.SetSetting("masterVolume", 0.8f);
SettingsManager.Instance.SetSetting("qualityLevel", 2);
SettingsManager.Instance.SaveSettings();

// 설정 로드
float volume = SettingsManager.Instance.GetSetting<float>("masterVolume");
int quality = SettingsManager.Instance.GetSetting<int>("qualityLevel");
```

#### 사용자 데이터 관리
```csharp
// 데이터 저장
UserDataManager.Instance.SetUserData("playerName", "Player1");
UserDataManager.Instance.SetUserData("level", 5);
UserDataManager.Instance.SetUserData("coins", 1000);
UserDataManager.Instance.SaveUserData();

// 데이터 로드
string playerName = UserDataManager.Instance.GetUserData<string>("playerName");
int level = UserDataManager.Instance.GetUserData<int>("level");
int coins = UserDataManager.Instance.GetUserData<int>("coins");
```

#### 오디오 관리
```csharp
// 볼륨 설정
AudioManager.Instance.SetMasterVolume(0.8f);
AudioManager.Instance.SetBGMVolume(0.7f);
AudioManager.Instance.SetSFXVolume(0.9f);

// 음소거
AudioManager.Instance.MuteAll();
AudioManager.Instance.UnmuteAll();
```

## 🔄 추천 개발 흐름

### 1. 프로젝트 초기 설정

#### 1단계: 환경 설정
```csharp
// Unity 에디터에서 실행
Game > Testing > Setup Test Environment
```

#### 2단계: 기본 매니저 확인
```csharp
// 모든 매니저가 정상적으로 생성되었는지 확인
if (SettingsManager.Instance != null &&
    UserDataManager.Instance != null &&
    AudioManager.Instance != null &&
    AutoSaveManager.Instance != null &&
    AppLifecycleManager.Instance != null &&
    PerformanceMonitor.Instance != null)
{
    Debug.Log("모든 매니저 초기화 완료");
}
```

### 2. 게임 개발 단계

#### 개발 초기 (프로토타입)
```csharp
public class GameController : MonoBehaviour
{
    private void Start()
    {
        // 기본 초기화
        InitializeGame();
        
        // 개발용 로그 활성화
        GameLogger.SetLogLevel(LogType.Debug);
    }
    
    private void InitializeGame()
    {
        // 설정 로드
        SettingsManager.Instance.LoadSettings();
        
        // 사용자 데이터 로드
        UserDataManager.Instance.LoadUserData();
        
        // 자동 저장 활성화
        AutoSaveManager.Instance.EnableAutoSave();
    }
}
```

#### 개발 중기 (기능 구현)
```csharp
public class GameplayManager : MonoBehaviour
{
    private void Start()
    {
        // 성능 모니터링 시작
        PerformanceMonitor.Instance.StartMonitoring();
        
        // 앱 생명주기 이벤트 구독
        AppLifecycleManager.OnAppPauseChanged += OnAppPauseChanged;
    }
    
    private void OnAppPauseChanged(bool isPaused)
    {
        if (isPaused)
        {
            // 게임 일시정지
            PauseGame();
        }
        else
        {
            // 게임 재개
            ResumeGame();
        }
    }
    
    private void PauseGame()
    {
        // 게임 상태 저장
        UserDataManager.Instance.SaveUserData();
        
        // 오디오 정지
        AudioManager.Instance.PauseAllAudio();
    }
    
    private void ResumeGame()
    {
        // 오디오 재개
        AudioManager.Instance.ResumeAllAudio();
    }
}
```

#### 개발 후기 (최적화)
```csharp
public class OptimizationManager : MonoBehaviour
{
    private void Start()
    {
        // 성능 이벤트 구독
        PerformanceMonitor.OnPerformanceWarning += OnPerformanceWarning;
        PerformanceMonitor.OnAutoOptimization += OnAutoOptimization;
    }
    
    private void OnPerformanceWarning(PerformanceData data)
    {
        if (data.fps < 30)
        {
            // FPS가 낮을 때 처리
            ApplyLowPerformanceMode();
        }
    }
    
    private void OnAutoOptimization(string message)
    {
        Debug.Log($"자동 최적화 적용: {message}");
    }
    
    private void ApplyLowPerformanceMode()
    {
        // 낮은 성능 모드 적용
        QualitySettings.SetQualityLevel(0);
        Application.targetFrameRate = 30;
    }
}
```

### 3. 테스트 및 배포

#### 테스트 실행
```csharp
// Unity 에디터에서 실행
Game > Testing > Test All Systems
```

#### 배포 전 최종 확인
```csharp
public class DeploymentChecker : MonoBehaviour
{
    private void Start()
    {
        // 모든 시스템 상태 확인
        CheckAllSystems();
        
        // 성능 테스트 실행
        RunPerformanceTest();
    }
    
    private void CheckAllSystems()
    {
        bool allSystemsOK = true;
        
        // 각 매니저 상태 확인
        if (SettingsManager.Instance == null) allSystemsOK = false;
        if (UserDataManager.Instance == null) allSystemsOK = false;
        if (AudioManager.Instance == null) allSystemsOK = false;
        if (AutoSaveManager.Instance == null) allSystemsOK = false;
        if (AppLifecycleManager.Instance == null) allSystemsOK = false;
        if (PerformanceMonitor.Instance == null) allSystemsOK = false;
        
        if (allSystemsOK)
        {
            Debug.Log("모든 시스템 정상");
        }
        else
        {
            Debug.LogError("일부 시스템에 문제가 있습니다");
        }
    }
    
    private void RunPerformanceTest()
    {
        // 성능 테스트 실행
        PerformanceMonitor.Instance.StartMonitoring();
        
        // 10초 후 결과 확인
        StartCoroutine(CheckPerformanceAfterDelay(10f));
    }
    
    private IEnumerator CheckPerformanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        var avgData = PerformanceMonitor.Instance.GetAveragePerformance();
        Debug.Log($"평균 FPS: {avgData.fps:F1}");
        Debug.Log($"평균 메모리 사용률: {avgData.memoryUsage:P1}");
    }
}
```

## 📋 시스템별 상세 가이드

### 1. SettingsManager
- **기능**: 게임 설정 저장/로드
- **파일**: `Scripts/Managers/SettingsManager.cs`
- **문서**: `Scripts/Managers/README_SettingsManager.md`

### 2. UserDataManager
- **기능**: 사용자 데이터 관리, 암호화, 마이그레이션
- **파일**: `Scripts/Managers/UserDataManager.cs`
- **문서**: `Scripts/Managers/README_UserDataManager.md`

### 3. AudioManager
- **기능**: 오디오 볼륨 관리, 재생 제어
- **파일**: `Scripts/Managers/AudioManager.cs`
- **문서**: `Scripts/Managers/README_AudioManager.md`

### 4. AutoSaveManager
- **기능**: 자동 저장, 수동 저장
- **파일**: `Scripts/Managers/AutoSaveManager.cs`
- **문서**: `Scripts/Managers/README_AutoSaveManager.md`

### 5. AppLifecycleManager
- **기능**: 앱 생명주기 관리, 자동 저장/복원
- **파일**: `Scripts/Managers/AppLifecycleManager.cs`
- **문서**: `Scripts/Managers/README_AppLifecycleManager.md`

### 6. PerformanceMonitor
- **기능**: 성능 모니터링, 자동 최적화
- **파일**: `Scripts/Managers/PerformanceMonitor.cs`
- **문서**: `Scripts/Managers/README_PerformanceMonitor.md`

### 7. GameSystemTester
- **기능**: 통합 테스트 시스템
- **파일**: `Scripts/Testing/GameSystemTester.cs`
- **문서**: `Scripts/Testing/README_GameSystemTester.md`

## 🛠️ 유틸리티 시스템

### FileHandler
- 안전한 파일 저장/로드
- 백업 관리
- 파일 존재 확인

### EncryptionHelper
- AES 암호화/복호화
- 디바이스 고유 키 생성
- 데이터 보안

### GameLogger
- 로그 레벨 관리
- 파일 저장
- 성능 통계

### ErrorHandler
- 에러 타입 분류
- 자동 복구
- 에러 팝업

### DeviceInfo
- 디바이스 정보 수집
- 성능 등급 판별
- 최적화 설정

## 🧪 테스트 시스템

### Unity 에디터 메뉴
```
Game > Testing > Test All Systems
Game > Testing > Test Settings System
Game > Testing > Test User Data System
Game > Testing > Test Audio System
Game > Testing > Test Auto Save System
Game > Testing > Test App Lifecycle System
Game > Testing > Test Performance Monitor System
Game > Testing > Generate Test Report
Game > Testing > Cleanup Test Data
Game > Testing > Setup Test Environment
```

### 코드에서 테스트
```csharp
// 전체 시스템 테스트
GameSystemTester.Instance.TestAllSystems();

// 개별 시스템 테스트
GameSystemTester.Instance.TestSettingsSystem();
GameSystemTester.Instance.TestUserDataSystem();
GameSystemTester.Instance.TestAudioSystem();
GameSystemTester.Instance.TestAutoSaveSystem();
GameSystemTester.Instance.TestAppLifecycleSystem();
GameSystemTester.Instance.TestPerformanceMonitorSystem();

// 테스트 결과 확인
GameSystemTester.Instance.GenerateTestReport();
```

## 📊 성능 모니터링

### 실시간 모니터링
```csharp
// 모니터링 시작
PerformanceMonitor.Instance.StartMonitoring();

// 성능 데이터 확인
var data = PerformanceMonitor.Instance.CheckPerformance();
Debug.Log($"FPS: {data.fps}, 메모리: {data.memoryUsage:P1}");

// 자동 최적화
PerformanceMonitor.Instance.AutoOptimize();
```

### 성능 UI
```csharp
// PerformanceUI 컴포넌트를 UI에 추가
// 실시간 성능 정보 표시
// 색상 기반 상태 표시 (녹색/노란색/빨간색)
```

## 🔒 데이터 보안

### 암호화 설정
```csharp
// UserDataManager에서 자동으로 암호화 적용
// AES 암호화로 데이터 보안
// 디바이스 고유 키 사용
```

### 백업 시스템
```csharp
// FileHandler에서 자동 백업 생성
// 데이터 손상 시 자동 복구
// 버전별 마이그레이션 지원
```

## 📱 모바일 최적화

### 디바이스별 최적화
```csharp
// DeviceInfo로 디바이스 성능 등급 판별
var performanceLevel = DeviceInfo.GetPerformanceLevel();
var recommendedSettings = DeviceInfo.GetRecommendedSettings();

// 자동 최적화 적용
DeviceInfo.OptimizeForDevice();
```

### 배터리 절약
```csharp
// AppLifecycleManager에서 자동 처리
// 배터리 부족 시 성능 최적화
// 백그라운드 실행 시 자동 저장
```

## 🚨 주의사항

### 1. 초기화 순서
- `GameInitializer`가 가장 먼저 초기화되어야 함
- 다른 매니저들은 `GameInitializer`에서 자동 초기화

### 2. 데이터 백업
- 테스트 전 중요 데이터 백업 필수
- 테스트 데이터와 실제 데이터 분리



**버전**: 1.0.0  
**최종 업데이트**: 2024년  
**Unity 버전**: 2021.3 LTS 이상 권장 