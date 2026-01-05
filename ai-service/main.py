from fastapi import FastAPI, UploadFile, File, Form, HTTPException
from fastapi.responses import StreamingResponse
from service.ai_engine import process_pdf
from service.pdf_generator import generate_pdf_from_text
import os
import uuid
from io import BytesIO
import time

app = FastAPI(title="PDF AI Generator")

MAX_QUESTIONS = 50
ALLOWED_TYPES = ["quiz", "bubblesheet", "summary"]
MAX_RETRIES = 2  # Retry for temporary AI failures


@app.post("/generate-pdf")
async def generate_ai_pdf(
    file: UploadFile = File(...), type_: str = Form(...), question_count: int = Form(30)
):
    # ✅ Validation
    if question_count < 1 or question_count > MAX_QUESTIONS:
        raise HTTPException(
            status_code=400,
            detail=f"Question count must be between 1 and {MAX_QUESTIONS}",
        )

    if type_.lower() not in ALLOWED_TYPES:
        raise HTTPException(
            status_code=400, detail=f"Type must be one of {ALLOWED_TYPES}"
        )

    # ✅ Temporary directories
    tmp_dir = os.path.join(os.getcwd(), "tmp")
    os.makedirs(tmp_dir, exist_ok=True)

    unique_filename = f"{uuid.uuid4()}_{file.filename}"
    tmp_path = os.path.join(tmp_dir, unique_filename)

    # Save uploaded file
    with open(tmp_path, "wb") as f:
        f.write(await file.read())

    # ✅ Generated PDFs folder
    generated_dir = os.path.join(os.getcwd(), "generated")
    os.makedirs(generated_dir, exist_ok=True)
    pdf_output_path = os.path.join(generated_dir, f"{type_}_{unique_filename}")

    last_exception = None

    # ✅ Retry mechanism
    for attempt in range(MAX_RETRIES + 1):
        try:
            # Process PDF to get AI text
            ai_text = process_pdf(
                tmp_path, type_=type_.lower(), question_count=question_count
            )

            # Generate PDF
            generate_pdf_from_text(ai_text, pdf_output_path)

            # Check PDF exists and is not empty
            if (
                not os.path.exists(pdf_output_path)
                or os.path.getsize(pdf_output_path) == 0
            ):
                raise Exception("Generated PDF is empty")

            # Return as StreamingResponse for preview
            with open(pdf_output_path, "rb") as f:
                pdf_bytes = f.read()

            return StreamingResponse(BytesIO(pdf_bytes), media_type="application/pdf")

        except Exception as e:
            msg = str(e)
            if "Rate limit reached" in msg:
                raise HTTPException(
                    status_code=429,
                    detail="Rate limit reached for OpenAI API. Please try again later.",
                )
    raise HTTPException(status_code=500, detail=f"PDF generation failed: {msg}")
