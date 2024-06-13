namespace Un.Collections;

public class Matrix : Val<List>
{
    public int row = 0, column = 0;

    public Matrix() : base("matrix", []) { }

    public Matrix(int x, int y) : base("matrix", [])
    {
        row = x;
        column = y;

        for (int i = 0; i < row; i++)
        {
            List list = [];
            for (int j = 0; j < column; j++)
                list.Append(new Int(0));
            Value.Append(list);
        }
    }

    public Matrix(Matrix matrix) : base("matrix", []) 
    {
        row = matrix.row;
        column = matrix.column;

        for (int i = 0; i < row; i++)
        {
            Value.Append(new List());
            for (int j = 0; j < column; j++)
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
            if (OutOfRange(index)) throw new IndexError("out of range");
            this.Value[index] = Value;
        }
    }

    public override Obj Init(List args)
    {
        if (args.Count == 1 && args[0] is List list)
        {
            row = (int)list.Len().Value;
            column = 0;
            Value = [];

            foreach (var item in list)
                column = Math.Max(column, (int)item.Len().Value);

            for (int i = 0; i < row; i++)
            {
                Value.Append(new List());

                foreach (var item in list[i].CList())
                    Value[i].Add(item);

                for (int j = 0; j < column - ((int)list[i].Len().Value); j++)
                    Value[i].Add(new Int());
            }
        }
        else if (args.Count == 1 && args[0] is Int x1)
        {
            row = column = (int)x1.Value;
            Value = [];           

            for (int i = 0; i < row; i++)
            {
                List buf = [];
                for (int j = 0; j < column; j++)
                    buf.Append(new Int());
                Value.Append(buf);
            }
        }     
        else if (args.Count == 2 && args[0] is Int x2 && args[1] is Bool b && b.Value)
        {
            row = column = (int)x2.Value;
            Value = [];

            for (int i = 0; i < row; i++)
            {
                List buf = [];
                for (int j = 0; j < column; j++)
                {
                    if (i == j) buf.Append(new Int(1));
                    else buf.Append(new Int());
                }
                Value.Append(buf);
            }
        }
        else if (args.Count == 2 && args[0] is Int x3 && args[1] is Int y)
        {
            row = (int)x3.Value;
            column = (int)y.Value;
            Value = [];

            column = (int)args[1].CInt().Value;

            for (int i = 0; i < row; i++)
            {
                List buf = [];
                for (int j = 0; j < column; j++)
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

        Matrix buf = new(row, column);

        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
                buf[i][j] = this[i][j].Add(matrix[i][j]);

        return buf;
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is not Matrix matrix || !IsEqualSize(matrix))
            return base.Sub(arg);

        Matrix buf = new(row, column);

        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
                buf[i][j] = this[i][j].Sub(matrix[i][j]);

        return buf;
    }

    public override Obj Mul(Obj arg)
    {
        Matrix buf;

        if (arg is Int || arg is Float)
        {
            buf = new(row, column);

            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    buf[i][j] = this[i][j].Mul(arg);

            return buf;
        }
        else if (arg is Matrix matrix && column == matrix.row)
        {
            buf = new(row, matrix.column);

            for (int i = 0; i < row; i++)
                for (int j = 0; j < matrix.column; j++)
                    for (int k = 0; k < column; k++)
                        buf[i][j] = buf[i][j].Add(this[i][k].Mul(matrix[k][j]));

            return buf;   
        }

        return base.Mul(arg);
    }

    public override Obj GetItem(List args)
    {
        if (args[0] is not Int i) 
            throw new IndexError("out of range");

        int index = (int)i.Value < 0 ? row + (int)i.Value : (int)i.Value;

        if (OutOfRange(index)) 
            throw new IndexError("out of range");

        return Value[index];
    }

    public override Obj SetItem(List args)
    {
        if (args[0] is not Int i) 
            throw new IndexError("out of range");

        int index = (int)i.Value < 0 ? row + (int)i.Value : (int)i.Value;

        if (OutOfRange(index)) 
            throw new IndexError("out of range");

        Value[(int)i.Value] = args[1];

        return Value[(int)i.Value];
    }

    public override Str CStr()
    {
        Obj str = new Str();

        foreach (var list in Value)
        {
            str = str.Add(list.CStr());
            str = str.Add(new("\n"));
        }
        
        return str.CStr();
    }

    public override Obj Clone() => new Matrix(this);        

    public override Obj Copy() => new Matrix(this);

    bool OutOfRange(int index) => index < 0 || index >= row;

    bool IsEqualSize(Matrix matrix) => row == matrix.row && column == matrix.column;
}        
