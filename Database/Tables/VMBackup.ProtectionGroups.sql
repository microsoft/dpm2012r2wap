CREATE TABLE [VMBackup].[ProtectionGroups]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[ProtectionGroupId] uniqueidentifier NOT NULL,
	[ProtectionGroupName] NVARCHAR(128) NOT NULL,
	[SubscriptionId] INT NOT NULL, 
    [BackupServerId] INT NOT NULL, 
    [ProtectionGroupStateId] INT NOT NULL, 
    CONSTRAINT [PK_ProtectionGroups] PRIMARY KEY CLUSTERED 
 (
	[Id] ASC
 )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
