import { useState } from "react";
import { GeneratePdf } from "../Services/EndPoint/GeneratePdf.EndPoint";

const GeneratePdfPage = () => {
  const [file, setFile] = useState<File | null>(null);
  const [type, setType] = useState("quiz");
  const [questionCount, setQuestionCount] = useState(30);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [pdfUrl, setPdfUrl] = useState<string | null>(null);

  const handleSubmit = async () => {
    if (!file) {
      setError("Please upload a file");
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

      const blob = new Blob([res.data], { type: "application/pdf" });
      const url = URL.createObjectURL(blob);

      // امسح أي PDF قديم
      if (pdfUrl) {
        URL.revokeObjectURL(pdfUrl);
      }

      setPdfUrl(url);
    } catch (err) {
      setError("Something went wrong while generating the PDF");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <div className="max-w-5xl mx-auto space-y-6">
        {/* Form */}
        <div className="bg-white shadow-xl rounded-2xl p-6 space-y-5 max-w-md mx-auto">
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
                value={questionCount}
                onChange={(e) => setQuestionCount(Number(e.target.value))}
                className="w-full rounded-lg border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
          )}

          {/* Error */}
          {error && <p className="text-red-600 text-sm text-center">{error}</p>}

          {/* Button */}
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
          <div className="bg-white shadow-xl rounded-2xl p-4 space-y-3">
            <div className="flex justify-between items-center">
              <h2 className="text-lg font-semibold text-gray-700">
                PDF Preview
              </h2>

              <a
                href={pdfUrl}
                download="generated.pdf"
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
    </div>
  );
};

export default GeneratePdfPage;
