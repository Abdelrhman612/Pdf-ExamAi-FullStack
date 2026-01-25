using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dto;
using backend.InterFaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class FastApiService : IFastApiService
    {
        private readonly HttpClient _httpClient;

        public FastApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> GeneratePdfAsync(GeneratePdfDto Dto)
        {
            var file = Dto.file;
            var type = Dto.type;
            var questionCount = Dto.questionCount;
            var content = new MultipartFormDataContent
            {
                {
                    new StreamContent(file.OpenReadStream()),
                    "file",
                    file.FileName
                },
                { new StringContent(type.ToString()), "type_" },
                { new StringContent(questionCount.ToString()), "question_count" }
            };

            var response = await _httpClient.PostAsync(
                "http://ai-service:8000/generate-pdf",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                throw new Exception($"FastAPI returned {response.StatusCode}: {errorContent}");
            }



            return await response.Content.ReadAsByteArrayAsync();
        }
    }

}