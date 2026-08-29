# AGENTS.md

이 프로젝트는 WPF와 MVVM 패턴을 학습하기 위한 C# 데스크톱 앱입니다. Codex는 변경을 제안하거나 구현할 때 학습자가 구조와 이유를 이해할 수 있도록 작고 명확한 단위로 작업합니다.

## 프로젝트 개요

- 앱 이름: TodoWpf
- 목적: WPF, XAML, MVVM, CommunityToolkit.Mvvm 학습
- 플랫폼: Windows 데스크톱
- 프레임워크: .NET `net10.0-windows`
- UI 기술: WPF
- MVVM 도구: `CommunityToolkit.Mvvm`

## 작업 원칙

- MVVM 구조를 유지합니다.
- View에는 화면 표현과 바인딩을 두고, 동작과 상태 관리는 ViewModel에 둡니다.
- 코드 비하인드는 `InitializeComponent`, `DataContext` 설정처럼 View 초기화에 가까운 역할만 맡기는 것을 기본으로 합니다.
- 학습 프로젝트이므로 과도한 추상화보다 이해하기 쉬운 구조를 우선합니다.
- 한 번에 많은 기능을 추가하기보다 작은 기능을 완성하고 설명 가능한 상태로 유지합니다.
- 기존 파일의 스타일을 존중하되, 새 코드는 가능하면 파일 범위 네임스페이스와 nullable 흐름을 유지합니다.

## 현재 구조

```text
TodoWpf/
├─ Models/
│  └─ TodoItems.cs
├─ ViewModels/
│  └─ MainWindowViewModel.cs
├─ App.xaml
├─ App.xaml.cs
├─ MainWindow.xaml
├─ MainWindow.xaml.cs
└─ TodoWpf.csproj
```

## 구현 가이드

- 새 UI 상태는 먼저 ViewModel 속성으로 표현합니다.
- 버튼 동작은 가능하면 `[RelayCommand]`를 사용합니다.
- 바인딩 대상 속성은 `[ObservableProperty]` 또는 명시적인 변경 알림을 사용합니다.
- 목록 데이터는 UI 갱신이 필요한 경우 `ObservableCollection<T>`를 사용합니다.
- XAML에서는 바인딩 경로와 `CommandParameter`를 명확하게 유지합니다.
- 모델은 할 일 데이터 자체를 표현하고, 화면 흐름이나 명령 로직은 ViewModel에 둡니다.

## 빌드와 확인

변경 후 가능한 경우 아래 명령으로 빌드를 확인합니다.

```powershell
cd D:\Dev\WPF\learning-wpf-todolist\TodoWpf
dotnet build
```

UI 동작이 바뀌는 경우에는 앱을 실행해서 다음 기본 흐름을 확인합니다.

- 앱이 정상 실행되는지
- 할 일을 입력하고 추가할 수 있는지
- 빈 입력은 추가되지 않는지
- 체크박스로 완료 상태가 바뀌는지
- 삭제 버튼으로 항목이 제거되는지

## 선호하는 다음 작업 단위

- README와 학습 노트 정리
- 할 일 수정 기능
- 완료 항목 삭제 기능
- 전체, 진행 중, 완료 필터
- JSON 저장/불러오기
- ViewModel 테스트 추가
- 스타일 리소스 분리

## 주의 사항

- `bin/`, `obj/`, `.vs/` 같은 빌드 또는 IDE 산출물은 직접 수정하지 않습니다.
- 학습자가 이해하기 어려운 대규모 리팩터링은 피합니다.
- 기능 추가 시 README 또는 주석이 실제 코드와 어긋나지 않도록 유지합니다.
- 외부 패키지는 필요성이 분명할 때만 추가합니다.
