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
        string path1 = Path.Combine(tempPath, request.File1.FileName);
        string path2 = Path.Combine(tempPath, request.File2.FileName);

        Task t1 = Task.Run(async () =>
        {
            using FileStream stream = new(path1, FileMode.Create);
            await request.File1.CopyToAsync(stream);
        });

        Task t2 = Task.Run(async () =>
        {
            using FileStream stream = new(path2, FileMode.Create);
            await request.File2.CopyToAsync(stream);
        });

        await Task.WhenAll(t1, t2);

        List<ComparedPage> sheets = await ExcelComparator.ReadAsync(path1, path2);

        ComparedFile fileData = new(request.File1.FileName, request.File2.FileName, sheets);

        //await FileManager.SaveAsJSONAsync(fileData); // for debug

        FileManager.DeleteFile(path1);
        FileManager.DeleteFile(path2);

        return Ok(new { message = "Files received and saved.", data = fileData });
    }
}