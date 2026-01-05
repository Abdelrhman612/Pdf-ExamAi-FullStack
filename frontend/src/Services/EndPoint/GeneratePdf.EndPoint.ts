import axios from "axios"
import { GeneratePdfUrl } from "../api"
import type { GeneratePdfEndPointInterface } from "./EndPoint.Interface";

export const GeneratePdf = async ({file, type, questionCount} : GeneratePdfEndPointInterface) => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('type', type);
    formData.append('questionCount', questionCount.toString()); 
    
    const res = axios.post(GeneratePdfUrl, formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
            'Origin': window.location.origin 
        },
        responseType: 'blob',
        withCredentials: true 
    });

    return res;
}