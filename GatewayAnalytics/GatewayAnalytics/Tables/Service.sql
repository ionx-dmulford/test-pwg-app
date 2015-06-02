CREATE TABLE [dbo].[Service]
( 
    [ServiceID] BIGINT NOT NULL IDENTITY, 
	[GatewayGuid] UNIQUEIDENTIFIER NOT NULL , 
    [ServiceName] NVARCHAR(500) NOT NULL, 
    [Status] NVARCHAR(100) NOT NULL,
    CONSTRAINT [FK_Service_Gateway] FOREIGN KEY ([GatewayGuid]) REFERENCES [Gateway]([GatewayGuid]), 
    PRIMARY KEY ([ServiceID])
)

GO

CREATE UNIQUE INDEX [IX_Service_UniqueServicePerGateway] ON [dbo].[Service] ([GatewayGuid],[ServiceName])
