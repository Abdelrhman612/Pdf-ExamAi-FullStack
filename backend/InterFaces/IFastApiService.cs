using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dto;

namespace backend.InterFaces
{
    public interface IFastApiService
    {
        Task<byte[]> GeneratePdfAsync( GeneratePdfDto generatePdfDtot);


    }
}