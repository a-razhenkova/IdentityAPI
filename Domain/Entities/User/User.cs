using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("user")]
    [Index(nameof(PublicId), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    public partial class User : EntityBase
    {
        [Required]
        [Column("public_id", Order = 2)]
        [MaxLength(UserConstants.PublicIdMaxLength), Unicode(false)]
        public string PublicId { get; set; }

        [Required]
        [Column("username", Order = 3)]
        [MaxLength(UserConstants.UsernameMaxLength), Unicode(false)]
        public string Username { get; set; }

        [Required]
        [Column("role", Order = 5)]
        public UserRoles Role { get; set; }

        [Required]
        [Column("otp_key", Order = 6)]
        [MaxLength(UserConstants.OtpKeyMaxLength), Unicode(false)]
        public string OtpKey { get; set; }

        [Column("email", Order = 7)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column("is_verified", Order = 8)]
        public bool IsVerified { get; set; } = false;

        [Required]
        [Column("registration_timestamp", Order = 9)]
        public DateTime RegistrationTimestamp { get; set; } = DateTime.UtcNow;

        #region Relationships

        [InverseProperty(nameof(Status.User))]
        public virtual UserStatus Status { get; set; }

        [InverseProperty(nameof(UserPassword.User))]
        public virtual UserPassword Password { get; set; }

        [InverseProperty(nameof(Login.User))]
        public virtual Login Login { get; set; }

        #endregion
    }
}