using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuditLogRepository
    {
        PagedAuditLogResponseDto GetAuditLogs(AuditLogFilterDto filter);
        PagedAuditLogResponseDto GetAuditLogsByDemoRequestId(int demoRequestId, AuditLogFilterDto filter);
    }
}
