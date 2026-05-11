using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("user_status")]
    public partial class UserStatus : EntityBase
    {
        [Required]
        [ForeignKey(nameof(User))]
        [Column("user_id", Order = 2)]
        public long UserId { get; set; }

        [Required]
        [Column("status", Order = 3)]
        public UserStatuses Value { get; set; }

        [Required]
        [Column("reason", Order = 4)]
        public UserStatusReasons Reason { get; set; }

        [Column("note", Order = 5)]
        [MaxLength(UserConstants.StatusNoteMaxLength)]
        public string Note { get; set; }

        #region Relationships

        [InverseProperty(nameof(User.Status))]
        public virtual User User { get; set; }

        #endregion
    }
}