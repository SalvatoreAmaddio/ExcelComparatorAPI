using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExcelComparatorAPI.Domain.Model;

public class UploadExcelRequest
{
    [FromForm(Name = "file1")]
    [SwaggerSchema("First Excel file")]
    public IFormFile File1 { get; set; } = null!;

    [FromForm(Name = "file2")]
    [SwaggerSchema("Second Excel file")]
    public IFormFile File2 { get; set; } = null!;
}