# Un

파이썬 스타일의 인터프리터 언어

## 동작 방식

C#으로 작성된 프로그램을 통해서 `.un`의 확장자를 가진 파일을 읽으며 동작된다.

## 구조
기본적으로 Un은 동적 타이핑을 지원하며, 변수를 선언할 때에는 다음과 같은 형태를 따른다:

```un
name1, name2, ... = value1, value2, ...
```

```un
a = 1
a, b, c = 1, 2, 3
a = (1, 2, 3)
```

## 기본 자료형

### int
부호가 있는 64bit 정수형 자료형이다.

```un
a = 3048275133
b = 0b11011010
c = 0o1274213
d = 0xff38ab
f = -39493
```

### float
부호가 있는 64bit 부동 소수점 자료형이다.

```un
a = 1937.393
b = float("inf")
c = -2004.22
d = 3.34e2
```

### str
문자열 자료형이다.
문자열은 `"` 또는 `'`을 사용하며, 보간 문자열을 사용할 때에는 **백틱**(```)을 사용한다.

```un
a = "hello world!"
b = 'my name is "lee"'
c = `{a} {b}`
```

### bool
Boolean 자료형이다.

```un
a = true
b = false
```

### date
날짜 자료형이다.

```un
a = date("2024.1.1")
```

### tuple
불변 형태로 다양한 자료형을 담을 수 있다.

```un
a = (1, 2, 3)
```

### list
다양한 자료형이 연속적으로 있는 자료형이다.

```un
a = [1, 4.3, "3", true, []]
```

#### 주요 메서드
1. `add(value)`: 리스트 끝에 요소 추가
2. `insert(index, value)`: 특정 위치에 요소 삽입
3. `extend(iterable)`: 리스트 확장
4. `remove(value)`: 특정 요소 삭제
5. `pop(index)`: 특정 위치의 요소 제거 후 반환
6. `sort()`: 리스트 정렬
7. `reverse()`: 리스트 뒤집기

### dict
(키: 값)의 형태로 접근하는 자료형이다.

```un
a = {1: "one", 5: 234, "hello": true}
```

### set
집합 형태의 자료형이다.

```un
a = {1, 2, 5.2, "hogo", ...}
```

## 함수
함수는 `fn` 키워드를 사용하여 선언한다.

```un
fn double(a)
    return 2 * a
    
fn multiple(a, b)
    return a * b
```

## 조건문
`if`, `elif`, `else` 키워드를 사용한다.

```un
n = int(readln())

if n < 10
    ...
elif n < 100
    ...
else
    ...
```

## 반복문
### for
`in` 키워드를 사용하며, iterable한 값이 와야 한다.

```un
for i in range(1, 100)
    writeln(i)
```

### while
조건이 `true`인 동안 실행된다.

```un
>>>>>>> c4ba701 (v1.0.1)
fib = array(0, 21)
fib[0] = 1
fib[1] = 1
i = 2
while (i < 20)
    fib[i] = fib[i - 1] + fib[i - 2]
    i += 1
writeln(fib)
```

## 클래스
클래스는 `class` 키워드를 사용하여 정의한다.
자기 자신은 `self` 키워드, 부모는 `super`를 사용한다.

```un
class point
    x = 0
    y = 0

    fn print()
        writeln(`{self.x} : {self.y}`)

p = point()
p.x = 10
p.y = -10
p.print()
```

### 오버로딩 함수
- `__init__`: 생성자
- `__add__`: 덧셈
- `__sub__`: 뺄셈
- `__mul__`: 곱셈
- `__div__`: 나눗셈
- `__eq__`: 비교 연산
- `__len__`: `len()` 지원
- `__getitem__`: 인덱싱 지원
- `__setitem__`: 인덱스 값 변경 지원

## lambda
익명 함수를 생성할 때 사용한다.

```un
f = (i) => i * i
```

## import
다른 코드나 패키지를 불러오기 위한 구문이다.

```un
import math
import thread as t
```

## 파일 입출력
파일을 읽고 쓰는 기능을 제공한다.

```un
file = open("data.txt", "w")
file.write("Hello, Un!")
file.close()
```

## 병렬 처리
멀티스레딩을 지원하며, `thread` 모듈을 활용할 수 있다.

```un
import thread

fn task1(i)
    writeln(`{i} : Running in parallel`)

fn task2()
    writeln("Running in parallel")

thread.foreach([1, 2, 3, 4], task1) # 4개의 스레드에서 실행
thread.run(10, task2) # 10개의 스레드에서 실행
```

## 예외 처리
예외를 처리하기 위해 `try`, `catch`, `fin`를 사용할 수 있다.

```un
try
    a = 10 / 0
catch e
    writeln(`Error: {e}`)
fin
    write("fin")
```

## using
자동으로 자원을 관리하는 문법이다.

```un
using file = open(file_name)
```

## enum
열거형을 정의할 수 있다.

```un
enum rank
    bronze, silver, gold, diamond
```

## slice
리스트 등의 자료형에서 일부를 추출할 수 있다.

```un
l = [1, 2, 3, 4, 5]
write(l[0:3]) # [1, 2, 3]
```

## async
비동기 함수를 만들 때 사용한다.

```un
async fn api(url)
    return https.get(url)
```

## await
비동기 함수가 완료될 때까지 기다린다.

```un
data = await api(url)
```

## del
변수를 삭제한다.

```un
a = 123
write(a)

del a

write(a) # error
```

## 보간 문자열
문자열 내부에 변수를 포함할 수 있다.

```un
name = "usejun"
age = 20
s = `i am {name}, my age is {age}`
<<<<<<< HEAD
````
## 타입 힌트
각 변수의 타입이나 함수의 반환값, 함수의 인자의 타입을 명시적으로 알려줄 수 있으며, 타입의 강제성은 없다.

```un
a = 1
b: int = 1
c: str = 2
d: my_custom_class = my_custom_class()

fn multiply(a: int, b: int) -> int
    return a * b 
```
