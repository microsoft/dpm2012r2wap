CREATE TABLE [VMBackup].[Subscriptions]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[State] [int] NOT NULL,
	[LifecycleState] [int] NOT NULL,
	[AccountAdminId] [nvarchar](128) NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_Subscriptions_Created]  DEFAULT (getutcdate()),
	[Modified] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_Subscription_Modified]  DEFAULT (getutcdate()),
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_Subscriptions_SubscriptionId] UNIQUE NONCLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
