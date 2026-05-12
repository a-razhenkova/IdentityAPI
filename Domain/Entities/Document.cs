using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("document")]
    public class Document : EntityBase
    {
        [Required]
        [Column("sign_timestamp", Order = 2)]
        public DateTime SignTimestamp { get; set; }

        [Required]
        [Column("name", Order = 3)]
        [MaxLength(DocumentConstants.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [Column("checksum", Order = 4)]
        [MaxLength(DocumentConstants.ChecksumMaxLength), Unicode(false)]
        public string Checksum { get; set; }

        [Required]
        [Column("type", Order = 5)]
        public DocumentTypes Type { get; set; }

        [Required]
        [Column("secret", Order = 6)]
        [MaxLength(DocumentConstants.SecretMaxLength), Unicode(false)]
        public string Secret { get; set; }

        #region Relationships

        [InverseProperty(nameof(Subscription.Contract))]
        public virtual Subscription Subscription { get; set; }

        #endregion
    }
}