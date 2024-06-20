namespace Un.Collections;

public class Matrix : Val<List>
{
    public int Row { get; private set; } = 0;
    public int Column { get; private set; } = 0;

    public Matrix() : base("matrix", []) { }

    public Matrix(int x, int y) : base("matrix", [])
    {
        Row = x;
        Column = y;

        for (int i = 0; i < Row; i++)
        {
            List list = [];
            for (int j = 0; j < Column; j++)
                list.Append(new Int(0));
            Value.Append(list);
        }
    }

    public Matrix(Matrix matrix) : base("matrix", []) 
    {
        Row = matrix.Row;
        Column = matrix.Column;

        for (int i = 0; i < Row; i++)
        {
            Value.Append(new List());
            for (int j = 0; j < Column; j++)
                Value[i].Add(matrix[i][j].Clone());
        }
    }

    public List this[int index]
    {
        get
        {
            if (OutOfRange(index)) throw new IndexError("out of range");
            return (List)Value[index];
        }
        set
        {
            if (OutOfRange(index) ) throw new IndexError("out of range");
            Value[index] = value;
        }
    }

    public override void Init()
    {
        field.Set("Row", new NativeFun("Row", 1, args =>
        {
            if (args[0] is not Matrix self)
                throw new ValueError("invalid argument");

            return new Int(self.Row);
        }));
        field.Set("Column", new NativeFun("Column", 1, args =>
        {
            if (args[0] is not Matrix self)
                throw new ValueError("invalid argument");

            return new Int(self.Column);
        }));
    }

    public override Obj Init(Tuple args)
    {
        if (args.Count == 1 && args[0] is List list)
        {
            Row = (int)list.Len().Value;
            Column = 0;
            Value = [];

            foreach (var item in list)
                Column = Math.Max(Column, (int)item.Len().Value);

            for (int i = 0; i < Row; i++)
            {
                Value.Append(new List());

                foreach (var item in list[i].CList())
                    Value[i].Add(item);

                for (int j = 0; j < Column - ((int)list[i].Len().Value); j++)
                    Value[i].Add(new Int());
            }
        }
        else if (args.Count == 1 && args[0] is Int x1)
        {
            Row = Column = (int)x1.Value;
            Value = [];           

            for (int i = 0; i < Row; i++)
            {
                List buf = [];
                for (int j = 0; j < Column; j++)
                    buf.Append(new Int());
                Value.Append(buf);
            }
        }     
        else if (args.Count == 2 && args[0] is Int x2 && args[1] is Bool b)
        {
            Row = Column = (int)x2.Value;
            Value = [];

            for (int i = 0; i < Row; i++)
            {
                List buf = [];
                for (int j = 0; j < Column; j++)
                {
                    if (i == j && b.Value) buf.Append(new Int(1));
                    else buf.Append(new Int());
                }
                Value.Append(buf);
            }
        }
        else if (args.Count == 2 && args[0] is Int x3 && args[1] is Int y)
        {
            Row = (int)x3.Value;
            Column = (int)y.Value;
            Value = [];

            Column = (int)args[1].CInt().Value;

            for (int i = 0; i < Row; i++)
            {
                List buf = [];
                for (int j = 0; j < Column; j++)
                    buf.Append(new Int());
                Value.Append(buf);
            }        
        }
        else throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg)
    {
        if (arg is not Matrix matrix || !IsEqualSize(matrix))
            return base.Add(arg);

        Matrix buf = new(Row, Column);

        for (int i = 0; i < Row; i++)
            for (int j = 0; j < Column; j++)
                buf[i][j] = this[i][j].Add(matrix[i][j]);

        return buf;
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is not Matrix matrix || !IsEqualSize(matrix))
            return base.Sub(arg);

        Matrix buf = new(Row, Column);

        for (int i = 0; i < Row; i++)
            for (int j = 0; j < Column; j++)
                buf[i][j] = this[i][j].Sub(matrix[i][j]);

        return buf;
    }

    public override Obj Mul(Obj arg)
    {
        Matrix buf;

        if (arg is Int || arg is Float)
        {
            buf = new(Row, Column);

            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Column; j++)
                    buf[i][j] = this[i][j].Mul(arg);

            return buf;
        }
        else if (arg is Matrix matrix && Column == matrix.Row)
        {
            buf = new(Row, matrix.Column);

            for (int i = 0; i < Row; i++)
                for (int j = 0; j < matrix.Column; j++)
                    for (int k = 0; k < Column; k++)
                        buf[i][j] = buf[i][j].Add(this[i][k].Mul(matrix[k][j]));

            return buf;   
        }

        return base.Mul(arg);
    }

    public override Obj GetItem(List args)
    {
        if (args[0] is not Int i) 
            throw new IndexError("out of range");

        int index = (int)i.Value < 0 ? Row + (int)i.Value : (int)i.Value;

        if (OutOfRange(index)) 
            throw new IndexError("out of range");

        return Value[index];
    }

    public override Obj SetItem(List args)
    {
        if (args[0] is not Int i) 
            throw new IndexError("out of range");

        int index = (int)i.Value < 0 ? Row + (int)i.Value : (int)i.Value;

        if (OutOfRange(index)) 
            throw new IndexError("out of range");

        Value[(int)i.Value] = args[1];

        return Value[(int)i.Value];
    }

    public override Str CStr() => new(string.Join('\n', Value.Select(i => i.CStr().Value)));

    public override Obj Clone() => new Matrix(this);        

    public override Obj Copy() => new Matrix(this);

    bool OutOfRange(int index) => index < 0 || index >= Row;

    bool IsEqualSize(Matrix matrix) => Row == matrix.Row && Column == matrix.Column;
}        
