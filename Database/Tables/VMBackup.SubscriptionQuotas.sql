CREATE TABLE [VMBackup].[SubscriptionQuotas]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [SubscriptionId] INT NOT NULL, 
    [ServerGroupId] INT NOT NULL, 
    [DiskRetentionDays] INT NOT NULL, 
    [DiskDaysOfTheWeek] NVARCHAR(50) NOT NULL, 
    [DiskTimesOfTheDay] NVARCHAR(128) NOT NULL, 
    [DataSourceQuota] INT NOT NULL, 
    [DiskQuotaGB] INT NOT NULL, 
    [DiskColocation] BIT NOT NULL, 
    [DiskAutoGrow] BIT NOT NULL, 
    [DiskReplicaNow] BIT NOT NULL, 
    [DiskReplicaTime] NVARCHAR(5) NULL, 
    [AutomaticProtection] BIT NOT NULL, 
    [ConsistencyCheck] BIT NOT NULL, 
    [TenantCreatedRestorePoints] BIT NOT NULL, 
    [OnlineFreqency] INT NULL, 
    [OnlineTimesOfTheDay] NVARCHAR(50) NULL, 
    [OnlineDailyRetentionDays] INT NULL, 
    [OnlineWeeklyDaysOfTheWeek] NVARCHAR(50) NULL, 
    [OnlineWeeklyTimesOfTheDay] NVARCHAR(50) NULL, 
    [OnlineWeeklyRetentionWeeks] INT NULL, 
    [OnlineMonthlyRelativeIntervals] NVARCHAR(128) NULL, 
    [OnlineMonthlyRelativeDaysOfTheWeek] NVARCHAR(50) NULL, 
    [OnlineMonthlyDaysOfTheMonth] NVARCHAR(128) NULL, 
    [OnlineMonthlyTimesOfTheDay] NVARCHAR(50) NULL, 
    [OnlineMonthlyRetentionMonths] INT NULL,
    [OnlineYearlyRelativeIntervals] NVARCHAR(128) NULL, 
    [OnlineYearlyRelativeDaysOfTheWeek] NVARCHAR(50) NULL, 
    [OnlineYearlyDaysOfTheMonth] NVARCHAR(128) NULL, 
    [OnlineYearlyTimesOfTheDay] NVARCHAR(50) NULL, 
    [OnlineYearlyRetentionYears] INT NULL,
	
  CONSTRAINT [PK_SubscriptionQuotas] PRIMARY KEY CLUSTERED 
  (
	[Id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
