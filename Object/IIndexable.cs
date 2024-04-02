namespace Un.Object
{
    public interface IIndexable
    {
        public Obj GetByIndex(Obj index);
        public void SetByIndex(Obj index, Obj value);
    }
}
