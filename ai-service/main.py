from fastapi import FastAPI, UploadFile, File, Form
from fastapi.responses import FileResponse
from service.ai_engine import process_pdf
from service.pdf_generator import generate_pdf_from_text
import os

app = FastAPI(title="PDF AI Generator")

@app.post("/generate-pdf")
async def generate_ai_pdf(
    file: UploadFile = File(...),
    type_: str = Form(...),
    question_count: int = Form(30)
):
    tmp_dir = os.path.join(os.getcwd(), "tmp")
    os.makedirs(tmp_dir, exist_ok=True)
    tmp_path = os.path.join(tmp_dir, file.filename)

    with open(tmp_path, "wb") as f:
        f.write(await file.read())

    try:
        
        ai_text = process_pdf(tmp_path, type_=type_.lower(), question_count=question_count)

    
        generated_dir = os.path.join(os.getcwd(), "generated")
        os.makedirs(generated_dir, exist_ok=True)
        pdf_output_path = os.path.join(generated_dir, f"{type_}_{file.filename}")
        
        generate_pdf_from_text(ai_text, pdf_output_path)

        return FileResponse(
            pdf_output_path,
            media_type="application/pdf",
            filename=os.path.basename(pdf_output_path)
        )

    except Exception as e:
        return {"status": "error", "message": str(e)}
