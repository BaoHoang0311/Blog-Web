using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using blog_web.Models;
using PagedList.Core;

namespace blog_web.Controllers
{
    public class PostsController : Controller
    {
        private readonly blogdbContext _context;

        public PostsController(blogdbContext context)
        {
            _context = context;
        }
        // GET: Posts
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var blogdbContext = _context.Posts
                                        .Include(p => p.Account)
                                        .Include(p => p.Cat);
            return View(await blogdbContext.ToListAsync());
        }
        //Get : list- danhmuc
        [Route("/{Alias}", Name = "List")]
        public async Task<IActionResult> List(string Alias, int? page)
        {
            if (string.IsNullOrEmpty(Alias)) return RedirectToAction("Index", "Home");
            var danhmuc = await _context.Categories.FirstOrDefaultAsync(x => x.Alias == Alias);
            if (danhmuc == null) return RedirectToAction("Index", "Home");
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pagesize = 2;
            var lspost = _context.Posts.Include(x => x.Cat)
                                        .AsNoTracking()
                                        .Where(x => x.CatId == danhmuc.CatId)
                                        .OrderByDescending(x => x.CreatedAt);
            PagedList<Post> posts = new PagedList<Post>(lspost, pageNumber, pagesize);
            //ViewBag.CurrentPage = pageNumber;
            ViewBag.Danhmuc = danhmuc.CatName;
            return View(posts);
        }
        [Route("/{Alias}.html", Name = "PostsDetails")]  // post
        public async Task<IActionResult> Details(string Alias)
        {
            if (string.IsNullOrEmpty(Alias))
            {
                return NotFound();
            }
            var post = await _context.Posts
                        .Include(p => p.Account)
                        .Include(p => p.Cat)
                        .FirstOrDefaultAsync(m => m.Alias == Alias);

            if (post == null)
            {
                return View("NOTFOUND");
            }
            return View(post);
        }
        // GET: Posts/Details/5
        // tin

        //public async Task<IActionResult> Details()
        //{
        //    var post = await _context.Posts
        //                .Include(p => p.Account)
        //                .Include(p => p.Cat)
        //                .FirstAsync();
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(post);
        //}
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
