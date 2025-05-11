using ExcelDataReader;
using System.Data;
using System.Text;

namespace ExcelComparatorAPI.xlComparator;

public class ExcelReader
{
    public static List<SpreadshetContent> ReadWorkbook(string filePath)
    {
        using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

        ExcelDataSetConfiguration conf = new()
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true
            }
        };

        DataSet result = reader.AsDataSet(conf);

        List<SpreadshetContent> workbookContent = [];

        for (int i = 0; i < result.Tables.Count; i++)
        {
            DataTable worksheet = result.Tables[i];
            string content = ExcelToMarkdown(worksheet);
            workbookContent.Add(new(i, worksheet.TableName, content));
        }

        return workbookContent;
    }

    public static Task<IEnumerable<ComparableSheet>> JoinWorkbooksAsync(List<SpreadshetContent> workbookContent1, List<SpreadshetContent> workbookContent2)
    {
        return Task.Run(() => JoinWorkbooks(workbookContent1, workbookContent2));
    }

    public static async Task CalculateDifferencesAsync(IEnumerable<ComparableSheet> comparable)
    {
        List<Task> tasks = [];

        foreach (var item in comparable)
            tasks.Add(item.BuildAsync());

        await Task.WhenAll(tasks);
    }

    public static IEnumerable<ComparableSheet> JoinWorkbooks(List<SpreadshetContent> workbookContent1, List<SpreadshetContent> workbookContent2)
    {
        return workbookContent1.Join(
            workbookContent2,
            item1 => item1.Name,           // Key selector for first collection
            item2 => item2.Name,           // Key selector for second collection
            (workbook1, workbook2) => new
            {
                Workbook1 = workbook1,
                Workbook2 = workbook2
            }
        ).Select(s => new ComparableSheet(s.Workbook1.Name, s.Workbook1.Content, s.Workbook2.Content)).ToList();
    }

    private static string ExcelToMarkdown(DataTable table)
    {
        if (table == null || table.Rows.Count == 0)
            return "";

        StringBuilder sb = new();

        // Extract all rows including headers
        List<List<string>> allRows = new();

        // Add header row
        List<string> headerRow = table.Columns
            .Cast<DataColumn>()
            .Select(col => col.ColumnName)
            .ToList();
        allRows.Add(headerRow);

        // Add data rows
        foreach (DataRow row in table.Rows)
        {
            List<string> rowData = new();
            foreach (DataColumn col in table.Columns)
            {
                string cell = row[col]?.ToString() ?? string.Empty;
                rowData.Add(cell);
            }
            allRows.Add(rowData);
        }

        // Get max number of columns across all rows
        int columnCount = allRows.Max(row => row.Count);

        // Calculate max width for each column
        int[] columnWidths = new int[columnCount];
        foreach (List<string> row in allRows)
        {
            for (int i = 0; i < row.Count; i++)
            {
                string cell = row[i] ?? string.Empty;
                columnWidths[i] = Math.Max(columnWidths[i], cell.Length);
            }
        }

        // Format a row with tab-aligned, padded cells
        string FormatRow(List<string> row)
        {
            var cells = new string[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                string cell = i < row.Count ? row[i] : string.Empty;
                cell = cell.Trim();
                cells[i] = cell.PadRight(columnWidths[i]);
            }
            return string.Join("\t", cells);
        }

        // Add header
        sb.AppendLine(FormatRow(allRows[0]));

        // Add data rows
        foreach (List<string> row in allRows.Skip(1))
            sb.AppendLine(FormatRow(row));

        return sb.ToString();
    }
}