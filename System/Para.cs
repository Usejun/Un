namespace Un
{
    public class Para(string name, Obj Value, bool isNamed = false)
    {
        public string Name { get; private set; } = name;
        public Obj Value { get; private set; } = Value;
        public bool IsNamed { get; private set; } = isNamed;
    }
}
