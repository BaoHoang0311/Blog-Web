using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Areas.Admin.Data.ViewModel
{
    public class RegisterViewModel
    {
        [Display(Name = "Tên của bạn")]
        [Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string FullName { get; set; }

        [Display(Name = "Email của bạn")]
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        public string EmailAddress { get; set; }

        [Display(Name = "Nhập Password của bạn")]
        [Required]
        //[DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password của bạn")]
        [Required]
        //[DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password không trùng nhau")]
        public string ConfirmPassWord { get; set; }
    }
}
