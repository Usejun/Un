using Un;
using Un.Function;

const string PATH = "D:\\User\\Un\\Code";
const string FILE = "sub.un";

Process.Initialize(PATH, FILE);

Process.Import(new Std());
Process.Import(new Un.Function.Math());
Process.Import("list.un");

Process.Run();