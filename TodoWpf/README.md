# TodoWpf

WPF와 MVVM 패턴을 학습하기 위한 간단한 할 일 관리 앱입니다. 현재 앱은 `CommunityToolkit.Mvvm`과 `Microsoft.Extensions.DependencyInjection`을 사용해 View, ViewModel, Model, Service의 역할을 나누고, 데이터 바인딩과 커맨드 기반 UI 흐름을 연습할 수 있도록 구성되어 있습니다.

## 학습 목표

- WPF 프로젝트 구조 이해
- XAML을 이용한 화면 구성
- `DataContext`와 데이터 바인딩 흐름 이해
- MVVM에서 ViewModel이 UI 상태와 동작을 관리하는 방식 학습
- `ObservableObject`, `[ObservableProperty]`, `[RelayCommand]` 사용법 익히기
- `ObservableCollection<T>`를 이용한 목록 UI 갱신 이해
- 생성자 주입을 통한 ViewModel과 Service 연결 이해

## 현재 기능

- 새 할 일 입력
- Enter 키 또는 추가 버튼으로 할 일 추가
- 앱 시작 및 할 일 추가 후 입력창 포커스 유지
- 체크박스로 완료 상태 변경
- 완료된 항목에 취소선 표시
- 삭제 버튼으로 할 일 제거
- 완료 항목 삭제 및 전체 삭제
- 할 일 제목 수정, 저장, 취소
- 수정 입력창에서 Enter 저장, Esc 취소
- 할 일 제목 앞뒤 공백 제거
- 빈 제목 및 100자 초과 제목 입력 제한
- 새 할 일과 수정 입력창의 검증 오류 메시지 표시
- 할 일 작성일, 수정일, 마감일 데이터 관리
- 목록에서 작성일, 수정일, 마감일 표시
- 할 일 목록 JSON 자동 저장 및 앱 시작 시 불러오기
- 전체 / 진행 중 / 완료 필터
- 할 일 제목 검색 및 Esc/버튼으로 검색어 초기화
- 최신순, 오래된순, 제목순, 미완료순 정렬
- 설정 창에서 시작 필터, 검색어 기억, 테마 옵션 관리
- 밝은 테마 / 어두운 테마 적용 및 저장
- ViewModel 단위 테스트
- 스타일과 리소스 딕셔너리 분리
- DI 컨테이너를 통한 ViewModel과 Service 생성

## 프로젝트 구조

```text
learning-wpf-todolist/
├─ TodoWpf/
│  ├─ App.xaml
│  ├─ App.xaml.cs
│  ├─ MainWindow.xaml
│  ├─ MainWindow.xaml.cs
│  ├─ Models/
│  │  ├─ AppSettings.cs
│  │  ├─ AppTheme.cs
│  │  ├─ TodoFilter.cs
│  │  ├─ TodoSortOption.cs
│  │  └─ TodoItems.cs
│  ├─ Services/
│  │  ├─ AppSettingsService.cs
│  │  ├─ ThemeService.cs
│  │  └─ TodoStorageService.cs
│  ├─ Styles/
│  │  ├─ Themes/
│  │  │  ├─ DarkTheme.xaml
│  │  │  └─ LightTheme.xaml
│  │  └─ TodoStyles.xaml
│  ├─ ViewModels/
│  │  ├─ MainWindowViewModel.cs
│  │  └─ SettingsWindowViewModel.cs
│  └─ TodoWpf.csproj
├─ TodoWpf.Tests/
│  ├─ MainWindowViewModelTests.cs
│  └─ TodoWpf.Tests.csproj
└─ docs/
   └─ wpf-csharp-development-guide.md
```

## 주요 파일

