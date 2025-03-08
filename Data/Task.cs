

namespace Un.Data
{
    public class Task(Task<Obj> value) : Ref<Task<Obj>>("task", value)
    {
        public override void Init()
        {            
            field.Set("is_complete", new NativeFun("is_complete", 0, field =>
            {
                if (!field[Literals.Self].As<Task>(out var self))
                    throw new ValueError("invalid argument");

                return new Bool(self.Value.IsCompleted);                
            }, []));
            field.Set("is_canceled", new NativeFun("is_canceled", 0, field =>
            {
                if (!field[Literals.Self].As<Task>(out var self))
                    throw new ValueError("invalid argument");

                return new Bool(self.Value.IsCanceled);
            }, []));
            field.Set("is_faulted", new NativeFun("is_faulted", 0, field =>
            {
                if (!field[Literals.Self].As<Task>(out var self))
                    throw new ValueError("invalid argument");

                return new Bool(self.Value.IsFaulted);
            }, []));
            field.Set("wait", new NativeFun("wait", 0, field =>
            {
                if (!field[Literals.Self].As<Task>(out var self))
                    throw new ValueError("invalid argument");    
                
                if (!field["milliseconds"].As<Int>(out var milliseconds))
                        throw new ValueError("invalid argument");
                    
                if (milliseconds.Value == -1)
                    self.Value.Wait();
                else
                    self.Value.Wait((int)milliseconds.Value);

                return None;
            }, [("milliseconds", Int.MinusOne)]));
            field.Set("start", new NativeFun("start", 0, field =>
            {
                if (!field[Literals.Self].As<Task>(out var self))
                    throw new ValueError("invalid argument");

                self.Value.Start();

                return None;
            }, []));
            field.Set("result", new NativeFun("result", 0, field =>
            {
                if (!field[Literals.Self].As<Task>(out var self))
                    throw new ValueError("invalid argument");                                              

                self.Value.Start();
                self.Value.Wait();

                return self.Value.Result;
            }, []));
        }

        public override Str CStr() => new(ClassName);

        public override Obj Copy() => this;        

        public override Obj Clone() => new Task(Value)
        {
            field = new(field)
        };        
    }
}
