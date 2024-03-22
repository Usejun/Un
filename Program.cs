using Un;

const string PATH = "D:\\User\\Un\\Code\\";
const string FILE = "main.un";

using StreamReader r = new(new FileStream(PATH + FILE, FileMode.Open));

Process.Code = r.ReadToEnd().Split('\n');   
Interpreter interpreter = new();

while (interpreter.TryInterpret()) ;
