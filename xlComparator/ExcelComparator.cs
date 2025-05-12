using ExcelComparatorAPI.Model;

namespace ExcelComparatorAPI.xlComparator;

public class ExcelComparator
{
    public static async Task<List<ComparedPage>> ReadAsync(string originalFilePath, string newFilePath)
    {
        Task<List<SpreadshetContent>> readExcelTask1 = Task.Run(() => ExcelReader.ReadWorkbook(originalFilePath));
        Task<List<SpreadshetContent>> readExcelTask2 = Task.Run(() => ExcelReader.ReadWorkbook(newFilePath));

        await Task.WhenAll(readExcelTask1, readExcelTask2);

        List<SpreadshetContent> workbookContent1 = readExcelTask1.Result;
        List<SpreadshetContent> workbookContent2 = readExcelTask2.Result;

        IEnumerable<ComparableSheet> comparableSheets = await ExcelReader.JoinWorkbooksAsync(workbookContent1, workbookContent2);

        await ExcelReader.CalculateDifferencesAsync(comparableSheets);

        List<ComparedPage> comparedPages = comparableSheets.Select(s=> new ComparedPage(s.Name, 
                                                                                        s.SideBySideResult?.OldText.Lines.ToList(), 
                                                                                        s.SideBySideResult?.NewText.Lines.ToList())).ToList();

        return comparedPages;
    }
}