using blog_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Data.ViewModel
{
    public class HomePageVM
    {
            public List<Post> Populars {get; set; }
            public List<Post> Inspiration { get; set; }
            public List<Post> Recents { get; set; }
            public List<Post> Trendings { get; set; }
            public List<Post> LatestPosts{ get; set; }
            public Post post { get; set; }
    }
}
