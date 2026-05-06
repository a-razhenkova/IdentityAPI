using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id", Order = 0)]
        public long Id { get; set; }

        [Required]
        [Timestamp]
        [Column("version", Order = 1)]
        public byte[] Version { get; set; } = [];
    }
}