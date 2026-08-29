# TodoWpf

WPF와 MVVM 패턴을 학습하기 위한 간단한 할 일 관리 앱입니다. 현재 앱은 `CommunityToolkit.Mvvm`을 사용해 View, ViewModel, Model의 역할을 나누고, 데이터 바인딩과 커맨드 기반 UI 흐름을 연습할 수 있도록 구성되어 있습니다.

## 학습 목표

- WPF 프로젝트 구조 이해
- XAML을 이용한 화면 구성
- `DataContext`와 데이터 바인딩 흐름 이해
- MVVM에서 ViewModel이 UI 상태와 동작을 관리하는 방식 학습
- `ObservableObject`, `[ObservableProperty]`, `[RelayCommand]` 사용법 익히기
- `ObservableCollection<T>`를 이용한 목록 UI 갱신 이해

## 현재 기능

- 새 할 일 입력
- Enter 키 또는 추가 버튼으로 할 일 추가
- 체크박스로 완료 상태 변경
- 완료된 항목에 취소선 표시
- 삭제 버튼으로 할 일 제거

## 프로젝트 구조

```text
TodoWpf/
├─ App.xaml
├─ App.xaml.cs
├─ MainWindow.xaml
├─ MainWindow.xaml.cs
├─ Models/
│  └─ TodoItems.cs
├─ ViewModels/
│  └─ MainWindowViewModel.cs
└─ TodoWpf.csproj
```

## 주요 파일

- `MainWindow.xaml`: 화면 레이아웃과 바인딩을 정의합니다.
- `MainWindow.xaml.cs`: Window 초기화와 `DataContext` 설정을 담당합니다.
- `ViewModels/MainWindowViewModel.cs`: 할 일 목록, 입력값, 추가/삭제 명령을 관리합니다.
- `Models/TodoItems.cs`: 할 일 항목의 데이터와 변경 알림 속성을 정의합니다.
- `TodoWpf.csproj`: WPF, .NET, NuGet 패키지 설정을 관리합니다.

## 실행 방법

Windows에서 .NET SDK가 설치되어 있어야 합니다.

```powershell
cd D:\Dev\WPF\learning-wpf-todolist\TodoWpf
dotnet restore
dotnet run
```

Visual Studio 또는 Rider에서 `TodoWpf.csproj`를 열어 실행할 수도 있습니다.

## 사용 기술

- C#
- WPF
- XAML
- .NET `net10.0-windows`
- CommunityToolkit.Mvvm `8.4.2`

## 다음 학습 과제

- 할 일 수정 기능 추가
- 전체 삭제 또는 완료 항목 삭제 기능 추가
- 필터링 기능 추가: 전체, 진행 중, 완료
- JSON 파일 저장/불러오기 추가
- ViewModel 단위 테스트 추가
- 스타일과 리소스 딕셔너리 분리
- ViewModel Locator 또는 의존성 주입 적용

## 메모

이 프로젝트는 학습용이므로 기능을 크게 늘리기보다, WPF와 MVVM의 기본 개념을 작은 단위로 반복해서 익히는 것을 우선합니다.
