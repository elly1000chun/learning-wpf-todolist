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
| `ResourceDictionary` | 스타일과 템플릿 같은 리소스를 별도 파일로 분리 |
| Code-behind | 포커스처럼 View에 가까운 화면 동작 처리 |
| 의존성 주입 | 필요한 객체 생성을 한 곳에서 관리하고 생성자로 전달 |
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

## 스타일과 리소스 딕셔너리 학습 메모

스타일 분리 단계에서는 `MainWindow.xaml` 안에 직접 적혀 있던 반복 UI 설정을 별도 리소스로 옮겼습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `Window.Resources`에 이름 있는 `Style`을 만들고 `StaticResource`로 사용한다.
2. 반복되는 `TextBox` 속성을 `InputTextBoxStyle`로 분리한다.
3. 필터 버튼의 기본 모양을 `FilterButtonBaseStyle`로 만들고 `BasedOn`으로 확장한다.
4. `ListBoxItem` 컨테이너 스타일을 `TodoListBoxItemStyle`로 분리한다.
5. `Styles/TodoStyles.xaml` 파일을 만들고 스타일들을 `ResourceDictionary`로 옮긴다.
6. `App.xaml`의 `Application.Resources`에서 `MergedDictionaries`로 스타일 파일을 전역 연결한다.
7. 할 일 한 줄 UI를 `TodoItemTemplate`이라는 `DataTemplate` 리소스로 분리한다.
8. 추가, 검색 초기화, 삭제 버튼의 크기 설정을 각각 스타일로 분리한다.

이번 단계에서 분리한 주요 리소스는 다음과 같습니다.

- `InputTextBoxStyle`: 새 할 일 입력창과 검색창의 공통 모양
- `FilterButtonBaseStyle`: 필터 버튼의 기본 모양
- `AllFilterButtonStyle`, `ActiveFilterButtonStyle`, `CompletedFilterButtonStyle`: 선택된 필터 버튼 강조
- `TodoListBoxItemStyle`: 목록 항목 컨테이너 정렬과 여백
- `TodoTitleTextBlockStyle`: 완료된 할 일의 회색 글자와 취소선
- `TodoItemTemplate`: 할 일 한 줄의 체크박스, 제목, 삭제 버튼 구조
- `AddButtonStyle`, `SmallButtonStyle`, `DeleteButtonStyle`: 버튼별 여백

핵심은 View인 `MainWindow.xaml`에는 화면 배치와 바인딩을 주로 남기고, 반복되는 시각 표현은 `TodoStyles.xaml`로 옮기는 것입니다. 이렇게 해두면 화면이 커져도 스타일을 한 곳에서 관리할 수 있습니다.

## 스타일 리소스 심화 학습 메모

스타일 리소스 심화 단계에서는 스타일 안에 직접 적혀 있던 색상과 간격 값을 이름 있는 리소스로 분리했습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. 반복해서 쓰는 색상을 `SolidColorBrush` 리소스로 정의한다.
2. 반복해서 쓰는 여백과 패딩을 `Thickness` 리소스로 정의한다.
3. 스타일의 `Foreground`, `Background`, `Margin`, `Padding` 값에서 직접 숫자나 색상 코드를 쓰지 않고 `StaticResource`를 참조한다.
4. `DataTemplate` 내부의 체크박스 여백처럼 항목 안에서 쓰는 값도 리소스로 분리한다.
5. 긴 `Binding` 표현은 줄을 나누고 들여쓰기를 맞춰 XAML 구조를 읽기 쉽게 정리한다.
6. 여러 버튼 스타일에서 공유할 설정은 `ActionButtonBaseStyle` 같은 기본 스타일로 만들고 `BasedOn`으로 확장한다.
7. 삭제처럼 의미가 강한 동작은 전용 색상 리소스를 만들어 일반 버튼과 구분한다.
8. 마지막으로 리소스, 스타일, 템플릿이 역할별로 읽히는지 전체 구조를 점검한다.

이번 단계에서 분리한 주요 리소스는 다음과 같습니다.

- `TodoTextBrush`: 기본 할 일 제목 색상
- `CompletedTodoTextBrush`: 완료된 할 일 제목 색상
- `FilterButtonBackgroundBrush`: 필터 버튼 기본 배경색
- `SelectedFilterButtonBackgroundBrush`: 선택된 필터 버튼 배경색
- `DeleteButtonForegroundBrush`: 삭제 버튼 글자색
- `InputTextBoxMargin`, `InputTextBoxPadding`: 입력창 여백과 안쪽 여백
- `FilterButtonPadding`, `SmallButtonPadding`, `AddButtonPadding`, `DeleteButtonPadding`: 버튼 종류별 안쪽 여백
- `TodoItemPadding`: 목록 항목의 안쪽 여백
- `TodoCheckBoxMargin`: 목록 항목 안에서 체크박스와 제목 사이의 여백

핵심은 스타일의 세부 값을 “그때그때 적는 값”이 아니라 “의미 있는 이름을 가진 리소스”로 관리하는 것입니다. 이렇게 하면 색상이나 간격을 바꿀 때 여러 XAML 요소를 찾아다니지 않고 리소스 값만 수정할 수 있습니다.

## 할 일 수정 기능 학습 메모

