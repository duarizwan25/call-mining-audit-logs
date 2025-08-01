USE CallMiningDB;
GO

-- Insert sample demo requests (if tables exist)
IF EXISTS (SELECT * FROM sysobjects WHERE name='DemoRequests' AND xtype='U')
BEGIN
    INSERT INTO DemoRequests (CompanyName, ContactName, Email, Phone, Status, Notes, RequestedDate, CreatedByUserId, CreatedDate)
    VALUES 
    ('Tech Corp', 'John Smith', 'john@techcorp.com', '555-0101', 'Pending', 'Initial demo request', '2025-07-15T10:00:00', 101, '2025-07-15T10:00:00'),
    ('Innovation Ltd', 'Jane Doe', 'jane@innovation.com', '555-0102', 'Scheduled', 'Follow-up demo', '2025-07-20T14:30:00', 102, '2025-07-20T14:30:00'),
    ('StartupX', 'Mike Johnson', 'mike@startupx.com', '555-0103', 'Completed', 'Demo completed successfully', '2025-07-25T09:15:00', 103, '2025-07-25T09:15:00');
END
GO

-- Insert sample audit log entries (if tables exist)
IF EXISTS (SELECT * FROM sysobjects WHERE name='DemoRequestLogs' AND xtype='U')
BEGIN
    INSERT INTO DemoRequestLogs (DemoRequestId, ActionType, ChangedField, OldValue, NewValue, ChangedByUserId, ChangedDate)
    VALUES 
    (1, 'Insert', 'Status', NULL, 'Pending', 101, '2025-07-15T10:00:00'),
    (1, 'Update', 'Status', 'Pending', 'In Progress', 105, '2025-07-16T11:30:00'),
    (2, 'Insert', 'Status', NULL, 'Pending', 102, '2025-07-20T14:30:00'),
    (2, 'Update', 'Status', 'Pending', 'Scheduled', 106, '2025-07-21T09:00:00'),
    (3, 'Insert', 'Status', NULL, 'Pending', 103, '2025-07-25T09:15:00'),
    (3, 'Update', 'Status', 'Pending', 'Completed', 107, '2025-07-26T16:45:00'),
    (1, 'Update', 'Notes', 'Initial demo request', 'Updated demo requirements', 109, '2025-07-28T13:20:00');
END
GO
