import "./App.css";
import { BrowserRouter, Route, Routes } from "react-router";
import GeneratePdfPage from "./pages/GeneratePdf";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<GeneratePdfPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
