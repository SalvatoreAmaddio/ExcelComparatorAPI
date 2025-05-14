using DiffPlex.DiffBuilder.Model;
using ExcelComparatorAPI.xlComparator;

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

    public static async Task<List<ComparedPage>> CreateAsync(List<SpreadshetContent> wrkbkContent1, List<SpreadshetContent> wrkbkContent2)
    {
        IEnumerable<ComparableSheet> comparableSheets = await wrkbkContent1.JoinWorkbooksAsync(wrkbkContent2);

        await comparableSheets.CalculateDifferencesAsync();

        return comparableSheets.Select(sheet => new ComparedPage(sheet.Name,
                                                                 sheet.SideBySideResult?.OldText.Lines.ToList(),
                                                                 sheet.SideBySideResult?.NewText.Lines.ToList())).ToList();
    }
}