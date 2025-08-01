-- Create Audit Trigger for DemoRequests table
USE CallMiningDB;
GO

-- Drop trigger if it exists
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_DemoRequests_Audit')
BEGIN
    DROP TRIGGER TR_DemoRequests_Audit;
END
GO

-- Create the audit trigger
CREATE TRIGGER TR_DemoRequests_Audit
ON DemoRequests
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ActionType NVARCHAR(50);
    DECLARE @UserId INT = 1; -- Default system user, should be replaced with actual user context
    
    -- Handle INSERT operations
    IF EXISTS (SELECT * FROM inserted) AND NOT EXISTS (SELECT * FROM deleted)
    BEGIN
        SET @ActionType = 'Insert';
        
        INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate, AdditionalInfo)
        SELECT 
            i.Id,
            @ActionType,
            'Record Created',
            NULL,
            CONCAT('CompanyName: ', i.CompanyName, '; ContactName: ', i.ContactName, '; Email: ', i.Email, '; Status: ', i.Status),
            ISNULL(i.CreatedByUserId, @UserId),
            GETUTCDATE(),
            'Full record inserted'
        FROM inserted i;
    END
    
    -- Handle DELETE operations
    IF EXISTS (SELECT * FROM deleted) AND NOT EXISTS (SELECT * FROM inserted)
    BEGIN
        SET @ActionType = 'Delete';
        
        INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate, AdditionalInfo)
        SELECT 
            d.Id,
            @ActionType,
            'Record Deleted',
            CONCAT('CompanyName: ', d.CompanyName, '; ContactName: ', d.ContactName, '; Email: ', d.Email, '; Status: ', d.Status),
            NULL,
            ISNULL(d.ModifiedByUserId, d.CreatedByUserId, @UserId),
            GETUTCDATE(),
            'Full record deleted'
        FROM deleted d;
    END
    
    -- Handle UPDATE operations
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
    BEGIN
        SET @ActionType = 'Update';
        
        -- Track CompanyName changes
        INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate)
        SELECT 
            i.Id,
            @ActionType,
            'CompanyName',
            d.CompanyName,
            i.CompanyName,
            ISNULL(i.ModifiedByUserId, @UserId),
            GETUTCDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.CompanyName != d.CompanyName;
        
        -- Track ContactName changes
        INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate)
        SELECT 
            i.Id,
            @ActionType,
            'ContactName',
            d.ContactName,
            i.ContactName,
            ISNULL(i.ModifiedByUserId, @UserId),
            GETUTCDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.ContactName != d.ContactName;
        
        -- Track Email changes
        INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate)
        SELECT 
            i.Id,
            @ActionType,
            'Email',
            d.Email,
            i.Email,
            ISNULL(i.ModifiedByUserId, @UserId),
            GETUTCDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.Email != d.Email;
        
        -- Track Phone changes
        INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate)
        SELECT 
            i.Id,
            @ActionType,
            'Phone',
            d.Phone,
            i.Phone,
            ISNULL(i.ModifiedByUserId, @UserId),
            GETUTCDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE ISNULL(i.Phone, '') != ISNULL(d.Phone, '');
        
        -- Track Status changes
        INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate)
        SELECT 
            i.Id,
            @ActionType,
            'Status',
            d.Status,
            i.Status,
            ISNULL(i.ModifiedByUserId, @UserId),
            GETUTCDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.Status != d.Status;
        
        -- Track Notes changes
        INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate)
        SELECT 
            i.Id,
            @ActionType,
            'Notes',
            d.Notes,
            i.Notes,
            ISNULL(i.ModifiedByUserId, @UserId),
            GETUTCDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE ISNULL(i.Notes, '') != ISNULL(d.Notes, '');
        
        -- Track ScheduledDate changes
        INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate)
        SELECT 
            i.Id,
            @ActionType,
            'ScheduledDate',
            CONVERT(NVARCHAR(50), d.ScheduledDate, 120),
            CONVERT(NVARCHAR(50), i.ScheduledDate, 120),
            ISNULL(i.ModifiedByUserId, @UserId),
            GETUTCDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE ISNULL(i.ScheduledDate, '1900-01-01') != ISNULL(d.ScheduledDate, '1900-01-01');
    END
END
GO

PRINT 'Audit trigger TR_DemoRequests_Audit created successfully';
