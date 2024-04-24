using Un.Supporter;

Console.InputEncoding = System.Text.Encoding.Unicode;
Console.OutputEncoding = System.Text.Encoding.Unicode;
Process.Initialize("D:\\User\\Un\\Package\\Code", "D:\\User\\Un\\Code");

string[] testcase = [
    "type.test.un",
    "class.test.un",
    "iter.test.un",
    "dict.test.un",
    "func.test.un",
    "fib.test.un"
];

string[] euler = [
    "Euler\\1.un",
    "Euler\\2.un",
    "Euler\\3.un",
];

Process.Test(euler);
Process.Test(testcase);
//Process.Run(euler[2]);