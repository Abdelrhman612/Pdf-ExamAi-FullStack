using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.data;
using backend.Dto;
using backend.InterFaces;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/pdf")]
    public class PdfController : ControllerBase
    {
        private readonly IFastApiService _fastApi;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public PdfController(
            IFastApiService fastApi,
            AppDbContext db,
            IWebHostEnvironment env)
        {
            _fastApi = fastApi;
            _db = db;
            _env = env;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate(
          [FromForm] IFormFile file,
            [FromForm] string type,
            [FromForm] int questionCount = 30)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File required");


            var pdfBytes = await _fastApi.GeneratePdfAsync(new GeneratePdfDto
            {
                file = file,
                type = type,
                questionCount = questionCount
            });

            var folder = Path.Combine(_env.ContentRootPath, "Uploads");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}.pdf";
            var fullPath = Path.Combine(folder, fileName);

            await System.IO.File.WriteAllBytesAsync(fullPath, pdfBytes);


            var pdf = new PdfFile
            {
                OriginalFileName = file.FileName,
                StoredFilePath = fullPath,
                CreatedAt = DateTime.UtcNow
            };

            _db.PdfFiles.Add(pdf);
            await _db.SaveChangesAsync();


            return File(pdfBytes, "application/pdf", $"{type}.pdf");
        }
    }

}