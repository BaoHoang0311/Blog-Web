using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace blog_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly blogdbContext _context;
        public SearchController(blogdbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult FindBaiViet(string keyword)
        {
            if (keyword != null)
            {
                var ls = _context.Posts
                            .Include(x => x.Cat)
                            .AsNoTracking()
                            .Where(x => x.Title.Contains(keyword) || x.Contents.Contains(keyword))
                            .OrderByDescending(x => x.CreatedAt)
                            .ToList();
                return PartialView("ListBaiVietSearchPartial", ls);
            }
            else
            {
                return PartialView("ListBaiVietSearchPartial", null);
            }
        }
    }
}
