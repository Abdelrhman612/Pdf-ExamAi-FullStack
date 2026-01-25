# 📄 PDF AI Generator - Exam & Summary Generator

A full-stack web application that uses AI to automatically generate quizzes, bubble sheets, and summaries from uploaded PDF documents. This system helps educators and students create exam materials and study aids efficiently.

## 🚀 Features

### **Core Functionality**
- **AI-Powered Content Generation**: Uses GPT-4 to analyze PDF content
- **Multiple Output Types**:
  - **Quiz**: Multiple-choice questions with 4 options each
  - **Bubble Sheet**: Answer sheets with correct answers marked (✓)
  - **Summary**: Intelligent content summarization for study purposes
- **Arabic Language Support**: Full Arabic text processing with proper RTL rendering
- **Bulk Generation**: Generate up to 50 questions in batches

### **Technical Features**
- **Full-Stack Architecture**: Microservices with containerized deployment
- **Database Storage**: Automatically saves generated PDFs with metadata
- **File Management**: Secure upload and storage system
- **Responsive UI**: Modern React frontend with Tailwind CSS
- **API Integration**: RESTful APIs with FastAPI and ASP.NET Core

## 🏗️ Architecture

### **System Components**

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   React Frontend│───▶│ ASP.NET Backend │───▶│  FastAPI AI     │
│   (Port 3000)   │    │  (Port 5000)    │    │  (Port 8000)    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                              │                        │
                              ▼                        ▼
                       ┌─────────────────┐    ┌─────────────────┐
                       │  SQL Server     │    │  OpenAI API     │
                       │  (Port 1433)    │    │  (External)     │
                       └─────────────────┘    └─────────────────┘
```

### **Technology Stack**

| Component | Technology | Purpose |
|-----------|------------|---------|
| **Frontend** | React 19 + TypeScript + Vite | User interface |
| **Backend** | ASP.NET Core 8.0 | Main API server & PDF management |
| **AI Service** | FastAPI (Python) | PDF processing & AI generation |
| **Database** | SQL Server 2022 | Store PDF metadata |
| **Containerization** | Docker + Docker Compose | Environment management |
| **PDF Processing** | PyPDF2, ReportLab | PDF manipulation |
| **AI Integration** | OpenAI GPT-4 | Content generation |

## 📂 Project Structure

```
pdf-exam-ai/
├── docker-compose.yml           # Multi-container orchestration
├── ai-service/                  # Python FastAPI AI service
│   ├── Dockerfile
│   ├── main.py                  # FastAPI endpoints
│   ├── service/
│   │   ├── ai_engine.py         # OpenAI integration
│   │   └── pdf_generator.py     # PDF generation with Arabic support
│   └── requirements.txt
├── backend/                     # ASP.NET Core API
│   ├── Controllers/PdfController.cs
│   ├── Services/FastApiService.cs
│   ├── Repository/PdfRepository.cs
│   ├── data/                    # Entity Framework models
│   ├── DTOs/                    # Data transfer objects
│   └── Migrations/              # Database migrations
├── frontend/                    # React application
│   ├── src/pages/GeneratePdf.tsx
│   ├── src/Services/            # API client
│   └── public/
└── backend.tests/               # Unit tests
```

## 🛠️ Installation & Setup

### **Prerequisites**
- Docker and Docker Compose
- OpenAI API key
- SQL Server (or use Docker version)

### **Environment Variables**

Create a `.env` file in the root directory:

```env
# Database
SA_PASSWORD=YourStrong@Password123
DB_PASSWORD=YourStrong@Password123

# OpenAI
OPEN_AI_KEY=sk-your-openai-api-key-here

# Backend Configuration
ConnectionStrings__Default=Server=sqlserver;Database=pdfdb;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True

# CORS
allowedOrigins__0=http://localhost:3000

# Frontend
VITE_API_SERVICE_URL=http://localhost:5000
```

### **Quick Start with Docker**

```bash
# 1. Clone the repository
git clone <repository-url>
cd pdf-exam-ai

# 2. Set up environment variables
cp .env.example .env
# Edit .env with your OpenAI API key

# 3. Build and run all services
docker-compose up --build

# 4. Access the application
# Frontend: http://localhost:3000
# Backend API: http://localhost:5000
# AI Service: http://localhost:8000
# Swagger UI: http://localhost:5000/swagger
```

### **Manual Setup (Development)**

#### **AI Service Setup**
```bash
cd ai-service
python -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate
pip install -r requirements.txt

