DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'score_documents') THEN
        CREATE SCHEMA score_documents;
    END IF;
END $EF$;
CREATE TABLE IF NOT EXISTS score_documents."__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM score_documents."__EFMigrationsHistory" WHERE "migration_id" = '20250315182056_InitalMigration') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'score_documents') THEN
            CREATE SCHEMA score_documents;
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM score_documents."__EFMigrationsHistory" WHERE "migration_id" = '20250315182056_InitalMigration') THEN
    CREATE TABLE score_documents.score_document (
        id uuid NOT NULL,
        user_id text NOT NULL,
        name text NOT NULL,
        views integer NOT NULL,
        created timestamp with time zone NOT NULL,
        modified timestamp with time zone NOT NULL,
        is_public boolean NOT NULL,
        CONSTRAINT pk_score_document PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM score_documents."__EFMigrationsHistory" WHERE "migration_id" = '20250315182056_InitalMigration') THEN
    CREATE TABLE score_documents.score_document_history (
        id uuid NOT NULL,
        score_document_id uuid NOT NULL,
        user_id text NOT NULL,
        created timestamp with time zone NOT NULL,
        CONSTRAINT pk_score_document_history PRIMARY KEY (id),
        CONSTRAINT fk_score_document_history_score_document_score_document_id FOREIGN KEY (score_document_id) REFERENCES score_documents.score_document (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM score_documents."__EFMigrationsHistory" WHERE "migration_id" = '20250315182056_InitalMigration') THEN
    CREATE TABLE score_documents.music_xml_document (
        id uuid NOT NULL,
        score_document_history_id uuid NOT NULL,
        content xml NOT NULL,
        CONSTRAINT pk_music_xml_document PRIMARY KEY (id),
        CONSTRAINT fk_music_xml_document_score_document_history_score_document_hi FOREIGN KEY (score_document_history_id) REFERENCES score_documents.score_document_history (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM score_documents."__EFMigrationsHistory" WHERE "migration_id" = '20250315182056_InitalMigration') THEN
    CREATE UNIQUE INDEX ix_music_xml_document_score_document_history_id ON score_documents.music_xml_document (score_document_history_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM score_documents."__EFMigrationsHistory" WHERE "migration_id" = '20250315182056_InitalMigration') THEN
    CREATE INDEX ix_score_document_history_score_document_id ON score_documents.score_document_history (score_document_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM score_documents."__EFMigrationsHistory" WHERE "migration_id" = '20250315182056_InitalMigration') THEN
    INSERT INTO score_documents."__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20250315182056_InitalMigration', '8.0.13');
    END IF;
END $EF$;
COMMIT;