할 일 수정 단계에서는 기존 항목을 바로 바꾸지 않고, 먼저 편집 상태와 편집용 입력값을 ViewModel에 따로 두는 방식을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `EditingTodo`로 현재 수정 중인 할 일을 기억한다.
2. `EditTodoTitle`로 수정 입력창의 임시 제목을 관리한다.
3. `StartEditCommand`로 선택한 할 일을 편집 대상으로 지정하고 기존 제목을 복사한다.
4. `SaveEditCommand`로 `EditTodoTitle`을 원본 `TodoItem.Title`에 반영한다.
5. `CancelEditCommand`로 원본을 바꾸지 않고 편집 상태만 초기화한다.
6. `CanSaveEdit()`로 편집 대상이 없거나 제목이 공백이면 저장할 수 없게 한다.
7. `EditPanelStyle`의 `DataTrigger`로 편집 중일 때만 편집 영역을 보여준다.
8. 편집 중인 항목을 삭제하면 편집 상태도 함께 초기화한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- 수정 버튼을 누르면 해당 할 일이 편집 대상으로 선택된다.
- 수정 입력창에는 기존 제목이 복사된다.
- 저장하면 원본 제목이 바뀌고 자동 저장된다.
- 취소하면 원본 제목은 바뀌지 않고 저장도 발생하지 않는다.
- 공백 제목은 저장할 수 없다.
- 편집 중인 항목을 삭제하면 편집 패널 상태가 남지 않는다.

핵심은 “화면에 입력 중인 값”과 “실제 저장된 값”을 분리하는 것입니다. 이 구조 덕분에 저장과 취소가 명확해지고, ViewModel 테스트로 편집 흐름을 안정적으로 검증할 수 있습니다.

## 완료 항목 삭제와 전체 삭제 학습 메모

삭제 확장 단계에서는 개별 항목 삭제에서 한 걸음 더 나아가, 조건에 맞는 여러 항목을 한 번에 삭제하는 흐름을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `CanClearCompleted()`로 완료된 항목이 있을 때만 완료 항목 삭제 명령을 실행할 수 있게 한다.
2. `ClearCompletedCommand`로 `IsDone`이 `true`인 항목만 골라 삭제한다.
3. 삭제 대상 목록을 먼저 `ToList()`로 복사한 뒤 `Todos`에서 제거한다.
4. `CanClearAll()`로 할 일이 하나 이상 있을 때만 전체 삭제 명령을 실행할 수 있게 한다.
5. `ClearAllCommand`로 완료/미완료 상태와 상관없이 모든 항목을 삭제한다.
6. 여러 항목을 삭제한 뒤 `SaveTodos()`를 한 번 호출해 최종 상태를 저장한다.
7. 삭제 후 `ClearCompletedCommand.NotifyCanExecuteChanged()`와 `ClearAllCommand.NotifyCanExecuteChanged()`를 호출해 버튼 활성화 상태를 갱신한다.
8. 편집 중인 항목이 삭제될 수 있으므로 `EditingTodo`와 `EditTodoTitle`도 함께 초기화한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- 완료 항목 삭제는 완료된 할 일만 제거하고 진행 중 할 일은 남긴다.
- 완료된 항목이 없으면 완료 항목 삭제 명령을 실행할 수 없다.
- 전체 삭제는 모든 할 일을 제거하고 빈 목록을 저장한다.
- 전체 삭제 후에는 전체 삭제 명령을 다시 실행할 수 없다.
- 편집 중인 항목이 삭제되면 편집 상태도 함께 사라진다.

핵심은 “목록을 바꾸는 동작”과 “그 동작 이후의 UI 상태”를 함께 생각하는 것입니다. MVVM에서는 목록 데이터뿐 아니라 버튼 활성화, 편집 패널 상태, 자동 저장까지 ViewModel이 일관되게 관리해야 합니다.

## 입력 포커스와 키보드 사용성 개선 학습 메모

사용성 개선 단계에서는 사용자가 마우스를 덜 쓰고 자연스럽게 입력을 이어갈 수 있도록 포커스와 키보드 처리를 추가했습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. XAML 요소에 `x:Name`을 붙여 code-behind에서 특정 입력창을 참조한다.
2. `Loaded` 이벤트에서 앱이 열린 직후 새 할 일 입력창에 포커스를 준다.
3. 추가 버튼의 `Click` 이벤트에서 할 일 추가 후 다시 새 할 일 입력창으로 포커스를 보낸다.
4. 수정 입력창의 `IsVisibleChanged` 이벤트에서 수정 패널이 보일 때 포커스를 주고 기존 제목을 선택한다.
5. 수정 입력창의 `KeyDown` 이벤트에서 Enter는 `SaveEditCommand`, Esc는 `CancelEditCommand`를 실행한다.
6. 검색 입력창의 `KeyDown` 이벤트에서 Esc를 누르면 `ClearSearchCommand`를 실행한다.
7. 키 처리가 끝난 뒤 `e.Handled = true`로 같은 키 입력이 다른 동작으로 이어지지 않게 한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- 앱이 시작되면 새 할 일 입력창에 바로 입력할 수 있다.
- 할 일을 추가한 뒤에도 다음 할 일을 바로 입력할 수 있다.
- 수정 버튼을 누르면 수정 입력창에 포커스가 가고 기존 제목이 선택된다.
- 수정 입력창에서 Enter를 누르면 저장되고, Esc를 누르면 취소된다.
- 검색창에서 Esc를 누르면 검색어가 초기화되고 새 할 일 입력창으로 돌아간다.

핵심은 ViewModel이 앱의 상태와 명령을 관리하고, code-behind는 포커스처럼 화면 요소에 직접 닿아야 하는 작은 동작만 맡는 것입니다. 이렇게 나누면 MVVM 구조를 유지하면서도 실제 사용감은 훨씬 좋아집니다.

## 의존성 주입과 서비스 계층 정리 학습 메모

