CREATE TABLE [VMBackup].[ServerGroups]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [GroupId] UNIQUEIDENTIFIER NOT NULL, 
    [GroupName] nvarchar(128) NOT NULL, 
    [AzureBackupEnabled] BIT NOT NULL
)

GO

CREATE UNIQUE INDEX [UX_GroupId] ON [VMBackup].[ServerGroups] ([GroupId])

GO

CREATE UNIQUE INDEX [UX_GroupName] ON [VMBackup].[ServerGroups] ([GroupName])
