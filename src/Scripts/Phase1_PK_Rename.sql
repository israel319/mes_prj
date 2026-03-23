BEGIN TRANSACTION;
ALTER TABLE [dbo].[T_RaisonsSortie] ADD [TypeMaterielDefaut] int NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260310085837_AddTypeMaterielDefautToRaisonSortie', N'10.0.2');

COMMIT;
GO

BEGIN TRANSACTION;
EXEC sp_rename N'[dbo].[T_SoldesMateriels].[Id]', N'IdSoldeMateriel', 'COLUMN';

EXEC sp_rename N'[dbo].[T_Sites].[Id]', N'IdSite', 'COLUMN';

EXEC sp_rename N'[dbo].[T_RaisonsSortie].[Id]', N'IdRaisonSortie', 'COLUMN';

EXEC sp_rename N'[dbo].[T_NotificationsRejet].[Id]', N'IdNotificationRejet', 'COLUMN';

EXEC sp_rename N'[dbo].[T_Employees].[Id]', N'IdEmployee', 'COLUMN';

EXEC sp_rename N'[dbo].[T_Contrats].[Id]', N'IdContrat', 'COLUMN';

EXEC sp_rename N'[dbo].[T_Compagnies].[Id]', N'IdCompagnie', 'COLUMN';

EXEC sp_rename N'[dbo].[T_CategoriesSortie].[Id]', N'IdCategorieSortie', 'COLUMN';

DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[T_NotificationsRejet]') AND [c].[name] = N'MotifRejet');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [dbo].[T_NotificationsRejet] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [dbo].[T_NotificationsRejet] ALTER COLUMN [MotifRejet] nvarchar(1000) NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260310135804_DB_Audit_Phase1', N'10.0.2');

COMMIT;
GO

