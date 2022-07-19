using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace blog_web.Models
{
    public partial class Category
    {
        public Category()
        {
            Posts = new HashSet<Post>();
        }
        public int CatId { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập")]
        public string CatName { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập")]
        public string Title { get; set; }

        public string Alias { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập")]
        public string MetaDesc { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập")]
        public string MetaKey { get; set; }

        public string Thumb { get; set; }

        public bool Published { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập")]

        public int? Ordering { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập")]

        public int? Parent { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập")]
        public int? Levels { get; set; }

        public string Icon { get; set; }
        public string Cover { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập")]
        public string Description { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
