using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("client")]
    [Index(nameof(Key), IsUnique = true)]
    public class Client : EntityBase
    {
        [Required]
        [Column("name", Order = 2)]
        [MaxLength(ClientConstants.NameMaxLength), Unicode(false)]
        public string Name { get; set; }

        [Required]
        [Column("key", Order = 3)]
        [MaxLength(ClientConstants.KeyMaxLength), Unicode(false)]
        public string Key { get; set; }

        [Required]
        [Column("secret", Order = 4)]
        [MaxLength(ClientConstants.SecretMaxLength), Unicode(false)]
        public string Secret { get; set; }

        [Required]
        [Column("wrong_login_attempts_counter", Order = 5)]
        public int WrongLoginAttemptsCounter { get; set; }

        [Required]
        [Column("is_internal", Order = 6)]
        public bool IsInternal { get; set; } = false;

        #region Relationships

        [InverseProperty(nameof(Status.Client))]
        public virtual ClientStatus Status { get; set; }

        [InverseProperty(nameof(Right.Client))]
        public virtual ClientRight Right { get; set; }

        [InverseProperty(nameof(ClientSubscription.Client))]
        public virtual ICollection<ClientSubscription> Subscriptions { get; set; } = new List<ClientSubscription>();

        #endregion
    }
}