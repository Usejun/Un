namespace Un
{
    public class Field 
    {
        public readonly static Field Null = new();

        public int Count => field.Count;
        public string[] Keys => [..field.Keys];
        public Obj[] Values => [..field.Values];

        private readonly Dictionary<string, Obj> field = [];

        public Field() { }

        public Field(Field other)
        {
            Copy(other);
        }

        public Field(params (string key, Obj value)[] pairs)
        {
            foreach (var (key, value) in pairs)
                Set(key, value);            
        }

        public bool Get(string name, out Obj value) => field.TryGetValue(name, out value!);

        public void Set(string name, Obj value)
        {
            if (!field.TryAdd(name, value))
                field[name] = value;
        }

        public void Add(Field field)
        {
            foreach ((var key, var value) in field.field)
                Set(key, value);            
        }

        public void Remove(string name)
        {
            field.Remove(name);
        }

        public bool Key(string name) => field.ContainsKey(name);

        public bool Value(Obj obj) => field.ContainsValue(obj);

        public void Copy(Field other)
        {
            foreach (var (key, value) in other.field)
                Set(key, value.Clone());
        }

        public void Clear() => field.Clear();   

        public Obj this[string name]
        {
            get => field[name];
            set => field[name] = value;
        }

        public static Field Self(Obj obj)
        {
            Field field = new();

            field.Set(Literals.Self, obj);
            if (obj.Super != null)
                field.Set(Literals.Super, obj.Super);

            return field;
        }
    }
}
