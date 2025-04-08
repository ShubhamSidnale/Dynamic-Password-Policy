using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    public class User
    {
        [Key]
        public Guid Guid { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        [NotMapped]
        public string ConfirmPassword { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsLocked { get; set; } = false;
        public bool IsNew { get; set; } = false;


        public int FailedAttempts { get; set; } = 0;


    }
}
