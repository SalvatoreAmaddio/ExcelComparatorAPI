namespace ExcelComparatorAPI.Domain.Model;

public class ComparedWorkbook(string fileName1, string fileName2, List<SheetComparisonResult> sheets)
{
    public string FileName1 { get; set; } = fileName1;
    public string FileName2 { get; set; } = fileName2;
    public List<SheetComparisonResult> Sheets { get; set; } = sheets;
    public bool HasChanges => Sheets.Count > 0;
    public override string ToString()
    {
        return $"{FileName1} vs {FileName2}: sheets {Sheets.Count}";
    }
}