CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'ScoreDocuments') THEN
            CREATE SCHEMA "ScoreDocuments";
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    CREATE TABLE "ScoreDocuments"."ScoreDocuments" (
        "Id" uuid NOT NULL,
        "UserId" text NOT NULL,
        "Name" text NOT NULL,
        "Views" integer NOT NULL,
        "Created" timestamp with time zone NOT NULL,
        "Modified" timestamp with time zone NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_ScoreDocuments" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    CREATE TABLE "ScoreDocuments"."ScoreDocumentHistories" (
        "Id" uuid NOT NULL,
        "ScoreDocumentId" uuid NOT NULL,
        "UserId" text NOT NULL,
        "MusicXmlId" uuid NOT NULL,
        "Created" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_ScoreDocumentHistories" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ScoreDocumentHistories_ScoreDocuments_ScoreDocumentId" FOREIGN KEY ("ScoreDocumentId") REFERENCES "ScoreDocuments"."ScoreDocuments" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    CREATE TABLE "ScoreDocuments"."MusicXmlDocuments" (
        "Id" uuid NOT NULL,
        "ScoreDocumentHistoryId" uuid NOT NULL,
        "Content" text NOT NULL,
        CONSTRAINT "PK_MusicXmlDocuments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_MusicXmlDocuments_ScoreDocumentHistories_ScoreDocumentHisto~" FOREIGN KEY ("ScoreDocumentHistoryId") REFERENCES "ScoreDocuments"."ScoreDocumentHistories" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    CREATE UNIQUE INDEX "IX_MusicXmlDocuments_ScoreDocumentHistoryId" ON "ScoreDocuments"."MusicXmlDocuments" ("ScoreDocumentHistoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    CREATE INDEX "IX_ScoreDocumentHistories_ScoreDocumentId" ON "ScoreDocuments"."ScoreDocumentHistories" ("ScoreDocumentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250301164034_InitialMigration', '8.0.13');
    END IF;
END $EF$;
COMMIT;

