CREATE OR ALTER VIEW CorporateUsersView
AS
SELECT
    u.userId,
    u.username,
    CAST(u.balance AS decimal(18,2)) AS balance,
    cc.corporateCustomerId,
    cc.cvr,
    CAST(cc.credit AS decimal(18,2)) AS credit,
    -- Maskér CVR delvist (vis fx kun sidste 2)
    REPLICATE('X', CASE WHEN LEN(cc.cvr) >= 2 THEN LEN(cc.cvr)-2 ELSE 0 END) + RIGHT(cc.cvr, CASE WHEN LEN(cc.cvr) >= 2 THEN 2 ELSE LEN(cc.cvr) END) AS cvrMasked
FROM dbo.[user] AS u
JOIN dbo.corporateCustomer AS cc
  ON cc.userId = u.userId;
GO