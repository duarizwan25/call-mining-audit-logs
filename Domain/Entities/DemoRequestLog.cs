using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("DemoRequestLogs")]
    public class DemoRequestLog
    {
        [Key]
        public long Id { get; set; }
        
        public int DemoRequestId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string ChangedField { get; set; } = string.Empty;
        
        [StringLength(4000)]
        public string? OldValue { get; set; }
        
        [StringLength(4000)]
        public string? NewValue { get; set; }
        
        public int ChangedByUserId { get; set; }
        
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(255)]
        public string? AdditionalInfo { get; set; }
    }
}
