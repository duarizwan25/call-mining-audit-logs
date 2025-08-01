using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly string _connectionString;

        public AuditLogRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public PagedAuditLogResponseDto GetAuditLogs(AuditLogFilterDto filter)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var (whereClause, parameters) = BuildWhereClause(filter, includeDemoRequestIdFilter: true);
            var orderByClause = BuildOrderByClause(filter.SortBy, filter.SortDirection);
            
            // Calculate offset for pagination
            var offset = (filter.PageNumber - 1) * filter.PageSize;
            
            // Get total count
            var countSql = $@"
                SELECT COUNT(*) 
                FROM DemoRequestLogs 
                {whereClause}";
            
            var totalCount = connection.QuerySingle<int>(countSql, parameters);
            
            // Get paged data
            var dataSql = $@"
                SELECT 
                    Id,
                    DemoRequestId,
                    ActionType,
                    ChangedField,
                    OldValue,
                    NewValue,
                    ChangedByUserId,
                    ChangedDate,
                    AdditionalInfo
                FROM DemoRequestLogs 
                {whereClause}
                {orderByClause}
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";
            
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", filter.PageSize);
            
            var auditLogs = connection.Query<AuditLogDto>(dataSql, parameters)
                .Select(log => new AuditLogDto
                {
                    Id = log.Id,
                    DemoRequestId = log.DemoRequestId,
                    ActionType = log.ActionType,
                    ChangedField = log.ChangedField,
                    OldValue = log.OldValue,
                    NewValue = log.NewValue,
                    ChangedByUserId = log.ChangedByUserId,
                    ChangedDate = DateTime.SpecifyKind(log.ChangedDate, DateTimeKind.Utc), // Ensure UTC
                    AdditionalInfo = log.AdditionalInfo
                }).ToList();
            
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);
            
            return new PagedAuditLogResponseDto
            {
                Data = auditLogs,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                HasNextPage = filter.PageNumber < totalPages,
                HasPreviousPage = filter.PageNumber > 1
            };
        }

        public PagedAuditLogResponseDto GetAuditLogsByDemoRequestId(int demoRequestId, AuditLogFilterDto filter)
        {
            // Set the DemoRequestId filter
            filter.DemoRequestId = demoRequestId;
            
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var (whereClause, parameters) = BuildWhereClause(filter, includeDemoRequestIdFilter: false);
            var orderByClause = BuildOrderByClause(filter.SortBy, filter.SortDirection);
            
            // Add DemoRequestId filter
            whereClause = string.IsNullOrEmpty(whereClause) 
                ? "WHERE DemoRequestId = @DemoRequestId" 
                : $"{whereClause} AND DemoRequestId = @DemoRequestId";
            parameters.Add("DemoRequestId", demoRequestId);
            
            // Calculate offset for pagination
            var offset = (filter.PageNumber - 1) * filter.PageSize;
            
            // Get total count
            var countSql = $@"
                SELECT COUNT(*) 
                FROM DemoRequestLogs 
                {whereClause}";
            
            var totalCount = connection.QuerySingle<int>(countSql, parameters);
            
            // Get paged data
            var dataSql = $@"
                SELECT 
                    Id,
                    DemoRequestId,
                    ActionType,
                    ChangedField,
                    OldValue,
                    NewValue,
                    ChangedByUserId,
                    ChangedDate,
                    AdditionalInfo
                FROM DemoRequestLogs 
                {whereClause}
                {orderByClause}
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";
            
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", filter.PageSize);
            
            var auditLogs = connection.Query<AuditLogDto>(dataSql, parameters)
                .Select(log => new AuditLogDto
                {
                    Id = log.Id,
                    DemoRequestId = log.DemoRequestId,
                    ActionType = log.ActionType,
                    ChangedField = log.ChangedField,
                    OldValue = log.OldValue,
                    NewValue = log.NewValue,
                    ChangedByUserId = log.ChangedByUserId,
                    ChangedDate = DateTime.SpecifyKind(log.ChangedDate, DateTimeKind.Utc), // Ensure UTC
                    AdditionalInfo = log.AdditionalInfo
                }).ToList();
            
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);
            
            return new PagedAuditLogResponseDto
            {
                Data = auditLogs,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                HasNextPage = filter.PageNumber < totalPages,
                HasPreviousPage = filter.PageNumber > 1
            };
        }

        private (string whereClause, DynamicParameters parameters) BuildWhereClause(AuditLogFilterDto filter, bool includeDemoRequestIdFilter)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            // ActionType filter
            if (!string.IsNullOrEmpty(filter.ActionType))
            {
                conditions.Add("ActionType = @ActionType");
                parameters.Add("ActionType", filter.ActionType);
            }

            // ChangedByUserId filter
            if (filter.ChangedByUserId.HasValue)
            {
                conditions.Add("ChangedByUserId = @ChangedByUserId");
                parameters.Add("ChangedByUserId", filter.ChangedByUserId.Value);
            }

            // ChangedField filter
            if (!string.IsNullOrEmpty(filter.ChangedField))
            {
                conditions.Add("ChangedField = @ChangedField");
                parameters.Add("ChangedField", filter.ChangedField);
            }

            // DemoRequestId filter (only for all-logs API)
            if (includeDemoRequestIdFilter && filter.DemoRequestId.HasValue)
            {
                conditions.Add("DemoRequestId = @DemoRequestId");
                parameters.Add("DemoRequestId", filter.DemoRequestId.Value);
            }

            // ChangedDate range filter
            if (filter.ChangedDateFrom.HasValue)
            {
                conditions.Add("ChangedDate >= @ChangedDateFrom");
                // Ensure UTC for database query
                var utcFromDate = filter.ChangedDateFrom.Value.Kind == DateTimeKind.Utc 
                    ? filter.ChangedDateFrom.Value 
                    : filter.ChangedDateFrom.Value.ToUniversalTime();
                parameters.Add("ChangedDateFrom", utcFromDate);
            }

            if (filter.ChangedDateTo.HasValue)
            {
                conditions.Add("ChangedDate <= @ChangedDateTo");
                // Ensure UTC for database query
                var utcToDate = filter.ChangedDateTo.Value.Kind == DateTimeKind.Utc 
                    ? filter.ChangedDateTo.Value 
                    : filter.ChangedDateTo.Value.ToUniversalTime();
                parameters.Add("ChangedDateTo", utcToDate);
            }

            var whereClause = conditions.Count > 0 ? $"WHERE {string.Join(" AND ", conditions)}" : string.Empty;
            return (whereClause, parameters);
        }

        private string BuildOrderByClause(string sortBy, string sortDirection)
        {
            // Validate sort column to prevent SQL injection
            var validSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id", "DemoRequestId", "ActionType", "ChangedField", 
                "ChangedByUserId", "ChangedDate", "AdditionalInfo"
            };

            if (!validSortColumns.Contains(sortBy))
            {
                sortBy = "ChangedDate"; // Default sort column
            }

            // Validate sort direction
            var direction = string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";

            return $"ORDER BY {sortBy} {direction}";
        }
    }
}
