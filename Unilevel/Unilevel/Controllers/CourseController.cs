using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Security.Claims;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "ManageCourse")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository) 
        {
            _courseRepository = courseRepository;
        }

        [HttpPost("import-course")]
        public async Task<IActionResult> ImportCourse(IFormFile file)
        {
            try
            {
                var list = new List<FileExcelCourse>();
                using (var Stream = new MemoryStream())
                {
                    await file.CopyToAsync(Stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(Stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowcount; row++)
                        {
                            list.Add(new FileExcelCourse()
                            {
                                Number = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                Title = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                FullName = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                Email = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                CodeOfCourse = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                StartDate = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                EndDate = worksheet.Cells[row, 7].Value.ToString().Trim(),
                            });
                        }
                    }
                }

                string userId = User.FindFirstValue("id");
                var result = await _courseRepository.ImportCourse(list, userId);

                if (result.Any())
                    return BadRequest(result);

                return Ok(new { Message = "successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
