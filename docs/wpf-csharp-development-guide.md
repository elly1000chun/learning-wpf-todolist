# WPF C# 개발환경 가이드

이 문서는 WPF와 C# 개발 환경을 준비하고, MVVM 패턴으로 간단한 할 일 앱을 만들면서 기본 개념을 익히기 위한 학습 노트입니다.

## 권장 개발 환경

처음 WPF를 학습한다면 Windows에서 Visual Studio Community와 최신 .NET SDK를 사용하는 구성이 가장 편합니다. WPF는 Windows 데스크톱 UI 기술이므로, XAML 디자이너와 디버거가 잘 통합된 IDE를 쓰면 입문 부담이 줄어듭니다.

| 선택지 | 장점 | 단점 | 추천 상황 |
|---|---|---|---|
| Visual Studio Community | WPF/XAML 디자이너, 디버깅, NuGet 관리가 편함 | 설치 용량이 큼 | WPF를 처음 배우는 경우 |
| JetBrains Rider | 코드 탐색과 리팩터링이 좋음 | 라이선스 확인 필요, WPF 디자이너 경험은 Visual Studio가 더 표준적 | JetBrains IDE에 익숙한 경우 |
| VS Code + .NET CLI | 가볍고 CLI 중심으로 작업 가능 | WPF 전용 도구와 XAML 디자이너가 부족함 | XAML을 직접 작성하며 가볍게 실습하는 경우 |

현재 프로젝트는 `net10.0-windows`를 대상으로 만들어져 있습니다. 따라서 로컬 환경에는 해당 대상 프레임워크를 빌드할 수 있는 .NET SDK가 필요합니다.

## WPF 프로젝트 구조

```text
TodoWpf/
├─ App.xaml
├─ App.xaml.cs
├─ MainWindow.xaml
├─ MainWindow.xaml.cs
├─ Models/
│  └─ TodoItems.cs
├─ Services/
│  └─ TodoStorageService.cs
├─ ViewModels/
│  └─ MainWindowViewModel.cs
└─ TodoWpf.csproj
```

WPF에서는 화면 구조와 스타일을 XAML로 선언하고, 화면에 연결되는 상태와 동작은 C# 코드로 작성합니다.

## MVVM 역할 분리

| 영역 | 현재 파일 | 역할 |
|---|---|---|
| View | `MainWindow.xaml` | 화면 구조와 바인딩 선언 |
| View Code-behind | `MainWindow.xaml.cs` | 화면 초기화와 `DataContext` 연결 |
| ViewModel | `MainWindowViewModel.cs` | 화면 상태와 명령 관리 |
| Model | `TodoItems.cs` | 할 일 항목 데이터 표현 |
| Service | `TodoStorageService.cs` | JSON 저장과 불러오기 담당 |

이 프로젝트에서는 코드 비하인드에 복잡한 화면 동작을 넣지 않고, `MainWindowViewModel`이 입력값, 목록, 추가/삭제 명령을 관리합니다.

## 현재 앱에서 익히는 핵심 개념

| 개념 | 의미 |
|---|---|
| XAML | 화면 구조와 스타일 선언 |
| `DataContext` | View가 사용할 ViewModel 연결 |
| `{Binding ...}` | UI 요소와 ViewModel/Model 속성 연결 |
| `ObservableObject` | 속성 변경을 UI에 알림 |
| `[ObservableProperty]` | 바인딩 가능한 속성 자동 생성 |
| `ObservableCollection<T>` | 목록 추가/삭제 시 UI 자동 갱신 |
| `[RelayCommand]` | 버튼과 ViewModel 메서드 연결 |
| `DataTemplate` | 목록 항목의 표시 방식 정의 |
| `DataTrigger` | 완료 여부에 따라 스타일 변경 |

## JSON 자동 저장 학습 메모

JSON 자동 저장 단계에서는 앱의 할 일 목록을 메모리가 아니라 파일에 보관합니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `Services/TodoStorageService.cs`를 만들어 저장/불러오기 책임을 분리한다.
2. 앱 시작 시 `Load()`로 저장된 할 일 목록을 읽는다.
3. 저장된 데이터가 없으면 기본 샘플 할 일을 보여준다.
4. 할 일을 추가하거나 삭제하면 `SaveTodos()`를 호출한다.
5. 체크박스로 `IsDone`이 바뀌면 `TodoItem.PropertyChanged` 이벤트를 통해 저장한다.

저장되는 시점은 다음과 같습니다.

- 새 할 일을 추가할 때
- 할 일을 삭제할 때
- 체크박스로 완료 상태를 바꿀 때

저장 파일은 사용자 로컬 앱 데이터 폴더 아래의 `TodoWpf\todos.json`입니다. 이 파일은 사용자별 실행 데이터이므로 Git에 포함하지 않습니다.

## 실행과 빌드

프로젝트 폴더에서 다음 명령을 사용할 수 있습니다.

```powershell
cd D:\Dev\WPF\learning-wpf-todolist\TodoWpf
dotnet restore
dotnet build
dotnet run
```

Visual Studio에서는 `TodoWpf.slnx` 또는 `TodoWpf.csproj`를 열고 `F5`로 실행할 수 있습니다.

## 추천 학습 순서

1. XAML 레이아웃: `Grid`, `StackPanel`, `DockPanel`
2. 스타일과 리소스: `Style`, `ResourceDictionary`
3. 데이터 바인딩과 `DataContext`
4. MVVM과 명령: `RelayCommand`, `CanExecute`
5. JSON 자동 저장과 불러오기
6. 완료/미완료 필터
7. 검색
8. ViewModel 단위 테스트
9. 의존성 주입과 서비스 계층 분리
10. 게시, 설치 파일, MSIX 배포

## 다음 실습 후보

1. 완료/미완료 필터
2. 검색 기능
3. ViewModel 단위 테스트
4. 스타일과 리소스 딕셔너리 분리

기능을 추가할 때는 한 번에 많은 구조를 바꾸기보다, ViewModel 속성 하나, Command 하나, XAML 바인딩 하나처럼 작은 단위로 이해하면서 확장하는 것을 권장합니다.
