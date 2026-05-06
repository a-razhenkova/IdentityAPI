using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("client_right")]
    public partial class ClientRight : EntityBase
    {
        [Required]
        [ForeignKey(nameof(Client))]
        [Column("client_id", Order = 2)]
        public long ClientId { get; set; }

        [Required]
        [Column("can_notify", Order = 3)]
        public bool CanNotify { get; set; }

        #region Relationships

        [InverseProperty(nameof(Client.Right))]
        public virtual Client Client { get; set; }

        #endregion
    }
}