DI 단계에서는 View와 ViewModel이 필요한 객체를 직접 만들지 않고, 앱 시작 시 구성한 컨테이너에서 필요한 객체를 받아 쓰는 구조로 바꾸었습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `Microsoft.Extensions.DependencyInjection` 패키지를 추가해 DI 컨테이너를 사용할 준비를 한다.
2. `App.xaml`의 `StartupUri`를 제거해 WPF의 자동 창 생성을 끊는다.
3. `App.xaml.cs`에서 `ServiceCollection`을 만들고 앱에 필요한 타입을 등록한다.
4. `ITodoStorageService`는 `TodoStorageService`로 연결한다.
5. `MainWindowViewModel`과 `MainWindow`를 DI 컨테이너에 등록한다.
6. `OnStartup()`에서 `GetRequiredService<MainWindow>()`로 창을 꺼내고 `Show()`로 표시한다.
7. `MainWindow`는 생성자로 `MainWindowViewModel`을 주입받아 `DataContext`에 연결한다.
8. `MainWindowViewModel`의 기본 생성자를 제거해 `TodoStorageService`를 직접 만들지 않게 한다.
9. `OnExit()`에서 `serviceProvider.Dispose()`를 호출해 앱 종료 시 컨테이너를 정리한다.

이번 단계에서 확인한 주요 변화는 다음과 같습니다.

- 앱 시작 흐름이 `StartupUri` 자동 생성에서 `App.xaml.cs`의 명시적 생성 흐름으로 바뀌었다.
- `MainWindow`는 더 이상 `new MainWindowViewModel()`을 호출하지 않는다.
- `MainWindowViewModel`은 더 이상 `new TodoStorageService()`를 호출하지 않는다.
- 실제 앱에서는 DI 컨테이너가 `TodoStorageService`를 넣어준다.
- 테스트에서는 여전히 `FakeTodoStorageService`를 직접 넣어 ViewModel을 검증할 수 있다.

핵심은 “필요한 것을 직접 만들지 않고 생성자로 요구한다”는 점입니다. 이렇게 하면 객체 생성 책임이 `App`에 모이고, ViewModel은 어떤 저장 방식이 쓰이는지 몰라도 자기 역할에 집중할 수 있습니다.

## 게시와 배포 학습 메모

게시와 배포 단계에서는 개발 중인 WPF 앱을 다른 환경에서도 실행할 수 있는 결과물로 만드는 과정을 배웁니다.

배포 방식을 이해할 때는 다음 세 가지를 구분하면 좋습니다.

- Framework-dependent 배포: 실행 PC에 .NET Runtime이 설치되어 있어야 한다. 결과물 크기가 작다.
- Self-contained 배포: 실행 PC에 .NET Runtime이 없어도 실행할 수 있다. 결과물 크기가 크다.
- Single-file 배포: 여러 실행 파일과 라이브러리를 하나의 exe 중심으로 묶는다. 전달과 보관이 간단하다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. 배포 전에 `Release` 구성으로 빌드가 되는지 먼저 확인한다.
2. `Debug` 빌드는 개발과 디버깅에, `Release` 빌드는 배포용 결과물 생성에 사용한다.
3. 게시 결과물은 보통 `bin` 또는 별도 `publish` 폴더 아래에 생성되며, Git에 포함하지 않는다.
4. Framework-dependent 게시를 실행하면 앱 실행 파일, 앱 DLL, 의존 라이브러리, `.deps.json`, `.runtimeconfig.json` 같은 파일이 함께 생성된다.
5. Framework-dependent 결과물은 크기가 작지만, 실행 PC에 대상 버전의 .NET Desktop Runtime이 설치되어 있어야 한다.
6. Self-contained 게시를 실행하면 앱 파일뿐 아니라 .NET 런타임, WPF 관련 DLL, 언어별 리소스 폴더까지 함께 생성된다.
7. Self-contained 결과물은 크기가 크지만, 실행 PC에 .NET Runtime을 따로 설치하지 않아도 실행할 수 있다.
8. Single-file 게시를 실행하면 대부분의 관리 코드와 런타임 파일이 큰 exe 하나로 묶인다.
9. WPF 앱은 그래픽 처리와 네이티브 구성 요소 때문에 Single-file 게시 후에도 일부 DLL이 함께 남을 수 있다.
10. 게시 결과물은 명령이 성공했는지만 보지 말고, 각 게시 폴더의 `TodoWpf.exe`를 직접 실행해 앱이 뜨는지 확인한다.
11. Framework-dependent, Self-contained, Single-file 게시 결과는 실행 조건과 파일 구성이 다르므로 각각 따로 실행 확인하는 습관을 들인다.
12. Visual Studio 게시 프로필은 `Properties/PublishProfiles/*.pubxml` 파일로 저장된다.
13. `.pubxml`에는 게시 구성, 대상 프레임워크, 게시 위치, self-contained 여부 같은 설정이 들어간다.
14. 팀에서 같은 게시 설정을 공유하고 싶다면 `.pubxml`은 Git에 포함할 수 있고, 개인별 민감 설정은 포함하지 않는다.
15. 지금까지 만든 `publish` 결과물은 폴더째 복사하거나 압축해서 전달하는 방식이다.
16. 설치 파일은 사용자가 설치 마법사를 통해 앱을 설치하고 제거할 수 있게 만든 배포 방식이다.
17. MSIX는 Windows 앱 패키징 방식이며 설치, 제거, 업데이트 관리가 깔끔한 것이 장점이다.
18. 작은 학습 앱이나 내부 도구는 `publish` 폴더 배포만으로 충분한 경우가 많고, 사용자에게 정식 설치 경험을 제공해야 한다면 설치 파일이나 MSIX를 검토한다.

현재 프로젝트의 게시 결과를 비교하면 다음과 같습니다.

| 게시 방식 | 파일 수 | 전체 크기 | exe 크기 | 특징 |
|---|---:|---:|---:|---|
| Framework-dependent | 8개 | 약 0.51MB | 약 0.15MB | 작지만 .NET Desktop Runtime 필요 |
| Self-contained | 403개 | 약 139.72MB | 약 0.15MB | 런타임 포함, 파일 수 많음 |
| Single-file | 7개 | 약 133.58MB | 약 125.71MB | 큰 exe 중심, 일부 네이티브 DLL 동반 |

