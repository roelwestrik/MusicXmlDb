import axios from "axios";

import { getUpdatedToken } from "@/services/auth";
import { BASE_ADRESS } from "@/services/environment";

export async function downloadScoreVersion(scoreId: string, historyId: string): Promise<void> {
    try {        
        const token = await getUpdatedToken();
        const response = await axios.get(`${BASE_ADRESS}/musicxmldocuments/${scoreId}/${historyId}`, {
            headers: { Authorization: `Bearer ${token}` },
            responseType: "blob", // Ensure file is received correctly
        });

        const fileBlob = new Blob([response.data], { type: "application/xml" });

        // Create an object URL for the Blob
        const url = window.URL.createObjectURL(fileBlob);

        // Open a Save As dialog using the File System API (modern browsers)
        const a = document.createElement("a");
        a.href = url;
        a.download = `Score_${historyId}.xml`; // User will see this default name
        document.body.appendChild(a);

        // Simulate a click to open the save file dialog
        a.click();
        document.body.removeChild(a);

        // Clean up object URL
        window.URL.revokeObjectURL(url);
    } catch (error) {
        console.error("Error fetching score version:", error);
        throw error;
    }
}