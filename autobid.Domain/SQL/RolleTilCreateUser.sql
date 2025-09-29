-- Rolle der kun kan køre de to SP'er
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'app_user_creator')
    CREATE ROLE app_user_creator AUTHORIZATION dbo;

GRANT EXECUTE ON dbo.usp_AppCreatePrivateUser  TO app_user_creator;
GRANT EXECUTE ON dbo.usp_AppCreateCorporateUser TO app_user_creator;

-- (valgfrit men tydeligt) nægt alt andet på dbo-schema
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO app_user_creator;

-- Opret contained DB-bruger til app'ens "signup" connection
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'autobid_creator')
BEGIN
    CREATE USER [autobid_creator] WITH PASSWORD = N'SætEnRigtigStærkKode!2025', DEFAULT_SCHEMA = dbo;
    EXEC(N'ALTER ROLE app_user_creator ADD MEMBER [autobid_creator]');
END
GO