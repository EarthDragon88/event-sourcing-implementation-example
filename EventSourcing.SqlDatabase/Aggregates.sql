CREATE TABLE [dbo].[Aggregates]
(
    [AggregateId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Type] NVARCHAR(255) NOT NULL,
    [Version] INT NOT NULL
)
