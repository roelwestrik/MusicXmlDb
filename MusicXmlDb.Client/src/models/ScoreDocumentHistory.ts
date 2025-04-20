import type { MusicXmlDocument } from "./MusicXmlDocument";

export interface ScoreDocumentHistory {
    id: string;
    created: Date;
    MusicXmlDocument: MusicXmlDocument | null
}

