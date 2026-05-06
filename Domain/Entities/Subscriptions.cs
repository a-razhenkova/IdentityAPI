using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("subscription")]
    public class Subscription : EntityBase
    {
        [Required]
        [Column("create_timestamp", Order = 2)]
        public DateTime CreateTimestamp { get; set; }

        [Required]
        [Column("expiration_date", Order = 3)]
        public DateTime ExpirationDate { get; set; }

        [Required]
        [ForeignKey(nameof(Contract))]
        [Column("contract_id", Order = 4)]
        public long ContractId { get; set; }

        #region Relationships

        [InverseProperty(nameof(Contract.Subscription))]
        public virtual Document Contract { get; set; }

        [InverseProperty(nameof(ClientSubscription.Subscription))]
        public virtual ICollection<ClientSubscription> ClientSubscriptions { get; set; } = new List<ClientSubscription>();

        #endregion
    }
}