using DiffPlex.DiffBuilder.Model;
using ExcelComparatorAPI.Domain.xlComparator;

namespace ExcelComparatorAPI.Domain.Model;

public class SheetComparisonResult
{
    public string SheetName { get; set; } = string.Empty;
    public List<DiffPiece>? OriginalOldLines { get; set; }
    public List<DiffPiece>? OriginalNewLines { get; set; }

    public SheetComparisonResult(string sheetName, List<DiffPiece>? originalOldLines, List<DiffPiece>? originalNewLines)
    {
        SheetName = sheetName;
        OriginalOldLines = originalOldLines;
        OriginalNewLines = originalNewLines;
    }
    
    public static async Task<List<SheetComparisonResult>> CompareAsync(List<SpreadshetContent> wrkbkContent1, List<SpreadshetContent> wrkbkContent2)
    {
        IEnumerable<ComparableSheet> comparableSheets = await wrkbkContent1.JoinWorkbooksAsync(wrkbkContent2);

        await comparableSheets.CalculateDifferencesAsync();

        return comparableSheets.Where(sheet => sheet.HasChanges).Select(sheet => new SheetComparisonResult(sheet.Name,
                                                                 sheet.SideBySideResult?.OldText.Lines.ToList(),
                                                                 sheet.SideBySideResult?.NewText.Lines.ToList())).ToList();
    }
}