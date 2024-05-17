namespace Un.Collections;

public class Matrix : Val<Iter>
{
    public int row = 0, column = 0;

    public Matrix() : base("matrix", []) { }

    public Matrix(int x, int y) : base("matrix", [])
    {
        row = x;
        column = y;

        for (int i = 0; i < row; i++)
        {
            Iter iter = [];
            for (int j = 0; j < column; j++)
                iter.Append(new Int(0));
            value.Append(iter);
        }
    }

    public Matrix(Matrix matrix) : base("matrix", []) 
    {
        row = matrix.row;
        column = matrix.column;

        for (int i = 0; i < row; i++)
        {
            value.Append(new Iter());
            for (int j = 0; j < column; j++)
                value[i].Add(matrix[i][j].Clone());
        }
    }

    public Iter this[int index]
    {
        get
        {
            if (OutOfRange(index)) throw new IndexError("out of range");
            return (Iter)value[index];
        }
        set
        {
            if (OutOfRange(index)) throw new IndexError("out of range");
            this.value[index] = value;
        }
    }

    public override Obj Init(Iter args)
    {
        if (args.Count == 1 && args[0] is Iter iter)
        {
            row = (int)iter.Len().value;
            column = 0;
            value = [];

            foreach (var item in iter)
                column = Math.Max(column, (int)item.Len().value);

            for (int i = 0; i < row; i++)
            {
                value.Append(new Iter());

                foreach (var item in iter[i].CIter())
                    value[i].Add(item);

                for (int j = 0; j < column - ((int)value[i].Len().value); j++)
                    value[i].Add(new Int());
            }
        }
        else if (args.Count == 1 && args[0] is Int x1)
        {
            row = column = (int)x1.value;
            value = [];           

            for (int i = 0; i < row; i++)
            {
                Iter buf = [];
                for (int j = 0; j < column; j++)
                    buf.Append(new Int());
                value.Append(buf);
            }
        }     
        else if (args.Count == 2 && args[0] is Int x2 && args[1] is Bool b && b.value)
        {
            row = column = (int)x2.value;
            value = [];

            for (int i = 0; i < row; i++)
            {
                Iter buf = [];
                for (int j = 0; j < column; j++)
                {
                    if (i == j) buf.Append(new Int(1));
                    else buf.Append(new Int());
                }
                value.Append(buf);
            }
        }
        else if (args.Count == 2 && args[0] is Int x3 && args[1] is Int y)
        {
            row = (int)x3.value;
            column = (int)y.value;
            value = [];

            column = (int)args[1].CInt().value;

            for (int i = 0; i < row; i++)
            {
                Iter buf = [];
                for (int j = 0; j < column; j++)
                    buf.Append(new Int());
                value.Append(buf);
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

    public override Obj GetItem(Iter args)
    {
        if (args[0] is not Int i) 
            throw new IndexError("out of range");

        int index = (int)i.value < 0 ? row + (int)i.value : (int)i.value;

        if (OutOfRange(index)) 
            throw new IndexError("out of range");

        return value[index];
    }

    public override Obj SetItem(Iter args)
    {
        if (args[0] is not Int i) 
            throw new IndexError("out of range");

        int index = (int)i.value < 0 ? row + (int)i.value : (int)i.value;

        if (OutOfRange(index)) 
            throw new IndexError("out of range");

        value[(int)i.value] = args[1];

        return value[(int)i.value];
    }

    public override Str CStr()
    {
        Obj str = new Str();

        foreach (var iter in value)
        {
            str = str.Add(iter.CStr());
            str = str.Add(new("\n"));
        }
        
        return str.CStr();
    }

    public override Obj Clone() => new Matrix(this);        

    public override Obj Copy() => new Matrix(this);

    bool OutOfRange(int index) => index < 0 || index >= row;

    bool IsEqualSize(Matrix matrix) => row == matrix.row && column == matrix.column;
}        
