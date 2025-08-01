-- Create CallMiningDB Database (if not exists)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CallMiningDB')
BEGIN
    CREATE DATABASE CallMiningDB;
END
GO

USE CallMiningDB;
GO

-- Create DemoRequests table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DemoRequests' AND xtype='U')
BEGIN
    CREATE TABLE DemoRequests (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CompanyName NVARCHAR(100) NOT NULL,
        ContactName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        Phone NVARCHAR(20) NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        Notes NVARCHAR(1000) NULL,
        RequestedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ScheduledDate DATETIME2 NULL,
        CreatedByUserId INT NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedByUserId INT NULL,
        ModifiedDate DATETIME2 NULL
    );
END
GO

-- Create DemoRequestLogs audit table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DemoRequestLogs' AND xtype='U')
BEGIN
    CREATE TABLE DemoRequestLogs (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        DemoRequestId INT NOT NULL,
        ActionType NVARCHAR(50) NOT NULL, -- Insert, Update, Delete
        ChangedField NVARCHAR(100) NOT NULL,
        OldValue NVARCHAR(4000) NULL,
        NewValue NVARCHAR(4000) NULL,
        ChangedByUserId INT NOT NULL,
        ChangedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AdditionalInfo NVARCHAR(255) NULL
    );
END
GO

-- Create indexes for performance
CREATE NONCLUSTERED INDEX IX_DemoRequestLogs_DemoRequestId 
ON DemoRequestLogs (DemoRequestId);

CREATE NONCLUSTERED INDEX IX_DemoRequestLogs_ChangedDate 
ON DemoRequestLogs (ChangedDate DESC);

CREATE NONCLUSTERED INDEX IX_DemoRequestLogs_ActionType 
ON DemoRequestLogs (ActionType);

CREATE NONCLUSTERED INDEX IX_DemoRequestLogs_ChangedByUserId 
ON DemoRequestLogs (ChangedByUserId);

CREATE NONCLUSTERED INDEX IX_DemoRequestLogs_ChangedField 
ON DemoRequestLogs (ChangedField);
GO
