import axios from "axios";
import type { ScoreDocument } from "@/models/ScoreDocument";
import { getUpdatedToken } from "@/services/auth"; 
import { BASE_ADRESS } from "@/services/environment";


const API_ROUTE = `${BASE_ADRESS}/View`

/**
 * Fetch score details by ID.
 */
export async function getScoreDetails(scoreId: string): Promise<ScoreDocument> {
    try {
        const token = await getUpdatedToken();
        const response = await axios.get<ScoreDocument>(`${API_ROUTE}?id=${scoreId}`, {
            headers: { Authorization: `Bearer ${token}` }
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching score details:", error);
        throw error;
    }
}


/**
 * Fetch MusicXML document for a specific score history entry.
 */
export async function getMusicXMLDocument(scoreId: string, historyId: string): Promise<string | null> {
    try {
        const token = await getUpdatedToken();
        const response = await axios.get(`${API_ROUTE}/${scoreId}/${historyId}`, {
            headers: { Authorization: `Bearer ${token}` },
            responseType: "text" // Ensures we receive plain text XML
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching MusicXML document:", error);
        return null;
    }
}
