CREATE TABLE [dbo].[Disk]
(
    [DiskID] BIGINT NOT NULL IDENTITY, 
	[GatewayGuid] UNIQUEIDENTIFIER NOT NULL , 
    [Label] NVARCHAR(100) NOT NULL, 
    [TotalSpace] BIGINT NOT NULL, 
    [UsedSpace] BIGINT NOT NULL, 
    CONSTRAINT [FK_Disk_Gateway] FOREIGN KEY ([GatewayGuid]) REFERENCES [Gateway]([GatewayGuid]), 
    PRIMARY KEY ([DiskID])
)

GO

CREATE INDEX [IX_Disk_UniqueLabelPerGateway] ON [dbo].[Disk] ([GatewayGuid],[Label])
