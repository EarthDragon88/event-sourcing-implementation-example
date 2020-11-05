CREATE TABLE [dbo].[Events]
(
    [EventId]       UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY NONCLUSTERED,
    [AggregateId]   UNIQUEIDENTIFIER    NOT NULL,
    [TimeStamp]     DATETIMEOFFSET      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [Version]       INT                 NOT NULL,
    [Data]          NVARCHAR(MAX)       NOT NULL,
    CONSTRAINT [FK_Events_Aggregates] FOREIGN KEY ([AggregateId]) REFERENCES [Aggregates]([AggregateId]), 
    CONSTRAINT [UQ_Events_AggregateId_Version] UNIQUE NONCLUSTERED ([AggregateId], [Version])
)

GO

CREATE CLUSTERED INDEX [IX_Events_TimeStamp] ON [dbo].[Events] ([TimeStamp])
