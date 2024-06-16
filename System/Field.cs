namespace Un
{
    public class Field 
    {
        public static Field Null = new();

        public int Count => field.Count;
        public string[] Keys => [..field.Keys];
        public Obj[] Values => [..field.Values];

        public Field() { }

        public Field(Field other)
        {
            Copy(other);
        }

        private readonly Dictionary<string, Obj> field = [];

        public bool Get(string name, out Obj value) => field.TryGetValue(name, out value!);

        public void Set(string name, Obj value)
        {
            if (!field.TryAdd(name, value))
                field[name] = value;
        }

        public void Remove(string name)
        {
            field.Remove(name);
        }

        public bool Key(string name) => field.ContainsKey(name);

        public bool Value(Obj obj) => field.ContainsValue(obj);

        public void Copy(Field other)
        {
            foreach ((var key, var Value) in other.field)
                Set(key, Value);
        }

        public void Clear() => field.Clear();   

        public Obj this[string name]
        {
            get => field[name];
            set => field[name] = value;
        }      
    }
}
