using Un;

Console.InputEncoding = System.Text.Encoding.Unicode;
Console.OutputEncoding = System.Text.Encoding.Unicode;
Process.Initialize("D:\\User\\Un\\Code");

string[] testcase = [
    "test.un",
    "Test\\set.test.un",
    "Test\\iter.test.un",
    "Test\\dict.test.un",
    "Test\\math.test.un",
    "Test\\type.test.un",
    "Test\\class.test.un",
    //"Test\\func.test.un",
    "Test\\fib.test.un"
];

string[] euler = [
    "Euler\\1.un",
    "Euler\\2.un",
    "Euler\\3.un",
];

//Process.Test(euler);
//Process.Test(testcase);
Process.Run(testcase[1]);