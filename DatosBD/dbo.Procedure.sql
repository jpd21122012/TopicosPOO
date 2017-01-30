CREATE PROCEDURE [dbo].[Procedure]
AS
BEGIN
SELECT COUNT(dbo.[user].matricula) as Registros
FROM dbo.[user]
END