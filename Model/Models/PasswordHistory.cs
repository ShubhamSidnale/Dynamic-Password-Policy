using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{ 


    public class PasswordHistory
    {
        [Key]
         public int ID { get; set; }
         public Guid Guid { get; set; }
        public string Password { get; set; } = string.Empty;
        public DateTime PasswordChangedDate { get; set; } = DateTime.Now;


    }
}
