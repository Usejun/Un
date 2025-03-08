namespace Un;

public class StringBuffer : IComparable<StringBuffer>, IComparable<string>
{
    public int Length => buffer.Count;
    public int Capacity => buffer.Capacity;

    private readonly List<char> buffer;

    public char this[int index]
    {
        get => buffer[index];
        set => buffer[index] = value;
    }
     
    public StringBuffer() 
    {
        buffer = [];
    }

    public StringBuffer(StringBuffer sb) : this()
    {
        Append(sb);
    }

    public StringBuffer(int capacity) 
    {
        buffer = new(capacity);
    }

    public StringBuffer(string text) : this()
    {
        Append(text);
    }

    public StringBuffer(List<char> chars) : this()
    {
        buffer.AddRange(chars); 
    }

    public StringBuffer Append(char value) 
    {
        buffer.Add(value);

        return this;
    }

    public StringBuffer Append(char value, int count) 
    {
        for (int i = 0; i < count; i++)
            buffer.Add(value);

        return this;
    }

    public StringBuffer Append(string value) 
    {
        foreach (var c in value)
            buffer.Add(c);

        return this;
    }

    public StringBuffer Append(StringBuffer sb)
    {
        for (int i = 0; i < sb.Length; i++)
            Append(sb[i]);

        return this;
    }

    public StringBuffer Append<T>(T value) => Append(value!.ToString()!);

    public StringBuffer AppendLine()
    {
        buffer.Add(Literals.NewLine);

        return this;
    }

    public StringBuffer AppendLine(char value) 
    {
        buffer.Add(value);
        
        return AppendLine();
    }

    public StringBuffer AppendLine(string value) 
    {
        foreach (var c in value)
            buffer.Add(c);

        return AppendLine();
    }

    public StringBuffer AppendLine(StringBuffer sb) => Append(sb).AppendLine();

    public StringBuffer AppendLine<T>(T value) => AppendLine(value!.ToString()!);

    public StringBuffer Slice(int start, int count) => new(buffer[start..(start + count)]);

    public StringBuffer[] Split(char c, int count = -1) 
    {
        var sbs = new List<StringBuffer>(count + 1);
        int start = 0, len = 0;

        for (int end = 0; len != count && end < Length; end++)        
            if (this[end] == c)
            {
                sbs.Add(this[start..end]);
                start = end + 1;
                len++;
            }

        sbs.Add(this[start..]);

        return [..sbs];
    }

    public StringBuffer[] Split(string s, int count = -1) 
    {
        var sbs = new List<StringBuffer>(count + 1);
        int start = 0, end = 0,  len = 0;

        for (end = 0; len != count && end < Length; end++)    
        {
            if (end + s.Length <= Length && Equals(s, end))
                {
                    sbs.Add(this[start..end]);
                    start = end + s.Length;
                    len++;
                }
        }                

        sbs.Add(this[start..]);

        return [..sbs];
    }

    public int IndexOf(char c, int index = 0)
    {
        for (int i = index; i < Length; i++)
            if (this[i] == c)
                return i;
        return -1;        
    }

    public int IndexOf(string s, int index = 0)
    {
        for (int i = index; i < Length; i++)
            if (Equals(s, i))
                return i;
        return -1;        
    }

    public void Clear()
    {
        buffer.Clear();
    }

    public bool Equals(string text, int index = 0) 
    {
        for (int i = 0; i < text.Length; i++)
            if (this[i + index] != text[i])
                return false;
        return true;
    }

    public override string ToString() => new([..buffer]);

    public override int GetHashCode()
    {
        long hash = 0, now = Time.Tick;

        for (int i = 0; i < Length; i++)
        {
            hash += this[i];
            hash |= now;        
            hash |= this[i];
            hash &= now;
            hash <<= (int)(now % 8);
            hash %= int.MaxValue;    
            hash = Math.Abs(hash);
        }

        return (int)hash;
    }

    public int CompareTo(string? other)
    {
        if (other is null) return -1;

        int length = Math.Min(other.Length, Length);

        for (int i = 0; i < length; i++)
        {
            int comp = this[i].CompareTo(other[i]);

            if (comp != 0)
                return comp;
        }

        if (other.Length == Length) return 0;
        if (other.Length > Length) return -1;
        return 1;
    }

    public int CompareTo(StringBuffer? other)
    {
        if (other is null) return -1;

        int length = Math.Min(other.Length, Length);

        for (int i = 0; i < length; i++)
        {
            int comp = this[i].CompareTo(other[i]);

            if (comp != 0)
                return comp;
        }

        if (other.Length == Length) return 0;
        if (other.Length > Length) return -1;
        return 1;
    }
}