- `App.xaml`: 앱 전역 리소스를 정의합니다.
- `App.xaml.cs`: DI 컨테이너 구성, 앱 시작 시 `MainWindow` 생성, 앱 종료 시 컨테이너 정리를 담당합니다.
- `MainWindow.xaml`: 화면 레이아웃과 바인딩을 정의합니다.
- `MainWindow.xaml.cs`: Window 초기화, `DataContext` 설정, 입력 포커스와 키보드 사용성 처리를 담당합니다.
- `ViewModels/MainWindowViewModel.cs`: 할 일 목록, 입력값, 입력 검증, 날짜 메타데이터, 추가/삭제/수정/필터/검색/정렬 명령을 관리합니다.
- `ViewModels/SettingsWindowViewModel.cs`: 설정 창에서 편집하는 사용자 옵션 상태를 관리합니다.
- `Models/AppSettings.cs`: 검색어 기억, 시작 필터, 테마 같은 사용자 설정 값을 정의합니다.
- `Models/AppTheme.cs`: 밝은 테마, 어두운 테마 값을 정의합니다.
- `Models/TodoFilter.cs`: 전체, 진행 중, 완료 필터 값을 정의합니다.
- `Models/TodoSortOption.cs`: 최신순, 오래된순, 제목순, 미완료순 정렬 옵션을 정의합니다.
- `Models/TodoItems.cs`: 할 일 제목, 완료 여부, 작성일, 수정일, 마감일 같은 항목 데이터를 정의합니다.
- `Services/AppSettingsService.cs`: 사용자 설정을 JSON 파일로 저장하고 불러옵니다.
- `Services/ThemeService.cs`: 설정된 테마에 맞게 앱 리소스 딕셔너리를 교체합니다.
- `Services/TodoStorageService.cs`: 할 일 목록을 JSON 파일로 저장하고 불러옵니다.
- `Styles/TodoStyles.xaml`: 화면에서 재사용하는 스타일과 할 일 항목 템플릿을 정의합니다.
- `Styles/Themes/LightTheme.xaml`: 밝은 테마 색상 리소스를 정의합니다.
- `Styles/Themes/DarkTheme.xaml`: 어두운 테마 색상 리소스를 정의합니다.
- `TodoWpf.Tests/MainWindowViewModelTests.cs`: ViewModel의 추가, 삭제, 저장, 필터, 검색 동작을 검증합니다.
- `TodoWpf.csproj`: WPF, .NET, NuGet 패키지 설정을 관리합니다.

## 실행 방법

Windows에서 .NET SDK가 설치되어 있어야 합니다.

```powershell
cd D:\Dev\WPF\learning-wpf-todolist\TodoWpf
dotnet restore
dotnet run
```

Visual Studio 또는 Rider에서 `TodoWpf.csproj`를 열어 실행할 수도 있습니다.

할 일 데이터는 사용자 로컬 앱 데이터 폴더의 `TodoWpf\todos.json` 파일에 저장됩니다.

## 테스트 실행

프로젝트 루트에서 다음 명령으로 단위 테스트를 실행할 수 있습니다.

```powershell
cd D:\Dev\WPF\learning-wpf-todolist
dotnet test
```

현재 테스트는 `MainWindowViewModel`을 대상으로 하며, 실제 JSON 파일을 쓰지 않도록 `FakeTodoStorageService`를 사용합니다. 추가, 삭제, 완료 항목 삭제, 전체 삭제, 저장, 필터, 검색, 정렬, 수정 시작, 수정 저장, 수정 취소, 입력 검증, 날짜 메타데이터 동작을 검증합니다.

## 사용 기술

- C#
- WPF
- XAML
- .NET `net10.0-windows`
- CommunityToolkit.Mvvm `8.4.2`
- Microsoft.Extensions.DependencyInjection `10.0.11`

## 학습 문서

- [WPF C# 개발환경 가이드](../docs/wpf-csharp-development-guide.md): 개발 환경 선택부터 MVVM 할 일 앱 구현 흐름까지 정리한 최초 학습 노트입니다.

## 다음 학습 과제

- 마감일 입력 UI
- 서비스 테스트 심화

## 메모

이 프로젝트는 학습용이므로 기능을 크게 늘리기보다, WPF와 MVVM의 기본 개념을 작은 단위로 반복해서 익히는 것을 우선합니다.
