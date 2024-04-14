using Un.Supporter;

Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

const string PATH = "D:\\User\\Un\\Code";

string[] testcase = [
    "type.test.un",
    "class.test.un",
    "list.test.un",
];

Process.Initialize(PATH);


Process.Test(testcase);
//Process.Run("class.test.un");