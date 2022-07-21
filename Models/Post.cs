using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace blog_web.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        [Required(ErrorMessage ="Không bỏ trống")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Không bỏ trống")]
        public string ShortContent { get; set; }
        [Required(ErrorMessage = "Không bỏ trống")]
        public string Contents { get; set; }
        [Required(ErrorMessage = "Không bỏ trống")]
        public string Thumb { get; set; }
        public bool Published { get; set; }
        public string Alias { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Author { get; set; }
        [Required(ErrorMessage = "Không bỏ trống")]
        public string Tags { get; set; }
        public bool IsHot { get; set; }
        public bool IsNewFeed { get; set; }
        public int? AccountId { get; set; }
        public int? CatId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Category Cat { get; set; }
    }
}
