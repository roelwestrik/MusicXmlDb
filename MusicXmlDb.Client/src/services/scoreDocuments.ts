import axios from "axios";
import type { ScoreDocument } from "@/models/ScoreDocument";
import { getUpdatedToken } from "@/services/auth"; 
import { BASE_ADRESS } from "@/services/environment";



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

        const result = await axios.post<ScoreDocument>(`${BASE_ADRESS}/ScoreDocuments/`, formData, {
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
 * Upload a new score version.
 */
export async function uploadScoreVersion(scoreId: string, file: File): Promise<boolean> {
    try {
        const token = await getUpdatedToken();
        const formData = new FormData();
        formData.append("scoreDocumentId", scoreId);
        formData.append("formFile", file);

        await axios.post(`${BASE_ADRESS}/ScoreDocuments/${scoreId}`, formData, {
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
 * Fetch all score documents with authentication.
 */
export async function getUserScores(): Promise<ScoreDocument[]> {
    try {
        const token = await getUpdatedToken(); // Retrieve latest auth token

        const response = await axios.get<ScoreDocument[]>(`${BASE_ADRESS}/scoredocuments`, {
            headers: {
                Authorization: `Bearer ${token}` // Attach token to request
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
        const response = await axios.get<ScoreDocument>(`${BASE_ADRESS}/scoredocuments/${scoreId}`, {
            headers: { Authorization: `Bearer ${token}` }
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching score details:", error);
        throw error;
    }
}


/**
 * Update a score document's name & visibility.
 */
export async function updateScore(scoreId: string, updatedScore: Partial<ScoreDocument>): Promise<boolean> {
    try {
        const token = await getUpdatedToken();
        await axios.put(`${BASE_ADRESS}/ScoreDocuments/${scoreId}`, updatedScore, {
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

        await axios.delete(`${BASE_ADRESS}/ScoreDocuments/${scoreId}`, {
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