# Un
파이썬 스타일의 인터프리터 언어
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
</ul>

## 함수

모든 함수의 매개변수는 **1개**로 고정되어 있고, 모든 함수는 **필수적**으로 인수를 넣어주어야 한다.
다수의 인수를 넣으려면 **iter**형으로 넘겨주어야 한다.

만약에 함수의 인수 값에서 사칙연산이 필요하지 않는 값이라면 괄호를 생략 할 수 있다.

**fn 키워드를 사용해서 직접 함수를 만들 수 있다.**


```
fn my_function(arg)
    writeln arg
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
	<li> int, float, str, iter, bool : 인수를 자료형으로 변환해서 반환
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
for i in range([1, 100])
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

## TODO

[x] 재귀
[ ] 클래스
[ ] . 연산자
[ ] import 기능
[ ] 함수 매개변수 추가