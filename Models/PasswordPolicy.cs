using System.ComponentModel.DataAnnotations;

namespace DynamicPasswordPolicy.Models
{
    public class PasswordPolicy
    {
        [Key]
        public int Id { get; set; }
        public bool IsNumber { get; set; }

        public bool IsSpecialChar { get; set; }

        public bool IsUpperLetter { get; set; }

        public bool IsLowerLetter { get; set; }

        public int PasswordLegth { get; set; } = 8;

        public int PasswordExpiredInDays { get; set; } = 30;

        public int PasswordAttemptCount { get; set; } = 3;

    }
}
