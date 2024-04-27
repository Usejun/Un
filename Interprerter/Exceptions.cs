namespace Un
{
    public class PropertyException : Exception
    {
        public PropertyException() { }
        public PropertyException(string message) : base(message) { }
        public PropertyException(string message, Exception inner) : base(message, inner) { }
    }

    public class InvalidConvertException : Exception
    {
        public InvalidConvertException() { }
        public InvalidConvertException(string message) : base(message) { }
        public InvalidConvertException(string message, Exception inner) : base(message, inner) { }
    }

    public class IndexerException : Exception
    {
        public IndexerException() { }
        public IndexerException(string message) : base(message) { }
        public IndexerException(string message, Exception inner) : base(message, inner) { }
    }

    public class UnreachableException : Exception
    {
        public UnreachableException() { }
        public UnreachableException(string message) : base(message) { }
        public UnreachableException(string message, Exception inner) : base(message, inner) { }
    }

    public class ImportException : Exception
    {
        public ImportException() { }
        public ImportException(string message) : base(message) { }
        public ImportException(string message, Exception inner) : base(message, inner) { }
    }

    public class InitializationException : Exception
    {
        public InitializationException() { }
        public InitializationException(string message) : base(message) { }
        public InitializationException(string message, Exception inner) : base(message, inner) { }
    }

    public class SyntaxException : Exception
    {
        public SyntaxException() { }
        public SyntaxException(string message) : base(message) { }
        public SyntaxException(string message, Exception inner) : base(message, inner) { }
    }

    public class AssertException : Exception
    {
        public AssertException() { }
        public AssertException(string message) : base(message) { }
        public AssertException(string message, Exception inner) : base(message, inner) { }
    }
}
