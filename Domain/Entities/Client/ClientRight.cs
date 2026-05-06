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
        [Column("can_notify_party", Order = 3)]
        public bool CanNotifyParty { get; set; }

        #region Relationships

        [InverseProperty(nameof(Client.Right))]
        public virtual Client Client { get; set; }

        #endregion
    }
}