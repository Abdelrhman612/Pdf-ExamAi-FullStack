using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.data;
using backend.InterFaces;

namespace backend.Repository
{
    public class PdfRepository : IPdfRepository
    {
        private readonly AppDbContext _db;
        public PdfRepository( AppDbContext db)
        {
            _db = db;
            
        }
        async public Task AddPdf(PdfFile pdfFile)
        {
            await _db.PdfFiles.AddAsync(pdfFile);
            await _db.SaveChangesAsync();
        }
        
    }
}