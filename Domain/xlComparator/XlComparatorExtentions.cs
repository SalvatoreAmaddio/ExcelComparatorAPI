using System.Data;
using System.Text;

namespace ExcelComparatorAPI.Domain.xlComparator;

public static class XlComparatorExtentions
{
    public static string? ExtractName(this string filePath, bool removeExt = false)
    {
        try
        {
            if (removeExt)
            {
                string? fileName = filePath.Split("\\").LastOrDefault();
                string? ext = Path.GetExtension(filePath)?.ToLowerInvariant();

                if (fileName != null && ext != null)
                {
                    return fileName.Split(ext)[0];
                }

                return string.Empty;
            }

            return filePath.Split("\\").LastOrDefault();
        }
        catch
        {
            return null;
        }
    }

    public static bool IsXLS(this string path)
    {
        string? ext = path.GetExt();
        return ext == XLExt.XLS.ToString();
    }

    public static bool IsXLSX(this string path)
    {
        string? ext = path.GetExt();
        return ext == XLExt.XLSM.ToString();
    }

    public static bool IsXLSM(this string path)
    {
        string? ext = path.GetExt();
        return ext == XLExt.XLSM.ToString(); ;
    }

    public static bool IsXLSB(this string path)
    {
        string? ext = path.GetExt();
        return ext == XLExt.XLSB.ToString();
    }

    public static bool IsCSV(this string path)
    {
        string? ext = path.GetExt();;
        return ext == XLExt.CSV.ToString();
    }

    public static bool IsExcelFile(this string path)
    {
        string? ext = path.GetExt();
        return Enum.GetValues<XLExt>().Select(s => s.ToString()).Any(s=>s.Equals(ext));
    }

    public static string? GetExt(this string path) 
    {
        return Path.GetExtension(path)?.ToUpperInvariant().Remove(0, 1);
    }

    public static DataTable ToDataTable(this string path, char delimiter = ',')
    {
        using var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(path)
        {
            TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited,
            Delimiters = [delimiter.ToString()],
            HasFieldsEnclosedInQuotes = true
        };

        DataTable table = new();

        string[]? firstLine = parser.ReadFields();

        if (firstLine == null)
            return table;

        for (int i = 0; i < firstLine.Length; i++)
            table.Columns.Add(firstLine[i], typeof(string));

        while (!parser.EndOfData)
        {
            string[]? c = parser.ReadFields();
            if (c != null)
                table.Rows.Add(c);
        }

        return table;
    }

    public static Task<IEnumerable<ComparableSheet>> JoinWorkbooksAsync(this List<SpreadshetContent> workbookContent1, List<SpreadshetContent> workbookContent2)
    {
        return Task.Run(() => workbookContent1.JoinWorkbooks(workbookContent2));
    }

    public static IEnumerable<ComparableSheet> JoinWorkbooks(this List<SpreadshetContent> workbookContent1, List<SpreadshetContent> workbookContent2)
    {
        return workbookContent1.Join(
            workbookContent2,
            item1 => item1.Index,           // Key selector for first collection
            item2 => item2.Index,           // Key selector for second collection
            (workbook1, workbook2) => new
            {
                Workbook1 = workbook1,
                Workbook2 = workbook2
            }
        ).Select(s => new ComparableSheet(s.Workbook1.Name, s.Workbook1.Content, s.Workbook2.Content)).ToList();
    }

    public static async Task CalculateDifferencesAsync(this IEnumerable<ComparableSheet> comparable)
    {
        List<Task> tasks = [];

        foreach (var item in comparable)
            tasks.Add(item.BuildAsync());

        await Task.WhenAll(tasks);
    }

    public static string ToMarkdown(this DataTable table)
    {
        if (table == null || table.Rows.Count == 0)
            return string.Empty;

        StringBuilder sb = new();

        // Extract all rows including headers
        List<List<string>> allRows = [];

        // Add header row
        List<string> headerRow = table.Columns
            .Cast<DataColumn>()
            .Select(col => col.ColumnName)
            .ToList();

        allRows.Add(headerRow);

        // Add data rows
        foreach (DataRow row in table.Rows)
        {
            List<string> rowData = [];
            
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

        // Local Function to format a row with tab-aligned, padded cells
        string FormatRow(List<string> row)
        {
            string[] cells = new string[columnCount];

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