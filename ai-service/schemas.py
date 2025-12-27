from typing import Optional
from pydantic import BaseModel

class AiGenerateRequest(BaseModel):
    type: str       
    questionCount: Optional[int] = 30


class AiGenerateResponse(BaseModel):
    status: str
    message: str
    pdfPath: str
