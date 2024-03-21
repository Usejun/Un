using Un;

const string PATH = "D:\\User\\Un\\Code\\main.un";

using StreamReader r = new(new FileStream(PATH, FileMode.Open));

Process.Code = r.ReadToEnd().Split('\n');
Interpreter interpreter = new();

while (interpreter.TryInterpret()) ;
