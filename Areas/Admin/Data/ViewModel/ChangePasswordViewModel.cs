using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Areas.Admin.Data.ViewModel
{
    public class ChangePasswordViewModel
    {
        public string Password_old { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        public string password_new {get; set; }
        [MinLength(5, ErrorMessage = "Bạn cần đặt mặt khẩu tối thiểu 5 ký tự")]
        [Compare("password_new", ErrorMessage = "Mật khẩu không giống nhau")]
        public string ConfirmPassword { get; set; }
     }   
}
