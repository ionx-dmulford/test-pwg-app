CREATE TABLE [dbo].[LatestEvent]
(
    [EventID] BIGINT NOT NULL IDENTITY, 
	[GatewayGuid] UNIQUEIDENTIFIER NOT NULL , 
    [EventDateUtc] DATETIME NOT NULL, 
    [AssetName] NVARCHAR(100) NOT NULL, 
    [EventName] NVARCHAR(100) NOT NULL, 
    [EventInfo] NVARCHAR(MAX) NOT NULL, 
    PRIMARY KEY ([EventID]), 
    CONSTRAINT [FK_LatestEvent_Gateway] FOREIGN KEY ([GatewayGuid]) REFERENCES [Gateway]([GatewayGuid])
)
