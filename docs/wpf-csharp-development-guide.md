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
| `ICollectionView` | 원본 목록은 유지하면서 화면 표시 목록을 제어 |
| `[RelayCommand]` | 버튼과 ViewModel 메서드 연결 |
| `DataTemplate` | 목록 항목의 표시 방식 정의 |
| `DataTrigger` | 완료 여부에 따라 스타일 변경 |
| 단위 테스트 | ViewModel 동작을 UI 실행 없이 검증 |

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

## 완료/미완료 필터 학습 메모

필터 단계에서는 원본 할 일 목록인 `Todos`를 직접 지우거나 다시 만들지 않고, 화면에 표시되는 목록만 바꾸는 방식을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `TodoFilter` enum으로 `All`, `Active`, `Completed` 필터 상태를 정의한다.
2. `SelectedFilter` 속성으로 현재 선택된 필터를 ViewModel에서 관리한다.
3. `CollectionViewSource.GetDefaultView(Todos)`로 `TodosView`를 만든다.
4. `TodosView.Filter`에 `FilterTodo()` 메서드를 연결한다.
5. 필터 버튼은 `SetFilterCommand`를 통해 `SelectedFilter`만 변경한다.
6. `SelectedFilter`가 바뀌면 `TodosView.Refresh()`를 호출해 화면 목록을 다시 계산한다.
7. 체크박스로 `IsDone`이 바뀔 때도 `TodosView.Refresh()`를 호출해 현재 필터 결과가 즉시 반영되게 한다.

핵심은 “데이터 원본”과 “화면에 보이는 목록”을 분리하는 것입니다.

- `Todos`: 실제 할 일 데이터 전체
- `TodosView`: 현재 필터 조건에 따라 화면에 표시되는 목록
- `SelectedFilter`: 사용자가 선택한 필터 상태
- `FilterTodo()`: 항목 하나를 화면에 보여줄지 판단하는 조건

XAML에서는 `SetFilterCommand`와 `CommandParameter`를 사용해 버튼에서 enum 값을 ViewModel로 전달했습니다. 선택된 필터 버튼을 강조할 때는 `DataTrigger`를 사용해 `SelectedFilter` 값에 따라 버튼 스타일을 바꾸었습니다.

## 검색 기능 학습 메모

검색 단계에서는 이미 만든 `TodosView.Filter` 안에 검색 조건을 함께 넣는 방식을 배웠습니다. 새 목록을 따로 만들지 않고, 원본 목록인 `Todos`는 유지한 채 화면에 표시되는 항목만 줄이는 구조입니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `SearchText` 속성으로 검색창 입력값을 ViewModel에서 관리한다.
2. `OnSearchTextChanged()`에서 `TodosView.Refresh()`를 호출해 입력 즉시 목록을 다시 계산한다.
3. `FilterTodo()`에서 완료/미완료 필터 조건을 먼저 확인한다.
4. 필터 조건을 통과한 항목에 대해서만 제목 검색 조건을 적용한다.
5. `StringComparison.OrdinalIgnoreCase`를 사용해 대소문자를 구분하지 않고 검색한다.
6. `ClearSearchCommand`로 검색어를 빈 문자열로 바꾸고, 기존 변경 감지 흐름을 통해 목록을 갱신한다.

검색 조건과 완료 필터는 서로 따로 동작하는 것이 아니라 함께 적용됩니다.

- `SelectedFilter = TodoFilter.All`, `SearchText = "wpf"`: 제목에 `wpf`가 들어간 모든 항목 표시
- `SelectedFilter = TodoFilter.Active`, `SearchText = "wpf"`: 진행 중 항목 중 제목에 `wpf`가 들어간 항목 표시
- `SelectedFilter = TodoFilter.Completed`, `SearchText = ""`: 완료된 모든 항목 표시

핵심은 `FilterTodo()`가 하나의 항목에 대해 “현재 필터 조건에도 맞고, 검색어 조건에도 맞는가?”를 판단한다는 점입니다.

## ViewModel 단위 테스트 학습 메모

단위 테스트 단계에서는 WPF 화면을 직접 실행하지 않고, `MainWindowViewModel`의 상태와 명령이 기대한 대로 동작하는지 확인했습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. 테스트 프로젝트 `TodoWpf.Tests`를 만들고 앱 프로젝트를 참조한다.
2. 앱 프로젝트와 호환되도록 테스트 프로젝트도 `net10.0-windows`를 대상으로 설정한다.
3. `ITodoStorageService` 인터페이스를 만들고, ViewModel이 구체 저장 클래스가 아니라 인터페이스에 의존하게 한다.
4. 테스트에서는 실제 파일을 쓰는 `TodoStorageService` 대신 `FakeTodoStorageService`를 사용한다.
5. `AddTodoCommand`, `RemoveTodoCommand`, `ClearSearchCommand`처럼 ViewModel 명령을 직접 실행해 결과를 검증한다.
6. `TodosView`를 열거해 필터와 검색 결과가 화면에 표시될 목록과 일치하는지 확인한다.
7. 체크박스로 바뀌는 `TodoItem.IsDone` 변경이 저장 호출로 이어지는지 확인한다.

이번 테스트에서 확인한 주요 동작은 다음과 같습니다.

- 저장된 할 일을 ViewModel 생성 시 불러온다.
- 새 할 일을 추가하면 목록에 들어가고 저장된다.
- 공백 제목은 추가되지 않는다.
- 할 일을 삭제하면 목록에서 빠지고 저장된다.
- `null` 삭제 요청은 아무 일도 하지 않는다.
- 완료 상태가 바뀌면 자동 저장된다.
- 완료/미완료 필터가 원본 목록을 지우지 않고 표시 목록만 바꾼다.
- 검색어가 제목과 대소문자 구분 없이 매칭된다.
- 완료 필터와 검색 조건이 함께 적용된다.
- 검색 초기화 명령이 `SearchText`를 빈 문자열로 되돌린다.

핵심은 “테스트하기 어려운 실제 파일 저장”을 ViewModel 밖으로 밀어내고, 테스트에서는 가짜 저장소를 넣어 ViewModel의 판단만 검증하는 것입니다. 이것이 의존성 주입을 사용하는 중요한 이유 중 하나입니다.

## 실행과 빌드

앱 프로젝트 폴더에서 다음 명령을 사용할 수 있습니다.

```powershell
cd D:\Dev\WPF\learning-wpf-todolist\TodoWpf
dotnet restore
dotnet build
dotnet run
```

테스트는 프로젝트 루트에서 실행합니다.

```powershell
cd D:\Dev\WPF\learning-wpf-todolist
dotnet test
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

1. 스타일과 리소스 딕셔너리 분리
2. 할 일 수정 기능
3. 전체 삭제 또는 완료 항목 삭제 기능

기능을 추가할 때는 한 번에 많은 구조를 바꾸기보다, ViewModel 속성 하나, Command 하나, XAML 바인딩 하나처럼 작은 단위로 이해하면서 확장하는 것을 권장합니다.
