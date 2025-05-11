namespace ExcelComparatorAPI.xlComparator;

public class SpreadshetContent(int index, string name, string content)
{
    public int Index { get; } = index;
    public string Name { get; } = name;
    public string Content { get; } = content;

    public override bool Equals(object? obj)
    {
        return obj is SpreadshetContent content &&
               Index == content.Index;
    }

    public override string ToString()
    {
        return Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index);
    }
}