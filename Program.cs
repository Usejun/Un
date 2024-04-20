using Un.Supporter;

Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;
Process.Initialize("D:\\User\\Un\\Package\\Code", "D:\\User\\Un\\Code");

string[] testcase = [
    "type.test.un",
    "class.test.un",
    "iter.test.un",
];

//Process.Test(testcase);
Process.Run("test.un");