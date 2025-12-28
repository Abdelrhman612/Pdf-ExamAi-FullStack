using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto
{
    public class GeneratePdfDto
    {

        public required IFormFile file { get; set; }

        public string type { get; set; } = "";
        public int questionCount { get; set; } = 30;

    }
}