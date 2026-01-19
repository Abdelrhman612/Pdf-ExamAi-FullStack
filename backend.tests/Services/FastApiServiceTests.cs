using System.Net;
using backend.Dto;
using backend.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Xunit;

public class FastApiServiceTests
{
    [Fact]
    public async Task GeneratePdfAsync_WhenApiReturnsSuccess_ShouldReturnPdfBytes()
    {
        // Arrange
        var expectedBytes = new byte[] { 1, 2, 3 };

        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(expectedBytes)
        };

        var httpClient = FakeHttpMessageHandler.Create(fakeResponse);
        var service = new FastApiService(httpClient);

        var fakeFile = A.Fake<IFormFile>();
        A.CallTo(() => fakeFile.OpenReadStream())
            .Returns(new MemoryStream(new byte[] { 10, 20 }));

        A.CallTo(() => fakeFile.FileName)
            .Returns("test.pdf");

        var dto = new GeneratePdfDto
        {
            file = fakeFile,
            type = "summary",
            questionCount = 5
        };

        // Act
        var result = await service.GeneratePdfAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedBytes, result);
    }
}
