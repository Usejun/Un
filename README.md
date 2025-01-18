# Un

파이썬 스타일의 인터프리터 언어

## 동작 방식

C#으로 작성된 프로그램을 통해서 ".un"의 확장자를 가진 파일을 읽으며 동작된다.

# 구조
기본적으로 un은 동적 타이핑을 지원하고, 변수를 선언 할 때에는 **\[name1, name2, ...] = \[value1, value2, ...]** 의 형태를 따른다.

## 기본 자료형
### int
부호가 있는 64bit 정수형 자료형이다.

```un
a = 3048275133
b = 0b11011010
c = 0o1274213
d = 0xff38ab
f = -39493
````
### float
부호가 있는 64bit 부동 실수형 자료형이다.

```un
a = 1937.393
b = float("inf")
c = -2004.22
````

### str
문자열 자료형이다.
문자열은 """와 "'"을 사용하며, 보간 문자열을 사용할 때에는 "\`"을 사용한다.

```un
a = "hello world!"
b = 'my name is "lee"'
c = `{a} {b}`
````

### bool
Boolean 자료형이다.

```un
a = true
b = false
````

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
다양한 자료형이 연속적으로 있는 자료형

```un
a = [1, 4.3, "3", true, []]
````
#### methods
1. add
2. insert
3. extend
4. extend_insert
5. remove
6. remove_at
8. index_of
9. contains
10. clone
11. sort
12. reverse
13. order
14. binary_search
15. lower_bound
16. upper_bound
17. hpush
18. hpop
19. pop

### dict
(키:값)의 형태로 접근하는 자료형

```un
a = {1:"one", 5:234, "hello":true}
````

### set
다양한 자료형을 담을 수 있는 집합 형태의 자료형

```un
a = { 1, 2, 5.2, "hogo", ...}
````

## 함수
**fn \[function name](parameters)** 의 형태로 만들 수 있다. 여러 개의 매개변수를 입력 받으려면 각 매개변수를 ","로 구분 지어준다.

```un
fn double(a)
    return 2 * a
    
fn multiple(a, b)
    return a * b
````

### 기본 함수
1. write
2. writeln
3. clear
4. readln
5. type
6. method
7. field
8. prop
9. package
10. len
11. range
12. hash
13. open
14. sum
15. max
16. min
17. abs
18. pow
19. ceil
20. floor
21. round
22. sqrt
23. sin
24. cos
25. tan
26. bin
27. oct
28. hex
29. breakpoint
31. exit
32. assert
33. array
34. eval
35. memo

## 조건문
**if, elif, else**의 키워드를 사용하고, 키워드 다음으로는 **bool** 형을 반환하는 식이 따라와야한다.

```un
n = int(readln())

if n < 10
    ...
elif n < 100
    ...
else
    ...
````

## 반복문
**for \[variable] in \[iterable]** 과 **while (bool)** 의 형식을 사용한다.

### for
in 키워드 후에는 **iterable한 값**이 와야한다. iterable의 순회가 끝내면 반복문이 끝난다.

```
for i in range(1, 100)
    writeln(i)
```

### while

while 키워드 후에는 **반드시 bool 형식의 값**이 와야한다. 조건식이 부합하면 while문 내부로 들어간다. 그렇지 않을 경우에는 while문을 탈출한다.

```
fib = [0] * 21
fib[0] = 1
fib[1] = 1
i = 2
while (i < 20)
    fib[i] = fib[i - 1] + fib[i - 2]
    i += 1
writeln(fib)
````

## 클래스
class 키워드 후에 클래스의 이름을 지어준다. 클래스는 필드와 메서드를 가지고 있다.

```un
class point
    x = 0
    y = 0

    fn print()
        writeln(`{this.x} : {this.y}`)

p = point()
p.x = 10
p.y = -10
p.print()
```

클래스의 예약 함수를 통해 여러 연산 기능을 추가할 수 있다.

### 오버로딩 함수

- \_\_init__ : 클래스 생성자
- \_\_add__ : 덧셈
- \_\_sub__ : 뺄셈
- \_\_mul__ : 곱셉
- \_\_div__ : 나눗셈
- \_\_idiv__ : 정수 나눗셈 
- \_\_mod__ : 나머지
- \_\_pow__ : 거듭 제곱
- \_\_at__ : @ 연산자
- \_\_eq__ : 같음 비교
- \_\_lt__ : 미만
- \_\_len__ : len()
- \_\_type__ : type()
- \_\_hash__ : hash()
- \_\_str__ : str로의 형변환
- \_\_int__ : int로의 형변환
- \_\_float__ : float로의 형변환
- \_\_bool__ : bool로의 형변환
- \_\_list__ : list로의 형변환
- \_\_getitem__ : get by index
- \_\_setitem__ : set by index
- \_\_band__ : bitwise AND 연산
- \_\_bor__ : bitwise OR 연산
- \_\_bxor__ : bitwise XOR 연산
- \_\_bnot__ : bitwise NOT 연산
- \_\_lsh__ : bit left-shift 연산
- \_\_rsh__ : bit right-shift 연산
- \_\_entry__ : using 입장
- \_\_exit__ : using 탈출

## lambda
**\([parameters]) => \[code]** 의 꼴로 사용되며, 함수와 동일한 기능을 한다.

```un
f = (i) => i * i
````

## import
**import \[package name, ...]** 의 꼴로 사용되며, 다른 코드나, 패키지를 불러오기 위한 구문이다.

as 키워드를 사용해서 하나의 네임스페이스를 만들 수 있다.

```un
import math
import parallel as pa
````

## using
**using \[name] = \[value]** 의 꼴로 사용되며, value에 들어갈 값은 반드시 entry 함수와 exit 함수가 정의되어 있어야 한다.

```un
using file = open(file_name)
````

## enum
**enum \[name]** 의 꼴로 사용되며, 열거형을 만들 수 있다. 모든 값들은 콤마로 구분된다.

```un
enum rank
    bronze, sliver, gold, diamond
```

## slice
**iterable\[start:end]** 꼴로 사용되며, 인덱싱이 구현된 자료형이라면 슬라이스를 사용할 수 있다.

```un
l = [1, 2, 3, 4, 5]
write(l[0:3]) # [1, 2, 3]
```

## async
함수의 앞에 붙는 키워드로 비동기 함수를 만들 수 있다.

```un
async fn api(url)
	return https.get(url);
```


## await
비동기 함수가 완료 될 때까지 기다린다.

```un
data = await api(url)
````

## del
변수를 삭제한다.

```un
a = 123
write(a)

del a

write(a) # error
````

## 보간 문자열
보간 문자열은 문자열 내부에 변수를 넣을 수 있는 문자열로, 백틱으로 감싼 문자열 내부의 중괄호 안의 값을 문자열로 바뀐다.

```un
name = "usejun"
age = 20
s = `i am {name}, my age is {age}`
````
