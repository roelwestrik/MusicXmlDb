START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    DROP TABLE "ScoreDocuments"."MusicXmlDocuments";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    DROP TABLE "ScoreDocuments"."ScoreDocumentHistories";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    DROP TABLE "ScoreDocuments"."ScoreDocuments";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250301164034_InitialMigration') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20250301164034_InitialMigration';
    END IF;
END $EF$;
COMMIT;

