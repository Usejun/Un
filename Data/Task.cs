namespace Un.Data
{
    public class Task(Task<Obj> value) : Ref<Task<Obj>>("task", value)
    {
        public override void Init()
        {            
            field.Set("is_complete", new NativeFun("is_complete", 1, args =>
            {
                if (args[0] is not Task self)
                    throw new ValueError("invalid argument");

                return new Bool(self.Value.IsCompleted);                
            }));
            field.Set("is_canceled", new NativeFun("is_canceled", 1, args =>
            {
                if (args[0] is not Task self)
                    throw new ValueError("invalid argument");

                return new Bool(self.Value.IsCanceled);
            }));
            field.Set("is_faulted", new NativeFun("is_faulted", 1, args =>
            {
                if (args[0] is not Task self)
                    throw new ValueError("invalid argument");

                return new Bool(self.Value.IsFaulted);
            }));
            field.Set("wait", new NativeFun("wait", -1, args =>
            {
                if (args[0] is not Task self)
                    throw new ValueError("invalid argument");              
                if (args.Count == 2)
                {
                    if (args[1] is not Int milliseconds)
                        throw new ValueError("invalid argument");
                    
                    self.Value.Wait((int)milliseconds.Value);
                }
                else
                {
                    self.Value.Wait();                
                }

                return None;
            }));
            field.Set("start", new NativeFun("start", 1, args =>
            {
                if (args[0] is not Task self)
                    throw new ValueError("invalid argument");

                self.Value.Start();

                return None;
            }));
            field.Set("run", new NativeFun("run", 1, args =>
            {
                if (args[0] is not Task self)
                    throw new ValueError("invalid argument");

                self.Value.Start();
                self.Value.Wait();

                return self.Value.Result;
            }));
            field.Set("result", new NativeFun("result", 1, args =>
            {
                if (args[0] is not Task self)
                    throw new ValueError("invalid argument");                               

                return self.Value.Result;
            }));
        }

        public override Obj Copy() => this;        

        public override Obj Clone() => new Task(Value)
        {
            field = new(field)
        };        
    }
}
