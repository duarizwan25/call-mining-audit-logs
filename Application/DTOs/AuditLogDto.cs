namespace Application.DTOs
{
    public class AuditLogDto
    {
        public long Id { get; set; }
        public int DemoRequestId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string ChangedField { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int ChangedByUserId { get; set; }
        public DateTime ChangedDate { get; set; }
        public string? AdditionalInfo { get; set; }
    }

    public class AuditLogFilterDto
    {
        public string? ActionType { get; set; }
        public int? ChangedByUserId { get; set; }
        public DateTime? ChangedDateFrom { get; set; }
        public DateTime? ChangedDateTo { get; set; }
        public string? ChangedField { get; set; }
        public int? DemoRequestId { get; set; }
        public string SortBy { get; set; } = "ChangedDate";
        public string SortDirection { get; set; } = "DESC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class PagedAuditLogResponseDto
    {
        public IEnumerable<AuditLogDto> Data { get; set; } = new List<AuditLogDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