배포 방식을 선택할 때는 다음 기준으로 판단할 수 있습니다.

- 내 PC나 개발 환경에서만 실행할 때는 Framework-dependent 방식이 가볍다.
- 실행할 PC에 .NET Desktop Runtime이 설치되어 있는지 확실하지 않다면 Self-contained 방식이 안전하다.
- 파일을 최대한 적게 전달하고 싶다면 Single-file 방식이 편하다.
- WPF Single-file 게시 결과도 네이티브 DLL 몇 개가 함께 남을 수 있으므로, exe 하나만 복사하면 된다고 단정하지 않는다.
- 같은 게시 옵션을 반복해서 사용한다면 Visual Studio 게시 프로필이나 `.pubxml` 파일을 활용한다.
- 정식 설치/제거/업데이트 경험이 필요하다면 설치 파일이나 MSIX 패키징을 검토한다.

핵심은 배포를 “코드 작성의 마지막 버튼”으로만 보지 않고, 어떤 PC에서 어떤 조건으로 실행할 것인지 결정하는 과정으로 이해하는 것입니다.

## 설정 화면 또는 사용자 옵션 저장 학습 메모

설정 단계에서는 할 일 데이터와는 별도로, 사용자가 선택한 앱 옵션을 저장하고 다시 불러오는 구조를 배웠습니다.

이번 구현에서는 먼저 작은 사용자 옵션인 `검색어 기억`을 추가했습니다. 이 옵션은 사용자가 앱을 닫았다가 다시 열었을 때 이전 검색어를 유지할지 결정합니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `AppSettings` 모델을 만들어 사용자 설정 값을 한 곳에 모은다.
2. `RememberSearchText`로 검색어를 기억할지 여부를 저장한다.
3. `SearchText`로 기억할 검색어 값을 저장한다.
4. `IAppSettingsService` 인터페이스를 만들어 ViewModel이 파일 저장 방식에 직접 의존하지 않게 한다.
5. `AppSettingsService`에서 설정을 JSON 파일로 저장하고 불러온다.
6. 설정 파일은 사용자별 로컬 앱 데이터 폴더인 `%LocalAppData%\TodoWpf\appsettings.json`에 저장한다.
7. 설정 파일이 없거나 JSON이 깨져 있으면 기본 설정인 `new AppSettings()`를 사용한다.
8. `App.xaml.cs`에서 `IAppSettingsService`와 `AppSettingsService`를 DI 컨테이너에 등록한다.
9. `MainWindowViewModel`은 생성자로 `IAppSettingsService`를 주입받는다.
10. ViewModel 생성 시 저장된 설정을 불러와 `RememberSearchText`와 `SearchText` 초기값에 반영한다.
11. `RememberSearchText`나 `SearchText`가 바뀌면 설정 서비스를 통해 현재 옵션을 저장한다.
12. XAML의 `CheckBox.IsChecked`를 `RememberSearchText`에 바인딩해 화면에서 옵션을 켜고 끌 수 있게 한다.
13. 테스트에서는 실제 JSON 파일을 만들지 않고 `FakeAppSettingsService`로 설정 저장 동작을 검증한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- `검색어 기억`을 켜고 검색어를 입력하면 설정 JSON에 검색어가 저장된다.
- 앱을 다시 실행하면 저장된 검색어가 검색창에 다시 표시된다.
- `검색어 기억`을 끄면 저장할 검색어 값은 빈 문자열로 정리된다.
- 할 일 목록 저장 파일과 사용자 설정 파일은 서로 다른 JSON 파일로 관리된다.
- ViewModel 단위 테스트는 실제 파일 시스템에 의존하지 않고 설정 동작을 검증할 수 있다.

핵심은 “사용자 데이터”와 “사용자 설정”을 구분하는 것입니다. 할 일 목록은 앱의 주요 데이터이고, 검색어 기억 같은 값은 앱 사용 방식을 조정하는 설정입니다. 이 둘을 별도 모델과 별도 서비스로 나누면 이후 테마, 기본 필터, 창 크기 같은 옵션도 같은 구조로 자연스럽게 확장할 수 있습니다.

## 설정 화면 분리 학습 메모

설정 화면 분리 단계에서는 메인 화면에 있던 사용자 옵션을 별도 창으로 옮기고, 설정 창에서 편집한 값을 저장할 때만 메인 ViewModel에 반영하는 흐름을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `SettingsWindowViewModel`을 만들어 설정 창 전용 상태를 관리한다.
2. 설정 창 ViewModel은 `AppSettings`를 받아 화면에서 편집할 값으로 복사한다.
3. `ToAppSettings()`로 설정 창의 현재 값을 다시 `AppSettings` 모델로 변환한다.
4. `SettingsWindow.xaml`을 만들어 `검색어 기억`, 검색어 입력, 저장, 취소 UI를 별도 창에 배치한다.
5. 설정 창의 `WindowStartupLocation`을 `CenterOwner`로 지정해 메인 창 기준 중앙에 열리게 한다.
6. 저장과 취소 버튼은 `Command`로 ViewModel 상태를 바꾸고, `Click` 이벤트에서 `DialogResult`를 설정해 창을 닫는다.
7. `App.xaml.cs`에서 `SettingsWindow`를 DI 컨테이너에 등록한다.
8. `MainWindow`는 `IServiceProvider`를 주입받아 설정 창을 DI에서 가져온다.
9. `MainWindowViewModel.ToAppSettings()`로 현재 메인 설정 상태를 설정 창에 전달한다.
10. 설정 창에서 `ShowDialog()` 결과가 `true`일 때만 `ApplyAppSettings()`로 메인 ViewModel에 반영한다.
11. `ApplyAppSettings()`에서는 검색어를 먼저 반영한 뒤 `RememberSearchText`를 반영해 속성 변경 저장 로직이 이전 검색어를 덮지 않게 한다.
12. 메인 화면에 있던 `검색어 기억` 체크박스를 제거해 옵션 편집 책임을 설정 창으로 모은다.
13. `MainWindowViewModel`의 설정 변환/적용 메서드와 `SettingsWindowViewModel`의 저장/취소 흐름을 단위 테스트로 검증한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- 메인 화면의 `설정` 버튼을 누르면 별도 설정 창이 열린다.
- 설정 창에서 `검색어 기억`과 검색어를 수정한 뒤 저장하면 메인 화면 검색어에 반영된다.
- 설정 창에서 취소하면 메인 화면 설정은 바뀌지 않는다.
- 설정 옵션은 메인 화면이 아니라 설정 창에서만 편집된다.
- 설정 창 ViewModel은 실제 창을 띄우지 않고도 단위 테스트로 검증할 수 있다.

