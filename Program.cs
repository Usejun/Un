﻿Console.InputEncoding = Process.Unicode;
Console.OutputEncoding = Process.Unicode;
Process.Initialize("D:\\User\\Un\\Code");

string[] testcase = [
    "test.un",
    "Test\\fib.test.un",
    "Test\\token.test.un",
    "Test\\type.test.un",
    "Test\\class.test.un",
    "Test\\set.test.un",
    "Test\\dict.test.un",
    "Test\\list.test.un",
    "Test\\math.test.un",
    "Test\\enum.test.un",
    //"Test\\func.test.un",
];

string[] euler = [
    "Euler\\1.un",
    "Euler\\2.un",
    "Euler\\3.un",
];

//Process.Test(euler);
//Process.Test(testcase);
//Process.Run(euler[0]);
Process.Run(testcase[0]);
