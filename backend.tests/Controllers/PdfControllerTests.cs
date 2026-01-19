using System;
using System.IO;
using System.Threading.Tasks;
using backend.Controllers;
using backend.Dto;
using backend.InterFaces;
using backend.data;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend.Tests.Controllers
{
    public class PdfControllerTests
    {
        [Fact]
       
        public async Task Generate_WhenFastApiReturnsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var fastApi = A.Fake<IFastApiService>();
            var repo = A.Fake<IPdfRepository>();
            var env = A.Fake<IWebHostEnvironment>();
            A.CallTo(() => env.ContentRootPath).Returns(Path.GetTempPath());

            var fakeFile = A.Fake<IFormFile>();
            A.CallTo(() => fakeFile.Length).Returns(10);
            A.CallTo(() => fakeFile.FileName).Returns("input.pdf");

            var dto = new GeneratePdfDto
            {
                file = fakeFile,           
                type = "summary",
                questionCount = 5
            };

            A.CallTo(() => fastApi.GeneratePdfAsync(dto))
                .Returns(Task.FromResult<byte[]>(Array.Empty<byte>()));

            var controller = new PdfController(fastApi, repo, env);

            // Act
            var result = await controller.Generate(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("PDF generation failed", badRequest.Value);
        }

        [Fact]
        public async Task Generate_WhenEverythingValid_ShouldReturnFileResult()
        {
            // Arrange
            var fastApi = A.Fake<IFastApiService>();
            var repo = A.Fake<IPdfRepository>();
            var env = A.Fake<IWebHostEnvironment>();
            A.CallTo(() => env.ContentRootPath).Returns(Path.GetTempPath());

            var pdfBytes = new byte[] { 1, 2, 3 };

            var fakeFile = A.Fake<IFormFile>();
            A.CallTo(() => fakeFile.Length).Returns(10);
            A.CallTo(() => fakeFile.FileName).Returns("input.pdf");

            var dto = new GeneratePdfDto
            {
                file = fakeFile,
                type = "summary",
                questionCount = 5
            };

            A.CallTo(() => fastApi.GeneratePdfAsync(dto))
                .Returns(pdfBytes);

            A.CallTo(() => repo.AddPdf(A<PdfFile>._))
                .Returns(Task.CompletedTask);

            var controller = new PdfController(fastApi, repo, env);

            // Act
            var result = await controller.Generate(dto);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal(pdfBytes, fileResult.FileContents);
        }
    }
}
