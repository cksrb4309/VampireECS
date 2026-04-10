# Graphify Workflow (Explicit)

이 프로젝트는 **자동 훅을 사용하지 않고**, 필요할 때만 명시적으로 그래프를 갱신하는 방식으로 운영한다.

## 핵심 원칙

- Git 훅 사용 금지
- 그래프 갱신은 수동 명령으로만 수행
- 그래프 산출물은 버전관리하지 않음
- 임시 캐시 디렉터리에서 먼저 빌드한 뒤, 성공한 결과만 `graphify-out/`으로 복사

## 갱신 방법

Windows:

```bat
tools\graphify-refresh.cmd .
```

직접 실행:

```powershell
python tools\graphify-refresh.py .
```

## 질의 방법

그래프가 갱신된 뒤에는 `graphify-out/graph.json`을 대상으로 질의한다.

```powershell
graphify query "show the auth flow" --graph graphify-out/graph.json
graphify path "DigestAuth" "Response" --graph graphify-out/graph.json
```

## 추천 운영 루프

- 구조 질문이 나오기 전: 1회 수동 갱신
- 큰 리팩터링/폴더 이동 이후: 1회 수동 갱신
- 그래프가 오래된 것 같다면: 다시 갱신

## 커밋하지 말아야 할 산출물

- `graphify-out/`
- `.graphify_*.json`
- `__pycache__/`
- 잘못 생긴 `Assets/**/graphify-out/`
