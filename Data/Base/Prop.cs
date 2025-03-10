namespace Un.Data;

public class Prop : Obj
{
    public Fun Getter { get; private set; }
    public Fun Setter { get; private set; }

    public Prop() : this(Lambda.Self, Lambda.Self) {}

    public Prop(Fun getter, Fun setter) : base("prop") 
    { 
        Getter = getter;
        Setter = setter;       
    }

    public override Obj Init(Collections.Tuple args, Field field) => Getter.Call([], new()).Init(args, field);
    
    public override Obj Get(string str, bool isOriginal = false) => Getter.Call([], new()).Get(str);

    public override void Set(string str, Obj value) => Getter.Call([], new()).Set(str, value);

    public override Obj Add(Obj arg, Field field) => Getter.Call([], new()).Add(arg, field);
    public override Obj Sub(Obj arg, Field field) => Getter.Call([], new()).Sub(arg, field);
    public override Obj Mul(Obj arg, Field field) => Getter.Call([], new()).Mul(arg, field);
    public override Obj Mod(Obj arg, Field field) => Getter.Call([], new()).Mod(arg, field);
    public override Obj Div(Obj arg, Field field) => Getter.Call([], new()).Div(arg, field);
    public override Obj IDiv(Obj arg, Field field) => Getter.Call([], new()).IDiv(arg, field);
    public override Obj Pow(Obj arg, Field field) => Getter.Call([], new()).Pow(arg, field);
    public override Obj LSh(Obj arg, Field field) => Getter.Call([], new()).LSh(arg, field);
    public override Obj RSh(Obj arg, Field field) => Getter.Call([], new()).RSh(arg, field);
    public override Obj BAnd(Obj arg, Field field) => Getter.Call([], new()).BAnd(arg, field);
    public override Obj BOr(Obj arg, Field field) => Getter.Call([], new()).BOr(arg, field);
    public override Obj BXor(Obj arg, Field field) => Getter.Call([], new()).BXor(arg, field);
    public override Obj BNot() => Getter.Call([], new()).BNot();
    public override Obj At(Obj arg, Field field) => Getter.Call([], new()).At(arg, field);
    public override Bool Eq(Obj arg, Field field) => Getter.Call([], new()).Eq(arg, field);
    public override Bool Lt(Obj arg, Field field)  => Getter.Call([], new()).Lt(arg, field);
    public override Int Len() => Getter.Call([], new()).Len();
    public Int Hash() => Getter.Call([], new()).Hash();
    public Str Type() => Getter.Call([], new()).Type();
    public override Str CStr() => Getter.Call([], new()).CStr();
    public override Bool CBool() => Getter.Call([], new()).CBool();
    public override Float CFloat() => Getter.Call([], new()).CFloat();
    public override Int CInt() => Getter.Call([], new()).CInt();
    public override List CList() => Getter.Call([], new()).CList();
    public override Obj Clone() => Getter.Call([], new()).Clone();
    public override Obj Copy() => Getter.Call([], new()).Copy();
    public override Obj GetItem(Obj arg, Field field) => Getter.Call([], new()).GetItem(arg, field);
    public override Obj SetItem(Obj arg, Obj index, Field field) => Getter.Call([], new()).SetItem(arg, index, field);
    public override Obj Entry() => Getter.Call([], new()).Entry();
    public override Obj Exit()  => Getter.Call([], new()).Exit();
}