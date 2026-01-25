# PDF AI Generator - Full Stack Application

A full-stack application that uses AI to generate educational materials (quizzes, bubble sheets, summaries) from PDF documents. The system consists of multiple microservices working together through Docker containers.

## 📋 Overview

This application allows users to upload PDF documents and automatically generate:
- **Quiz PDFs**: Multiple-choice questions for exam preparation
- **Bubble Sheet PDFs**: Answer sheets with correct answers marked
- **Summary PDFs**: Concise summaries of the PDF content

The AI analyzes the PDF content using OpenAI's GPT models to create educational materials in Arabic.

## 🏗️ Architecture

The system follows a microservices architecture with the following components:

```
┌─────────────────┐
│   Frontend      │ (React + TypeScript + Vite)
│   (Port: 3000)  │
└────────┬────────┘
         │
┌────────▼────────┐
│    Backend      │ (ASP.NET Core Web API)
│   (Port: 5000)  │
└────────┬────────┘
         │
┌────────▼────────┐
│   AI Service    │ (FastAPI Python Service)
│   (Port: 8000)  │
└────────┬────────┘
         │
┌────────▼────────┐
│  SQL Server     │ (Database)
│  (Port: 1433)   │
└─────────────────┘
```

## 🚀 Features

### Frontend (React + TypeScript)
- Modern, responsive UI built with Tailwind CSS
- PDF upload interface with drag-and-drop support
- Real-time PDF preview in browser
- Download generated PDFs
- Configurable options (type, question count)

### Backend (ASP.NET Core)
- RESTful API endpoints
- File handling and storage
- Database integration with Entity Framework
- CORS configuration for frontend communication
- Error handling and logging

### AI Service (FastAPI + Python)
- PDF text extraction using PyPDF2
- OpenAI GPT-4 integration for content generation
- Arabic text processing with arabic_reshaper and python-bidi
- PDF generation using ReportLab
- Rate limiting and retry logic

### Database (SQL Server)
- Persistent storage of generated PDF metadata
- Entity Framework migrations
- Dockerized SQL Server instance

## 🛠️ Technology Stack

### Backend Services
- **.NET 8.0** - ASP.NET Core Web API
- **Entity Framework Core 8.0** - ORM for database operations
- **SQL Server 2022** - Database
- **xUnit** - Testing framework
- **FakeItEasy** - Mocking library for tests

### AI Service
- **Python 3.11** - Core runtime
- **FastAPI** - Web framework
- **OpenAI GPT-4** - AI model for content generation
- **ReportLab** - PDF generation
- **PyPDF2** - PDF text extraction
- **arabic_reshaper + python-bidi** - Arabic text processing

### Frontend
- **React 19** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **Tailwind CSS 4** - Styling
- **Axios** - HTTP client

### Infrastructure
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **Nginx** - Web server for frontend

## 📁 Project Structure

```
Pdf-Exam-ai/
├── docker-compose.yml          # Multi-container orchestration
├── ai-service/                 # Python AI service
│   ├── main.py                 # FastAPI application
│   ├── service/
│   │   ├── ai_engine.py       # OpenAI integration
│   │   └── pdf_generator.py   # PDF generation logic
│   ├── schemas.py             # Pydantic models
│   └── Dockerfile             # Python container
├── backend/                    # .NET Core API
│   ├── Controllers/           # API endpoints
│   ├── Services/              # Business logic
│   ├── data/                  # Database context
│   ├── Dto/                   # Data transfer objects
│   ├── InterFaces/            # Service interfaces
│   ├── Repository/            # Data access layer
│   └── Dockerfile             # .NET container
├── frontend/                   # React application
│   ├── src/
│   │   ├── pages/             # React components
│   │   ├── Services/          # API client
│   │   └── App.tsx           # Main application
│   └── Dockerfile             # React container
└── backend.tests/             # Unit tests
```

## 🚀 Quick Start

### Prerequisites
- Docker and Docker Compose
- OpenAI API key
- SQL Server (optional - included in Docker)

### Environment Setup

1. **Create a `.env` file in the root directory**:
```env
# OpenAI API Key
OPEN_AI_KEY=your_openai_api_key_here

# SQL Server
SA_PASSWORD=YourStrong!Passw0rd

# Frontend
PORT=3000
VITE_API_SERVICE_URL=http://localhost:5000
```

2. **Build and run with Docker Compose**:
```bash
docker-compose up --build
```

This will start all services:
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- AI Service: http://localhost:8000
- SQL Server: localhost:1433

### Manual Setup (Development)

