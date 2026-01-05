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
        private readonly IPdfRepository _repo;
        private readonly IWebHostEnvironment _env;

        public PdfController(
            IFastApiService fastApi,
            IPdfRepository repo,
            IWebHostEnvironment env)
        {
            _fastApi = fastApi;
            _repo = repo;
            _env = env;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate(GeneratePdfDto dto)
        {
            var file = dto.file;
            var type = dto.type;
            if (file == null || file.Length == 0)
                return BadRequest("File required");

            var pdfBytes = await _fastApi.GeneratePdfAsync(dto);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return BadRequest("PDF generation failed");
            }

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


            await _repo.AddPdf(pdf);

            return File(pdfBytes, "application/pdf", $"{type}.pdf");
        }
    }

}