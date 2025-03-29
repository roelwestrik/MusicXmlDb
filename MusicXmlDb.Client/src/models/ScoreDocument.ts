import type { ScoreDocumentHistory } from "./ScoreDocumentHistory";

export interface ScoreDocument {
    id: string;
    userId: string;
    history: ScoreDocumentHistory[];
    name: string;
    views: number;
    created: Date;
    modified: Date;
    isPublic: boolean;
}


