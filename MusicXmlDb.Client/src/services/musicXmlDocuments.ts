import axios from "axios"

import { getUpdatedToken } from "./auth";
import { BASE_ADRESS } from "./environment";

/**
 * Fetch MusicXML document for a specific score history entry.
 */
export async function getMusicXMLDocument(scoreId: string, historyId: string): Promise<string> {
    try {
        const token = await getUpdatedToken();
        const response = await axios.get(`${BASE_ADRESS}/musicxmldocuments/${scoreId}/${historyId}`, {
            headers: { Authorization: `Bearer ${token}` },
            responseType: "text" // Ensures we receive plain text XML
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching MusicXML document:", error);
        return "";
    }
}
