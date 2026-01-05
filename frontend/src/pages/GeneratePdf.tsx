import { useState } from "react";
import { GeneratePdf } from "../Services/EndPoint/GeneratePdf.EndPoint";

const MAX_QUESTIONS = 50;

const GeneratePdfPage = () => {
  const [file, setFile] = useState<File | null>(null);
  const [type, setType] = useState("quiz");
  const [questionCount, setQuestionCount] = useState(10);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [pdfUrl, setPdfUrl] = useState<string | null>(null);

  const handleSubmit = async () => {
    if (!file) {
      setError("Please upload a file");
      return;
    }

    if (questionCount < 1 || questionCount > MAX_QUESTIONS) {
      setError(`Number of questions must be between 1 and ${MAX_QUESTIONS}`);
      return;
    }

    setError("");
    setLoading(true);

    try {
      const res = await GeneratePdf({
        file,
        type,
        questionCount: questionCount.toString(),
      });

      // ✅ تحقق إن الـ response blob
      const blob = new Blob([res.data], { type: "application/pdf" });

      if (blob.size === 0) {
        throw new Error("PDF is empty or failed to generate");
      }

      const url = URL.createObjectURL(blob);

      // Clean up old URL
      if (pdfUrl) {
        URL.revokeObjectURL(pdfUrl);
      }

      setPdfUrl(url);
    } catch (err: any) {
      console.error(err);
      if (err.response?.status === 429) {
        setError(
          "OpenAI rate limit reached. Please wait a while and try again."
        );
      } else {
        setError(
          err?.message ||
            "Failed to generate PDF. Try reducing the number of questions."
        );
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 p-6 flex flex-col items-center">
      {/* Form */}
      <div className="bg-white shadow-xl rounded-2xl p-6 space-y-5 w-full max-w-md">
        <h1 className="text-2xl font-bold text-center text-gray-800">
          AI PDF Generator
        </h1>

        {/* File Upload */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Upload PDF
          </label>
          <input
            type="file"
            accept=".pdf"
            onChange={(e) => setFile(e.target.files?.[0] || null)}
            className="block w-full text-sm text-gray-600
                      file:mr-4 file:py-2 file:px-4
                      file:rounded-lg file:border-0
                      file:text-sm file:font-semibold
                      file:bg-blue-50 file:text-blue-700
                      hover:file:bg-blue-100"
          />
        </div>

        {/* Type */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Output Type
          </label>
          <select
            value={type}
            onChange={(e) => setType(e.target.value)}
            className="w-full rounded-lg border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="quiz">Quiz</option>
            <option value="bubblesheet">Bubble Sheet</option>
            <option value="summary">Summary</option>
          </select>
        </div>

        {/* Question Count */}
        {type !== "summary" && (
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Number of Questions
            </label>
            <input
              type="number"
              min={1}
              max={MAX_QUESTIONS}
              value={questionCount}
              onChange={(e) => setQuestionCount(Number(e.target.value))}
              className="w-full rounded-lg border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
        )}

        {/* Error */}
        {error && <p className="text-red-600 text-sm text-center">{error}</p>}

        {/* Submit Button */}
        <button
          onClick={handleSubmit}
          disabled={loading}
          className={`w-full py-2 rounded-lg text-white font-semibold transition
                      ${
                        loading
                          ? "bg-gray-400 cursor-not-allowed"
                          : "bg-blue-600 hover:bg-blue-700"
                      }`}
        >
          {loading ? "Generating..." : "Generate PDF"}
        </button>
      </div>

      {/* PDF Preview */}
      {pdfUrl && (
        <div className="bg-white shadow-xl rounded-2xl p-4 mt-6 w-full max-w-5xl space-y-3">
          <div className="flex justify-between items-center mb-2">
            <h2 className="text-lg font-semibold text-gray-700">PDF Preview</h2>
            <a
              href={pdfUrl}
              download={`generated_${type}.pdf`}
              className="px-4 py-1.5 text-sm bg-green-600 text-white rounded-lg hover:bg-green-700"
            >
              Download
            </a>
          </div>
          <iframe
            src={pdfUrl}
            title="PDF Preview"
            className="w-full h-[700px] border rounded-lg"
          />
        </div>
      )}
    </div>
  );
};

export default GeneratePdfPage;
