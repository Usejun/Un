using Un.Supporter;

Console.InputEncoding = System.Text.Encoding.Unicode;
Console.OutputEncoding = System.Text.Encoding.Unicode;
Process.Initialize("D:\\User\\Un\\Package\\Code", "D:\\User\\Un\\Code");

string[] testcase = [
    "type.test.un",
    "class.test.un",
    "iter.test.un",
    "dict.test.un",
];

//Process.Test(testcase);
Process.Run("dict.test.un");