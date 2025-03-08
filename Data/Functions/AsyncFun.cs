using Un.Interpreter;

namespace Un.Data;

public class AsyncFun : LocalFun
{
    public AsyncFun(string name) : base(name) { }

    public AsyncFun(string name, string[] code, int i = 4) : base(name, code, i) { }

    public override Obj Call(Field field)
    {
        Field local = new(this.field);
        local.Merge(field);

        return new Task(new Task<Obj>(() => {
            Parser sub = new(code, new(local), line:1, nesting: nesting);

            while (sub.TryInterpret()) { };

            return sub.ReturnValue ?? None;
        }));
    }

    public override AsyncFun Clone()
    {
        return new(Name)
        {
            code = code,
            nesting = nesting,
            Args = Args,
            field = new(field)
        };
    }
}
