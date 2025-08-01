# Audit Logging System Implementation

This document explains the implemented audit logging system for the DemoRequest entity.

## Overview

The audit logging system automatically captures all changes (INSERT, UPDATE, DELETE) made to the `DemoRequests` table and provides APIs to query these audit logs with advanced filtering, sorting, and pagination.

## Components

### 1. Database Schema

#### DemoRequestLogs Table
- **Id**: BIGINT IDENTITY - Primary key
- **DemoRequestId**: INT - Foreign key to DemoRequests table
- **ActionType**: NVARCHAR(50) - 'Insert', 'Update', or 'Delete'
- **ChangedField**: NVARCHAR(100) - Name of the field that changed
- **OldValue**: NVARCHAR(4000) - Previous value (NULL for inserts)
- **NewValue**: NVARCHAR(4000) - New value (NULL for deletes)
- **ChangedByUserId**: INT - ID of user who made the change
- **ChangedDate**: DATETIME2 - UTC timestamp of the change
- **AdditionalInfo**: NVARCHAR(255) - Optional additional information

#### Indexes
- `IX_DemoRequestLogs_DemoRequestId` - For filtering by demo request
- `IX_DemoRequestLogs_ChangedDate` - For sorting by date (DESC)
- `IX_DemoRequestLogs_ActionType` - For filtering by action type
- `IX_DemoRequestLogs_ChangedByUserId` - For filtering by user
- `IX_DemoRequestLogs_ChangedField` - For filtering by field

### 2. Database Trigger

**TR_DemoRequests_Audit** - Automatically captures changes to DemoRequests table:

- **INSERT Operations**: Logs a single record with 'Insert' action
- **UPDATE Operations**: Logs individual field changes as separate records
- **DELETE Operations**: Logs a single record with 'Delete' action

Tracked fields for UPDATE operations:
- CompanyName
- ContactName
- Email
- Phone
- Status
- Notes
- ScheduledDate

### 3. API Endpoints

#### GET /api/Audit/audit-logs
Fetch audit logs for all demo requests with filters and pagination.

**Query Parameters:**
- `actionType` (string, optional): Filter by 'Insert', 'Update', or 'Delete'
- `changedByUserId` (int, optional): Filter by user ID
- `changedDateFrom` (DateTime, optional): Filter from date (UTC)
- `changedDateTo` (DateTime, optional): Filter to date (UTC)
- `changedField` (string, optional): Filter by specific field name
- `demoRequestId` (int, optional): Filter by specific demo request ID
- `sortBy` (string, default: "ChangedDate"): Sort column
- `sortDirection` (string, default: "DESC"): Sort direction (ASC/DESC)
- `pageNumber` (int, default: 1): Page number
- `pageSize` (int, default: 50): Records per page

#### GET /api/Audit/{demoRequestId}/audit-logs
Fetch audit logs for a specific demo request by ID.

**Path Parameters:**
- `demoRequestId` (int): The demo request ID

**Query Parameters:**
- Same as above except `demoRequestId` (automatically set from path)

### 4. Response Format

```json
{
  "data": [
    {
      "id": 1,
      "demoRequestId": 123,
      "actionType": "Update",
      "changedField": "Status",
      "oldValue": "Pending",
      "newValue": "Approved",
      "changedByUserId": 1,
      "changedDate": "2025-01-31T13:55:00Z",
      "additionalInfo": null
    }
  ],
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 50,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

## Setup Instructions

### 1. Database Setup

1. Ensure the `DemoRequestLogs` table exists (created by `01_CreateTables.sql`)
2. Run the audit trigger script:
   ```powershell
   .\DatabaseScripts\RunAuditTriggerScript.ps1
   ```
   Or manually execute:
   ```sql
   sqlcmd -S localhost -d CallMiningDB -E -i "DatabaseScripts\03_CreateAuditTrigger.sql"
   ```

### 2. Application Setup

The repository is automatically configured in `Program.cs` with Dapper implementation.

## Features Implemented

### ✅ Security & Performance
- **Parameterized Queries**: All SQL queries use Dapper with parameters to prevent SQL injection
- **Column Validation**: Sort columns are validated against a whitelist
- **Efficient Indexes**: Proper indexing for query performance
- **Connection Management**: Proper disposal of database connections

### ✅ UTC Timestamps
- Database uses `GETUTCDATE()` for all timestamps
- Repository ensures all returned dates are treated as UTC
- Input date filters are converted to UTC before querying

### ✅ Advanced Filtering
- Filter by action type (Insert/Update/Delete)
- Filter by user who made changes
- Filter by date range
- Filter by specific field names
- Filter by demo request ID

### ✅ Flexible Sorting
- Sort by any column with validation
- Ascending or descending order
- Default sort by ChangedDate DESC

### ✅ Pagination
- Configurable page size
- Page navigation information
- Total count included

### ✅ Audit Trigger
- Automatic logging of all changes
- Field-level change tracking for updates
- Captures user context when available
- UTC timestamps for all operations

## Usage Examples

### Get all audit logs with default pagination
```
GET /api/Audit/audit-logs
```

### Get audit logs for a specific demo request
```
GET /api/Audit/123/audit-logs
```

### Filter by action type and user
```
GET /api/Audit/audit-logs?actionType=Update&changedByUserId=1
```

### Filter by date range
```
GET /api/Audit/audit-logs?changedDateFrom=2025-01-01T00:00:00Z&changedDateTo=2025-01-31T23:59:59Z
```

### Custom sorting and pagination
```
GET /api/Audit/audit-logs?sortBy=ActionType&sortDirection=ASC&pageNumber=2&pageSize=25
```

## Technical Implementation Details

### Repository Pattern
- Uses Dapper for high-performance data access
- Implements proper connection management
- Dynamic SQL generation with parameterization
- UTC timestamp handling throughout

### Error Handling
- Proper exception handling in controllers
- Validation of sort parameters
- Safe default values for invalid inputs

### Performance Considerations
- Efficient pagination using OFFSET/FETCH
- Strategic indexing on commonly filtered columns
- Minimal data transfer with projection
- Connection pooling support

## Next Steps

To complete the full implementation:

1. **Security**: Enable authentication/authorization for the API endpoints
2. **Testing**: Add unit and integration tests
3. **User Context**: Implement proper user context in triggers (replace hardcoded user ID)
4. **Monitoring**: Add logging and metrics for audit operations

The current implementation provides a solid foundation for audit logging with enterprise-grade features including security, performance, and maintainability.
