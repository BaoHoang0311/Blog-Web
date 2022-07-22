using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Data.Areas.Data.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "Email của bạn")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        [EmailAddress(ErrorMessage ="Vui long nhap email")]
        public string EmailAddress { get; set; }
        [Display(Name = "Password của bạn")]
        [Required(ErrorMessage ="vui long nhap mat khau cua bạn")]
        [StringLength(10,MinimumLength =6)]
        public string Password { get; set; }
    }
}
