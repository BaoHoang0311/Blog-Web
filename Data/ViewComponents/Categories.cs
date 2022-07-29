using blog_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Data.ViewComponents
{
    public class Categories : ViewComponent
    {
        private readonly blogdbContext _context;
        public Categories(blogdbContext context)
        {
            _context = context;
        }
        // số ở giỏ hàng
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cate = await _context.Categories.ToArrayAsync();
            return View(cate);
        }
    }
}
