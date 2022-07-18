using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace blog_web.Models
{
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        public int RoleId { get; set; }
        [Required(ErrorMessage ="Yêu cầu nhập")]
        public string RoleName { get; set; }
        [Required(ErrorMessage ="Yêu cầu nhập")]
        public string Description { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
