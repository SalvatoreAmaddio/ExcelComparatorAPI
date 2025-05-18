using ExcelComparatorAPI.Model;
using ExcelComparatorAPI.Utils;
using ExcelComparatorAPI.xlComparator;
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

        string tempPath = Path.GetTempPath();

        Guid ui1 = Guid.NewGuid();
        Guid ui2 = Guid.NewGuid();

        string path1 = Path.Combine(tempPath, $"{ui1}_{request.File1.FileName}");
        string path2 = Path.Combine(tempPath, $"{ui2}_{request.File2.FileName}");

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

            Task<List<SpreadshetContent>> readExcelTask1 = Task.Run(() => ContentFileReader.Read(path1));
            Task<List<SpreadshetContent>> readExcelTask2 = Task.Run(() => ContentFileReader.Read(path2));

            await Task.WhenAll(readExcelTask1, readExcelTask2);

            List<SpreadshetContent> workbookContent1 = readExcelTask1.Result;
            List<SpreadshetContent> workbookContent2 = readExcelTask2.Result;

            List<ComparedPage> comparedPages = await ComparedPage.CreateAsync(workbookContent1, workbookContent2);

            ComparedFile fileData = new(request.File1.FileName, request.File2.FileName, comparedPages);

            if (fileData.Pages.Count == 0)
            {
                return NoContent();
            }
            else
            {
                //await FileManager.SaveAsJSONAsync(fileData); // for debug
                return Ok(new { message = "Files received and processed.", data = fileData });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
        finally
        {
            FileManager.DeleteFile(path1);
            FileManager.DeleteFile(path2);
        }
    }
}