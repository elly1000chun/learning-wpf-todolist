# TodoWpf Release Checklist

WPF 학습 앱을 배포하거나 백업하기 전에 반복해서 확인할 항목입니다.

## 1. 변경 사항 확인

```powershell
cd D:\Dev\WPF\learning-wpf-todolist
git status
```

확인할 내용:

- 의도하지 않은 파일이 수정되지 않았는지 확인합니다.
- 새로 추가한 파일이 누락되지 않았는지 확인합니다.
- `bin/`, `obj/`, 사용자 로컬 데이터 파일처럼 커밋하지 않을 파일이 포함되지 않았는지 확인합니다.

## 2. Debug 테스트 실행

```powershell
dotnet test .\TodoWpf.Tests\TodoWpf.Tests.csproj --no-restore
```

확인할 내용:

- 모든 단위 테스트가 통과하는지 확인합니다.
- ViewModel, Service, 설정 저장, 필터, 정렬, 입력 검증 동작이 깨지지 않았는지 확인합니다.

## 3. Release 빌드 확인

```powershell
dotnet build .\TodoWpf\TodoWpf.csproj --configuration Release --no-restore
```

확인할 내용:

- Release 구성에서도 빌드 오류가 없는지 확인합니다.
- Debug에서는 보이지 않던 설정 차이 문제가 없는지 확인합니다.

## 4. 게시 파일 생성

```powershell
dotnet publish .\TodoWpf\TodoWpf.csproj --configuration Release --no-restore
```

기본 게시 위치:

```text
D:\Dev\WPF\learning-wpf-todolist\TodoWpf\bin\Release\net10.0-windows\publish\
```

확인할 내용:

- 게시 폴더에 실행 파일과 필요한 DLL이 생성되는지 확인합니다.
- 게시 결과물은 빌드 산출물이므로 Git 커밋 대상에 포함하지 않습니다.

## 5. 앱 실행 확인

게시 폴더의 `TodoWpf.exe`를 실행해 다음 흐름을 확인합니다.

- 새 할 일이 추가되는지 확인합니다.
- 완료 체크, 수정, 삭제가 동작하는지 확인합니다.
- 검색, 상태 필터, 마감일 필터, 정렬이 함께 동작하는지 확인합니다.
- 설정 창에서 시작 필터, 기본 정렬, 검색어 기억, 테마 저장이 동작하는지 확인합니다.
- 앱을 종료했다가 다시 실행했을 때 할 일과 설정이 복원되는지 확인합니다.

## 6. 문서 확인

확인할 파일:

- `TodoWpf\README.md`
- `docs\wpf-csharp-development-guide.md`
- `docs\release-checklist.md`

확인할 내용:

- 현재 기능 목록이 실제 앱과 맞는지 확인합니다.
- 테스트 실행 방법과 게시 방법이 최신 상태인지 확인합니다.
- 보류한 학습 후보가 따로 남아 있는지 확인합니다.

## 7. 마무리 기준

릴리스 전 점검은 아래 조건을 만족하면 완료로 봅니다.

- `dotnet test` 통과
- `dotnet build --configuration Release` 통과
- `dotnet publish --configuration Release` 통과
- 게시된 앱 수동 실행 확인
- README와 학습 문서 업데이트
