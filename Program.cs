using Un.Supporter;

Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

//string[] testcase = [
//    "type.test.un",
//    "class.test.un",
//    "list.test.un",
//];

Process.Initialize("D:\\User\\Un\\Package\\Code", "D:\\User\\Un\\Code");

//Process.Test(testcase);
Process.Run("test.un");