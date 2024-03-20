using Un;

const string PATH = "D:\\User\\Un\\Code\\main.un";

using StreamReader r = new(new FileStream(PATH, FileMode.Open));

Tokenizer tokenizer = new(r.ReadToEnd().Split('\n'));

while (tokenizer.TryInterpret()) ;