핵심은 “설정을 편집하는 화면 상태”와 “앱에 실제 적용된 설정 상태”를 분리하는 것입니다. 설정 창에서 값을 바꾸는 동안에는 임시 ViewModel만 변경하고, 사용자가 저장을 선택했을 때만 메인 ViewModel과 JSON 저장소에 반영하면 취소 동작이 자연스럽고 안전해집니다.

## 기본 필터 저장 학습 메모

기본 필터 저장 단계에서는 설정 화면에 앱 시작 시 사용할 필터 옵션을 추가하고, 저장된 설정을 다음 실행 때 초기 필터로 적용하는 흐름을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `AppSettings`에 `DefaultFilter` 속성을 추가해 시작 필터 설정을 저장한다.
2. `DefaultFilter`의 기본값은 `TodoFilter.All`로 두어 기존 동작을 유지한다.
3. 앱 시작 시 `appSettingsService.Load()`로 설정을 읽은 뒤 `selectedFilter = appSettings.DefaultFilter`로 초기 필터를 반영한다.
4. 생성자에서는 `SelectedFilter = ...` 대신 필드인 `selectedFilter = ...`를 사용해 초기화 중 불필요한 변경 반응을 피한다.
5. `MainWindowViewModel.ToAppSettings()`에 `DefaultFilter = SelectedFilter`를 포함해 현재 필터 상태를 설정 창으로 전달한다.
6. `ApplyAppSettings()`에서 설정 창이 돌려준 `DefaultFilter`를 `SelectedFilter`에 반영한다.
7. `SaveAppSettings()`에도 `appSettings.DefaultFilter = SelectedFilter`를 추가해 실제 JSON 저장값에 포함한다.
8. `SettingsWindowViewModel`에 `DefaultFilter` 속성을 추가해 설정 창에서 선택한 필터를 임시 상태로 관리한다.
9. 설정 창의 `ComboBoxItem.Tag`에 `TodoFilter.All`, `TodoFilter.Active`, `TodoFilter.Completed` enum 값을 넣는다.
10. `ComboBox.SelectedValue`를 `DefaultFilter`에 바인딩하고 `SelectedValuePath="Tag"`로 실제 선택값을 enum으로 전달한다.
11. 설정 항목이 늘어나면서 창 높이를 조정해 검색어 입력창이 가려지지 않게 한다.
12. 기본 `System.Text.Json`은 enum을 문자열이 아니라 숫자로 저장한다는 점을 확인한다.
13. `MainWindowViewModelTests`와 `SettingsWindowViewModelTests`에 기본 필터 복사, 적용, 저장 테스트를 보강한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- 설정 창에서 `시작 필터`를 선택할 수 있다.
- `완료`를 기본 필터로 저장하면 앱을 다시 실행했을 때 완료 필터가 선택된 상태로 시작된다.
- `DefaultFilter`는 설정 JSON에 숫자 값으로 저장될 수 있다.
- `ToAppSettings()`, `ApplyAppSettings()`, `SaveAppSettings()`에 모두 새 설정값을 포함해야 누락 없이 흐름이 이어진다.
- 설정 창 ViewModel은 필터 선택값도 `AppSettings`에서 복사하고 다시 `AppSettings`로 돌려준다.

핵심은 새 설정 항목을 추가할 때 “모델, 메인 ViewModel, 설정 창 ViewModel, XAML, 테스트”를 한 줄로 이어서 생각하는 것입니다. 설정 모델에 속성만 추가하면 끝나는 것이 아니라, 값을 읽고 보여주고 수정하고 저장하고 검증하는 모든 지점에 같은 설정이 통과해야 합니다.

## 테스트 정리 학습 메모

테스트 정리 단계에서는 이미 통과하던 테스트의 동작은 유지하면서, 반복되는 준비 코드를 helper 메서드로 모아 테스트 본문을 더 읽기 쉽게 만드는 흐름을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `CreateViewModel()` helper를 추가해 `MainWindowViewModel` 생성 방식을 한 곳에 모은다.
2. 기본 테스트에서는 `CreateViewModel()` 또는 `CreateViewModel(storage)`를 사용한다.
3. 설정 서비스가 필요한 테스트에서는 `CreateViewModel(storage, appSettingsService)`처럼 helper의 두 번째 인자를 사용한다.
4. helper 호출 뒤에는 C# 객체 초기화자를 붙일 수 없으므로, 속성 설정은 별도 대입문으로 나눈다.
5. `CreateTodo()` helper를 추가해 반복되는 `TodoItem` 테스트 데이터 생성을 한 곳에 모은다.
6. 완료 상태가 필요한 데이터는 `CreateTodo("완료된 할 일", isDone: true)`처럼 이름 있는 인자를 사용해 의도를 드러낸다.
7. 테스트 본문에는 준비 데이터, 실행, 검증이 잘 보이도록 생성 세부 코드를 줄인다.
8. 기능 코드는 바꾸지 않고 테스트 코드만 정리한 뒤 전체 테스트를 실행해 동작이 유지되는지 확인한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- `new MainWindowViewModel(...)`은 `CreateViewModel()` helper 내부에만 남았다.
- `new TodoItem`은 `CreateTodo()` helper 내부에만 남았다.
- 필터, 검색, 수정, 삭제, 설정 관련 테스트가 같은 ViewModel 생성 방식을 사용하게 됐다.
- 객체 초기화자가 필요한 테스트는 helper 호출 후 속성 대입으로 안전하게 정리했다.
- 전체 테스트 통과로 리팩터링이 기존 동작을 깨지 않았음을 확인했다.

