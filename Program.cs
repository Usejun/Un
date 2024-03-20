using Un;

const string PATH = "D:\\User\\Un\\Code\\main.un";

using StreamReader r = new(new FileStream(PATH, FileMode.Open));

Interpreter interpreter = new(r.ReadToEnd().Split('\n'));

while (interpreter.TryInterpret()) ;
