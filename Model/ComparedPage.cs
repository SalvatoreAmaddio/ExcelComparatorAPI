using DiffPlex.DiffBuilder.Model;

namespace ExcelComparatorAPI.Model;

public class ComparedPage
{
    public string SheetName { get; set; } = string.Empty;
    public List<DiffPiece>? OriginalOldLines { get; set; }
    public List<DiffPiece>? OriginalNewLines { get; set; }

    public ComparedPage(string sheetName, List<DiffPiece>? originalOldLines, List<DiffPiece>? originalNewLines)
    {
        SheetName = sheetName;        
        OriginalOldLines = originalOldLines;
        OriginalNewLines = originalNewLines;
    }
}