핵심은 테스트 리팩터링도 기능 코드 리팩터링과 똑같이 “작게 바꾸고 자주 확인하는 것”입니다. 테스트 코드가 짧아질수록 어떤 조건을 준비하고 어떤 명령을 실행하며 무엇을 검증하는지가 더 잘 보입니다.

## TodoFilter 구조 정리 학습 메모

구조 정리 단계에서는 `TodoFilter` enum을 `ViewModels`에서 `Models`로 옮기며, 프로젝트 안의 의존 방향을 더 자연스럽게 정리하는 흐름을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `TodoFilter`는 화면 동작만의 값이 아니라 할 일 필터 상태를 표현하는 공통 모델 값으로 볼 수 있다.
2. `AppSettings`가 `DefaultFilter`를 저장하려면 `TodoFilter`를 알아야 한다.
3. `AppSettings`는 `Models`에 있으므로, `TodoFilter`가 `ViewModels`에 있으면 `Models`가 `ViewModels`를 바라보는 구조가 된다.
4. 공통 값인 `TodoFilter`를 `Models/TodoFilter.cs`로 옮겨 의존 방향을 `ViewModels -> Models`로 정리한다.
5. `AppSettings.cs`에서 불필요해진 `using TodoWpf.ViewModels;`를 제거한다.
6. `MainWindowViewModel`과 `SettingsWindowViewModel`은 `using TodoWpf.Models;`를 통해 `TodoFilter`를 사용한다.
7. XAML에서 enum을 참조하는 `x:Static` 경로도 `models:TodoFilter`로 바꾼다.
8. `MainWindow.xaml`, `SettingsWindow.xaml`, `Styles/TodoStyles.xaml`처럼 enum을 참조하는 XAML 파일을 함께 점검한다.
9. 더 이상 사용하지 않는 XAML namespace는 제거해 파일 상단을 단정하게 유지한다.
10. 빌드와 테스트를 실행해 C# 코드, XAML 컴파일, ViewModel 테스트가 모두 같은 enum 위치를 바라보는지 확인한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- `TodoFilter` 정의는 `Models/TodoFilter.cs` 한 곳에만 남았다.
- `AppSettings`가 더 이상 `ViewModels` namespace에 의존하지 않는다.
- 필터 버튼과 스타일 `DataTrigger`가 `models:TodoFilter`를 참조한다.
- 설정 창의 시작 필터 선택도 `models:TodoFilter`를 참조한다.
- 빌드와 테스트 통과로 enum 이동 후에도 기존 동작이 유지됨을 확인했다.

핵심은 “어느 계층의 코드가 어느 계층을 알아도 되는가”를 보는 것입니다. ViewModel은 화면 상태를 만들기 위해 Model을 알아도 자연스럽지만, Model이 ViewModel을 알아야 한다면 보통 위치를 다시 생각해볼 신호입니다.

## 테마 옵션 추가 학습 메모

테마 옵션 추가 단계에서는 설정 화면에서 밝은 테마와 어두운 테마를 선택하고, 선택한 값을 저장한 뒤 앱 리소스에 적용하는 흐름을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `AppTheme` enum을 만들어 `Light`, `Dark` 테마 값을 표현한다.
2. `AppSettings`에 `Theme` 속성을 추가해 선택한 테마를 사용자 설정 JSON에 저장한다.
3. `SettingsWindowViewModel`에 `Theme` 속성을 추가해 설정 창에서 테마 값을 편집한다.
4. `SettingsWindow.xaml`에 테마 선택 `ComboBox`를 추가하고 `ComboBoxItem.Tag`에 `models:AppTheme` 값을 연결한다.
5. `MainWindowViewModel.ToAppSettings()`, `ApplyAppSettings()`, `SaveAppSettings()`에 `Theme` 값을 포함한다.
6. `LightTheme.xaml`, `DarkTheme.xaml` 리소스 딕셔너리를 만들어 테마별 색상 brush를 분리한다.
7. `App.xaml`에서 기본 테마 리소스를 `TodoStyles.xaml`보다 먼저 병합한다.
8. `TodoStyles.xaml`의 고정 색상 값을 `PrimaryTextBrush`, `ButtonBackgroundBrush`, `InputBackgroundBrush` 같은 테마 brush 참조로 바꾼다.
9. 앱 실행 중 리소스가 바뀔 수 있는 값은 `DynamicResource`로 참조한다.
10. `IThemeService`와 `ThemeService`를 만들어 현재 테마 리소스 딕셔너리를 교체하는 책임을 서비스로 분리한다.
11. `MainWindowViewModel`은 생성자에서 저장된 테마를 적용하고, `Theme`이 바뀔 때 `themeService.ApplyTheme()`을 호출한다.
12. 테스트에서는 실제 WPF 리소스를 바꾸지 않도록 `FakeThemeService`를 사용한다.
13. `MainWindowViewModelTests`와 `SettingsWindowViewModelTests`에 테마 복사, 적용, 저장 테스트를 보강한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- 설정 창에서 `밝게`와 `어둡게` 테마를 선택할 수 있다.
- 테마 설정을 저장하면 메인 화면이 선택한 테마 색상으로 바뀐다.
- 앱을 다시 실행해도 저장된 테마가 유지된다.
- `TextBox`, `Button`, 할 일 제목, 완료된 항목 색상이 테마 리소스를 따른다.
- 빌드와 전체 테스트 통과로 설정 저장, 테마 적용, ViewModel 테스트가 함께 유지됨을 확인했다.

