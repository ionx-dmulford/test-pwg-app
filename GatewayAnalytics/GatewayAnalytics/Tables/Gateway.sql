CREATE TABLE [dbo].[Gateway]
(
	[GatewayGuid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [TeamViewerID] NVARCHAR(100) NOT NULL, 
    [MachineName] NVARCHAR(500) NOT NULL,
    [LastReportDate] DATETIME NOT NULL,
    [RunningSince] DATETIME NULL, 
    [OperatingSystem] NVARCHAR(500) NULL, 
    [CpuInfo] NVARCHAR(500) NULL, 
    [MemorySize] BIGINT NULL, 
    [Latitude] FLOAT NULL, 
    [Longitude] FLOAT NULL, 
    [LocationDate] DATETIME NULL, 
)

GO

CREATE UNIQUE INDEX [IX_Gateway_UniqueTeamViewerID] ON [dbo].[Gateway] ([TeamViewerID])
