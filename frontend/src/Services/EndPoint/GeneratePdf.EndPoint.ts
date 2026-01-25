import axios from "axios";
import { GeneratePdfUrl } from "../api";
import type { GeneratePdfEndPointInterface } from "./EndPoint.Interface";

export const GeneratePdf = async ({
  file,
  type,
  questionCount,
}: GeneratePdfEndPointInterface) => {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("type", type);
  formData.append("questionCount", questionCount.toString());

  const res = await axios.post(
    GeneratePdfUrl,
    formData,
    {
      responseType: "blob"
    }
  );

  return res;
};
