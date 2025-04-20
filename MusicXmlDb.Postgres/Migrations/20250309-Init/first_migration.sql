BEGIN;

-- Create the schema if it doesn't exist.
CREATE SCHEMA IF NOT EXISTS score_documents;

-- Create table for score_document.
CREATE TABLE IF NOT EXISTS score_documents.score_document (
    id uuid NOT NULL,
    user_id text NOT NULL,
    name text NOT NULL,
    views integer NOT NULL,
    created timestamp with time zone NOT NULL,
    modified timestamp with time zone NOT NULL,
    is_public boolean NOT NULL,
    CONSTRAINT pk_score_document PRIMARY KEY (id)
);

-- Create indexes for score_document.
CREATE INDEX IF NOT EXISTS ix_score_document_is_public
    ON score_documents.score_document (is_public);

CREATE INDEX IF NOT EXISTS ix_score_document_user_id
    ON score_documents.score_document (user_id);

-- Create table for score_document_history.
CREATE TABLE IF NOT EXISTS score_documents.score_document_history (
    id uuid NOT NULL,
    score_document_id uuid NOT NULL,
    user_id text NOT NULL,
    created timestamp with time zone NOT NULL,
    CONSTRAINT pk_score_document_history PRIMARY KEY (id),
    CONSTRAINT fk_score_document_history_score_document_score_document_id 
        FOREIGN KEY (score_document_id) REFERENCES score_documents.score_document (id) ON DELETE CASCADE
);

-- Create table for music_xml_document.
CREATE TABLE IF NOT EXISTS score_documents.music_xml_document (
    id uuid NOT NULL,
    score_document_history_id uuid NOT NULL,
    content xml NOT NULL,
    CONSTRAINT pk_music_xml_document PRIMARY KEY (id),
    CONSTRAINT fk_music_xml_document_score_document_history_score_document_hi 
        FOREIGN KEY (score_document_history_id) REFERENCES score_documents.score_document_history (id) ON DELETE CASCADE
);

-- Create a unique index on music_xml_document.score_document_history_id.
CREATE UNIQUE INDEX IF NOT EXISTS ix_music_xml_document_score_document_history_id
ON score_documents.music_xml_document (score_document_history_id);

-- Create an index on score_document_history.score_document_id.
CREATE INDEX IF NOT EXISTS ix_score_document_history_score_document_id
ON score_documents.score_document_history (score_document_id);

COMMIT;
