using Un;
using Un.Function;

const string PATH = "D:\\User\\Un\\Code\\";
const string FILE = "main.un";

using StreamReader r = new(new FileStream(PATH + FILE, FileMode.Open));

Process.Code = r.ReadToEnd().Split('\n');
Process.Import(new Std());
Process.Import(new Un.Function.Math());

Interpreter interpreter = new(Process.Code);

while (interpreter.TryInterpret()) ;
