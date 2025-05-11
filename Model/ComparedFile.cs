namespace ExcelComparatorAPI.Model;

public class ComparedFile(string fileName1, string fileName2, List<ComparedPage> pages)
{
    public string FileName1 { get; set; } = fileName1;
    public string FileName2 { get; set; } = fileName2;
    public List<ComparedPage> Pages { get; set; } = pages;
    public override string ToString()
    {
        return $"{FileName1} vs {FileName2}: sheets {Pages.Count}";
    }
}