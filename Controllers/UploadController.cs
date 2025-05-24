using ExcelComparatorAPI.Domain.Model;
using ExcelComparatorAPI.Domain.xlComparator;
using ExcelComparatorAPI.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ExcelComparatorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    [HttpPost("excel")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadExcelRequest request)
    {
        if (request.File1 == null || request.File2 == null)
            return BadRequest("Both files must be provided.");

        if (!request.File1.FileName.IsExcelFile() || !request.File2.FileName.IsExcelFile())
            return BadRequest("One or both of the files provided are not valid excel files.");

        string tempPath = Path.GetTempPath();

        Guid guid1 = Guid.NewGuid();
        Guid guid2 = Guid.NewGuid();

        string path1 = Path.Combine(tempPath, $"{guid1}_{request.File1.FileName}");
        string path2 = Path.Combine(tempPath, $"{guid2}_{request.File2.FileName}");

        try
        {
            Task copyFile1Task = Task.Run(async () =>
            {
                using FileStream stream = new(path1, FileMode.Create);
                await request.File1.CopyToAsync(stream);
            });

            Task copyFile2Task = Task.Run(async () =>
            {
                using FileStream stream = new(path2, FileMode.Create);
                await request.File2.CopyToAsync(stream);
            });

            await Task.WhenAll(copyFile1Task, copyFile2Task);

            Task<List<SpreadshetContent>> readXlTask1 = Task.Run(() => XLContentFileReader.Read(path1));
            Task<List<SpreadshetContent>> readXlTask2 = Task.Run(() => XLContentFileReader.Read(path2));

            await Task.WhenAll(readXlTask1, readXlTask2);

            List<SpreadshetContent> workbookContent1 = readXlTask1.Result;
            List<SpreadshetContent> workbookContent2 = readXlTask2.Result;

            List<SheetComparisonResult> comparedSheets = await SheetComparisonResult.CompareAsync(workbookContent1, workbookContent2);

            ComparedWorkbook comparedWorkbook = new(request.File1.FileName, request.File2.FileName, comparedSheets);

            if (comparedWorkbook.HasChanges)
            {
                //await FileManager.SaveAsJSONAsync(fileData); // for debug purposes
                return Ok(new { message = "Files received and processed.", data = comparedWorkbook });
            }
            else
            {
                return NoContent();
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
        }
        finally
        {
            FileManager.DeleteFile(path1);
            FileManager.DeleteFile(path2);
        }
    }
}