namespace ExcelComparatorAPI.Domain.xlComparator;

public enum XLExt
{
    XLS = 0, // Excel 97-2003 Workbook
    XLSX = 1, // Excel Workbook
    XLSM = 2, // Macro-enabled Workbook
    XLSB = 3, // Binary Workbook
    CSV = 4, // CSV, often opened with Excel
}