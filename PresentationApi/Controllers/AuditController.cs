using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Application.DTOs;

namespace PresentationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // TODO: Enable authorization when authentication is configured
    public class AuditController : ControllerBase
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditController(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        /// <summary>
        /// Get audit logs for all demo requests with advanced filters, sorting, and pagination
        /// </summary>
        [HttpGet("audit-logs")]
        public ActionResult<PagedAuditLogResponseDto> GetAuditLogs([FromQuery] AuditLogFilterDto filter)
        {
            try
            {
                var result = _auditLogRepository.GetAuditLogs(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit logs", error = ex.Message });
            }
        }

        /// <summary>
        /// Get audit logs for a specific demo request by ID
        /// </summary>
        [HttpGet("{demoRequestId}/audit-logs")]
        public ActionResult<PagedAuditLogResponseDto> GetAuditLogsByDemoRequestId(
            int demoRequestId, 
            [FromQuery] AuditLogFilterDto filter)
        {
            try
            {
                if (demoRequestId <= 0)
                {
                    return BadRequest(new { message = "Invalid demo request ID" });
                }

                var result = _auditLogRepository.GetAuditLogsByDemoRequestId(demoRequestId, filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit logs", error = ex.Message });
            }
        }
    }
}
