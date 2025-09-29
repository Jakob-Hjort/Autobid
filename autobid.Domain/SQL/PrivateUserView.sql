CREATE OR ALTER VIEW vwPrivateUsers
AS
SELECT
  u.userId, 
  u.username, 
  CAST(u.balance AS DECIMAL(18,2)) AS balance,
  pc.privateCustomerId, 
  pc.cpr,
  STUFF(pc.cpr, 3, CASE WHEN LEN(pc.cpr)>=4 THEN LEN(pc.cpr)-4 ELSE 0 END,
                 REPLICATE('X', CASE WHEN LEN(pc.cpr)>=4 THEN LEN(pc.cpr)-4 ELSE 0 END)) AS cprMasked
FROM dbo.[user] u
JOIN dbo.privateCustomer pc ON pc.userId = u.userId;
GO