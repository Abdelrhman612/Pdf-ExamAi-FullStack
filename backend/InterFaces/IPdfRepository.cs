using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.InterFaces
{
    public interface IPdfRepository
    {
        Task AddPdf(PdfFile pdfFile);
    }
}