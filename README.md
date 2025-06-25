Un 언어 공식 문서 (초안)

개요

Un은 파이썬 스타일의 문법을 기반으로 하면서도 간결함과 비동기 친화성을 핵심으로 설계된 동적 스크립트 언어입니다. 기본적으로 모든 함수는 비동기 실행이 가능하며, 병렬 처리와 Future 기반 실행 모델을 직관적으로 표현할 수 있도록 구성되어 있습니다.

Un은 C#으로 구현된 인터프리터를 기반으로 하며, 직렬 AST 구조를 따릅니다. 이 언어는 포트폴리오 목적의 실험적 언어이며, 간단한 문법과 비동기 중심 실행 흐름을 지원합니다.

언어 설계 철학

간결성: 최소한의 키워드, 콜론 없는 문법, 들여쓰기 기반 구조

비동기 중심 구조: 모든 함수는 비동기로 실행 가능하며, go, wait 키워드로 병렬 실행 제어

유연한 타입 시스템: 완전 동적 타입, 타입 힌트는 선택적

확장성: .un 스크립트와 C# 네이티브 객체를 혼합하여 사용 가능

문법 개요

1. 함수 정의

fn greet(name)
    write("Hello, " or name)

2. 조건문

if x > 0
    write("Positive")
elif x == 0
    write("Zero")
else
    write("Negative")

3. 반복문

for i in 1 to 5
    write(i)

in 뒤에는 반드시 iterable 값이 와야 함

4. 패턴 매칭

match x
    1 or 2: write("one or two")
    str: write("it's a string")
    int: write("it's an integer")
    _: write("something else")

값 기반 + 타입 기반 매칭 모두 지원

5. 비동기 실행

go fetchData()
data = wait fetchData()

go는 Future(Task) 시작

wait는 해당 Future 완료 대기 및 결과 획득

6. 반환 문법

fn add(x, y)
    -> x + y

return 키워드 대신 -> 사용

7. 연산자 오버로딩

class Vec2
    fn __add__(a, b)
        -> Vec2(a.x + b.x, a.y + b.y)

Python과 유사한 연산자 오버로딩 지원

타입 시스템

모든 값은 동적 타입

선택적으로 타입 힌트를 줄 수 있음 (fn greet(name: str))

타입은 런타임에 검사되며, 정적 타입 체크는 하지 않음

모듈과 패키지

use 키워드로 모듈 또는 네이티브 클래스 로드

use math
use "./lib/mylib.un"

.un 스크립트 파일 또는 C#으로 정의된 네이티브 타입 모두 로드 가능

실행 모델

인터프리터는 C# 기반으로 작성됨

AST는 직렬 형태로 구성되어 있음

Future는 C#의 Task를 래핑하여 구현됨

병렬 처리는 BlockingCollection을 이용하여 비동기 이벤트 큐로 처리

C#의 기본 GC를 그대로 사용함

미구현 기능

예외 처리 (try, catch) 미지원

스레드/프로세스 수준의 병렬 처리 미지원 (비동기 이벤트 기반만 지원)

라이선스

MIT License

