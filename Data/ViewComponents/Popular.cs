using blog_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Data.ViewComponents
{
    public class Popular : ViewComponent
    {
        private readonly blogdbContext _context;
        public Popular(blogdbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Post[] list_popular_post = await _context.Posts
                                    .Where(x => x.IsHot == true)
                                    .OrderByDescending(x=>x.CreatedAt)                           
                                    .ToArrayAsync();
            return View(list_popular_post);
        }
    }
}
