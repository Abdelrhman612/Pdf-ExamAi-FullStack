from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.pdfgen import canvas
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
import arabic_reshaper
from bidi.algorithm import get_display


def generate_pdf_from_text(
    text: str, pdf_path: str, font_path="fonts/Amiri-Regular.ttf"
):
    
    pdfmetrics.registerFont(TTFont("Arabic", font_path))


    c = canvas.Canvas(pdf_path, pagesize=A4)
    width, height = A4
    margin = 40
    y = height - margin
    line_height = 20  

  
    lines = text.split("\n")
    for line in lines:
        if not line.strip():
            y -= line_height
            continue

      
        is_correct = "✓" in line
        
        clean_line = line.replace("✓", "").replace("*", "").replace("#", "").strip()
        
        reshaped_text = arabic_reshaper.reshape(clean_line)
        
        bidi_text = get_display(reshaped_text)

        
        if is_correct:
            c.setFillColor(colors.green)
        else:
            c.setFillColor(colors.black)

   
        c.setFont("Arabic", 14)
        c.drawRightString(width - margin, y, bidi_text)

        
        y -= line_height

        
        if y < margin:
            c.showPage()
            y = height - margin

  
    c.save()
    return pdf_path
