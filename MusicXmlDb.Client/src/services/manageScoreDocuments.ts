import axios from "axios";
import type { ScoreDocument } from "@/models/ScoreDocument";
import { getUpdatedToken } from "@/services/auth"; 
import { BASE_ADRESS } from "@/services/environment";
import type { MusicXmlDocument } from "@/models/MusicXmlDocument";


const API_ROUTE = `${BASE_ADRESS}/Manage`

/**
 * Upload a new score version.
 */
export async function createScore(name: string, isPublic: boolean, file: File): Promise<ScoreDocument | null> {
    try {
        const token = await getUpdatedToken();
        const formData = new FormData();
        formData.append("name", name);
        formData.append("isPublic", isPublic.toString())
        formData.append("formFile", file);

        const result = await axios.post<ScoreDocument>(`${API_ROUTE}`, formData, {
            headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "multipart/form-data",
            },
        });

        return result.data;
    } catch (error) {
        console.error("Error uploading score version:", error);
        return null;
    }
}


/**
 * Fetch all score documents with authentication.
 */
export async function getUserScores(): Promise<ScoreDocument[]> {
    try {
        const token = await getUpdatedToken(); 

        const response = await axios.get<ScoreDocument[]>(`${API_ROUTE}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });

        return response.data;
    } catch (error) {
        console.error("Error fetching score documents:", error);
        return [];
    }
}


/**
 * Fetch score details by ID.
 */
export async function getScoreDetails(scoreId: string): Promise<ScoreDocument> {
    try {
        const token = await getUpdatedToken();
        const response = await axios.get<ScoreDocument>(`${API_ROUTE}/${scoreId}`, {
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
export async function getMusicXMLDocument(scoreId: string, historyId: string): Promise<MusicXmlDocument | null> {
    try {
        const token = await getUpdatedToken();
        const response = await axios.get<MusicXmlDocument>(`${BASE_ADRESS}/musicxmldocuments/${scoreId}/${historyId}`, {
            headers: { Authorization: `Bearer ${token}` },
            responseType: "text" // Ensures we receive plain text XML
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching MusicXML document:", error);
        return null;
    }
}


/**
 * Download a music xml document
 */
export async function downloadMusicXmlDocument(scoreId: string, historyId: string): Promise<void> {
    try {        
        const token = await getUpdatedToken();
        const response = await axios.get(`${API_ROUTE}/${scoreId}/${historyId}/download`, {
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


/**
 * Upload a new score version.
 */
export async function uploadScoreVersion(scoreId: string, file: File): Promise<boolean> {
    try {
        const token = await getUpdatedToken();
        const formData = new FormData();
        formData.append("scoreDocumentId", scoreId);
        formData.append("formFile", file);

        await axios.post(`${API_ROUTE}/${scoreId}`, formData, {
            headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "multipart/form-data",
            },
        });

        return true;
    } catch (error) {
        console.error("Error uploading score version:", error);
        return false;
    }
}


/**
 * Update a score document's name & visibility.
 */
export async function updateScore(scoreId: string, updatedScore: Partial<ScoreDocument>): Promise<boolean> {
    try {
        const token = await getUpdatedToken();
        await axios.put(`${API_ROUTE}/${scoreId}`, updatedScore, {
            headers: { Authorization: `Bearer ${token}` }
        });
        return true; // Success
    } catch (error) {
        console.error("Error updating score document:", error);
        return false;
    }
}


/**
 * Delete a new score version.
 */
export async function deleteScoreDocument(scoreId: string): Promise<boolean> {
    try {
        const token = await getUpdatedToken();

        await axios.delete(`${API_ROUTE}/${scoreId}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });

        return true;
    } catch (error) {
        console.error("Error deleting score:", error);
        return false;
    }
}


/**
 * Delete a new score version.
 */
export async function deleteScoreVersion(scoreId: string, versionId: string): Promise<boolean> {
    try {
        const token = await getUpdatedToken();

        await axios.delete(`${BASE_ADRESS}/ScoreDocuments/${scoreId}/${versionId}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });

        return true;
    } catch (error) {
        console.error("Error deleting score version:", error);
        return false;
    }
}