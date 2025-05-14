using ExcelDataReader;
using System.Data;

namespace ExcelComparatorAPI.xlComparator;

public class ContentFileReader
{
    public static List<SpreadshetContent> Read(string filePath)
    {
        if (filePath.IsExcelFile())
        {
            if (filePath.IsCSV())
            {
                return ReadCSV(filePath);
            }

            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return ReadXL(stream);
        }

        return [];
    }

    private static List<SpreadshetContent> ReadXL(FileStream stream)
    {
        using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

        ExcelDataSetConfiguration conf = new()
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true
            }
        };

        DataSet dataSet = reader.AsDataSet(conf);

        List<SpreadshetContent> workbookContent = [];

        int sheetCount = dataSet.Tables.Count;

        for (int i = 0; i < sheetCount; i++)
        {
            DataTable worksheet = dataSet.Tables[i];
            string content = worksheet.ToMarkdown();
            workbookContent.Add(new(i, worksheet.TableName, content));
        }

        return workbookContent;
    }

    private static List<SpreadshetContent> ReadCSV(string path)
    {
        List<SpreadshetContent> workbookContent = [];
        DataTable worksheet = path.ToDataTable();
        worksheet.TableName = path.ExtractName(true);
        string content = worksheet.ToMarkdown();
        workbookContent.Add(new(0, worksheet.TableName, content));
        return workbookContent;
    }
}