using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Areas.Admin.Data.ViewModel
{
    public class ChangePasswordViewModel
    {
        public int AccountId { get; set; }
        [Display(Name = "Mật khẩu hiện tại")]
        public string Password { get; set; }
        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        public string password {get; set; }
        [MinLength(5, ErrorMessage = "Bạn cần đặt mặt khẩu tối thiểu 5 ký tự")]
        [Display(Name = "Nhập lại mật khẩu mới")]
        [Compare("Password", ErrorMessage = "Mật khẩu không giống nhau")]
        public string ConfirmPassword { get; set; }
     }   
}