#### Backend (.NET)
```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

#### AI Service (Python)
```bash
cd ai-service
pip install -r requirements.txt
uvicorn main:app --reload --port 8000
```

#### Frontend (React)
```bash
cd frontend
npm install
npm run dev
```

## 📖 API Documentation

### Generate PDF Endpoint
**POST** `/api/pdf/generate`

**Form Data:**
- `file`: PDF file to process
- `type`: Output type (`quiz`, `bubblesheet`, `summary`)
- `questionCount`: Number of questions (1-50, default: 30)

**Response:**
- Returns generated PDF file
- HTTP 200 on success
- HTTP 400/500 on error

### AI Service Endpoint
**POST** `/generate-pdf`

**Form Data:**
- `file`: PDF file
- `type_`: Output type
- `question_count`: Number of questions

**Response:**
- PDF file stream

## 🧪 Testing

### Backend Tests
```bash
cd backend.tests
dotnet test
```

Tests include:
- `PdfControllerTests` - API endpoint tests
- `FastApiServiceTests` - Service layer tests

## 🐳 Docker Services

### SQL Server Container
- Image: `mcr.microsoft.com/mssql/server:2022-latest`
- Port: 1433
- Volume: `sql_data` for persistent storage
- Environment: SA_PASSWORD from .env

### AI Service Container
- Base: Python 3.11-slim
- Port: 8000
- Dependencies: OpenAI, FastAPI, PDF libraries
- Fonts: Amiri Arabic font for PDF generation

### Backend Container
- Base: .NET 8.0 SDK and runtime
- Port: 5000 (mapped to 8080 internally)
- Dependencies: EF Core, SQL Server client

### Frontend Container
- Build: Node 20 + Vite
- Runtime: Nginx
- Port: 3000

## 🔧 Configuration

### AI Service Configuration
- **Max Questions**: 50 (configurable)
- **Retry Attempts**: 2 for temporary failures
- **Model**: GPT-4o-mini
- **Temperature**: 0.6 for consistent output

### PDF Generation
- **Page Size**: A4
- **Font**: Amiri Regular (Arabic support)
- **Colors**: Black for questions, Green for correct answers
- **Language**: Arabic with proper shaping and bidirectional support

## 🎨 Frontend Features

### User Interface
- Clean, modern design with Tailwind CSS
- Responsive layout for all devices
- Real-time feedback and error messages
- PDF preview embedded in the page
- Download button for generated files

### Form Validation
- File type validation (PDF only)
- Question count range validation (1-50)
- Error handling for API failures
- Loading states during processing

## 📊 Database Schema

### PdfFiles Table
```sql
CREATE TABLE PdfFiles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OriginalFileName NVARCHAR(MAX) NOT NULL,
    StoredFilePath NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
)
```

## 🔒 Security Considerations

- **Environment Variables**: Sensitive data stored in .env file
- **CORS**: Configured for frontend origins only
- **File Validation**: Only PDF files accepted
- **Rate Limiting**: AI service includes retry logic for API limits
- **Error Handling**: Comprehensive error messages without exposing internals

## 🚨 Error Handling

### Common Issues

1. **OpenAI Rate Limits**: System retries twice before failing
2. **Empty PDFs**: Validation to ensure generated PDFs are valid
3. **Arabic Text Issues**: Proper shaping and bidirectional support
4. **File Upload Errors**: Clear feedback for invalid files
5. **Database Connectivity**: Graceful degradation

### Troubleshooting

```bash
# Check all containers are running
docker-compose ps

# View logs for specific service
docker-compose logs ai-service

# Rebuild and restart
docker-compose down && docker-compose up --build

# Check database connectivity
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD
```

## 📈 Performance Considerations

- **Chunking**: Large PDFs are split into chunks for AI processing
- **Caching**: Generated PDFs are stored for potential reuse
- **Async Processing**: Non-blocking API calls for better responsiveness
- **Container Optimization**: Multi-stage Docker builds for smaller images

## 🔮 Future Enhancements

Potential improvements:
- User authentication and PDF history
- Batch processing of multiple PDFs
- Support for more output formats (Word, HTML)
- Custom templates for PDF generation
- Progress tracking for large files
- Multiple language support beyond Arabic
- Cloud storage integration (AWS S3, Azure Blob)

## 📄 License

This project is for educational purposes. Please ensure you comply with OpenAI's terms of service when using their API.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📧 Support

For issues and questions:
1. Check the troubleshooting section
2. Review the error logs
3. Ensure all environment variables are set
4. Verify Docker containers are running properly

---

**Note**: This application requires an OpenAI API key with GPT-4 access. Ensure you have appropriate credits and comply with usage policies.