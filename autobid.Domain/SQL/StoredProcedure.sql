-- Privat
CREATE OR ALTER PROCEDURE dbo.AppCreatePrivateUser
    @Username        NVARCHAR(32),
    @PasswordHash    NVARCHAR(256),
    @CPR             VARCHAR(16),
    @InitialBalance  DECIMAL(18,2) = 0,
    @NewUserId       INT OUTPUT
WITH EXECUTE AS OWNER
AS
BEGIN
  SET NOCOUNT ON;
  SET XACT_ABORT ON;

  IF EXISTS (SELECT 1 FROM dbo.[user] WHERE username=@Username)
      THROW 50004,'Brugernavn findes allerede.',1;
  IF EXISTS (SELECT 1 FROM dbo.privateCustomer WHERE cpr=@CPR)
      THROW 50005,'CPR findes allerede.',1;

  BEGIN TRAN;
    INSERT dbo.[user](username,passwordHash,balance)
      VALUES (@Username,@PasswordHash,@InitialBalance);
    SET @NewUserId = SCOPE_IDENTITY();

    INSERT dbo.privateCustomer(cpr,userId)
      VALUES (@CPR,@NewUserId);
  COMMIT;

  SELECT userId=@NewUserId;
END
GO

-- Corporate
CREATE OR ALTER PROCEDURE dbo.AppCreateCorporateUser
    @Username        NVARCHAR(32),
    @PasswordHash    NVARCHAR(256),
    @CVR             VARCHAR(8),
    @Credit          DECIMAL(18,2) = 0,
    @InitialBalance  DECIMAL(18,2) = 0,
    @NewUserId       INT OUTPUT
WITH EXECUTE AS OWNER
AS
BEGIN
  SET NOCOUNT ON;
  SET XACT_ABORT ON;

  IF EXISTS (SELECT 1 FROM dbo.[user] WHERE username=@Username)
      THROW 50014,'Brugernavn findes allerede.',1;
  IF EXISTS (SELECT 1 FROM dbo.corporateCustomer WHERE cvr=@CVR)
      THROW 50015,'CVR findes allerede.',1;

  BEGIN TRAN;
    INSERT dbo.[user](username,passwordHash,balance)
      VALUES (@Username,@PasswordHash,@InitialBalance);
    SET @NewUserId = SCOPE_IDENTITY();

    INSERT dbo.corporateCustomer(cvr,credit,userId)
      VALUES (@CVR,@Credit,@NewUserId);
  COMMIT;

  SELECT userId=@NewUserId;
END
GO
