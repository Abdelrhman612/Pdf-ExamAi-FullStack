import os
from dotenv import load_dotenv
from openai import OpenAI
from PyPDF2 import PdfReader

load_dotenv()
OPENAI_API_KEY = os.getenv("OPEN_AI_KEY")
client = OpenAI(api_key=OPENAI_API_KEY)

def summary_prompt(content: str) -> str:
    return f"""
أنت مدرس جامعي محترف.
المطلوب:
- تلخيص المحتوى بطريقة ذكية.
- شرح المفاهيم كأنك تشرح لطالب سيذاكر للامتحان.
- استخدم نقاط واضحة.
- أمثلة إذا وجدت.
- لغة عربية فصيحة.

المحتوى:
{content}
"""


def quiz_prompt(understood_content: str, count: int) -> str:
    return f"""
أنت مدرس خبير.
المطلوب:
- أنشئ {count} سؤال اختيار من متعدد من المحتوى التالي.
- أسئلة للفهم وليس الحفظ.
- كل سؤال 4 اختيارات: أ، ب، ج، د.
- لا تكرر الأفكار.
- لا تضع الإجابات.
- لغة عربية فصيحة.

المحتوى:
{understood_content}
"""


def bubble_sheet_prompt(understood_content: str, count: int) -> str:
    return f"""
انت مدرس خبير
المطلوب:
1-أنشئ {count} سؤال اختيار من متعدد من المحتوى التالي.
2- كل سؤال 4 اختيارات: أ، ب، ج، د.
3- لكل سؤال إجابة واحدة صحيحة فقط.
4- ضع علامة (✓) فقط أمام الاختيار الصحيح.
   - هذا السطر بالذات سيظهر باللون الأخضر في الـ PDF.
   - بقية الأسطر بدون ✓ ستظهر باللون الأسود.
5- لا تكرر نفس الفكرة أو السؤال.
6- اللغة: العربية الفصحى.
7- التنسيق:
   - ترقيم واضح للأسئلة
   - ترتيب منظم
   - جاهز للطباعة مباشرة
المحتوى الذي تعتمد عليه في وضع الأسئلة:
{understood_content}
"""


def extract_pdf_text(file_path: str) -> str:
    reader = PdfReader(file_path)
    full_text = ""
    for page in reader.pages:
        page_text = page.extract_text()
        if page_text:
            full_text += page_text + "\n"
    return full_text


def chunk_text(text: str, max_length: int = 2500) -> list[str]:
    words = text.split()
    chunks = []
    current = []
    for w in words:
        if len(" ".join(current + [w])) < max_length:
            current.append(w)
        else:
            chunks.append(" ".join(current))
            current = [w]
    if current:
        chunks.append(" ".join(current))
    return chunks


def generate_bulk(content: str, total_questions: int, mode: str = "quiz") -> str:
    questions_per_batch = 20
    full_output = ""
    batches = total_questions // questions_per_batch
    remainder = total_questions % questions_per_batch

    def select_prompt(mode, cnt):
        if mode == "quiz":
            return quiz_prompt(content, cnt)
        elif mode == "bubblesheet":
            return bubble_sheet_prompt(content, cnt)
        else:
            raise ValueError("mode must be 'quiz' or 'bubblesheet'")

    for _ in range(batches):
        prompt = select_prompt(mode, questions_per_batch)
        response = client.chat.completions.create(
            model="gpt-4o-mini",
            messages=[{"role": "user", "content": prompt}],
            temperature=0.6,
        )
        full_output += response.choices[0].message.content + "\n\n"

    if remainder > 0:
        prompt = select_prompt(mode, remainder)
        response = client.chat.completions.create(
            model="gpt-4o-mini",
            messages=[{"role": "user", "content": prompt}],
            temperature=0.6,
        )
        full_output += response.choices[0].message.content

    return full_output


def process_pdf(file_path: str, type_: str = "summary", question_count: int = 30):

    text = extract_pdf_text(file_path)
    chunks = chunk_text(text)
    full_content = "\n".join(chunks)

    if type_ == "summary":
        prompt = summary_prompt(full_content)
        response = client.chat.completions.create(
            model="gpt-4o-mini",
            messages=[{"role": "user", "content": prompt}],
            temperature=0.6,
        )
        result_text = response.choices[0].message.content
    elif type_ in ["quiz", "bubblesheet"]:
        result_text = generate_bulk(full_content, question_count, mode=type_)
    else:
        raise ValueError("type_ must be summary / quiz / bubblesheet")

    return result_text