이번 단계에서 주의할 점도 확인했습니다. `Grid`에는 `Foreground` 속성이 없으므로, Grid 아래 텍스트 색상을 상속시키고 싶을 때는 `TextElement.Foreground` attached property를 사용합니다.

```xml
<Grid TextElement.Foreground="{DynamicResource PrimaryTextBrush}">
```

핵심은 테마를 단순히 색상 몇 개를 바꾸는 작업으로 보지 않고, “사용자 설정, 리소스 딕셔너리, 서비스, ViewModel, 테스트”가 연결된 기능으로 보는 것입니다. 이렇게 나누면 이후 테마 종류가 늘어나거나 색상 리소스가 많아져도 구조를 크게 흔들지 않고 확장할 수 있습니다.

## 입력 검증 개선 학습 메모

입력 검증 개선 단계에서는 새 할 일 추가와 기존 할 일 수정에서 같은 제목 규칙을 사용하도록 정리했습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `MaxTodoTitleLength` 상수를 두어 제목 길이 제한을 한 곳에서 관리한다.
2. `NormalizeTodoTitle()` helper로 앞뒤 공백 제거 규칙을 재사용한다.
3. `IsValidTodoTitle()` helper로 빈 제목과 100자 초과 제목을 같은 방식으로 검사한다.
4. `GetTodoTitleErrorMessage()` helper로 검증 실패 이유를 사용자에게 보여줄 문자열로 만든다.
5. `CanAddTodo()`와 `CanSaveEdit()`에서 같은 검증 규칙을 사용해 버튼 활성화 조건을 정리한다.
6. `AddTodo()`와 `SaveEdit()`에서도 한 번 더 유효성을 확인해 명령이 직접 실행되어도 잘못된 값이 저장되지 않게 한다.
7. `NewTodoTitleErrorMessage`, `EditTodoTitleErrorMessage` 속성을 추가해 새 입력창과 수정 입력창의 오류 상태를 각각 관리한다.
8. `OnNewTodoTitleChanged()`, `OnEditTodoTitleChanged()`에서 입력값이 바뀔 때마다 오류 메시지를 갱신한다.
9. `MainWindow.xaml`에 오류 메시지 `TextBlock`을 추가하고 `ErrorTextBlockStyle`로 공통 스타일을 적용한다.
10. ViewModel 테스트에서 공백 제거, 빈 제목 차단, 긴 제목 차단, 오류 메시지 표시와 초기화를 함께 검증한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- 새 할 일 제목의 앞뒤 공백은 저장 전에 제거된다.
- 수정한 할 일 제목의 앞뒤 공백도 저장 전에 제거된다.
- 빈 제목이나 공백만 있는 제목은 추가하거나 저장할 수 없다.
- 100자를 초과하는 제목은 추가하거나 저장할 수 없다.
- 잘못된 제목을 입력하면 입력창 아래에 오류 메시지가 표시된다.
- 다시 올바른 제목을 입력하면 오류 메시지가 사라진다.
- 전체 테스트 통과로 추가와 수정 양쪽의 검증 규칙이 유지됨을 확인했다.

핵심은 입력 검증을 XAML의 화면 제약으로만 처리하지 않고, ViewModel의 명령 실행 규칙으로도 보호하는 것입니다. 버튼 비활성화는 사용자 경험이고, ViewModel의 재검증은 데이터가 잘못 저장되지 않게 하는 안전장치입니다.

## 데이터 모델 확장 학습 메모

데이터 모델 확장 단계에서는 `TodoItem`에 작성일, 수정일, 마감일 정보를 추가하고 화면과 테스트까지 연결하는 흐름을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. `TodoItem`에 `CreatedAt` 속성을 추가해 할 일이 만들어진 시각을 저장한다.
2. 새 할 일을 추가할 때 `CreatedAt = DateTime.Now`를 명시해 생성 시점을 ViewModel에서 정한다.
3. 할 일 항목 템플릿에서 `CreatedAt`을 바인딩해 목록 화면에 작성일을 표시한다.
4. `UpdatedAt`은 수정된 적이 없는 항목도 표현할 수 있도록 `DateTime?`로 둔다.
5. 제목을 수정할 때 `UpdatedAt = DateTime.Now`를 설정한다.
6. 완료 상태가 바뀔 때도 `UpdatedAt`을 갱신해 체크 변경을 항목 변경으로 다룬다.
7. `DueDate`는 모든 할 일에 필요한 값이 아니므로 `DateTime?`로 추가한다.
8. XAML에서는 `UpdatedAt`, `DueDate`가 `null`일 때 해당 줄을 숨기는 스타일을 사용한다.
9. 기존 JSON에 새 날짜 속성이 없어도 앱이 깨지지 않는지 실행해서 확인한다.
10. ViewModel 테스트에서 새 할 일의 작성일, 제목 수정 시 수정일, 완료 상태 변경 시 수정일을 검증한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- 새 할 일을 추가하면 `CreatedAt`이 저장된다.
- 제목을 수정하면 `UpdatedAt`이 저장된다.
- 완료 상태를 바꾸면 `UpdatedAt`이 저장된다.
- 목록 화면에서 작성일, 수정일, 마감일을 메타 정보로 표시한다.
- 수정일과 마감일이 없는 항목은 빈 줄을 보여주지 않는다.
- 기존 JSON 데이터에 새 속성이 없어도 앱 실행은 유지된다.
- 마이그레이션은 이번 학습 범위에서 제외했다.
- 전체 테스트 통과로 날짜 메타데이터 동작이 유지됨을 확인했다.

