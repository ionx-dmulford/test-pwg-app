CREATE TABLE [dbo].[PerformanceMetric]
(
	[GatewayGuid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [TotalEventsProcessed] INT NULL, 
    [EventsProcessedPerSec] INT NULL, 
    [CurrentSignalQuality] INT NULL, 
    [AverageSignalQuality] INT NULL, 
    [Reliability] INT NULL, 
    [Stability] INT NULL, 
    [Latency] INT NULL, 
    [ConnectedNodes] INT NULL, 
    CONSTRAINT [FK_PerformanceMetric_Gateway] FOREIGN KEY ([GatewayGuid]) REFERENCES [Gateway]([GatewayGuid])
)
