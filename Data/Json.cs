public class Json : Obj
{
    private const int TABSIZE = 2;

    public Obj top;

    public Json() : base("json")
    {
        top = None;
    }

    public Json(string str) : this()
    {
        top = new JsonParser(str).Parse();  
    }
    public Json(Str str) : this()
    {
        top = new JsonParser(str.Value).Parse();  
    }

    public override Obj Init(Un.Collections.Tuple args, Field field)
    {
        field.Merge(args, [("value", null!)], 1);

        if (!field["value"].As<Str>(out var str))
            top = new JsonParser(str.Value).Parse();
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override Str CStr()
    {
        StringBuffer sb = new();

        ToString(top);

        return new Str(sb.ToString());

        void ToString(Obj obj, int depth = 1)
        {
            if (obj.As<List>(out var list))
            {
                if (list.Count == 0)
                    sb.Append(Literals.LBrack);
                else
                    sb.AppendLine(Literals.LBrack);

                foreach (var value in list)
                {
                    sb.Append(Literals.Space, depth * TABSIZE);
                    ToString(value, depth + 1);
                    sb.AppendLine(Literals.CComma);
                }

                if (list.Count > 0) 
                    sb.Append(Literals.Space, (depth - 1) * TABSIZE);
                sb.Append(Literals.RBrack);
            }
            else if (obj.As<Dict>(out var dict))
            {
                if (dict.Value.Count == 0)
                    sb.Append(Literals.LBrace);
                else    
                    sb.AppendLine(Literals.LBrace);

                foreach ((var key, var value) in dict.Value)
                {
                    sb.Append(Literals.Space, depth * TABSIZE);
                    sb.Append(Literals.Double).Append(key.CStr().Value).Append(Literals.Double)
                       .Append(Literals.Space).Append(Literals.CColon).Append(Literals.Space, TABSIZE);
                    ToString(value, depth + 1);
                    sb.AppendLine(Literals.CComma);
                }
                if (dict.Value.Count > 0) 
                    sb.Append(Literals.Space, (depth - 1) * TABSIZE);
                sb.Append(Literals.RBrace);
            }
            else 
            {
                if (obj.As<Str>(out var str))
                    sb.Append(Literals.Double).Append(str.Value).Append(Literals.Double);
                else
                    sb.Append(obj.CStr().Value);
            }
        }
    }

    public override Obj Clone() => new Json() { top = top };

    private class JsonParser(string str)
    {
        private int index = 0;

        public Obj Parse()
        {
            SkipWhiteSpace();
            return str[index] switch
            {
                Literals.CLBrace => ParseDict(),
                Literals.CLBrack => ParseList(),
                Literals.EDouble => ParseStr(),
                Literals.t => ParseTrue(),
                Literals.f => ParseFalse(),
                Literals.n => ParseNull(),
                _ => ParseNum(),
            };
        }

        private void SkipWhiteSpace()
        {
            while (index < str.Length && char.IsWhiteSpace(str[index]))
                index++;
        }

        private Obj ParseNum()
        {
            StringBuffer sb = new();

            while (index < str.Length && char.IsDigit(str[index]))
            {
                sb.Append(str[index]);
                index++;
            }

            if (str[index] != Literals.CDot)
                return new Int(sb.ToString());
            
            sb.Append(str[index]);

            while (index < str.Length && (char.IsDigit(str[index]) || str[index] == 'e' || str[index] == 'E'))
            {
                sb.Append(str[index]);
                index++;
            }

            return new Float(sb.ToString());
        }

        private Obj ParseNull()
        {
            if (str.Substring(index, 4) == Literals.Null)
            {
                index += 4;
                return None;
            }

            throw new ValueError("invalid json, parse error");
        }

        private Bool ParseFalse()
        {
            if (str.Substring(index, 5) == Literals.False)
            {
                index += 5;
                return Bool.False;
            }

            throw new ValueError("invalid json, parse error");
        }

        private Bool ParseTrue()
        {
            if (str.Substring(index, 4) == Literals.True)
            {
                index += 4;
                return Bool.True;
            }

            throw new ValueError("invalid json, parse error");
        }

        private Str ParseStr()
        {
            StringBuffer sb = new();    
            index++;

            while (str[index] != Literals.Double)
            {
                if (str[index] == Literals.Escape && Token.Escape.TryGetValue(str[index + 1], out var c))
                {
                    index += 2;
                    sb.Append(c);
                }
                else if (str[index] == Literals.Escape && str[index + 1] == Literals.u)
                {
                    index += 2;
                    sb.Append((char)System.Convert.ToInt32(str.Substring(index, 4), 16));
                    index += 4;
                }
                else if (str[index] == Literals.Escape && str[index + 1] == Literals.CSlash)
                {
                    index += 2;
                    sb.Append(Literals.CSlash); 
                }
                else
                {
                    sb.Append(str[index]);
                    index++;
                }                
            }
            
            index++;

            return new Str(sb.ToString());
        }

        private List ParseList()
        {
            List list = [];
            index++;

            while (str[index] != Literals.CRBrack)
            {
                SkipWhiteSpace();
                var value = Parse();
                SkipWhiteSpace();

                list.Append(value);

                if (str[index] == Literals.CComma) 
                    index++;                            
            }
            index++;

            return list;
        }

        private Dict ParseDict()
        {
            Dict dict = new();
            index++;

            while (str[index] != Literals.CRBrace)
            {                
                SkipWhiteSpace();
                var name = ParseStr();
                SkipWhiteSpace();                

                if (str[index] != Literals.CColon)
                    throw new ValueError("invalid json, parse error");
                index++;

                SkipWhiteSpace();
                var value = Parse();
                SkipWhiteSpace();

                dict.Value.Add(name, value);

                if (str[index] == Literals.CComma)
                {
                    index++;
                    SkipWhiteSpace();                                
                }
            }           

            return dict; 
        }
    }
}