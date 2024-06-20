namespace Un.Data
{
    public class Para
    {
        public string Name { get; protected set; } 
        public Obj DefaultValue { get; protected set; }
        
        public Para(string name, Obj defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }

        public Para(List<Token> tokens)
        {
            if (tokens.Count == 1)
            {
                Name = tokens[0].Value;
                DefaultValue = Obj.None;
            }
            else if (tokens.Count == 3 && tokens[1].type == Token.Type.Assign)
            {
                Name = tokens[0].Value;
                DefaultValue = Obj.Literal(tokens[2].Value);
            }
            else throw new SyntaxError();
        }


    }
}
