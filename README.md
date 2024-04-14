# Un
파이썬 스타일의 스크립트 언어
<br>

## 방식
C#으로 작성된 프로그램을 통해 Un으로 작성된 스크립트를 한 줄씩 읽어 코드를 해석해 동작시킨다.
<br>

# 구조

## 기본 자료형
<ul>
	<li>Int : 부호가 있는 64bit 정수형
	<li>Float : 부호가 있는 64bit 부동 실수형
	<li>Str : 문자열
	<li>Bool : Boolean 
	<li>Iter : 다양한 자료형이 반복적으로 있는 구조
	<li>Date : 날짜
</ul>

## 함수

**fn [Function Name] ([Parameter Names ... ])** 형식으로 함수를 만들 수 있다. 다수의 매개변수를 입력하려면 ',' 를 사용해 구분해준다. 


```
fn my_function(arg)
    writeln(arg)
    return 2
```


### 기본 함수
<ul>
	<li> write : 줄바꿈 없는 출력
	<li> writeln : 줄바꿈 있는 출력
	<li> readln : 한 줄을 읽어 Str로 반환
	<li> type : 인수로 받은 값의 자료형을 반환
	<li> func : 구현된 모든 함수를 Iter로 반환
	<li> len : Iter나 Str의 길이를 반환
	<li> range : [start, length] 형식의 인자를 받아 iter를 반환
</ul>

## 조건문

if, elif, else의 키워드를 사용하고, 키워드 다음으로는 **Bool**형을 반환하는 식이 따라와야한다.
<br>

```
n = int(readln(0))

if n < 10
    writeln("10보다 작음")
elif (n < 100)
    writeln "10보단 크지만 100보단 작음"
else
    writeln ("100보다" + "큼")
```

## 반복문

**for [Variable] in [Iter]** 과 **while (Bool)** 의 형식을 사용한다.

### for

in 키워드 후에는 **반드시 Iter 형식의 값**이 와야한다.
Iter 내부를 순회를 끝내면 반복문이 끝난다.

```
for i in range(1, 100)
    writeln(i)
```

### while 

while 키워드 후에는 **반드시 Bool 형식의 값**이 와야한다.
조건이 부합하면 while문 내부로 들어간다. 그렇지 않을 경우에는 while문을 탈출한다.

```
fib = [0] * 21
fib[0] = 1
fib[1] = 1
i = 2
while (i < 20)
    fib[i] = fib[i - 1] + fib[i - 2]
    i = i + 1
writeln(fib)
```

## 클래스

class 키워드 후에 클래스의 이름을 지어준다.
클래스는 필드와 메서드를 가지고 있다.

클래스의 예약 함수를 통해 여러 연산 기능을 추가할 수 있다.

### 예약 함수
<ul>
	<li> __init__ : 클래스 생성자
	<li> __add__ : 덧셈
	<li> __sub__ : 뺄셈
	<li> __mul__ : 곱셉
	<li> __div__ : 나눗셈
	<li> __idiv__ : 정수 나눗셈
	<li> __mod__ : 나머지
	<li> __eq__ : 같음 비교
	<li> __lt__ : 미만
	<li> __len__ : len()
	<li> __type__ : type()
	<li> __hash__ : hash()
	<li> __str__ : str로의 형변환
	<li> __int__ : int로의 형변환
	<li> __float__ : float로의 형변환
	<li> __bool__ : bool로의 형변환
	<li> __iter__ : iter로의 형변환
	<li> __getitem__ : getter
	<li> __setitem__ : setter
	<li> __and__ : and 연산
	<li> __or__ : or 연산
	<li> __xor__ : xor 연산
</ul>