# Place Arabic font in ai-service/fonts/
mkdir fonts
# Copy Amiri-Regular.ttf to fonts/
```

#### **Backend Setup**
```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

#### **Frontend Setup**
```bash
cd frontend
npm install
npm run dev
```

## 📝 API Documentation

### **Main Endpoints**

#### **1. Generate PDF (Backend)**
```
POST /api/pdf/generate
Content-Type: multipart/form-data

Parameters:
- file: PDF file (required)
- type: "quiz" | "bubblesheet" | "summary" (required)
- questionCount: number (optional, default: 30, max: 50)
```

#### **2. AI Service Endpoint**
```
POST /generate-pdf
Content-Type: multipart/form-data

Parameters:
- file: PDF file
- type_: "quiz" | "bubblesheet" | "summary"
- question_count: number
```

### **Example Usage**

```bash
# Using curl
curl -X POST http://localhost:5000/api/pdf/generate \
  -F "file=@document.pdf" \
  -F "type=quiz" \
  -F "questionCount=20" \
  --output generated_quiz.pdf
```

## 🧪 Testing

```bash
# Run backend tests
cd backend.tests
dotnet test

# Test individual components
dotnet test --filter "Category=Unit"
```

## 🔧 Configuration

### **AI Service Configuration**
- **Max Questions**: 50 (configurable in `ai-service/main.py`)
- **Retry Logic**: 2 retries for temporary failures
- **Rate Limiting**: Automatic handling of OpenAI rate limits
- **Arabic Font**: Uses Amiri-Regular.ttf for proper Arabic rendering

### **Database Configuration**
The application uses Entity Framework Core with code-first migrations:

```csharp
// To create new migration:
dotnet ef migrations add MigrationName
dotnet ef database update
```

## 🚢 Deployment

### **Production Considerations**

1. **Environment Variables**: Secure sensitive data
2. **Database**: Use persistent volumes for SQL Server
3. **OpenAI API**: Monitor usage and implement caching
4. **Security**: Enable HTTPS, CORS restrictions
5. **Scaling**: Consider separating services for high load

### **Docker Production Build**
```bash
# Build optimized images
docker-compose -f docker-compose.yml build --no-cache

# Run in detached mode
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

## 📊 Database Schema

```sql
CREATE TABLE PdfFiles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OriginalFileName NVARCHAR(MAX) NOT NULL,
    StoredFilePath NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
```

## 🎨 Frontend Features

- **File Upload**: Drag & drop or file selector
- **Real-time Preview**: PDF preview in browser
- **Download Option**: Direct download of generated PDFs
- **Error Handling**: User-friendly error messages
- **Loading States**: Visual feedback during processing

## 🔍 Troubleshooting

### **Common Issues**

1. **SQL Server Connection Error**
   ```bash
   # Check if SQL Server container is running
   docker-compose ps
   
   # View logs
   docker-compose logs sqlserver
   ```

2. **OpenAI API Errors**
   - Verify `OPEN_AI_KEY` in `.env`
   - Check rate limits in OpenAI dashboard
   - Ensure sufficient credits

3. **Arabic Text Rendering**
   - Ensure `fonts/Amiri-Regular.ttf` exists in ai-service
   - Check PDF generation logs for font errors

4. **Port Conflicts**
   - Check if ports 3000, 5000, 8000, 1433 are available
   - Modify `docker-compose.yml` for different ports

### **Logs & Monitoring**

```bash
# View all service logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f ai-service
docker-compose logs -f backend
docker-compose logs -f sqlserver

# Check service health
curl http://localhost:5000/  # Should return "Server is running..."
```

## 📈 Performance Tips

1. **PDF Size**: Optimize PDFs before upload (smaller files process faster)
2. **Question Count**: Start with fewer questions for faster results
3. **Caching**: Implement Redis for frequently processed documents
4. **Batch Processing**: Queue large jobs for background processing

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit changes
4. Push to the branch
5. Create a Pull Request

### **Development Guidelines**
- Write unit tests for new features
- Follow existing code style
- Update documentation
- Use meaningful commit messages

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- OpenAI for GPT-4 API
- ReportLab for PDF generation
- Microsoft for .NET and SQL Server
- React and FastAPI communities

## 📞 Support

For issues, questions, or contributions:
1. Check existing issues
2. Create a new issue with detailed description
3. Include error logs and reproduction steps

---

**Note**: This application uses OpenAI's GPT-4 API. Users are responsible for complying with OpenAI's usage policies and managing API costs.