이번 단계에서 주의할 점도 확인했습니다. `TodoItem`의 속성이 바뀔 때마다 `PropertyChanged`가 발생하고, ViewModel이 이를 받아 자동 저장합니다. 그래서 제목 수정처럼 `Title`과 `UpdatedAt`을 함께 바꾸는 동작에서는 저장이 중복 호출될 수 있습니다. 이번 구현에서는 `isUpdatingTodo` 플래그로 한 번의 사용자 동작을 하나의 저장 흐름으로 묶었습니다.

핵심은 모델에 속성을 추가하는 일이 단순히 C# property 하나를 늘리는 작업으로 끝나지 않는다는 점입니다. 저장 JSON, ViewModel 생성/수정 규칙, XAML 표시, 기존 데이터 호환성, 테스트까지 함께 보아야 실제 기능으로 안정됩니다.

## 정렬 기능 학습 메모

정렬 기능 단계에서는 `Todos` 원본 컬렉션을 직접 재배열하지 않고, 화면에 연결된 `TodosView`에 정렬 조건을 적용하는 흐름을 배웠습니다.

이번 구현에서 배운 흐름은 다음과 같습니다.

1. 처음에는 `SelectedSort`를 문자열로 두어 최신순과 오래된순 정렬을 작게 구현한다.
2. `TodosView.SortDescriptions`에 `CreatedAt` 정렬 조건을 추가해 작성일 기준 정렬을 적용한다.
3. `SelectedSort`가 바뀔 때 `ApplySort()`를 호출해 화면 목록 정렬을 갱신한다.
4. `MainWindow.xaml`에 정렬 `ComboBox`를 추가하고 `SelectedSort`와 양방향 바인딩한다.
5. 문자열 비교 방식에서 `TodoSortOption` enum으로 바꿔 오타에 강한 구조로 정리한다.
6. XAML의 `ComboBoxItem.Tag`에 `TodoSortOption` 값을 넣고 `SelectedValuePath="Tag"`로 enum 값을 바인딩한다.
7. `TitleAscending` 옵션을 추가해 제목 오름차순 정렬을 구현한다.
8. `IncompleteFirst` 옵션을 추가해 미완료 항목을 먼저 표시한다.
9. `IncompleteFirst`에서는 `IsDone` 오름차순 정렬 뒤에 `CreatedAt` 내림차순 정렬을 추가해 같은 그룹 안에서는 최신순을 유지한다.
10. ViewModel 테스트에서 최신순, 오래된순, 제목순, 미완료순, 완료 상태 변경 후 재정렬을 검증한다.

이번 단계에서 확인한 주요 동작은 다음과 같습니다.

- 기본 정렬은 작성일 최신순이다.
- `오래된순`을 선택하면 작성일 오름차순으로 표시된다.
- `제목순`을 선택하면 제목 오름차순으로 표시된다.
- `미완료순`을 선택하면 `IsDone == false` 항목이 먼저 표시된다.
- 미완료/완료 그룹 안에서는 작성일 최신순이 유지된다.
- 체크박스로 완료 상태를 바꾸면 `TodosView.Refresh()`를 통해 정렬 결과도 다시 갱신된다.
- 전체 테스트 통과로 필터, 검색, 정렬이 함께 동작함을 확인했다.

이번 단계에서 주의할 점도 확인했습니다. `bool` 값은 오름차순 정렬에서 `false`가 `true`보다 먼저 옵니다. 그래서 `IsDone`을 `ListSortDirection.Ascending`으로 정렬하면 미완료 항목이 먼저 표시됩니다.

```csharp
TodosView.SortDescriptions.Add(
    new SortDescription(nameof(TodoItem.IsDone), ListSortDirection.Ascending));
```

핵심은 정렬을 데이터 자체의 저장 순서와 분리하는 것입니다. `ObservableCollection<T>`는 원본 데이터 역할을 하고, `CollectionView`는 사용자가 현재 보고 싶은 필터와 정렬 상태를 표현합니다.

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
9. 스타일과 리소스 딕셔너리 분리
10. 할 일 수정 기능
11. 완료 항목 삭제와 전체 삭제
12. 입력 포커스와 키보드 사용성 개선
13. 의존성 주입과 서비스 계층 분리
14. 스타일 리소스 심화
15. 게시, 설치 파일, MSIX 배포
16. 설정 화면 또는 사용자 옵션 저장
17. 설정 화면 분리
18. 기본 필터 저장
19. 테스트 정리와 중복 줄이기
20. `TodoFilter` enum 위치 재검토와 구조 정리
21. 설정 화면 확장: 테마 옵션 추가
22. 입력 검증 개선: 빈 제목, 긴 제목, 공백 처리 정리
23. 데이터 모델 확장: 생성일, 수정일, 마감일 추가
24. 정렬 기능: 생성일순, 완료 여부순, 제목순 정렬

## 다음 실습 후보

1. 마감일 입력 UI: DatePicker로 할 일 마감일 지정
2. 서비스 테스트 심화: JSON 저장소와 설정 저장소 테스트 분리
3. 정렬 설정 저장: 마지막으로 선택한 정렬 옵션 기억

기능을 추가할 때는 한 번에 많은 구조를 바꾸기보다, ViewModel 속성 하나, Command 하나, XAML 바인딩 하나처럼 작은 단위로 이해하면서 확장하는 것을 권장합니다.
